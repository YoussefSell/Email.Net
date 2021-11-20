namespace Email.NET.EDP.MailKit
{
    using Email.NET.Exceptions;

    /// <summary>
    /// the options for configuring the MailKit email delivery provider
    /// </summary>
    public class MailKitEmailDeliveryProviderOptions
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
                    nameof(SmtpOptions), "the given MailKitEmailDeliveryProviderOptions.SmtpOptions are null, you must supply a valid smtpOptions.");

            SmtpOptions.Validate();
        }
    }
}
