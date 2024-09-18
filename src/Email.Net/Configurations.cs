namespace Email.Net
{
    using Microsoft.Extensions.DependencyInjection;
    using Email.Net.Channel;
    using Email.Net.Channel.Smtp;
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
        /// <param name="defaultChannelName">name of the default channel to be used.</param>
        public static EmailNetBuilder AddEmailNet(this IServiceCollection serviceCollection, string defaultChannelName)
            => AddEmailNet(serviceCollection, options => options.DefaultEmailDeliveryChannel = defaultChannelName);

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
        /// add the SMTP Channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSmtp(this EmailNetBuilder builder, Action<SmtpEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new SmtpEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryChannel, SmtpEmailDeliveryChannel>();
            builder.ServiceCollection.AddScoped<ISmtpEmailDeliveryChannel, SmtpEmailDeliveryChannel>();

            return builder;
        }
    }
}
