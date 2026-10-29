namespace Email.Net.Mailgun.Test
{
    using Email.Net.Channel.Mailgun;
    using Email.Net.Exceptions;
    using Email.Net.Factories;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Xunit;

    public class MailgunEmailDeliveryChannelshould
    {
        const string TEST_DOMAIN = "mg.email.net";
        const string TEST_API_KEY = "test-api-key";

        [Fact]
        public void ThorwIfOptionsIsNull()
        {
            // arrange
            MailgunEmailDeliveryChannelOptions? options = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var channel = new MailgunEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsNull()
        {
            // arrange
            var options = new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = null,
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new MailgunEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsEmpty()
        {
            // arrange
            var options = new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new MailgunEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_Domain_IsNull()
        {
            // arrange
            var options = new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = null,
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new MailgunEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_Domain_IsEmpty()
        {
            // arrange
            var options = new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new MailgunEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_BaseUrl_IsNull()
        {
            // arrange
            var options = new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
                BaseUrl = null
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new MailgunEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_BaseUrl_IsEmpty()
        {
            // arrange
            var options = new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
                BaseUrl = ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>>(() =>
            {
                // act
                var channel = new MailgunEmailDeliveryChannel(options);
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
                var channel = new MailgunEmailDeliveryChannel(new MailgunEmailDeliveryChannelOptions
                {
                    ApiKey = TEST_API_KEY,
                    Domain = TEST_DOMAIN,
                }, httpClient);
            });
        }

        [Fact]
        public async Task CreateMailMessageFromMessage()
        {
            // arrange
            var channel = new MailgunEmailDeliveryChannel(new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
            });

            var message = CreateMessage().Build();

            // act
            var content = await channel.CreateMessage(message).ReadAsStringAsync(TestContext.Current.CancellationToken);

            // assert
            AssertFormField(content, "from", "from@email.net");
            AssertFormField(content, "h:Reply-To", "replayto@email.net");
            AssertFormField(content, "to", "to@email.net");
            AssertFormField(content, "subject", "test subject");
            AssertFormField(content, "text", "this is a test");
            AssertFormField(content, "html", "<p>this is a test</p>");
            AssertFormField(content, "bcc", "bcc@email.net");
            AssertFormField(content, "cc", "cc@email.net");
            AssertFormField(content, "h:key", "value");
        }

        [Fact]
        public void SendEmail()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK);
            var channel = CreateChannel(handler);
            var message = CreateMessage()
                .UseTestMode()
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Equal(MailgunEmailDeliveryChannel.Name, result.ChannelName);

            var request = Assert.Single(handler.Requests);
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal($"https://api.mailgun.net/v3/{TEST_DOMAIN}/messages", request.Uri?.ToString());
            Assert.Equal("Basic", request.Request.Headers.Authorization?.Scheme);
            Assert.Equal(Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{TEST_API_KEY}")), request.Request.Headers.Authorization?.Parameter);
            AssertFormField(request.Body, "to", "to@email.net");
            AssertFormField(request.Body, "o:testmode", "yes");
        }

        [Fact]
        public async Task SendEmailAsync()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK);
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
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK);
            var channel = CreateChannel(handler);

            // act
            var result1 = channel.Send(CreateMessage().Build());
            var result2 = await channel.SendAsync(CreateMessage().Build(), TestContext.Current.CancellationToken);

            // assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.Equal(2, handler.Requests.Count);
            Assert.All(handler.Requests, r => Assert.NotNull(r.Request.Headers.Authorization));
        }

        [Fact]
        public void SendEmailUsingHttp_WhenSSLIsDisabled()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK);
            var channel = new MailgunEmailDeliveryChannel(new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
                BaseUrl = "api.eu.mailgun.net/v3",
                UseSSL = false,
            }, handler.CreateClient());

            // act
            var result = channel.Send(CreateMessage().Build());

            // assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"http://api.eu.mailgun.net/v3/{TEST_DOMAIN}/messages", handler.LastRequest.Uri?.ToString());
        }

        [Fact]
        public void SendEmailWithBase64Attachement()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK);
            var channel = CreateChannel(handler);
            var message = CreateMessage()
                .IncludeAttachment(new Base64Attachement(@"test_file.txt", MockData.TestFileBase64Value))
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);

            var request = Assert.Single(handler.Requests);
            Assert.Contains("name=attachment; filename=test_file.txt", request.Body);
            Assert.Contains(Encoding.UTF8.GetString(Convert.FromBase64String(MockData.TestFileBase64Value)), request.Body);
        }

        [Fact]
        public void SendEmailWithFilePathAttachement()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK);
            var channel = CreateChannel(handler);
            var message = CreateMessage()
                .IncludeAttachment(new FilePathAttachment(MockData.TestFilePath))
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);

            var request = Assert.Single(handler.Requests);
            Assert.Contains($"name=attachment; filename={MockData.TestFileName}", request.Body);
            Assert.Contains(Encoding.UTF8.GetString(Convert.FromBase64String(MockData.TestFileBase64Value)), request.Body);
        }

        [Fact]
        public void ReturnFailureWithErrors_WhenApiReturnsError()
        {
            // arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.Unauthorized, "Forbidden");
            var channel = CreateChannel(handler);
            var message = CreateMessage().Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Forbidden", result.GetMetaData<string>("api_response"));

            var error = Assert.Single(result.Errors);
            Assert.Equal("401", error.Code);
            Assert.Equal(nameof(HttpStatusCode.Unauthorized), error.Message);
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

        // the field name is quoted when it contains special chars, e.g. name="h:Reply-To"
        private static void AssertFormField(string content, string name, string value)
            => Assert.Matches($"name=\"?{Regex.Escape(name)}\"?\\r\\n\\r\\n{Regex.Escape(value)}\\r\\n", content);

        private static MailgunEmailDeliveryChannel CreateChannel(FakeHttpMessageHandler handler)
            => new MailgunEmailDeliveryChannel(new MailgunEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
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
