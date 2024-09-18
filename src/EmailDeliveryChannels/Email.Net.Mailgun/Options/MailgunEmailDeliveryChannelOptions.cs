namespace Email.Net.Channel.Mailgun
{
    using Exceptions;

    /// <summary>
    /// the options for configuring the Mailgun email delivery channel
    /// </summary>
    public class MailgunEmailDeliveryChannelOptions
    {
        /// <summary>
        /// Get or set your Twilio SendGrid Api key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Get pr set the base address of the mailgun api, excluding the scheme, default is api.mailgun.net/v3.
        /// </summary>
        public string BaseUrl { get; set; } = "api.mailgun.net/v3";

        /// <summary>
        /// Get or set whether to call the Mailgun API using HTTPS or not. default is True.
        /// </summary>
        public bool UseSSL { get; set; } = true;

        /// <summary>
        /// Get or set The mailgun working domain to use. 
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// validate if the options are all set correctly
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>(
                    $"{nameof(ApiKey)}", "the given MailgunEmailDeliveryChannelOptions.ApiKey value is null or empty.");

            if (string.IsNullOrWhiteSpace(Domain))
                throw new RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>(
                    $"{nameof(Domain)}", "the given MailgunEmailDeliveryChannelOptions.Domain value is null or empty.");

            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryChannelOptions>(
                    $"{nameof(BaseUrl)}", "the given MailgunEmailDeliveryChannelOptions.BaseUrl value is null or empty.");
        }
    }
}
