namespace Email.NET.Test.EDP
{
    using Email.NET.EDP;
    using Email.NET.EDP.Smtp;
    using Email.NET.Exceptions;
    using System;
    using System.IO;
using System.Linq;
    using Xunit;
    using Xunit.Sdk;

    public class SmtpEmailDeliveryProviderShould : IDisposable
    {
        private readonly string tempOutDirectory;

        public SmtpEmailDeliveryProviderShould()
        {
            tempOutDirectory = Path.Combine(Path.GetTempPath(), "Email.NET");
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
            SmtpEmailDeliveryProviderOptions? options = null;

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var edp = new SmtpEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptionsIsNull()
        {
            // arrange
            var options = new SmtpEmailDeliveryProviderOptions();

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<EmailServiceOptions>>(() =>
            {
                // act
                var edp = new SmtpEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptions_HostIsNull()
        {
            // arrange
            var options = new SmtpEmailDeliveryProviderOptions()
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
                var edp = new SmtpEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void ThorwIfOptionsNotValid_SmtpOptions_PortIsZero()
        {
            // arrange
            var options = new SmtpEmailDeliveryProviderOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    Host = "smtp.emil.net",
                    Port = 0,
                }
            };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<SmtpOptions>>(() =>
            {
                // act
                var edp = new SmtpEmailDeliveryProvider(options);
            });
        }

        [Fact]
        public void CreateMailMessageFromMessage()
        {
            // arrange
            var edp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
                }
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
            Assert.Equal(message.From.Address, mailMessage.From?.Address);
            Assert.Equal(message.From.DisplayName, mailMessage.From?.DisplayName);
            Assert.Equal(message.ReplyTo.First().Address, mailMessage.ReplyToList.First().Address);
            Assert.Equal(message.ReplyTo.First().DisplayName, mailMessage.ReplyToList.First().DisplayName);
            Assert.Equal(message.To.First().Address, mailMessage.To.First().Address);
            Assert.Equal(message.To.First().DisplayName, mailMessage.To.First().DisplayName);
            Assert.Equal(message.Subject, mailMessage.Subject);
            Assert.Equal(message.PlainTextBody, mailMessage.Body);
            Assert.False(mailMessage.IsBodyHtml);
            Assert.Equal("text/html", mailMessage.AlternateViews.First().ContentType.MediaType);
            Assert.Equal(message.Charset, mailMessage.BodyEncoding?.BodyName);
            Assert.Equal(message.Charset, mailMessage.SubjectEncoding?.BodyName);
            Assert.Equal(message.Bcc.First().Address, mailMessage.Bcc.First().Address);
            Assert.Equal(message.Bcc.First().DisplayName, mailMessage.Bcc.First().DisplayName);
            Assert.Equal(message.Cc.First().Address, mailMessage.CC.First().Address);
            Assert.Equal(message.Cc.First().DisplayName, mailMessage.CC.First().DisplayName);
            Assert.Equal(message.Headers.First().Value, mailMessage.Headers["key"]);
        }

        [Fact]
        public void CreateMailMessageFromMessage_WithHtmlContnetOnly()
        {
            // arrange
            var edp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
                }
            });

            var message = Message.Compose()
                .From("from@email.net")
                .ReplyTo("replayto@email.net")
                .To("to@email.net")
                .WithSubject("test subject")
                .WithHtmlContent("<p>this is a test</p>")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net")
                .WithCc("cc@email.net")
                .WithHeader("key", "value")
                .Build();

            // act
            var mailMessage = edp.CreateMessage(message);

            // assert
            Assert.Equal(message.From.Address, mailMessage.From?.Address);
            Assert.Equal(message.From.DisplayName, mailMessage.From?.DisplayName);
            Assert.Equal(message.ReplyTo.First().Address, mailMessage.ReplyToList.First().Address);
            Assert.Equal(message.ReplyTo.First().DisplayName, mailMessage.ReplyToList.First().DisplayName);
            Assert.Equal(message.To.First().Address, mailMessage.To.First().Address);
            Assert.Equal(message.To.First().DisplayName, mailMessage.To.First().DisplayName);
            Assert.Equal(message.Subject, mailMessage.Subject);
            Assert.Equal(message.HtmlBody, mailMessage.Body);
            Assert.True(mailMessage.IsBodyHtml);
            Assert.Empty(mailMessage.AlternateViews);
            Assert.Equal(message.Charset, mailMessage.BodyEncoding?.BodyName);
            Assert.Equal(message.Charset, mailMessage.SubjectEncoding?.BodyName);
            Assert.Equal(message.Bcc.First().Address, mailMessage.Bcc.First().Address);
            Assert.Equal(message.Bcc.First().DisplayName, mailMessage.Bcc.First().DisplayName);
            Assert.Equal(message.Cc.First().Address, mailMessage.CC.First().Address);
            Assert.Equal(message.Cc.First().DisplayName, mailMessage.CC.First().DisplayName);
            Assert.Equal(message.Headers.First().Value, mailMessage.Headers["key"]);
        }

