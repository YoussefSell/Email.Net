namespace Email.NET.Test.Services
{
    using Email.NET.Exceptions;
    using Email.NET.Providers.SmtpClient;
    using Moq;
    using System;
    using Xunit;

    public class EmailServiceShould
    {
        private const string _edp1_name = "mock1_edp";
        private const string _edp2_name = "mock2_edp";
        private readonly IEmailDeliveryProvider _edp1;
        private readonly IEmailDeliveryProvider _edp2;

        public EmailServiceShould()
        {
            var edpMock1 = new Mock<IEmailDeliveryProvider>();
            edpMock1.Setup(e => e.Name).Returns(_edp1_name);
            edpMock1.Setup(e => e.Send(It.IsAny<Message>())).Returns(EmailSendingResult.Success(_edp1_name));
            _edp1 = edpMock1.Object;

            var edpMock2 = new Mock<IEmailDeliveryProvider>();
            edpMock2.Setup(e => e.Name).Returns(_edp2_name);
            edpMock2.Setup(e => e.Send(It.IsAny<Message>())).Returns(EmailSendingResult.Success(_edp2_name));
            _edp2 = edpMock2.Object;
        }

        [Fact]
        public void ThorwIfEmailDeliveryProvidersNull()
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
        public void ThorwIfEmailDeliveryProvidersEmpty()
        {
            // arrange
            var options = new EmailServiceOptions();
            var edps = Array.Empty<IEmailDeliveryProvider>();

            // act

            // assert
            Assert.Throws<ArgumentException>(() =>
            {
                var service = new EmailService(edps, options);
            });
        }

        [Fact]
        public void ThorwIfOptionsAreNull()
        {
            // arrange
            EmailServiceOptions options = null;

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new EmailService(new[] { _edp1 }, options);
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
                var service = new EmailService(new[] { _edp1 }, options);
            });
        }

        [Fact]
        public void ThorwIfDefaultEdpNotFound()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = "not_exist_edp" };

            // act

            // assert
            Assert.Throws<EmailDeliveryProviderNotFoundException>(() =>
            {
                var service = new EmailService(new[] { _edp1 }, options);
            });
        }

        [Fact]
        public void SendMessageWithDefaultProvider()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name };
            var service = new EmailService(new[] { _edp1, _edp2 }, options);
            var message = Message.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .Content(new PlainTextContent())
                .Build();

            // act
            var result = service.Send(message);

            // assert
            Assert.Equal(_edp1_name, result.EdpName);
        }

        [Fact]
        public void SendMessageWithProviderOfGivenName()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name };
            var service = new EmailService(new[] { _edp1, _edp2 }, options);
            var message = Message.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .Content(new PlainTextContent())
                .Build();

            // act
            var result = service.Send(message, _edp2_name);

            // assert
            Assert.Equal(_edp2_name, result.EdpName);
        }

        [Fact]
        public void ThrowIfGivenEdpNameForSendMessageNotExist()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name };
            var service = new EmailService(new[] { _edp1 }, options);
            var message = Message.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .Content(new PlainTextContent())
                .Build();

            // assert
            Assert.Throws<EmailDeliveryProviderNotFoundException>(() =>
            {
                // act
                var result = service.Send(message, _edp2_name);
            });
        }

        [Fact]
        public void ThrowIfGivenEdpNameForSendMessageIsNull()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name };
            var service = new EmailService(new[] { _edp1 }, options);
            var message = Message.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .Content(new PlainTextContent())
                .Build();

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var result = service.Send(message, providerName: null);
            });
        }

        [Fact]
        public void SendMessageWithTheGivenProviderInstance()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name };
            var service = new EmailService(new[] { _edp1 }, options);
            var message = Message.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .Content(new PlainTextContent())
                .Build();

            // act
            var result = service.Send(message, _edp2);

            // assert
            Assert.Equal(_edp2_name, result.EdpName);
        }

        [Fact]
        public void ThrowIfGivenEdpInstanceForSendIsNull()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name };
            var service = new EmailService(new[] { _edp1 }, options);
            var message = Message.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .Content(new PlainTextContent())
                .Build();

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // act
                var result = service.Send(message, provider: null);
            });
        }

        [Fact]
        public void ThrowOnSendWhenMessageInstanceIsNull()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name };
            var service = new EmailService(new[] { _edp1 }, options);

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
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name, DefaultFrom = new System.Net.Mail.MailAddress("default@email.net") };
            var service = new EmailService(new[] { _edp1 }, options);
            var message = Message.Compose()
                .To("to@email.net")
                .Content(new PlainTextContent())
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
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name };
            var service = new EmailService(new[] { _edp1 }, options);

            var message = Message.Compose()
                .To("to@email.net")
                .Content(new PlainTextContent())
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
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = _edp1_name, PauseSending = true };
            var service = new EmailService(new[] { _edp1 }, options);
            var message = Message.Compose()
                .From("from@email.net")
                .To("to@email.net")
                .Content(new PlainTextContent())
                .Build();

            // act
            var result = service.Send(message);

            // assert
            Assert.True(result.GetMetaData<bool>(EmailSendingResult.MetaDataKeys.SendingPaused));
        }
    }
}
