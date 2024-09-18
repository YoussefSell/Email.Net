namespace Email.Net.Test.Factories
{
    using Email.Net.Channel;
    using Email.Net.Factories;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using Xunit;

    /// <summary>
    /// the test class for the <see cref="EmailMessageComposer"/>
    /// </summary>
    public class MessageComposerShould
    {
        [Fact]
        public void CreateMessageWithAllProps()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .To("to@email.net", "to")
                .From("from@email.net", "from")
                .ReplyTo("replayto@email.net", "replayto")
                .WithSubject("test subject")
                .WithHtmlContent("<p>test content</p>")
                .WithPlainTextContent("test content")
                .SetCharsetTo("utf-8")
                .WithBcc("bcc@email.net", "bcc")
                .WithCc("cc@email.net", "cc")
                .WithHeader("key", "value")
                .UseCustomApiKey("KEY-1")
                .IncludeAttachment(new Base64Attachement(@"test_file.txt", MockData.TestFileBase64Value));

            // act
            var message = composser.Build();

            // assert
            Assert.Equal("test subject", message.Subject);
            Assert.Equal("utf-8", message.Charset);

            Assert.Equal("<p>test content</p>", message.HtmlBody);

            Assert.Equal("test content", message.PlainTextBody);

            Assert.Single(message.To);
            Assert.Equal("to", message.To.First().DisplayName);
            Assert.Equal("to@email.net", message.To.First().Address);

            Assert.Equal("from", message.From.DisplayName);
            Assert.Equal("from@email.net", message.From.Address);

            Assert.Single(message.Bcc);
            Assert.Equal("bcc", message.Bcc.First().DisplayName);
            Assert.Equal("bcc@email.net", message.Bcc.First().Address);

            Assert.Single(message.Cc);
            Assert.Equal("cc", message.Cc.First().DisplayName);
            Assert.Equal("cc@email.net", message.Cc.First().Address);

            Assert.Single(message.Headers);
            Assert.Equal("key", message.Headers.First().Key);
            Assert.Equal("value", message.Headers.First().Value);

            Assert.Single(message.Attachments);
            Assert.Equal("test_file.txt", message.Attachments.First().FileName);

            Assert.Single(message.ChannelData);
            Assert.Equal(ChannelData.Keys.ApiKey, message.ChannelData.First().Key);
            Assert.Equal("KEY-1", message.ChannelData.First().Value);
        }

        #region Message "Content" value test

        [Fact]
        public void AddPlainTextContent()
        {
            // arrange
            var composser = EmailMessage.Compose().To("to@email.net");

            // act
            var message = composser
                .WithPlainTextContent("test content")
                .Build();

            // assert
            Assert.Equal("test content", message.PlainTextBody);
            Assert.Null(message.HtmlBody);
        }

        [Fact]
        public void AddHtmlContent()
        {
            // arrange
            var composser = EmailMessage.Compose().To("to@email.net");

            // act
            var message = composser
                .WithHtmlContent("<p>test content</p>")
                .Build();

            // assert
            Assert.Equal("<p>test content</p>", message.HtmlBody);
            Assert.Null(message.PlainTextBody);
        }

        #endregion

        #region Message "From" value tests

        [Fact]
        public void CreateMessageWithFrom_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("test@email.net");

            var expected = "example@email.net";

            // act
            var message = composser
                .From("example@email.net")
                .Build();

            // assert
            Assert.Equal(expected, message.From.Address);
        }

        [Fact]
        public void CreateMessageWithFromHasName_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("test@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .From("example@email.net", "user")
                .Build();

            // assert
            Assert.Equal(expectedEmail, message.From.Address);
            Assert.Equal(expectedName, message.From.DisplayName);
        }

        [Fact]
        public void CreateMessageWithFrom_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To(new MailAddress("test@email.net"));

            var expected = "example@email.net";

            // act
            var message = composser
                .From(new MailAddress("example@email.net"))
                .Build();

            // assert
            Assert.Equal(expected, message.From.Address);
        }

        [Fact]
        public void CreateMessageWithFromHasName_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("test@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .From(new MailAddress("example@email.net", "user"))
                .Build();

            // assert
            Assert.Single(message.To);
            Assert.Equal(expectedEmail, message.From.Address);
            Assert.Equal(expectedName, message.From.DisplayName);
        }

        #endregion

        #region Message "To" value tests

        [Fact]
        public void ThrowExceptionIfNoToEmail()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content");

            // act

            // assert
            Assert.Throws<ArgumentException>(() => composser.Build());
        }

        [Fact]
        public void CreateMessageWithTo_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose().WithPlainTextContent("test content");
            var expected = "example@email.net";

            // act
            var message = composser
                .To("example@email.net")
                .Build();

            // assert
            Assert.Single(message.To);
            Assert.Equal(expected, message.To.First().Address);
        }

        [Fact]
        public void CreateMessageWithToHasName_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose().WithPlainTextContent("test content");
            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .To("example@email.net", "user")
                .Build();

            // assert
            Assert.Single(message.To);
            Assert.Equal(expectedEmail, message.To.First().Address);
            Assert.Equal(expectedName, message.To.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleTo_FromString()
        {
            // arrange 
            var composser = EmailMessage.Compose().WithPlainTextContent("test content");
            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .To("example1@email.net; example2@email.net; example3@email.net")
                .Build();

            // assert
            Assert.Equal(3, message.To.Count);
            Assert.Equal(expected1, message.To.First().Address);
            Assert.Equal(expected2, message.To.Skip(1).First().Address);
            Assert.Equal(expected3, message.To.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithMultipleToFromStringWithCustomSeparator()
        {
            // arrange 
            var composser = EmailMessage.Compose().WithPlainTextContent("test content");
            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .To("example1@email.net, example2@email.net, example3@email.net", delimiter: ',')
                .Build();

            // assert
            Assert.Equal(3, message.To.Count);
            Assert.Equal(expected1, message.To.First().Address);
            Assert.Equal(expected2, message.To.Skip(1).First().Address);
            Assert.Equal(expected3, message.To.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithTo_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose().WithPlainTextContent("test content");
            var expected = "example@email.net";

            // act
            var message = composser
                .To(new MailAddress("example@email.net"))
                .Build();

            // assert
            Assert.Single(message.To);
            Assert.Equal(expected, message.To.First().Address);
        }

        [Fact]
        public void CreateMessageWithToHasName_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose().WithPlainTextContent("test content");
            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .To(new MailAddress("example@email.net", "user"))
                .Build();

            // assert
            Assert.Single(message.To);
            Assert.Equal(expectedEmail, message.To.First().Address);
            Assert.Equal(expectedName, message.To.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleTo_FromMailAddress()
        {
            // arrange 
            var composser = EmailMessage.Compose().WithPlainTextContent("test content");
            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .To(new[] {
                    new MailAddress("example1@email.net"),
                    new MailAddress("example2@email.net"),
                    new MailAddress("example3@email.net"),
                })
                .Build();

            // assert
            Assert.Equal(3, message.To.Count);
            Assert.Equal(expected1, message.To.First().Address);
            Assert.Equal(expected2, message.To.Skip(1).First().Address);
            Assert.Equal(expected3, message.To.Skip(2).First().Address);
        }

        #endregion

        #region Message "ReplyTo" value tests

        [Fact]
        public void CreateMessageWithReplyTo_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected = "example@email.net";

            // act
            var message = composser
                .ReplyTo("example@email.net")
                .Build();

            // assert
            Assert.Single(message.ReplyTo);
            Assert.Equal(expected, message.ReplyTo.First().Address);
        }

        [Fact]
        public void CreateMessageWithReplyToHasName_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .ReplyTo("example@email.net", "user")
                .Build();

            // assert
            Assert.Single(message.ReplyTo);
            Assert.Equal(expectedEmail, message.ReplyTo.First().Address);
            Assert.Equal(expectedName, message.ReplyTo.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleReplyTo_FromString()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .ReplyTo("example1@email.net; example2@email.net; example3@email.net")
                .Build();

            // assert
            Assert.Equal(3, message.ReplyTo.Count);
            Assert.Equal(expected1, message.ReplyTo.First().Address);
            Assert.Equal(expected2, message.ReplyTo.Skip(1).First().Address);
            Assert.Equal(expected3, message.ReplyTo.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithMultipleReplyToFromStringWithCusReplyTomSeparaReplyTor()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .ReplyTo("example1@email.net, example2@email.net, example3@email.net", delimiter: ',')
                .Build();

            // assert
            Assert.Equal(3, message.ReplyTo.Count);
            Assert.Equal(expected1, message.ReplyTo.First().Address);
            Assert.Equal(expected2, message.ReplyTo.Skip(1).First().Address);
            Assert.Equal(expected3, message.ReplyTo.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithReplyTo_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected = "example@email.net";

            // act
            var message = composser
                .ReplyTo(new MailAddress("example@email.net"))
                .Build();

            // assert
            Assert.Single(message.ReplyTo);
            Assert.Equal(expected, message.ReplyTo.First().Address);
        }

        [Fact]
        public void CreateMessageWithReplyToHasName_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .ReplyTo(new MailAddress("example@email.net", "user"))
                .Build();

            // assert
            Assert.Single(message.ReplyTo);
            Assert.Equal(expectedEmail, message.ReplyTo.First().Address);
            Assert.Equal(expectedName, message.ReplyTo.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleReplyTo_FromMailAddress()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .ReplyTo(new[] {
                    new MailAddress("example1@email.net"),
                    new MailAddress("example2@email.net"),
                    new MailAddress("example3@email.net"),
                })
                .Build();

            // assert
            Assert.Equal(3, message.ReplyTo.Count);
            Assert.Equal(expected1, message.ReplyTo.First().Address);
            Assert.Equal(expected2, message.ReplyTo.Skip(1).First().Address);
            Assert.Equal(expected3, message.ReplyTo.Skip(2).First().Address);
        }

        #endregion

        #region Message "Bcc" value tests

        [Fact]
        public void CreateMessageWithBcc_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected = "example@email.net";

            // act
            var message = composser
                .WithBcc("example@email.net")
                .Build();

            // assert
            Assert.Single(message.Bcc);
            Assert.Equal(expected, message.Bcc.First().Address);
        }

        [Fact]
        public void CreateMessageWithBccHasName_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .WithBcc("example@email.net", "user")
                .Build();

            // assert
            Assert.Single(message.Bcc);
            Assert.Equal(expectedEmail, message.Bcc.First().Address);
            Assert.Equal(expectedName, message.Bcc.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleBcc_FromString()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .WithBcc("example1@email.net; example2@email.net; example3@email.net")
                .Build();

            // assert
            Assert.Equal(3, message.Bcc.Count);
            Assert.Equal(expected1, message.Bcc.First().Address);
            Assert.Equal(expected2, message.Bcc.Skip(1).First().Address);
            Assert.Equal(expected3, message.Bcc.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithMultipleBccFromStringWithCusBccmSeparaBccr()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .WithBcc("example1@email.net, example2@email.net, example3@email.net", delimiter: ',')
                .Build();

            // assert
            Assert.Equal(3, message.Bcc.Count);
            Assert.Equal(expected1, message.Bcc.First().Address);
            Assert.Equal(expected2, message.Bcc.Skip(1).First().Address);
            Assert.Equal(expected3, message.Bcc.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithBcc_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected = "example@email.net";

            // act
            var message = composser
                .WithBcc(new MailAddress("example@email.net"))
                .Build();

            // assert
            Assert.Single(message.Bcc);
            Assert.Equal(expected, message.Bcc.First().Address);
        }

        [Fact]
        public void CreateMessageWithBccHasName_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .WithBcc(new MailAddress("example@email.net", "user"))
                .Build();

            // assert
            Assert.Single(message.Bcc);
            Assert.Equal(expectedEmail, message.Bcc.First().Address);
            Assert.Equal(expectedName, message.Bcc.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleBcc_FromMailAddress()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .WithBcc(new[] {
                    new MailAddress("example1@email.net"),
                    new MailAddress("example2@email.net"),
                    new MailAddress("example3@email.net"),
                })
                .Build();

            // assert
            Assert.Equal(3, message.Bcc.Count);
            Assert.Equal(expected1, message.Bcc.First().Address);
            Assert.Equal(expected2, message.Bcc.Skip(1).First().Address);
            Assert.Equal(expected3, message.Bcc.Skip(2).First().Address);
        }

        #endregion

        #region Message "Cc" value tests

        [Fact]
        public void CreateMessageWithCc_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected = "example@email.net";

            // act
            var message = composser
                .WithCc("example@email.net")
                .Build();

            // assert
            Assert.Single(message.Cc);
            Assert.Equal(expected, message.Cc.First().Address);
        }

        [Fact]
        public void CreateMessageWithCcHasName_FromString()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .WithCc("example@email.net", "user")
                .Build();

            // assert
            Assert.Single(message.Cc);
            Assert.Equal(expectedEmail, message.Cc.First().Address);
            Assert.Equal(expectedName, message.Cc.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleCc_FromString()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .WithCc("example1@email.net; example2@email.net; example3@email.net")
                .Build();

            // assert
            Assert.Equal(3, message.Cc.Count);
            Assert.Equal(expected1, message.Cc.First().Address);
            Assert.Equal(expected2, message.Cc.Skip(1).First().Address);
            Assert.Equal(expected3, message.Cc.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithMultipleCcFromStringWithCusCcmSeparaCcr()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .WithCc("example1@email.net, example2@email.net, example3@email.net", delimiter: ',')
                .Build();

            // assert
            Assert.Equal(3, message.Cc.Count);
            Assert.Equal(expected1, message.Cc.First().Address);
            Assert.Equal(expected2, message.Cc.Skip(1).First().Address);
            Assert.Equal(expected3, message.Cc.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithCc_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected = "example@email.net";

            // act
            var message = composser
                .WithCc(new MailAddress("example@email.net"))
                .Build();

            // assert
            Assert.Single(message.Cc);
            Assert.Equal(expected, message.Cc.First().Address);
        }

        [Fact]
        public void CreateMessageWithCcHasName_FromMailAddress()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .WithCc(new MailAddress("example@email.net", "user"))
                .Build();

            // assert
            Assert.Single(message.Cc);
            Assert.Equal(expectedEmail, message.Cc.First().Address);
            Assert.Equal(expectedName, message.Cc.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleCc_FromMailAddress()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .WithCc(new[] {
                    new MailAddress("example1@email.net"),
                    new MailAddress("example2@email.net"),
                    new MailAddress("example3@email.net"),
                })
                .Build();

            // assert
            Assert.Equal(3, message.Cc.Count);
            Assert.Equal(expected1, message.Cc.First().Address);
            Assert.Equal(expected2, message.Cc.Skip(1).First().Address);
            Assert.Equal(expected3, message.Cc.Skip(2).First().Address);
        }

        #endregion

        #region Message Attachments tests

        [Fact]
        public void AddByteArrayAttachment()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var attachment = new ByteArrayAttachment(@"test_file.txt", MockData.TestFileAsByteArray());

            // act
            var message = composser
                .IncludeAttachment(attachment)
                .Build();

            // assert
            Assert.Single(message.Attachments);
            Assert.Equal(attachment, message.Attachments.First());
        }

        [Fact]
        public void AddBase64Attachment()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var attachment = new Base64Attachement(@"test_file.txt", MockData.TestFileBase64Value);

            // act
            var message = composser
                .IncludeAttachment(attachment)
                .Build();

            // assert
            Assert.Single(message.Attachments);
            Assert.Equal(attachment, message.Attachments.First());
        }

        [Fact]
        public void AddFilePathAttachment()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            var attachment = new FilePathAttachment(MockData.TestFilePath);

            // act
            var message = composser
                .IncludeAttachment(attachment)
                .Build();

            // assert
            Assert.Single(message.Attachments);
            Assert.Equal(attachment, message.Attachments.First());
        }

        [Fact]
        public void AddMultipleAttachments()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .IncludeAttachment(new Net.Attachment[] {
                    new FilePathAttachment(MockData.TestFilePath),
                    new Base64Attachement(@"test_file_1.txt", MockData.TestFileBase64Value),
                    new ByteArrayAttachment(@"test_file_2.txt", MockData.TestFileAsByteArray())
                })
                .Build();

            // assert
            Assert.Equal(3, message.Attachments.Count);
        }

        #endregion

        #region Message Headers tests

        [Fact]
        public void AddHeader()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .WithHeader("key", "value")
                .Build();

            // assert
            Assert.Single(message.Headers);
            Assert.Equal("key", message.Headers.First().Key);
            Assert.Equal("value", message.Headers.First().Value);
        }

        [Fact]
        public void AddMultipleHeaders()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .WithHeaders(new Dictionary<string, string>()
                {
                    { "key1", "value" },
                    { "key2", "value" },
                    { "key3", "value" },
                })
                .Build();

            // assert
            Assert.Equal(3, message.Headers.Count);

            Assert.Equal("key1", message.Headers.First().Key);
            Assert.Equal("value", message.Headers.First().Value);

            Assert.Equal("key2", message.Headers.Skip(1).First().Key);
            Assert.Equal("value", message.Headers.Skip(1).First().Value);

            Assert.Equal("key3", message.Headers.Skip(2).First().Key);
            Assert.Equal("value", message.Headers.Skip(2).First().Value);
        }

        [Fact]
        public void AddHeadersFromADictionaryAppendIntenalList()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .WithHeader("key0", "value0")
                .To("to@email.net");

            // act
            var message = composser
                .WithHeaders(new Dictionary<string, string>()
                {
                    { "key1", "value1" },
                    { "key2", "value2" },
                    { "key3", "value3" },
                })
                .Build();

            // assert
            Assert.Equal(4, message.Headers.Count);

            Assert.Equal("key0", message.Headers.First().Key);
            Assert.Equal("value0", message.Headers.First().Value);

            Assert.Equal("key1", message.Headers.Skip(1).First().Key);
            Assert.Equal("value1", message.Headers.Skip(1).First().Value);

            Assert.Equal("key2", message.Headers.Skip(2).First().Key);
            Assert.Equal("value2", message.Headers.Skip(2).First().Value);

            Assert.Equal("key3", message.Headers.Skip(3).First().Key);
            Assert.Equal("value3", message.Headers.Skip(3).First().Value);
        }

        #endregion

        #region Message "ChannelData" value tests

        [Fact]
        public void CreateMessageWithChannelData_FromKeyValue()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .PassChannelData("key", "value")
                .Build();

            // assert
            Assert.Single(message.ChannelData);
            Assert.Equal("key", message.ChannelData.First().Key);
            Assert.Equal("value", message.ChannelData.First().Value);
        }

        [Fact]
        public void CreateMessageWithChannelData_FromInstance()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .PassChannelData(new ChannelData("key", "value"))
                .Build();

            // assert
            Assert.Single(message.ChannelData);
            Assert.Equal("key", message.ChannelData.First().Key);
            Assert.Equal("value", message.ChannelData.First().Value);
        }

        [Fact]
        public void CreateMessageWithChannelData_FromListInstance()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .PassChannelData(new[] { new ChannelData("key1", "value"), new ChannelData("key2", "value") })
                .Build();

            // assert
            Assert.Equal(2, message.ChannelData.Count);
            Assert.Equal("key1", message.ChannelData.First().Key);
            Assert.Equal("value", message.ChannelData.First().Value);
            Assert.Equal("key2", message.ChannelData.Skip(1).First().Key);
            Assert.Equal("value", message.ChannelData.Skip(1).First().Value);
        }

        [Fact]
        public void CreateMessageWithChannelData_FromExtensionMethods()
        {
            // arrange 
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .UseCustomApiKey("API-KEY")
                .SetMailingId("messageId")
                .Build();

            // assert
            Assert.Equal(2, message.ChannelData.Count);
            Assert.Equal(ChannelData.Keys.ApiKey, message.ChannelData.First().Key);
            Assert.Equal("API-KEY", message.ChannelData.First().Value);
            Assert.Equal(ChannelData.Keys.MailingId, message.ChannelData.Skip(1).First().Key);
            Assert.Equal("messageId", message.ChannelData.Skip(1).First().Value);
        }

        #endregion

        #region Message Priority tests

        [Fact]
        public void MarkMessageWithHighPriority()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .WithHighPriority()
                .Build();

            // assert
            Assert.Equal(Priority.High, message.Priority);
        }

        [Fact]
        public void MarkMessageWithLowPriority()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .WithLowPriority()
                .Build();

            // assert
            Assert.Equal(Priority.Low, message.Priority);
        }

        [Fact]
        public void MarkMessageWithNormalPriority()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content")
                .To("to@email.net");

            // act
            var message = composser
                .WithNormalPriority()
                .Build();

            // assert
            Assert.Equal(Priority.Normal, message.Priority);
        }

        #endregion

        #region Message Addresses tests

        [Fact]
        public void SplitEmailAddressWithEmptySpaces()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content");

            // act
            var message = composser
                .To("to@email.net  ;            to2@email.net       ")
                .Build();

            // assert
            Assert.Equal(2, message.To.Count);
            Assert.Equal("to@email.net", message.To.First().Address);
            Assert.Equal("to2@email.net", message.To.Skip(1).First().Address);
        }

        [Fact]
        public void ThorwIfEmailIsInInvalidFormat()
        {
            // arrange
            var composser = EmailMessage.Compose()
                .WithPlainTextContent("test content");

            // assert
            Assert.Throws<FormatException>(() =>
            {
                // act
                var message = composser
                    .To("to@emailnet;to2email.net")
                    .Build();
            });
        }

        #endregion
    }
}