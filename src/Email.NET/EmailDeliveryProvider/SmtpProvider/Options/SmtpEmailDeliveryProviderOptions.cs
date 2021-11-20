namespace Email.NET.EDP.Smtp
{
    using Exceptions;

    /// <summary>
    /// the options for configuring the Smtp email delivery provider
    /// </summary>
    public class SmtpEmailDeliveryProviderOptions
    {
        /// <summary>
        /// the smtp configuration
        /// </summary>
        public SmtpOptions SmtpOptions { get; set; }

        /// <summary>
        /// validate if the options are all set correctly
        /// </summary>
        internal void Validate()
        {
            if (SmtpOptions is null)
                throw new RequiredOptionValueNotSpecifiedException<EmailServiceOptions>(
                    nameof(SmtpOptions), "the given SmtpEmailDeliveryProviderOptions.SmtpOptions are null, you must supply a valid smtpOptions.");

            SmtpOptions.Validate();
        }
    }
}
