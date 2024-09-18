namespace Email.Net
{
    using Email.Net.Channel;
    using Email.Net.Channel.SendGrid;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the SendGrid Channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="apiKey">set your Twilio SendGrid Api key.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSendGrid(this EmailNetBuilder builder, string apiKey)
            => builder.UseSendGrid(op => op.ApiKey = apiKey);

        /// <summary>
        /// add the SendGrid Channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSendGrid(this EmailNetBuilder builder, Action<SendgridEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new SendgridEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryChannel, SendgridEmailDeliveryChannel>();
            builder.ServiceCollection.AddScoped<ISendgridEmailDeliveryChannel, SendgridEmailDeliveryChannel>();

            return builder;
        }
    }
}
