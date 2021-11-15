namespace Email.NET.EDP.AmazonSES
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// the AmazonSES client email delivery provider
    /// </summary>
    public partial class AmazonSESEmailDeliveryProvider : IAmazonSESEmailDeliveryProvider
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
    /// partial part for <see cref="AmazonSESEmailDeliveryProvider"/>
    /// </summary>
    public partial class AmazonSESEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "amazon_ses_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly AmazonSESEmailDeliveryProviderOptions _options;

        public AmazonSESEmailDeliveryProvider(AmazonSESEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }
    }
}