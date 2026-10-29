namespace Email.Net.AmazonSES.Test
{
    using Email.Net.Channel.AmazonSES;
    using Amazon.Runtime;
    using Amazon.SimpleEmail;
    using Amazon.SimpleEmail.Model;
    using Email.Net.Exceptions;
    using NSubstitute;
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class AmazonSESEmailDeliveryChannelshould
    {
        [Fact]
        public void ThorwIfOptionsIsNull()
        {
            // arrange
            AmazonSESEmailDeliveryChannelOptions? options = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var channel = new AmazonSESEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_OptionsValuesAreNull()
        {
            // arrange
            var options = new AmazonSESEmailDeliveryChannelOptions();

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<AmazonSESEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new AmazonSESEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_RegionEndpoint_IsNull()
        {
            // arrange
            var options = new AmazonSESEmailDeliveryChannelOptions()
            {
                RegionEndpoint = null,
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<AmazonSESEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new AmazonSESEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void CreateMailMessageFromMessage()
        {
            // arrange
            var channel = new AmazonSESEmailDeliveryChannel(new AmazonSESEmailDeliveryChannelOptions()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USWest2,
            });

            var message = EmailMessage.Compose()
                .From("from@email.net")
                .ReplyTo("replayto@email.net")
                .To("to@email.net")
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .WithHtmlContent("<p>this is a test</p>")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net")
                .WithCc("cc@email.net")
                .WithHeader("key", "value")
                .Build();

            // act
            var mailMessage = channel.CreateMessage(message);

            // assert
            Assert.Equal(message.From.Address, mailMessage.Source);
            Assert.Equal(message.ReplyTo.First().Address, mailMessage.ReplyToAddresses.First());
            Assert.Equal(message.To.First().Address, mailMessage.Destination.ToAddresses.First());
            Assert.Equal(message.Subject, mailMessage.Message.Subject.Data);
            Assert.Equal(message.PlainTextBody, mailMessage.Message.Body.Text.Data);
            Assert.Equal(message.HtmlBody, mailMessage.Message.Body.Html.Data);
            Assert.Equal(message.Charset, mailMessage.Message.Body.Text.Charset);
            Assert.Equal(message.Charset, mailMessage.Message.Body.Html.Charset);
            Assert.Equal(message.Charset, mailMessage.Message.Subject.Charset);
            Assert.Equal(message.Bcc.First().Address, mailMessage.Destination.BccAddresses.First());
            Assert.Equal(message.Cc.First().Address, mailMessage.Destination.CcAddresses.First());
        }

        [Fact]
        public void ThorwIfClientIsNull()
        {
            // arrange
            IAmazonSimpleEmailService? client = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var channel = new AmazonSESEmailDeliveryChannel(new AmazonSESEmailDeliveryChannelOptions()
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USWest2,
                }, client);
            });
        }

        [Fact]
        public void SendEmail()
        {
            // arrange
            var client = CreateClient(new SendEmailResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                MessageId = "test-message-id",
                ResponseMetadata = new ResponseMetadata { RequestId = "test-request-id" },
            });

            var channel = CreateChannel(client);
            var message = CreateMessage();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Equal(AmazonSESEmailDeliveryChannel.Name, result.ChannelName);
            Assert.Equal("test-message-id", result.GetMetaData<string>("message_id"));
            Assert.Equal("test-request-id", result.GetMetaData<string>("request_id"));

            client.Received(1).SendEmailAsync(
                Arg.Is<SendEmailRequest>(r =>
                    r.Source == "from@email.net" &&
                    r.Destination.ToAddresses.Single() == "to@email.net" &&
                    r.Message.Subject.Data == "test subject"),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task SendEmailAsync()
        {
            // arrange
            var client = CreateClient(new SendEmailResponse { HttpStatusCode = HttpStatusCode.OK });
            var channel = CreateChannel(client);
            var message = CreateMessage();

            // act
            var result = await channel.SendAsync(message, TestContext.Current.CancellationToken);

            // assert
            Assert.True(result.IsSuccess);
            await client.Received(1).SendEmailAsync(Arg.Any<SendEmailRequest>(), TestContext.Current.CancellationToken);
        }

        [Fact]
        public void NotDisposeTheGivenClient()
        {
            // arrange
            var client = CreateClient(new SendEmailResponse { HttpStatusCode = HttpStatusCode.OK });
            var channel = CreateChannel(client);

            // act
            channel.Send(CreateMessage());
            channel.Send(CreateMessage());

            // assert
            client.DidNotReceive().Dispose();
            client.Received(2).SendEmailAsync(Arg.Any<SendEmailRequest>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public void ReturnFailureWithMetaData_WhenResponseIsNotOk()
        {
            // arrange
            var responseMetadata = new ResponseMetadata { RequestId = "test-request-id" };
            responseMetadata.Metadata.Add("error", "test error");

            var client = CreateClient(new SendEmailResponse
            {
                HttpStatusCode = HttpStatusCode.BadRequest,
                MessageId = "test-message-id",
                ResponseMetadata = responseMetadata,
            });

            var channel = CreateChannel(client);

            // act
            var result = channel.Send(CreateMessage());

            // assert
            Assert.False(result.IsSuccess);
            Assert.Equal("test-message-id", result.GetMetaData<string>("message_id"));
            Assert.Equal("test-request-id", result.GetMetaData<string>("request_id"));
            Assert.Equal("test error", result.GetMetaData<string>("error"));
        }

        [Fact]
        public void ReturnFailure_WhenClientThrows()
        {
            // arrange
            var client = Substitute.For<IAmazonSimpleEmailService>();
            client.SendEmailAsync(Arg.Any<SendEmailRequest>(), Arg.Any<CancellationToken>())
                .Returns<Task<SendEmailResponse>>(_ => throw new MessageRejectedException("Email address is not verified."));

            var channel = CreateChannel(client);

            // act
            var result = channel.Send(CreateMessage());

            // assert
            Assert.False(result.IsSuccess);

            var error = Assert.Single(result.Errors);
            Assert.IsType<MessageRejectedException>(error.Exception);
        }

        private static IAmazonSimpleEmailService CreateClient(SendEmailResponse response)
        {
            var client = Substitute.For<IAmazonSimpleEmailService>();
            client.SendEmailAsync(Arg.Any<SendEmailRequest>(), Arg.Any<CancellationToken>())
                .Returns(response);

            return client;
        }

        private static AmazonSESEmailDeliveryChannel CreateChannel(IAmazonSimpleEmailService client)
            => new AmazonSESEmailDeliveryChannel(new AmazonSESEmailDeliveryChannelOptions()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USWest2,
            }, client);

        private static EmailMessage CreateMessage()
            => EmailMessage.Compose()
                .From("from@email.net")
                .ReplyTo("replayto@email.net")
                .To("to@email.net")
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .WithHtmlContent("<p>this is a test</p>")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net")
                .WithCc("cc@email.net")
                .WithHeader("key", "value")
                .Build();
    }
}
