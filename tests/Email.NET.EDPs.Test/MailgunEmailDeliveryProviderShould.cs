namespace Email.NET.Mailgun.Test
{
    using Email.NET.EDP;
    using Email.NET.EDP.Mailgun;
    using Email.NET.Exceptions;
    using System;
    using System.Linq;
    using Xunit;

    public class MailgunEmailDeliveryProviderShould
    {
        static string TEST_To_EMAI = Environment.GetEnvironmentVariable("EmailNET_TestToEmail", EnvironmentVariableTarget.Machine) ?? throw new ArgumentNullException();
        static string TEST_FROM_EMAI = Environment.GetEnvironmentVariable("EmailNET_TestFromEmail", EnvironmentVariableTarget.Machine) ?? throw new ArgumentNullException();
        static string TEST_DOMAIN = Environment.GetEnvironmentVariable("EmailNET_MailgunDomain", EnvironmentVariableTarget.Machine) ?? throw new ArgumentNullException();
        static string TEST_API_KEY = Environment.GetEnvironmentVariable("EmailNET_MailgunApiKey", EnvironmentVariableTarget.Machine) ?? throw new ArgumentNullException();

        [Fact]
        public void ThorwIfOptionsIsNull()
        {
            // arrange
            MailgunEmailDeliveryProviderOptions? options = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var edp = new MailgunEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsNull()
        {
            // arrange
            var options = new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = null,
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new MailgunEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsEmpty()
        {
            // arrange
            var options = new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new MailgunEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_Domain_IsNull()
        {
            // arrange
            var options = new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = null,
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new MailgunEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_Domain_IsEmpty()
        {
            // arrange
            var options = new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new MailgunEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_BaseUrl_IsNull()
        {
            // arrange
            var options = new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
                BaseUrl = null
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new MailgunEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_BaseUrl_IsEmpty()
        {
            // arrange
            var options = new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
                BaseUrl= ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new MailgunEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public async void CreateMailMessageFromMessage()
        {
            // arrange
            var edp = new MailgunEmailDeliveryProvider(new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
            });

            var message = Message.Compose()
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
            var mailMessage = edp.CreateMessage(message);

            // assert
            //Assert.Equal(message.From.Address, mailMessage.From.Email);
            //Assert.Equal(message.From.DisplayName, mailMessage.From.FriendlyName);
            //Assert.Equal(message.ReplyTo.First().Address, mailMessage.ReplyTo.Email);
            //Assert.Equal(message.ReplyTo.First().DisplayName, mailMessage.ReplyTo.FriendlyName);
            //Assert.Equal(message.To.First().Address, mailMessage.To.First().Email);
            //Assert.Equal(message.To.First().DisplayName, mailMessage.To.First().FriendlyName);
            //Assert.Equal(message.Subject, mailMessage.Subject);
            //Assert.Equal(message.PlainTextBody, mailMessage.PlainTextBody);
            //Assert.Equal(message.HtmlBody, mailMessage.HtmlBody);
            //Assert.Equal(message.Charset, mailMessage.CharSet);
            //Assert.Equal(message.Bcc.First().Address, mailMessage.Bcc.First().Email);
            //Assert.Equal(message.Bcc.First().DisplayName, mailMessage.Bcc.First().FriendlyName);
            //Assert.Equal(message.Cc.First().Address, mailMessage.Cc.First().Email);
            //Assert.Equal(message.Cc.First().DisplayName, mailMessage.Cc.First().FriendlyName);
            //Assert.Equal(message.Headers.First().Key, mailMessage.CustomHeaders.First().Name);
            //Assert.Equal(message.Headers.First().Value, mailMessage.CustomHeaders.First().Value);
        }

        [Fact]
        public void SendEmail()
        {
            // arrange
            var edp = new MailgunEmailDeliveryProvider(new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
            });

            var message = Message.Compose()
                .From(TEST_FROM_EMAI)
                .ReplyTo("replayto@email.net")
                .To(TEST_To_EMAI)
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .WithHtmlContent("<p>this is a test</p>")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net")
                .WithCc("cc@email.net")
                .WithHeader("key", "value")
                .UseTestMode()
                .Build();

            // act
            var result = edp.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void SendEmailWithBase64Attachement()
        {
            // arrange
            var edp = new MailgunEmailDeliveryProvider(new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
            });

            var message = Message.Compose()
                .From(TEST_FROM_EMAI)
                .ReplyTo("replayto@email.net")
                .To(TEST_To_EMAI)
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .WithHtmlContent("<p>this is a test</p>")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net")
                .WithCc("cc@email.net")
                .WithHeader("key", "value")
                .IncludeAttachment(new Base64Attachement(@"test_file.txt", MockData.TestFileBase64Value))
                .Build();

            // act
            var result = edp.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void SendEmailWithFilePathAttachement()
        {
            // arrange
            var edp = new MailgunEmailDeliveryProvider(new MailgunEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                Domain = TEST_DOMAIN,
            });

            var message = Message.Compose()
                .From(TEST_FROM_EMAI)
                .ReplyTo("replayto@email.net")
                .To(TEST_To_EMAI)
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .WithHtmlContent("<p>this is a test</p>")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net")
                .WithCc("cc@email.net")
                .WithHeader("key", "value")
                .IncludeAttachment(new FilePathAttachment(MockData.TestFilePath))
                .Build();

            // act
            var result = edp.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }
    }
}