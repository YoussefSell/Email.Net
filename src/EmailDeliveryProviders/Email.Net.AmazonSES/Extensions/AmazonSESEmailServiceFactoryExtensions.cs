namespace Email.Net
{
    using Email.Net.EDP.AmazonSES;
    using Email.Net.Factories;
    using System;

    /// <summary>
    /// the extensions methods over the <see cref="EmailServiceFactory"/> factory.
    /// </summary>
    public static class AmazonSESEmailServiceFactoryExtensions
    {
        /// <summary>
        /// add the AmazonSES EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseAmazonSES(this EmailServiceFactory builder, Action<AmazonSESEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new AmazonSESEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            // add the Edp to the emails service factory
            builder.UseEDP(new AmazonSESEmailDeliveryProvider(configuration));

            return builder;
        }
    }
}
