namespace Email.Net.Socketlabs.Test
{
    using Email.Net.Channel.SocketLabs;
    using Email.Net.Exceptions;
    using Email.Net.Factories;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class SocketLabsEmailDeliveryChannelshould
    {
        const string TEST_API_KEY = "test-api-key";
        const int TEST_SERVER = 12345;
        const string SUCCESS_RESPONSE = "{\"ErrorCode\":\"Success\",\"MessageResults\":[],\"TransactionReceipt\":\"test-receipt\"}";

        [Fact]
        public void ThorwIfOptionsIsNull()
        {
            // arrange
            SocketLabsEmailDeliveryChannelOptions? options = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var channel = new SocketLabsEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptionsIsNull()
        {
            // arrange
            var options = new SocketLabsEmailDeliveryChannelOptions();

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new SocketLabsEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_DefaultServerId_IsZero()
        {
            // arrange
            var options = new SocketLabsEmailDeliveryChannelOptions()
            {
                DefaultServerId = 0
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new SocketLabsEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsNull()
        {
            // arrange
            var options = new SocketLabsEmailDeliveryChannelOptions()
            {
                DefaultServerId = 152,
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new SocketLabsEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsEmpty()
        {
            // arrange
            var options = new SocketLabsEmailDeliveryChannelOptions()
            {
                DefaultServerId = 152,
                ApiKey = ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new SocketLabsEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfHttpClientIsNull()
        {
            // arrange
            HttpClient? httpClient = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var channel = new SocketLabsEmailDeliveryChannel(new SocketLabsEmailDeliveryChannelOptions
                {
                    ApiKey = TEST_API_KEY,
                    DefaultServerId = TEST_SERVER,
                }, httpClient);
            });
        }

        [Fact]
        public void CreateMailMessageFromMessage()
        {
            // arrange
            var channel = new SocketLabsEmailDeliveryChannel(new SocketLabsEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                DefaultServerId = TEST_SERVER,
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
            Assert.Equal(message.From.Address, mailMessage.From.Email);
            Assert.Equal(message.From.DisplayName, mailMessage.From.FriendlyName);
            Assert.Equal(message.ReplyTo.First().Address, mailMessage.ReplyTo.Email);
            Assert.Equal(message.ReplyTo.First().DisplayName, mailMessage.ReplyTo.FriendlyName);
            Assert.Equal(message.To.First().Address, mailMessage.To.First().Email);
            Assert.Equal(message.To.First().DisplayName, mailMessage.To.First().FriendlyName);
            Assert.Equal(message.Subject, mailMessage.Subject);
            Assert.Equal(message.PlainTextBody, mailMessage.PlainTextBody);
            Assert.Equal(message.HtmlBody, mailMessage.HtmlBody);
            Assert.Equal(message.Charset, mailMessage.CharSet);
            Assert.Equal(message.Bcc.First().Address, mailMessage.Bcc.First().Email);
            Assert.Equal(message.Bcc.First().DisplayName, mailMessage.Bcc.First().FriendlyName);
            Assert.Equal(message.Cc.First().Address, mailMessage.Cc.First().Email);
            Assert.Equal(message.Cc.First().DisplayName, mailMessage.Cc.First().FriendlyName);
            Assert.Equal(message.Headers.First().Key, mailMessage.CustomHeaders.First().Name);
            Assert.Equal(message.Headers.First().Value, mailMessage.CustomHeaders.First().Value);
        }

        [Fact]
        public void SendEmail()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, SUCCESS_RESPONSE);
            var channel = CreateChannel(handler);
            var message = CreateMessage().Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Equal(SocketLabsEmailDeliveryChannel.Name, result.ChannelName);
            Assert.Equal("test-receipt", result.GetMetaData<string>("transaction_receipt_key"));

            var request = Assert.Single(handler.Requests);
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://inject.socketlabs.com/api/v1/email", request.Uri?.ToString());
            Assert.Contains(TEST_SERVER.ToString(), request.Body);
            Assert.Contains("\"from@email.net\"", request.Body);
            Assert.Contains("\"to@email.net\"", request.Body);
            Assert.Contains("\"test subject\"", request.Body);
        }

        [Fact]
        public async Task SendEmailAsync()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, SUCCESS_RESPONSE);
            var channel = CreateChannel(handler);
            var message = CreateMessage().Build();

            // act
            var result = await channel.SendAsync(message, TestContext.Current.CancellationToken);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Single(handler.Requests);
        }

        [Fact]
        public async Task SendMultipleEmailsUsingTheSameHttpClient()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, SUCCESS_RESPONSE);
            var channel = CreateChannel(handler);

            // act
            var result1 = channel.Send(CreateMessage().Build());
            var result2 = await channel.SendAsync(CreateMessage().Build(), TestContext.Current.CancellationToken);

            // assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.Equal(2, handler.Requests.Count);
        }

        [Fact]
        public void SendEmailWithBase64Attachement()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, SUCCESS_RESPONSE);
            var channel = CreateChannel(handler);
            var message = CreateMessage()
                .IncludeAttachment(new Base64Attachement(@"test_file.txt", MockData.TestFileBase64Value))
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);

            var request = Assert.Single(handler.Requests);
            Assert.Contains("\"test_file.txt\"", request.Body);
            Assert.Contains(MockData.TestFileBase64Value, request.Body);
        }

        [Fact]
        public void SendEmailWithFilePathAttachement()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, SUCCESS_RESPONSE);
            var channel = CreateChannel(handler);
            var message = CreateMessage()
                .IncludeAttachment(new FilePathAttachment(MockData.TestFilePath))
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);

            var request = Assert.Single(handler.Requests);
            Assert.Contains($"\"{MockData.TestFileName}\"", request.Body);
            Assert.Contains(MockData.TestFileBase64Value, request.Body);
        }

        [Fact]
        public void SendEmailUsingCustomServerId()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, SUCCESS_RESPONSE);
            var channel = CreateChannel(handler);
            var message = CreateMessage()
                .UseCustomServerId(98765)
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Contains("98765", handler.LastRequest.Body);
        }

        [Fact]
        public void ReturnFailureWithErrors_WhenApiReturnsError()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK,
                "{\"ErrorCode\":\"InvalidAuthentication\",\"MessageResults\":[],\"TransactionReceipt\":null}");

            var channel = CreateChannel(handler);
            var message = CreateMessage().Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.False(result.IsSuccess);

            var error = Assert.Single(result.Errors);
            Assert.Equal("InvalidAuthentication", error.Code);
        }

        [Fact]
        public void ReturnFailure_WhenRequestThrows()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(_ => throw new HttpRequestException("network error"));
            var channel = CreateChannel(handler);
            var message = CreateMessage().Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }

        private static SocketLabsEmailDeliveryChannel CreateChannel(FakeHttpMessageHandler handler)
            => new SocketLabsEmailDeliveryChannel(new SocketLabsEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                DefaultServerId = TEST_SERVER,
            }, handler.CreateClient());

        private static EmailMessageComposer CreateMessage()
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
                .WithHeader("key", "value");
    }
}
