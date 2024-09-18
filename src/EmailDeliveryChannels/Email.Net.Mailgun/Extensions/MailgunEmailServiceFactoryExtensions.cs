namespace Email.Net
{
    using Email.Net.Channel.Mailgun;
    using Email.Net.Factories;
    using System;

    /// <summary>
    /// the extensions methods over the <see cref="EmailServiceFactory"/> factory.
    /// </summary>
    public static class MailgunEmailServiceFactoryExtensions
    {
        /// <summary>
        /// add the Mailgun channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="apiKey"> set your Twilio SendGrid Api key.</param>
        /// <param name="domain">set The mailgun working domain to use.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseMailgun(this EmailServiceFactory builder, string apiKey, string domain)
            => builder.UseMailgun(op => { op.ApiKey = apiKey; op.Domain = domain; });

        /// <summary>
        /// add the Mailgun channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseMailgun(this EmailServiceFactory builder, Action<MailgunEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new MailgunEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            // add the channel to the emails service factory
            builder.UseChannel(new MailgunEmailDeliveryChannel(configuration));

            return builder;
        }
    }
}
