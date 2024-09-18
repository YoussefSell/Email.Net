namespace Email.Net
{
    using Email.Net.Channel;
    using Email.Net.Channel.SocketLabs;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the Socketlabs channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="apiKey">Set your Socketlabs Api key.</param>
        /// <param name="serverId">Set the id of the default server to be used for sending the emails on Socketlabs.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSocketlabs(this EmailNetBuilder builder, string apiKey, int serverId)
            => builder.UseSocketlabs(op => { op.DefaultServerId = serverId; op.ApiKey = apiKey; });

        /// <summary>
        /// add the Socketlabs channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSocketlabs(this EmailNetBuilder builder, Action<SocketLabsEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new SocketLabsEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryChannel, SocketLabsEmailDeliveryChannel>();
            builder.ServiceCollection.AddScoped<ISocketLabsEmailDeliveryChannel, SocketLabsEmailDeliveryChannel>();

            return builder;
        }
    }
}
