namespace Email.NET
{
    using EDP;
    using System.Threading.Tasks;

    /// <summary>
    /// the email service used to abstract the email sending
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryProvider"/>.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        EmailSendingResult Send(Message message);

        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryProvider"/>.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        Task<EmailSendingResult> SendAsync(Message message);

        /// <summary>
        /// Sends the specified email message using the email delivery provider with the given name.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="providerName">the name of the email delivery provider used for sending the email message.</param>
        EmailSendingResult Send(Message message, string providerName);

        /// <summary>
        /// Sends the specified email message using the email delivery provider with the given name.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="providerName">the name of the email delivery provider used for sending the email message.</param>
        Task<EmailSendingResult> SendAsync(Message message, string providerName);

        /// <summary>
        /// Sends the specified email message using the given email delivery provider.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="provider">the email delivery provider used for sending the email message.</param>
        EmailSendingResult Send(Message message, IEmailDeliveryProvider provider);

        /// <summary>
        /// Sends the specified email message using the given email delivery provider.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="provider">the email delivery provider used for sending the email message.</param>
        Task<EmailSendingResult> SendAsync(Message message, IEmailDeliveryProvider provider);
    }
}