namespace Email.Net.Socketlabs.Test
{
    using Email.Net.EDP.SocketLabs;
    using Email.Net.Exceptions;
    using System;
    using System.Linq;
    using Xunit;

    public class SocketLabsEmailDeliveryProviderShould
    {
        static readonly string TEST_TO_EMAIL = EnvVariable.Load("EMAIL_NET_TO_EMAIL");
        static readonly string TEST_FROM_EMAIL = EnvVariable.Load("EMAIL_NET_FROM_EMAIL");
        static readonly string TEST_API_KEY = EnvVariable.Load("EMAIL_NET_SOCKETLABS_API_KEY");
        static readonly int TEST_SERVER = int.Parse(EnvVariable.Load("EMAIL_NET_SOCKETLABS_SERVER"));

        [Fact] 
        public void ThorwIfOptionsIsNull()
        {
            // arrange
            SocketLabsEmailDeliveryProviderOptions? options = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var edp = new SocketLabsEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptionsIsNull()
        {
            // arrange
            var options = new SocketLabsEmailDeliveryProviderOptions();

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new SocketLabsEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_DefaultServerId_IsZero()
        {
            // arrange
            var options = new SocketLabsEmailDeliveryProviderOptions()
            {
                DefaultServerId = 0
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new SocketLabsEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsNull()
        {
            // arrange
            var options = new SocketLabsEmailDeliveryProviderOptions()
            {
                DefaultServerId = 152,
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new SocketLabsEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_ApiKey_IsEmpty()
        {
            // arrange
            var options = new SocketLabsEmailDeliveryProviderOptions()
            {
                DefaultServerId = 152,
                ApiKey = ""
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryProviderOptions>>(() =>
            {
                // act
                var edp = new SocketLabsEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void CreateMailMessageFromMessage()
        {
            // arrange
            var edp = new SocketLabsEmailDeliveryProvider(new SocketLabsEmailDeliveryProviderOptions()
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
            var mailMessage = edp.CreateMessage(message);

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
            var edp = new SocketLabsEmailDeliveryProvider(new SocketLabsEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                DefaultServerId = TEST_SERVER,
            });

            var message = EmailMessage.Compose()
                .From(TEST_FROM_EMAIL)
                .ReplyTo("replayto@email.net")
                .To(TEST_TO_EMAIL)
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .WithHtmlContent("<p>this is a test</p>")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net")
                .WithCc("cc@email.net")
                .WithHeader("key", "value")
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
            var edp = new SocketLabsEmailDeliveryProvider(new SocketLabsEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                DefaultServerId = TEST_SERVER,
            });

            var message = EmailMessage.Compose()
                .From(TEST_FROM_EMAIL)
                .ReplyTo("replayto@email.net")
                .To(TEST_TO_EMAIL)
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
            var edp = new SocketLabsEmailDeliveryProvider(new SocketLabsEmailDeliveryProviderOptions()
            {
                ApiKey = TEST_API_KEY,
                DefaultServerId = TEST_SERVER,
            });

            var message = EmailMessage.Compose()
                .From(TEST_FROM_EMAIL)
                .ReplyTo("replayto@email.net")
                .To(TEST_TO_EMAIL)
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