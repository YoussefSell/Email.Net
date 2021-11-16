namespace Email.NET.EDP.SendGrid
{
    using System;
    using global::SendGrid;
    using System.Threading.Tasks;
using global::SendGrid.Helpers.Mail;
using System.Linq;
    using System.Collections.Generic;

/// <summary>
/// the SendGrid client email delivery provider
/// </summary>
    public partial class SendgridEmailDeliveryProvider : ISendgridEmailDeliveryProvider
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
    /// partial part for <see cref="SendgridEmailDeliveryProvider"/>
    /// </summary>
    public partial class SendgridEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "sendgrid_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly SendgridEmailDeliveryProviderOptions _options;

        public SendgridEmailDeliveryProvider(SendgridEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }


        /// <summary>
        /// create an instance of <see cref="BasicMessage"/> from the given <see cref="Message"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <param name="data">the edp data instance</param>
        /// <returns>instance of <see cref="BasicMessage"/></returns>
        public SendGridMessage CreateBasicMessage(Message message, EdpData[] data)
        {
            var mailMessage = new SendGridMessage
            {
                Subject = message.Subject?.Content,
                HtmlContent = message.HtmlBody?.Content,
                PlainTextContent = message.PlainTextBody?.Content,
                From = new EmailAddress(message.From.Address, message.From.DisplayName),
            };

            foreach (var email in message.To)
                mailMessage.AddTo(email.Address, email.DisplayName);

            if (!(message.Bcc is null) && message.Bcc.Any())
            {
                foreach (var email in message.Bcc)
                    mailMessage.AddBcc(email.Address, email.DisplayName);
            }

            if (!(message.Cc is null) && message.Cc.Any())
            {
                foreach (var email in message.Cc)
                    mailMessage.AddBcc(email.Address, email.DisplayName);
            }

            if (!(message.Headers is null && message.Headers.Any()))
            {
                foreach (var header in message.Headers)
                    mailMessage.AddHeader(header.Key, header.Value);
            }

            SetAttachments(mailMessage, message.Attachments);

            return mailMessage;
        }

        /// <summary>
        /// add the given list of attachments to the <see cref="SendGridMessage"/> instance
        /// </summary>
        /// <param name="message">the <see cref="SendGridMessage"/> instance</param>
        /// <param name="attachments">the list of attachments to add</param>
        public void SetAttachments(SendGridMessage message, IEnumerable<NET.Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
                return;

            foreach (var attachment in attachments)
            {
                message.AddAttachment(attachment.FileName, attachment.GetAsBase64(), type: attachment.FileType);
            }
        }
    }
}