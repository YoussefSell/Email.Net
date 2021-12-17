namespace Email.Net
{
    using Email.Net.EDP.MailKit;
    using Email.Net.Factories;
    using System;

    /// <summary>
    /// the extensions methods over the <see cref="EmailServiceFactory"/> factory.
    /// </summary>
    public static class MailKitEmailServiceFactoryExtensions
    {
        /// <summary>
        /// add the MailKit EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseMailKit(this EmailServiceFactory builder, Action<MailKitEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new MailKitEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            // add the Edp to the emails service factory
            builder.UseEDP(new MailKitEmailDeliveryProvider(configuration));

            return builder;
        }
    }
}
