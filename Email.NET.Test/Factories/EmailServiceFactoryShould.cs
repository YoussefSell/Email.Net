namespace Email.NET.Test.Factories
{
    using Email.NET.Exceptions;
    using Email.NET.Factories;
    using Email.NET.Providers.SmtpClient;
    using System.Net.Mail;
    using Xunit;

    public class EmailServiceFactoryShould
    {
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
                    options.DefaultEmailDeliveryProvider = SmtpEmailDeliveryProvider.Name;
                })
                .UseEDP(new SmtpEmailDeliveryProvider(new SmtpEmailDeliveryProviderOptions()))
                .Create();

            // assert
            Assert.Single(service.Edps);
            Assert.Equal(defaultEmail, service.Options.DefaultFrom);
            Assert.Equal(SmtpEmailDeliveryProvider.Name, service.DefaultEdp.Name);
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
