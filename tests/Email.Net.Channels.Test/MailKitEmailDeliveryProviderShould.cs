namespace Email.Net.MailKit.Test
{
    using Email.Net.Channel;
    using Email.Net.Channel.MailKit;
    using Email.Net.Exceptions;
    using MimeKit;
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class MailKitEmailDeliveryChannelshould : IDisposable
    {
        private readonly string tempOutDirectory;

        public MailKitEmailDeliveryChannelshould()
        {
            tempOutDirectory = Path.Combine(Path.GetTempPath(), "Email.Net");
            Directory.CreateDirectory(tempOutDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(tempOutDirectory))
                Directory.Delete(tempOutDirectory, true);
        }

        [Fact]
        public void ThorwIfOptionsIsNull()
        {
            // arrange
            MailKitEmailDeliveryChannelOptions? options = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var channel = new MailKitEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptionsIsNull()
        {
            // arrange
            var options = new MailKitEmailDeliveryChannelOptions();

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<EmailServiceOptions>>(() =>
            {
                // act
                var channel = new MailKitEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptions_HostIsNull()
        {
            // arrange
            var options = new MailKitEmailDeliveryChannelOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    Host = null,
                }
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SmtpOptions>>(() =>
            {
                // act
                var channel = new MailKitEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptions_PortIsZero()
        {
            // arrange
            var options = new MailKitEmailDeliveryChannelOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    Host = "MailKit.emil.net",
                    Port = 0,
                }
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SmtpOptions>>(() =>
            {
                // act
                var channel = new MailKitEmailDeliveryChannel(options);
            });
        }

        [Fact]
        public void CreateMailMessageFromMessage()
        {
            // arrange
            var channel = new MailKitEmailDeliveryChannel(new MailKitEmailDeliveryChannelOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = MailKitDeliveryMethod.SpecifiedPickupDirectory
                }
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
            Assert.Equal(message.From.Address, ((MailboxAddress)mailMessage.From.First()).Address);
            Assert.Equal(message.From.DisplayName, ((MailboxAddress)mailMessage.From.First()).Name);
            Assert.Equal(message.ReplyTo.First().Address, ((MailboxAddress)mailMessage.ReplyTo.First()).Address);
            Assert.Equal(message.ReplyTo.First().DisplayName, ((MailboxAddress)mailMessage.ReplyTo.First()).Name);
            Assert.Equal(message.To.First().Address, ((MailboxAddress)mailMessage.To.First()).Address);
            Assert.Equal(message.To.First().DisplayName, ((MailboxAddress)mailMessage.To.First()).Name);
            Assert.Equal(message.Subject, mailMessage.Subject);
            Assert.Equal(message.Bcc.First().Address, ((MailboxAddress)mailMessage.Bcc.First()).Address);
            Assert.Equal(message.Bcc.First().DisplayName, ((MailboxAddress)mailMessage.Bcc.First()).Name);
            Assert.Equal(message.Cc.First().Address, ((MailboxAddress)mailMessage.Cc.First()).Address);
            Assert.Equal(message.Cc.First().DisplayName, ((MailboxAddress)mailMessage.Cc.First()).Name);
            Assert.Equal(message.Headers.First().Value, mailMessage.Headers["key"]);
        }

        [Fact]
        public void SendEmail()
        {
            // arrange
            var channel = new MailKitEmailDeliveryChannel(new MailKitEmailDeliveryChannelOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = MailKitDeliveryMethod.SpecifiedPickupDirectory
                }
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
            var result = channel.Send(message);
            var files = Directory.EnumerateFiles(tempOutDirectory, "*.eml");

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(files);
        }

        [Fact]
        public void SendEmailUsingCustomSmtpOptions()
        {
            // arrange
            var channel = new MailKitEmailDeliveryChannel(new MailKitEmailDeliveryChannelOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    Host = "email.MailKit.net",
                    Port = 25,
                }
            });

            var message = EmailMessage.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .PassChannelData(new ChannelData(ChannelData.Keys.SmtpOptions, new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = MailKitDeliveryMethod.SpecifiedPickupDirectory
                }))
                .Build();

            // act
            var result = channel.Send(message);

            var files = Directory.EnumerateFiles(tempOutDirectory, "*.eml");

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(files);
        }

        [Fact]
        public void SendEmailWithBase64Attachement()
        {
            // arrange
            var channel = new MailKitEmailDeliveryChannel(new MailKitEmailDeliveryChannelOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = MailKitDeliveryMethod.SpecifiedPickupDirectory
                }
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
                .IncludeAttachment(new Base64Attachement(@"test_file.txt", MockData.TestFileBase64Value))
                .Build();

            // act
            var result = channel.Send(message);
            var files = Directory.EnumerateFiles(tempOutDirectory, "*.eml");

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(files);
        }

        [Fact]
        public void SendEmailWithFilePathAttachement()
        {
            // arrange
            var channel = new MailKitEmailDeliveryChannel(new MailKitEmailDeliveryChannelOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = MailKitDeliveryMethod.SpecifiedPickupDirectory
                }
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
                .IncludeAttachment(new FilePathAttachment(MockData.TestFilePath))
                .Build();

            // act
            var result = channel.Send(message);
            var files = Directory.EnumerateFiles(tempOutDirectory, "*.eml");

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(files);
        }
    }
}