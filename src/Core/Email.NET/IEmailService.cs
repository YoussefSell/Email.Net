namespace Email.NET
{
    using EDP;
    using Email.NET.Exceptions;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// the email service used to abstract the email sending
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryProvider"/>.
        /// </summary>
        /// <param name="message">the email message to be send.</param>
        /// <remarks>
        /// the default email delivery provider should be specified 
        /// in <see cref="EmailServiceOptions.DefaultEmailDeliveryProvider"/> option supplied to the EmailService instance.
        /// </remarks>
        /// <exception cref="ArgumentNullException">the given message instance is null.</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        EmailSendingResult Send(Message message);

        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryProvider"/>.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <remarks>
        /// the default email delivery provider should be specified 
        /// in <see cref="EmailServiceOptions.DefaultEmailDeliveryProvider"/> option supplied to the EmailService instance.
        /// </remarks>
        /// <exception cref="ArgumentNullException">the given message instance is null.</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        Task<EmailSendingResult> SendAsync(Message message);

        /// <summary>
        /// Sends the specified email message using the email delivery provider with the given name.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="edp_name">the name of the email delivery provider used for sending the email message.</param>
        /// <exception cref="ArgumentNullException">the given message instance is null, or the provider name is null</exception>
        /// <exception cref="EmailDeliveryProviderNotFoundException">couldn't find any EDP with the given name</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        EmailSendingResult Send(Message message, string edp_name);

        /// <summary>
        /// Sends the specified email message using the email delivery provider with the given name.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="edp_name">the name of the email delivery provider used for sending the email message.</param>
        /// <exception cref="ArgumentNullException">the given message instance is null, or the provider name is null</exception>
        /// <exception cref="EmailDeliveryProviderNotFoundException">couldn't find any EDP with the given name</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        Task<EmailSendingResult> SendAsync(Message message, string edp_name);

        /// <summary>
        /// Sends the specified email message using the given email delivery provider.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="edp">the email delivery provider used for sending the email message.</param>
        /// <exception cref="ArgumentNullException">the given message instance is null, or the provider is null</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        EmailSendingResult Send(Message message, IEmailDeliveryProvider edp);

        /// <summary>
        /// Sends the specified email message using the given email delivery provider.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="edp">the email delivery provider used for sending the email message.</param>
        /// <exception cref="ArgumentNullException">the given message instance is null, or the provider is null</exception>
        /// <exception cref="ArgumentException">
        /// the given message doesn't contain a 'From' (sender) email, 
        /// and no default 'From' (sender) email is set in the <see cref="EmailServiceOptions.DefaultFrom"/> option supplied to the EmailService instance.
        /// </exception>
        Task<EmailSendingResult> SendAsync(Message message, IEmailDeliveryProvider edp);
    }
}