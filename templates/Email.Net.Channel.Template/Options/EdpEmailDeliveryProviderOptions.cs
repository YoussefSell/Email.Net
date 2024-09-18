﻿namespace Email.Net.Channel.Channel_name
{
    using Email.Net.Exceptions;

    /// <summary>
    /// the options for configuring the SocketLabs email delivery channel
    /// </summary>
    public class ChannelEmailDeliveryChannelOptions
    {
        /// <summary>
        /// Get or Set the id of the default server to be used for sending the emails on Socketlabs.
        /// </summary>
        public int DefaultServerId { get; set; }

        /// <summary>
        /// Get or Set your Socketlabs Api key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// validate if the options are all set correctly
        /// </summary>
        public void Validate()
        {
            if (DefaultServerId <= 0)
                throw new RequiredOptionValueNotSpecifiedException<ChannelEmailDeliveryChannelOptions>(
                    $"{nameof(DefaultServerId)}", "the given SocketLabsEmailDeliveryChannelOptions.DefaultServerId value less then or equals to Zero.");

            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new RequiredOptionValueNotSpecifiedException<ChannelEmailDeliveryChannelOptions>(
                    $"{nameof(ApiKey)}", "the given SocketLabsEmailDeliveryChannelOptions.ApiKey value is null or empty.");
        }
    }
}