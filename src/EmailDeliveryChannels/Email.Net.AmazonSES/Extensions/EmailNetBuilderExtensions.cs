namespace Email.Net
{
    using Email.Net.Channel;
    using Email.Net.Channel.AmazonSES;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the AmazonSES Channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseAmazonSES(this EmailNetBuilder builder, Action<AmazonSESEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new AmazonSESEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryChannel, AmazonSESEmailDeliveryChannel>();
            builder.ServiceCollection.AddScoped<IAmazonSESEmailDeliveryChannel, AmazonSESEmailDeliveryChannel>();

            return builder;
        }
    }
}
