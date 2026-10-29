namespace Email.Net.Sendgrid.Test
{
    using Email.Net.Channel.SendGrid;
    using Email.Net.Exceptions;
    using Email.Net.Factories;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class SendgridEmailDeliveryChannelshould
    {
        const string TEST_API_KEY = "SG.test-api-key";

        [Fact]
        public void ThorwIfOptionsIsNull()
        {
            // arrange
            SendgridEmailDeliveryChannelOptions? options = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var channel = new SendgridEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptionsIsNull()
        {
            // arrange
            var options = new SendgridEmailDeliveryChannelOptions();

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SendgridEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new SendgridEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsNull()
        {
            // arrange
            var options = new SendgridEmailDeliveryChannelOptions()
            {
                ApiKey = null,
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SendgridEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new SendgridEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsEmpty()
        {
            // arrange
            var options = new SendgridEmailDeliveryChannelOptions()
            {
                ApiKey = ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SendgridEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new SendgridEmailDeliveryChannel(options);
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
                var channel = new SendgridEmailDeliveryChannel(new SendgridEmailDeliveryChannelOptions { ApiKey = TEST_API_KEY }, httpClient);
            });
        }

        [Fact]
        public void CreateMailMessageFromMessage()
        {
            // arrange
            var channel = new SendgridEmailDeliveryChannel(new SendgridEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
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
            var personalizations = mailMessage.Personalizations.First();

            Assert.Equal(message.From.Address, mailMessage.From.Email);
            Assert.Equal(message.From.DisplayName, mailMessage.From.Name);
            Assert.Equal(message.ReplyTo.First().Address, mailMessage.ReplyTo.Email);
            Assert.Equal(message.ReplyTo.First().DisplayName, mailMessage.ReplyTo.Name);
            Assert.Equal(message.To.First().Address, personalizations.Tos.First().Email);
            Assert.Equal(message.To.First().DisplayName, personalizations.Tos.First().Name);
            Assert.Equal(message.Subject, mailMessage.Subject);
            Assert.Equal(message.PlainTextBody, mailMessage.PlainTextContent);
            Assert.Equal(message.HtmlBody, mailMessage.HtmlContent);
            Assert.Equal(message.Bcc.First().Address, personalizations.Bccs.First().Email);
            Assert.Equal(message.Bcc.First().DisplayName, personalizations.Bccs.First().Name);
            Assert.Equal(message.Cc.First().Address, personalizations.Ccs.First().Email);
            Assert.Equal(message.Cc.First().DisplayName, personalizations.Ccs.First().Name);
            Assert.Equal(message.Headers.First().Key, mailMessage.Headers.First().Key);
            Assert.Equal(message.Headers.First().Value, mailMessage.Headers.First().Value);
        }

        [Fact]
        public void SendEmail()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.Accepted);
            var channel = CreateChannel(handler);
            var message = CreateMessage().Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Equal(SendgridEmailDeliveryChannel.Name, result.ChannelName);

            var request = Assert.Single(handler.Requests);
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.sendgrid.com/v3/mail/send", request.Uri?.ToString());
            Assert.Equal("Bearer", request.Request.Headers.Authorization?.Scheme);
            Assert.Equal(TEST_API_KEY, request.Request.Headers.Authorization?.Parameter);
            Assert.Contains("\"from@email.net\"", request.Body);
            Assert.Contains("\"to@email.net\"", request.Body);
            Assert.Contains("\"test subject\"", request.Body);
        }

        [Fact]
        public async Task SendEmailAsync()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.Accepted);
            var channel = CreateChannel(handler);
            var message = CreateMessage().Build();

            // act
            var result = await channel.SendAsync(message, TestContext.Current.CancellationToken);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Single(handler.Requests);
        }

        [Fact]
        public void SendEmailWithBase64Attachement()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.Accepted);
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
            var handler = new FakeHttpMessageHandler(HttpStatusCode.Accepted);
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
        public void SendEmailUsingCustomApiKey()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.Accepted);
            var channel = CreateChannel(handler);
            var message = CreateMessage()
                .UseCustomApiKey("SG.custom-api-key")
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Equal("SG.custom-api-key", handler.LastRequest.Request.Headers.Authorization?.Parameter);
        }

        [Fact]
        public void ReturnFailureWithErrors_WhenApiReturnsError()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.BadRequest,
                "{\"errors\":[{\"message\":\"The from email does not contain a valid address.\",\"field\":\"from.email\",\"help\":null}]}");

            var channel = CreateChannel(handler);
            var message = CreateMessage().Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.False(result.IsSuccess);
            Assert.Equal(nameof(HttpStatusCode.BadRequest), result.GetMetaData<string>("status_code"));

            var error = Assert.Single(result.Errors);
            Assert.Equal("from.email", error.Code);
            Assert.Equal("The from email does not contain a valid address.", error.Message);
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

            var error = Assert.Single(result.Errors);
            Assert.IsType<HttpRequestException>(error.Exception);
        }

        private static SendgridEmailDeliveryChannel CreateChannel(FakeHttpMessageHandler handler)
            => new SendgridEmailDeliveryChannel(new SendgridEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
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
