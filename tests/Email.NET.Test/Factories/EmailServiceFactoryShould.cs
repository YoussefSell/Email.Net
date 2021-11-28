namespace Email.NET.Test.Factories
{
    using Email.NET.EDP;
    using Email.NET.Exceptions;
    using Email.NET.Factories;
    using Moq;
    using System.Net.Mail;
    using Xunit;

    public class EmailServiceFactoryShould
    {
        private const string _edp1_name = "mock1_edp";
        private readonly IEmailDeliveryProvider _edp1;

        public EmailServiceFactoryShould()
        {
            var edpMock1 = new Mock<IEmailDeliveryProvider>();
            edpMock1.Setup(e => e.Name).Returns(_edp1_name);
            edpMock1.Setup(e => e.Send(It.IsAny<Message>())).Returns(EmailSendingResult.Success(_edp1_name));
            _edp1 = edpMock1.Object;
        }

        [Fact]
        public void CreateEmailServiceWithOptionsAndSmtpEdp()
        {
            // arrange
            var factorty = EmailServiceFactory.Instance;
            var defaultEmail = new MailAddress("test@email.net");

            // act
            var service = factorty
                .UseOptions(options =>
                {
                    options.PauseSending = false;
                    options.DefaultFrom = defaultEmail;
                    options.DefaultEmailDeliveryProvider = _edp1_name;
                })
                .UseEDP(_edp1)
                .Create() as EmailService;

            // assert
            if (service is not null)
            {
                Assert.Single(service.Edps);
                Assert.Equal(_edp1_name, service.DefaultEdp.Name);
                Assert.Equal(defaultEmail, service.Options.DefaultFrom);
            }
        }

        [Fact]
        public void ThrowIfOptionsNotValid()
        {
            // arrange
            var factorty = EmailServiceFactory.Instance;
            var defaultEmail = new MailAddress("test@email.net");

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<EmailServiceOptions>>(() =>
            {
                // act
                factorty
                    .UseOptions(options =>
                    {
                        options.PauseSending = false;
                        options.DefaultFrom = defaultEmail;
                        options.DefaultEmailDeliveryProvider = null;
                    });
            });
        }
    }
}
