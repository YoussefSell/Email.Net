namespace Email.NET.EDP.Smtp
{
    using Exceptions;
using System.Net;

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
        public void Validate()
        {
            if (SmtpOptions is null)
                throw new RequiredOptionValueNotSpecifiedException<EmailServiceOptions>(
                    nameof(SmtpOptions), "the given SmtpEmailDeliveryProviderOptions.SmtpOptions are null, you must supply a valid smtpOptions.");

            SmtpOptions.Validate();
        }

        /// <summary>
        /// set the <see cref="SmtpOptions"/> to use the Gmail Smtp configuration.
        /// </summary>
        /// <param name="email">your Gamil email.</param>
        /// <param name="password">your Gamil password.</param>
        /// <remarks>
        /// this will set the following properties:
        /// <para><see cref="SmtpOptions.Host"/> => smtp.gmail.com</para>
        /// <para><see cref="SmtpOptions.Port"/> => 587</para>
        /// <para><see cref="SmtpOptions.EnableSsl"/> => true</para>
        /// <para>don't forget to allow less secure apps, to be able to send emails from your Gmail account. from info check this <see href="https://support.google.com/accounts/answer/6010255?hl=en">link</see></para>
        /// </remarks>
        public void UseGmailSmtp(string email, string password)
        {
            SmtpOptions = new SmtpOptions
            {
                Port = 587,
                EnableSsl = true,
                Host = "smtp.gmail.com",
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email, password),
            };
        }

        /// <summary>
        /// set the <see cref="SmtpOptions"/> to use the Outlook Smtp configuration.
        /// </summary>
        /// <param name="email">your outlook email.</param>
        /// <param name="password">your outlook password.</param>
        /// <remarks>
        /// this will set the following properties:
        /// <para><see cref="SmtpOptions.Host"/> => smtp-mail.outlook.com</para>
        /// <para><see cref="SmtpOptions.Port"/> => 587</para>
        /// <para><see cref="SmtpOptions.EnableSsl"/> => true</para>
        /// </remarks>
        public void UseOutlookSmtp(string email, string password)
        {
            SmtpOptions = new SmtpOptions
            {
                Port = 587,
                EnableSsl = true,
                Host = "smtp-mail.outlook.com",
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email, password),
            };
        }
    }
}
