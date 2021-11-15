namespace Email.NET.EDP.Mailgun
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// the Mailgun client email delivery provider
    /// </summary>
    public partial class MailgunEmailDeliveryProvider : IMailgunEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, params EdpData[] data)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(Message message, params EdpData[] data)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// partial part for <see cref="MailgunEmailDeliveryProvider"/>
    /// </summary>
    public partial class MailgunEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "mailgun_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly MailgunEmailDeliveryProviderOptions _options;

        public MailgunEmailDeliveryProvider(MailgunEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }
    }
}