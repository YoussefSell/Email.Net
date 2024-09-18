namespace Email.Net.Test.Factories
{
    using Email.Net.Channel;
    using Email.Net.Exceptions;
    using Email.Net.Factories;
    using NSubstitute;
    using System.Net.Mail;
    using Xunit;

    public class EmailServiceFactoryShould
    {
        private const string _channel1_name = "mock1_channel";
        private readonly IEmailDeliveryChannel _channel1;

        public EmailServiceFactoryShould()
        {
            _channel1 = Substitute.For<IEmailDeliveryChannel>();
            _channel1.Name.Returns(_channel1_name);
            _channel1.Send(message: Arg.Any<EmailMessage>()).Returns(EmailSendingResult.Success(_channel1_name));
        }

        [Fact]
        public void CreateEmailServiceWithOptionsAndSmtpChannel()
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
                    options.DefaultEmailDeliveryChannel = _channel1_name;
                })
                .UseChannel(_channel1)
                .Create() as EmailService;

            // assert
            if (service is not null)
            {
                Assert.Single(service.Channels);
                Assert.Equal(_channel1_name, service.DefaultChannel.Name);
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
                        options.DefaultEmailDeliveryChannel = null;
                    });
            });
        }
    }
}