        [Fact]
        public void CreateMailMessageFromMessage_WithPlainTextContnetOnly()
        {
            // arrange
            var edp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
                }
            });

            var message = Message.Compose()
                .From("from@email.net")
                .ReplyTo("replayto@email.net")
                .To("to@email.net")
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net")
                .WithCc("cc@email.net")
                .WithHeader("key", "value")
                .Build();

            // act
            var mailMessage = edp.CreateMessage(message);

            // assert
            Assert.Equal(message.From.Address, mailMessage.From?.Address);
            Assert.Equal(message.From.DisplayName, mailMessage.From?.DisplayName);
            Assert.Equal(message.ReplyTo.First().Address, mailMessage.ReplyToList.First().Address);
            Assert.Equal(message.ReplyTo.First().DisplayName, mailMessage.ReplyToList.First().DisplayName);
            Assert.Equal(message.To.First().Address, mailMessage.To.First().Address);
            Assert.Equal(message.To.First().DisplayName, mailMessage.To.First().DisplayName);
            Assert.Equal(message.Subject, mailMessage.Subject);
            Assert.Equal(message.PlainTextBody, mailMessage.Body);
            Assert.False(mailMessage.IsBodyHtml);
            Assert.Empty(mailMessage.AlternateViews);
            Assert.Equal(message.Charset, mailMessage.BodyEncoding?.BodyName);
            Assert.Equal(message.Charset, mailMessage.SubjectEncoding?.BodyName);
            Assert.Equal(message.Bcc.First().Address, mailMessage.Bcc.First().Address);
            Assert.Equal(message.Bcc.First().DisplayName, mailMessage.Bcc.First().DisplayName);
            Assert.Equal(message.Cc.First().Address, mailMessage.CC.First().Address);
            Assert.Equal(message.Cc.First().DisplayName, mailMessage.CC.First().DisplayName);
            Assert.Equal(message.Headers.First().Value, mailMessage.Headers["key"]);
        }

        [Fact]
        public void SendEmail()
        {
            // arrange
            var edp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
                }
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
            var result = edp.Send(message);
            var files = Directory.EnumerateFiles(tempOutDirectory, "*.eml");

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(files);
        }

        [Fact]
        public void SendEmailUsingCustomSmtpOptions()
        {
            // arrange
            var edp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    Host = "email.smtp.net",
                    Port = 25,
                }
            });

            var message = Message.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithSubject("test subject")
                .WithPlainTextContent("this is a test")
                .Build();

            // act
            var result = edp.Send(message, new EdpData(EdpData.Keys.SmtpOptions, new SmtpOptions
            {
                PickupDirectoryLocation = tempOutDirectory,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
            }));

            var files = Directory.EnumerateFiles(tempOutDirectory, "*.eml");

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(files);
        }

        [Fact]
        public void SendEmailWithBase64Attachement()
        {
            // arrange
            var edp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
                }
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
                .IncludeAttachment(new Base64Attachement(@"test_file.txt", MockData.TestFileBase64Value))
                .Build();

            // act
            var result = edp.Send(message);
            var files = Directory.EnumerateFiles(tempOutDirectory, "*.eml");

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(files);
        }

        [Fact]
        public void SendEmailWithFilePathAttachement()
        {
            // arrange
            var edp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions()
            {
                SmtpOptions = new SmtpOptions
                {
                    PickupDirectoryLocation = tempOutDirectory,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
                }
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
                .IncludeAttachment(new FilePathAttachment(MockData.TestFilePath))
                .Build();

            // act
            var result = edp.Send(message);
            var files = Directory.EnumerateFiles(tempOutDirectory, "*.eml");

            // assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(files);
        }
    }
}
