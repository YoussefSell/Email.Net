namespace Email.NET.Test.Messages
{
    using Email.NET.Factories;
    using System;
    using System.Linq;
    using System.Net.Mail;
    using Xunit;

    /// <summary>
    /// the test class for the <see cref="MessageComposer"/>
    /// </summary>
    public class MessageComposerShould
    {
        [Fact]
        public void ThrowExceptionIfNoContent()
        {
            // arrange
            var composser = Message.Compose();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => composser.Build());
        }

        [Fact]
        public void ThrowExceptionIfNoToEmail()
        {
            // arrange
            var composser = Message.Compose()
                .Content(new PlainTextContent());

            // act

            // assert
            Assert.Throws<ArgumentException>(() => composser.Build());
        }

        [Fact]
        public void CreateMessageWithToFromString()
        {
            // arrange
            var composser = Message.Compose();
            var expected = "example@email.net";

            // act
            var message = composser
                .Content(new PlainTextContent())
                .To("example@email.net")
                .Build();

            // assert
            Assert.Equal(1, message.To.Count);
            Assert.Equal(expected, message.To.First().Address);
        }

        [Fact]
        public void CreateMessageWithToHasNameFromString()
        {
            // arrange
            var composser = Message.Compose();
            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .Content(new PlainTextContent())
                .To("example@email.net", "user")
                .Build();

            // assert
            Assert.Equal(1, message.To.Count);
            Assert.Equal(expectedEmail, message.To.First().Address);
            Assert.Equal(expectedName, message.To.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleToFromString()
        {
            // arrange 
            var composser = Message.Compose();
            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .Content(new PlainTextContent())
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
            var composser = Message.Compose();
            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .Content(new PlainTextContent())
                .To("example1@email.net, example2@email.net, example3@email.net", delimiter: ',')
                .Build();

            // assert
            Assert.Equal(3, message.To.Count);
            Assert.Equal(expected1, message.To.First().Address);
            Assert.Equal(expected2, message.To.Skip(1).First().Address);
            Assert.Equal(expected3, message.To.Skip(2).First().Address);
        }

        [Fact]
        public void CreateMessageWithToFromMailAddress()
        {
            // arrange
            var composser = Message.Compose();
            var expected = "example@email.net";

            // act
            var message = composser
                .Content(new PlainTextContent())
                .To(new MailAddress("example@email.net"))
                .Build();

            // assert
            Assert.Equal(1, message.To.Count);
            Assert.Equal(expected, message.To.First().Address);
        }

        [Fact]
        public void CreateMessageWithToHasNameFromMailAddress()
        {
            // arrange
            var composser = Message.Compose();
            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .Content(new PlainTextContent())
                .To(new MailAddress("example@email.net", "user"))
                .Build();

            // assert
            Assert.Equal(1, message.To.Count);
            Assert.Equal(expectedEmail, message.To.First().Address);
            Assert.Equal(expectedName, message.To.First().DisplayName);
        }

        [Fact]
        public void CreateMessageWithMultipleToFromMailAddress()
        {
            // arrange 
            var composser = Message.Compose();
            var expected1 = "example1@email.net";
            var expected2 = "example2@email.net";
            var expected3 = "example3@email.net";

            // act
            var message = composser
                .Content(new PlainTextContent())
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

        [Fact]
        public void CreateMessageWithFrom_FromString()
        {
            // arrange
            var composser = Message.Compose()
                .Content(new PlainTextContent())
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
            var composser = Message.Compose()
                .Content(new PlainTextContent())
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
            var composser = Message.Compose()
                .Content(new PlainTextContent())
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
            var composser = Message.Compose()
                .Content(new PlainTextContent())
                .To("test@email.net");

            var expectedEmail = "example@email.net";
            var expectedName = "user";

            // act
            var message = composser
                .From(new MailAddress("example@email.net", "user"))
                .Build();

            // assert
            Assert.Equal(1, message.To.Count);
            Assert.Equal(expectedEmail, message.From.Address);
            Assert.Equal(expectedName, message.From.DisplayName);
        }
    }
}
