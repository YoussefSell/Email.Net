namespace Microsoft.Extensions.DependencyInjection
{
    using Email.Net.EDP;
    using Email.Net.EDP.Mailgun;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the Mailgun EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="apiKey"> set your Twilio SendGrid Api key.</param>
        /// <param name="domain">set The mailgun working domain to use.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseMailgun(this EmailNetBuilder builder, string apiKey, string domain)
            => builder.UseMailgun(op => { op.ApiKey = apiKey; op.Domain = domain; });

        /// <summary>
        /// add the Mailgun EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseMailgun(this EmailNetBuilder builder, Action<MailgunEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new MailgunEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryProvider, MailgunEmailDeliveryProvider>();
            builder.ServiceCollection.AddScoped<IMailgunEmailDeliveryProvider, MailgunEmailDeliveryProvider>();

            return builder;
        }
    }
}
