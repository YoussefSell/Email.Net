namespace Email.Net
{
    using Email.Net.Channel.SocketLabs;
    using Email.Net.Factories;
    using System;

    /// <summary>
    /// the extensions methods over the <see cref="EmailServiceFactory"/> factory.
    /// </summary>
    public static class SocketLabsEmailServiceFactoryExtensions
    {
        /// <summary>
        /// add the Socketlabs channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="apiKey">Set your Socketlabs Api key.</param>
        /// <param name="serverId">Set the id of the default server to be used for sending the emails on Socketlabs.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseSocketlabs(this EmailServiceFactory builder, string apiKey, int serverId)
            => builder.UseSocketlabs(op => { op.DefaultServerId = serverId; op.ApiKey = apiKey; });

        /// <summary>
        /// add the Socketlabs channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseSocketlabs(this EmailServiceFactory builder, Action<SocketLabsEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new SocketLabsEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            // add the channel to the emails service factory
            builder.UseChannel(new SocketLabsEmailDeliveryChannel(configuration));

            return builder;
        }
    }
}
