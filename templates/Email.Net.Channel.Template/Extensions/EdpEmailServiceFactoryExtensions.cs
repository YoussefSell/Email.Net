namespace Email.Net
{
    using Email.Net.Channel.Channel_name;
    using Email.Net.Factories;
    using System;

    /// <summary>
    /// the extensions methods over the <see cref="EmailServiceFactory"/> factory.
    /// </summary>
    public static class ChannelEmailServiceFactoryExtensions
    {
        /// <summary>
        /// add the channel-name Channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseChannelName(this EmailServiceFactory builder, Action<ChannelEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new ChannelEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            // add the Channel to the emails service factory
            builder.UseChannel(new ChannelEmailDeliveryChannel(configuration));

            return builder;
        }
    }
}
