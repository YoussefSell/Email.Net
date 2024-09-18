namespace Email.Net
{
    using Channel.Smtp;
    using Exceptions;
    using System.Net.Mail;
    using Utilities;

    /// <summary>
    /// the options for configuring the email service
    /// </summary>
    public class EmailServiceOptions
    {
        /// <summary>
        /// the name of the default email delivery channel, by default is set to <see cref="SmtpEmailDeliveryChannel.Name"/>
        /// </summary>
        public string DefaultEmailDeliveryChannel { get; set; }

        /// <summary>
        /// the default email to be used as the "From" value.
        /// </summary>
        public MailAddress DefaultFrom { get; set; }

        /// <summary>
        /// Get or set if the email sending is paused, if set to true no email will be sent. by default is set to false
        /// </summary>
        public bool PauseSending { get; set; }

        /// <summary>
        /// validate if the options are all set correctly
        /// </summary>
        /// <exception cref="RequiredOptionValueNotSpecifiedException{TOptions}">if the required options are not specified</exception>
        public void Validate()
        {
            if (!DefaultEmailDeliveryChannel.IsValid())
                throw new RequiredOptionValueNotSpecifiedException<EmailServiceOptions>(
                    nameof(DefaultEmailDeliveryChannel),
                    "you must specify a valid EmailDeliveryName to be used as the default channel");
        }
    }
}
