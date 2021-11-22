namespace Email.NET.EDP.SocketLabs
{
    using Email.NET.EDP.Smtp;
    using Email.NET.Exceptions;

    /// <summary>
    /// the options for configuring the SocketLabs email delivery provider
    /// </summary>
    public class SocketLabsEmailDeliveryProviderOptions
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
                throw new RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryProviderOptions>(
                    $"{nameof(DefaultServerId)}", "the given SocketLabsEmailDeliveryProviderOptions.DefaultServerId value less then or equals to Zero.");

            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new RequiredOptionValueNotSpecifiedException<SocketLabsEmailDeliveryProviderOptions>(
                    $"{nameof(ApiKey)}", "the given SocketLabsEmailDeliveryProviderOptions.ApiKey value is null or empty.");
        }
    }
}
