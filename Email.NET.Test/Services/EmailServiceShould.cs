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
            var smtpEdp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions());

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new EmailService(new[] { smtpEdp }, options);
            });
        }

        [Fact]
        public void ThorwIfOptionsAreNotValid()
        {
            // arrange
            var options = new EmailServiceOptions();
            var smtpEdp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions());

            // act

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<EmailServiceOptions>>(() =>
            {
                var service = new EmailService(new[] { smtpEdp }, options);
            });
        }

        [Fact]
        public void ThorwIfDefaultEdpNotFound()
        {
            // arrange
            var options = new EmailServiceOptions { DefaultEmailDeliveryProvider = "not_exist_edp" };
            var smtpEdp = new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions());

            // act

            // assert
            Assert.Throws<EmailDeliveryProviderNotFoundException>(() =>
            {
                var service = new EmailService(new[] { smtpEdp }, options);
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


    }
}
