namespace Email.NET
{
    using Email.NET.EDP.Smtp;
    using Email.NET.Factories;
    using System;

    /// <summary>
    /// the extensions methods over the <see cref="EmailServiceFactory"/> factory.
    /// </summary>
    public static class SmtpEmailServiceFactoryExtensions
    {
        /// <summary>
        /// add the SMTP EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseSmtp(this EmailServiceFactory builder, Action<SmtpEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new SmtpEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            // add the Edp to the emails service factory
            builder.UseEDP(new SmtpEmailDeliveryProvider(configuration));

            return builder;
        }
    }
}
