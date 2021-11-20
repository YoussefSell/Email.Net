namespace Microsoft.Extensions.DependencyInjection
{
    using Email.NET.EDP;
    using Email.NET.EDP.AmazonSES;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the AmazonSES EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseAmazonSES(this EmailNetBuilder builder, Action<AmazonSESEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new AmazonSESEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryProvider, AmazonSESEmailDeliveryProvider>();
            builder.ServiceCollection.AddScoped<IAmazonSESEmailDeliveryProvider, AmazonSESEmailDeliveryProvider>();

            return builder;
        }
    }
}
