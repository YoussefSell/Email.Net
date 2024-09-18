namespace Email.Net.AmazonSES.Test
{
    using Email.Net.Channel.AmazonSES;
    using Email.Net.Exceptions;
    using System;
    using System.Linq;
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

        [Fact(Skip = "no Amazon account for testing")]
        public void SendEmail()
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
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }

        [Fact(Skip = "no Amazon account for testing")]
        public void SendEmailWithBase64Attachement()
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
                .IncludeAttachment(new Base64Attachement(@"test_file.txt", MockData.TestFileBase64Value))
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }

        [Fact(Skip = "no Amazon account for testing")]
        public void SendEmailWithFilePathAttachement()
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
                .IncludeAttachment(new FilePathAttachment(MockData.TestFilePath))
                .Build();

            // act
            var result = channel.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }
    }
}