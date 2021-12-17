namespace Email.Net.EDP
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
        EmailSendingResult Send(Message message);

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        Task<EmailSendingResult> SendAsync(Message message);
    }
}
