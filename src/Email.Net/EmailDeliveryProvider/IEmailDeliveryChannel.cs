namespace Email.Net.Channel
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// email delivery channel used for performing the email sending.
    /// </summary>
    public interface IEmailDeliveryChannel
    {
        /// <summary>
        /// a unique name of the email delivery channel
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <returns>the email sending result</returns>
        EmailSendingResult Send(EmailMessage message);

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>a task that resolve to the email sending result</returns>
        Task<EmailSendingResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
    }
}
