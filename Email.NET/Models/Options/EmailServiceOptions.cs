namespace Email.NET
{
    using Providers.SmtpClient;
    using System;
    using System.Net.Mail;

    /// <summary>
    /// the options for configuring the email service
    /// </summary>
    public class EmailServiceOptions
    {
        /// <summary>
        /// the name of the default email delivery provider, by default is set to <see cref="SmtpEmailDeliveryProvider.Name"/>
        /// </summary>
        public string DefaultEmailDeliveryProvider { get; set; } = SmtpEmailDeliveryProvider.Name;

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
        internal void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
