namespace Email.Net
{
    using Email.Net.Channel;
    using Email.Net.Exceptions;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// the email service used to abstract the email sending
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryChannel"/>.
        /// </summary>
        /// <param name="message">the email message to be send.</param>
        /// <remarks>
        /// the default email delivery channel should be specified 
        /// in <see cref="EmailServiceOptions.DefaultEmailDeliveryChannel"/> option supplied to the EmailService instance.
        /// </remarks>
        /// <exception cref="ArgumentNullException">the given message instance is null.</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        EmailSendingResult Send(EmailMessage message);

        /// <summary>
        /// Sends the specified email message using the email delivery channel with the given name.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="channel_name">the name of the email delivery channel used for sending the email message.</param>
        /// <exception cref="ArgumentNullException">the given message instance is null, or the channel name is null</exception>
        /// <exception cref="EmailDeliveryChannelNotFoundException">couldn't find any Channel with the given name</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        EmailSendingResult Send(EmailMessage message, string channel_name);

        /// <summary>
        /// Sends the specified email message using the given email delivery channel.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="channel">the email delivery channel used for sending the email message.</param>
        /// <exception cref="ArgumentNullException">the given message instance is null, or the channel is null</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        EmailSendingResult Send(EmailMessage message, IEmailDeliveryChannel channel);

        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryChannel"/>.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <remarks>
        /// the default email delivery channel should be specified 
        /// in <see cref="EmailServiceOptions.DefaultEmailDeliveryChannel"/> option supplied to the EmailService instance.
        /// </remarks>
        /// <exception cref="ArgumentNullException">the given message instance is null.</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        Task<EmailSendingResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends the specified email message using the email delivery channel with the given name.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="channel_name">the name of the email delivery channel used for sending the email message.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">the given message instance is null, or the channel name is null</exception>
        /// <exception cref="EmailDeliveryChannelNotFoundException">couldn't find any Channel with the given name</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        Task<EmailSendingResult> SendAsync(EmailMessage message, string channel_name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends the specified email message using the given email delivery channel.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="channel">the email delivery channel used for sending the email message.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">the given message instance is null, or the channel is null</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        Task<EmailSendingResult> SendAsync(EmailMessage message, IEmailDeliveryChannel channel, CancellationToken cancellationToken = default);
    }
}