namespace Email.Net
{
    using Email.Net.Channel;
    using Email.Net.Channel.MailKit;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the MailKit channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseMailKit(this EmailNetBuilder builder, Action<MailKitEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new MailKitEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryChannel, MailKitEmailDeliveryChannel>();
            builder.ServiceCollection.AddScoped<IMailKitEmailDeliveryChannel, MailKitEmailDeliveryChannel>();

            return builder;
        }
    }
}
