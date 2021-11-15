namespace Email.NET.EDP.MailKit
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// the MailKit client email delivery provider
    /// </summary>
    public partial class MailKitEmailDeliveryProvider : IMailKitEmailDeliveryProvider
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
    /// partial part for <see cref="MailKitEmailDeliveryProvider"/>
    /// </summary>
    public partial class MailKitEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "mailkit_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly MailKitEmailDeliveryProviderOptions _options;

        public MailKitEmailDeliveryProvider(MailKitEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }
    }
}