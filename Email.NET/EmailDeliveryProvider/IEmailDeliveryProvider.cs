namespace Email.NET
{
    using System.Threading.Tasks;

    /// <summary>
    /// email delivery provider used for performing the email sending.
    /// </summary>
    public interface IEmailDeliveryProvider
    {
        /// <summary>
        /// a unique name of the email delivery provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        EmailSendingResult Send(Message message, params EmailDeliveryProviderData[] data);

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        Task<EmailSendingResult> SendAsync(Message message, params EmailDeliveryProviderData[] data);
    }
}
