﻿namespace Email.Net
{
    using Email.Net.Channel.SendGrid;
    using Email.Net.Factories;
    using System;

    /// <summary>
    /// the extensions methods over the <see cref="EmailServiceFactory"/> factory.
    /// </summary>
    public static class SendGridEmailServiceFactoryExtensions
    {
        /// <summary>
        /// add the SendGrid Channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="apiKey">set your Twilio SendGrid Api key.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseSendGrid(this EmailServiceFactory builder, string apiKey)
            => builder.UseSendGrid(op => op.ApiKey = apiKey);

        /// <summary>
        /// add the SendGrid Channel to be used with your email service.
        /// </summary>
        /// <param name="builder">the <see cref="EmailServiceFactory"/> instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable methods chaining.</returns>
        public static EmailServiceFactory UseSendGrid(this EmailServiceFactory builder, Action<SendgridEmailDeliveryChannelOptions> config)
        {
            // load the configuration
            var configuration = new SendgridEmailDeliveryChannelOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            // add the channel to the emails service factory
            builder.UseChannel(new SendgridEmailDeliveryChannel(configuration));

            return builder;
        }
    }
}
