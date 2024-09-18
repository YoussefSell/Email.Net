namespace Email.Net
{
    using Email.Net.Channel.Smtp;
    using Email.Net.Factories;
    using System;

    /// <summary>
    /// the extensions methods over the <see cref="EmailServiceFactory"/> factory.
    /// </summary>
    public static class SmtpEmailServiceFactoryExtensions
    {
        /// <summary>
        /// add the SMTP Channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseSmtp(this EmailServiceFactory builder, Action<SmtpEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new SmtpEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            // add the channel to the emails service factory
            builder.UseChannel(new SmtpEmailDeliveryChannel(configuration));

            return builder;
        }
    }
}
