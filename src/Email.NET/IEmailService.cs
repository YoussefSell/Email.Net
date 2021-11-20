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
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        EmailSendingResult Send(Message message, params EdpData[] data);

        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryProvider"/>.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        Task<EmailSendingResult> SendAsync(Message message, params EdpData[] data);

        /// <summary>
        /// Sends the specified email message using the email delivery provider with the given name.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="providerName">the name of the email delivery provider used for sending the email message.</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        EmailSendingResult Send(Message message, string providerName, params EdpData[] data);

        /// <summary>
        /// Sends the specified email message using the email delivery provider with the given name.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="providerName">the name of the email delivery provider used for sending the email message.</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        Task<EmailSendingResult> SendAsync(Message message, string providerName, params EdpData[] data);

        /// <summary>
        /// Sends the specified email message using the given email delivery provider.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="provider">the email delivery provider used for sending the email message.</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        EmailSendingResult Send(Message message, IEmailDeliveryProvider provider, params EdpData[] data);

        /// <summary>
        /// Sends the specified email message using the given email delivery provider.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="provider">the email delivery provider used for sending the email message.</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        Task<EmailSendingResult> SendAsync(Message message, IEmailDeliveryProvider provider, params EdpData[] data);
    }
}