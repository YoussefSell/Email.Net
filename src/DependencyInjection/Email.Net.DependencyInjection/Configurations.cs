namespace Microsoft.Extensions.DependencyInjection
{
    using Email.Net;
    using Email.Net.EDP;
    using Email.Net.EDP.Smtp;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the Email.Net services and configuration.
        /// </summary>
        /// <param name="serviceCollection">the service collection instant</param>
        /// <param name="defaultEdpName">name of the default edp to be used.</param>
        public static EmailNetBuilder AddEmailNet(this IServiceCollection serviceCollection, string defaultEdpName)
            => AddEmailNet(serviceCollection, options => options.DefaultEmailDeliveryProvider = defaultEdpName);

        /// <summary>
        /// add the Email.Net services and configuration.
        /// </summary>
        /// <param name="serviceCollection">the service collection instant</param>
        /// <param name="config">the configuration initializer.</param>
        public static EmailNetBuilder AddEmailNet(this IServiceCollection serviceCollection, Action<EmailServiceOptions> config)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            // load the configuration
            var configuration = new EmailServiceOptions();
            config(configuration);

            serviceCollection.AddSingleton((s) => configuration);
            serviceCollection.AddScoped<IEmailService, EmailService>();

            // register the countries service
            return new EmailNetBuilder(serviceCollection, configuration);
        }

        /// <summary>
        /// add the SMTP EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSmtp(this EmailNetBuilder builder, Action<SmtpEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new SmtpEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryProvider, SmtpEmailDeliveryProvider>();
            builder.ServiceCollection.AddScoped<ISmtpEmailDeliveryProvider, SmtpEmailDeliveryProvider>();

            return builder;
        }
    }
}
