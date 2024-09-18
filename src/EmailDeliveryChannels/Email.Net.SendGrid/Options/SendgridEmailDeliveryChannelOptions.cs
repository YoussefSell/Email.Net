namespace Email.Net.Channel.SendGrid
{
    using Email.Net.Exceptions;

    /// <summary>
    /// the options for configuring the Smtp email delivery channel
    /// </summary>
    public class SendgridEmailDeliveryChannelOptions
    {
        /// <summary>
        /// Get or set your Twilio SendGrid Api key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// validate if the options are all set correctly
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new RequiredOptionValueNotSpecifiedException<SendgridEmailDeliveryChannelOptions>(
                    $"{nameof(ApiKey)}", "the given SendgridEmailDeliveryChannelOptions.ApiKey value is null or empty.");
        }
    }
}
