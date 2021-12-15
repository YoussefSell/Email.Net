namespace Email.Net.EDP.SendGrid
{
    using Email.Net.Exceptions;

    /// <summary>
    /// the options for configuring the Smtp email delivery provider
    /// </summary>
    public class SendgridEmailDeliveryProviderOptions
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
                throw new RequiredOptionValueNotSpecifiedException<SendgridEmailDeliveryProviderOptions>(
                    $"{nameof(ApiKey)}", "the given SendgridEmailDeliveryProviderOptions.ApiKey value is null or empty.");
        }
    }
}
