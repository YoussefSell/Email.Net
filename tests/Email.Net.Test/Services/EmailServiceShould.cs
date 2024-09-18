namespace Email.Net.Test.Services
{
    using Email.Net.Channel;
    using Email.Net.Exceptions;
    using NSubstitute;
    using System;
    using Xunit;

    public class EmailServiceShould
    {
        private const string _channel1_name = "mock1_channel";
        private const string _channel2_name = "mock2_channel";
        private readonly IEmailDeliveryChannel _channel1;
        private readonly IEmailDeliveryChannel _channel2;

        public EmailServiceShould()
        {
            _channel1 = Substitute.For<IEmailDeliveryChannel>();
            _channel1.Name.Returns(_channel1_name);
            _channel1.Send(message: Arg.Any<EmailMessage>()).Returns(EmailSendingResult.Success(_channel1_name));

            _channel2 = Substitute.For<IEmailDeliveryChannel>();
            _channel2.Name.Returns(_channel2_name);
            _channel2.Send(message: Arg.Any<EmailMessage>()).Returns(EmailSendingResult.Success(_channel2_name));
        }

        [Fact]
        public void ThorwIfEmailDeliveryChannelsNull()
        {
            // arrange
            var options = new EmailServiceOptions();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new EmailService(null, options);
            });
        }

        [Fact]
        public void ThorwIfEmailDeliveryChannelsEmpty()
        {
            // arrange
            var options = new EmailServiceOptions();
            var channels = Array.Empty<IEmailDeliveryChannel>();

            // act

            // assert
            Assert.Throws<ArgumentException>(() =>
            {
                var service = new EmailService(channels, options);
            });
        }

        [Fact]
        public void ThorwIfOptionsAreNull()
        {
            // arrange
            EmailServiceOptions? options = null;

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new EmailService(new[] { _channel1 }, options);
            });
        }

        [Fact]
        public void ThorwIfOptionsAreNotValid()
        {
            // arrange
            var options = new EmailServiceOptions();

            // act

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<EmailServiceOptions>>(() =>
            {
                var service = new EmailService(new[] { _channel1 }, options);
            });
        }

        [Fact]
        public void ThorwIfDefaultChannelNotFound()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = "not_exist_channel" };

            // act

            // assert
            Assert.Throws<EmailDeliveryChannelNotFoundException>(() =>
            {
                var service = new EmailService(new[] { _channel1 }, options);
            });
        }

        [Fact]
        public void SendMessageWithDefaultChannel()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name };
            var service = new EmailService(new[] { _channel1, _channel2 }, options);
            var message = EmailMessage.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // act
            var result = service.Send(message);

            // assert
            Assert.Equal(_channel1_name, result.ChannelName);
        }

        [Fact]
        public void SendMessageWithChannelOfGivenName()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name };
            var service = new EmailService(new[] { _channel1, _channel2 }, options);
            var message = EmailMessage.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // act
            var result = service.Send(message, _channel2_name);

            // assert
            Assert.Equal(_channel2_name, result.ChannelName);
        }

        [Fact]
        public void ThrowIfGivenChannelNameForSendMessageNotExist()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name };
            var service = new EmailService(new[] { _channel1 }, options);
            var message = EmailMessage.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // assert
            Assert.Throws<EmailDeliveryChannelNotFoundException>(() =>
            {
                // act
                var result = service.Send(message, _channel2_name);
            });
        }

        [Fact]
        public void ThrowIfGivenChannelNameForSendMessageIsNull()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name };
            var service = new EmailService(new[] { _channel1 }, options);
            var message = EmailMessage.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var result = service.Send(message, channel_name: null);
            });
        }

        [Fact]
        public void SendMessageWithTheGivenChannelInstance()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name };
            var service = new EmailService(new[] { _channel1 }, options);
            var message = EmailMessage.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // act
            var result = service.Send(message, _channel2);

            // assert
            Assert.Equal(_channel2_name, result.ChannelName);
        }

        [Fact]
        public void ThrowIfGivenChannelInstanceForSendIsNull()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name };
            var service = new EmailService(new[] { _channel1 }, options);
            var message = EmailMessage.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var result = service.Send(message, channel: null);
            });
        }

        [Fact]
        public void ThrowOnSendWhenMessageInstanceIsNull()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name };
            var service = new EmailService(new[] { _channel1 }, options);

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var result = service.Send(null);
            });
        }

        [Fact]
        public void UseDefaultSenderEmailSetInEmailServiceOptions()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name, DefaultFrom = new System.Net.Mail.MailAddress("default@email.net") };
            var service = new EmailService(new[] { _channel1 }, options);
            var message = EmailMessage.Compose()
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // act
            var result = service.Send(message);

            // assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void ThrowIfSenderEmailIsNotSetNeitherInOptionsOrMessage()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name };
            var service = new EmailService(new[] { _channel1 }, options);

            var message = EmailMessage.Compose()
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // assert
            Assert.Throws<ArgumentException>(() =>
            {
                // act
                var result = service.Send(message);
            });
        }

        [Fact]
        public void NotSendIfSendingIsPaused()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryChannel = _channel1_name, PauseSending = true };
            var service = new EmailService(new[] { _channel1 }, options);
            var message = EmailMessage.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .WithPlainTextContent("test content")
                .Build();

            // act
            var result = service.Send(message);

            // assert
            Assert.True(result.GetMetaData<bool>(EmailSendingResult.MetaDataKeys.SendingPaused));
        }
    }
}
