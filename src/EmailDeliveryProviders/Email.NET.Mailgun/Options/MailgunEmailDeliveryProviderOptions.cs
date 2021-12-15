namespace Email.Net.EDP.Mailgun
{
    using Exceptions;

    /// <summary>
    /// the options for configuring the Mailgun email delivery provider
    /// </summary>
    public class MailgunEmailDeliveryProviderOptions
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
                throw new RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>(
                    $"{nameof(ApiKey)}", "the given MailgunEmailDeliveryProviderOptions.ApiKey value is null or empty.");
            
            if (string.IsNullOrWhiteSpace(Domain))
                throw new RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>(
                    $"{nameof(Domain)}", "the given MailgunEmailDeliveryProviderOptions.Domain value is null or empty.");

            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new RequiredOptionValueNotSpecifiedException<MailgunEmailDeliveryProviderOptions>(
                    $"{nameof(BaseUrl)}", "the given MailgunEmailDeliveryProviderOptions.BaseUrl value is null or empty.");
        }
    }
}
