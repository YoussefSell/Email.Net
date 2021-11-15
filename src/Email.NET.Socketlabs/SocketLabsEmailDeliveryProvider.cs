namespace Email.NET.EDP.SocketLabs
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// the SocketLabs client email delivery provider
    /// </summary>
    public partial class SocketLabsEmailDeliveryProvider : ISocketLabsEmailDeliveryProvider
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
    /// partial part for <see cref="SocketLabsEmailDeliveryProvider"/>
    /// </summary>
    public partial class SocketLabsEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "socketlabs_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly SocketLabsEmailDeliveryProviderOptions _options;

        public SocketLabsEmailDeliveryProvider(SocketLabsEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }
    }
}