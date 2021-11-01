namespace Email.NET.Providers.SmtpClient
{
    using System.Threading.Tasks;

    /// <summary>
    /// the smtp client email delivery provider
    /// </summary>
    public partial class SmtpClientEmailDeliveryProvider : IEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, params EmailDeliveryProviderData[] data)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(Message message, params EmailDeliveryProviderData[] data)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class SmtpClientEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "smtp_client_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        public SmtpClientEmailDeliveryProvider()
        {

        }
    }
}
