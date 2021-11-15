namespace Email.NET.EDP.Sendgrid
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// the Sendgrid client email delivery provider
    /// </summary>
    public partial class SendgridEmailDeliveryProvider : ISendgridEmailDeliveryProvider
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
    /// partial part for <see cref="SendgridEmailDeliveryProvider"/>
    /// </summary>
    public partial class SendgridEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "sendgrid_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly SendgridEmailDeliveryProviderOptions _options;

        public SendgridEmailDeliveryProvider(SendgridEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }
    }
}