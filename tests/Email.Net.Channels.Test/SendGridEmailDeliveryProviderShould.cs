namespace Email.Net.Sendgrid.Test
{
    using Email.Net.Channel.SendGrid;
    using Email.Net.Exceptions;
    using System;
    using System.Linq;
    using Xunit;

    public class SendgridEmailDeliveryChannelshould
    {
        static readonly string TEST_TO_EMAIL = EnvVariable.Load("EMAIL_NET_TO_EMAIL");
        static readonly string TEST_FROM_EMAIL = EnvVariable.Load("EMAIL_NET_FROM_EMAIL");
        static readonly string TEST_API_KEY = EnvVariable.Load("EMAIL_NET_SENDGRID_API_KEY");

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
            var channel = new SendgridEmailDeliveryChannel(new SendgridEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
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
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void SendEmailWithBase64Attachement()
        {
            // arrange
            var channel = new SendgridEmailDeliveryChannel(new SendgridEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
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
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void SendEmailWithFilePathAttachement()
        {
            // arrange
            var channel = new SendgridEmailDeliveryChannel(new SendgridEmailDeliveryChannelOptions()
            {
                ApiKey = TEST_API_KEY,
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
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }
    }
}