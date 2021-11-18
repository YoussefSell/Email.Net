namespace Email.NET.EDP.SendGrid
{
    using global::SendGrid;
    using global::SendGrid.Helpers.Mail;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// the SendGrid client email delivery provider
    /// </summary>
    public partial class SendgridEmailDeliveryProvider : ISendgridEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, params EdpData[] data)
        {
            try
            {
                var client = CreateClient(data);
                var mailMessage = CreateMessage(message, data);

                var response = client.SendEmailAsync(mailMessage)
                    .ConfigureAwait(false)
                    .GetAwaiter().GetResult();

                // build the result object and return
                return BuildResultObject(response);
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }

        /// <inheritdoc/>
        public async Task<EmailSendingResult> SendAsync(Message message, params EdpData[] data)
        {
            try
            {
                var client = CreateClient(data);
                var mailMessage = CreateMessage(message, data);

                var response = await client.SendEmailAsync(mailMessage)
                    .ConfigureAwait(false);

                // build the result object and return
                return BuildResultObject(response);
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
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

        private SendGridClient CreateClient(EdpData[] data)
        {
            var apiKey = _options.ApiKey;

            // get the apiKey & serverId from the data list if any.
            var apikeyEdpData = data.GetData(EdpData.Keys.ApiKey);

            if (!apikeyEdpData.IsEmpty())
                apiKey = apikeyEdpData.GetValue<string>();

            return new SendGridClient(apiKey);
        }

        private static EmailSendingResult BuildResultObject(Response result)
        {
            // check if we have success operations
            if (result.IsSuccessStatusCode)
            {
                // return the result
                return EmailSendingResult.Success(Name);
            }

            // create the failure result & return the result
            return EmailSendingResult.Failure(Name)
                .AddMetaData("status_code", result.StatusCode.ToString());
        }

        /// <summary>
        /// create an instance of <see cref="BasicMessage"/> from the given <see cref="Message"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <param name="data">the edp data instance</param>
        /// <returns>instance of <see cref="BasicMessage"/></returns>
        public SendGridMessage CreateMessage(Message message, EdpData[] data)
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

            if (!(message.ReplyTo is null) && message.ReplyTo.Any())
            {
                var replayTo = message.ReplyTo.First();
                mailMessage.ReplyTo = new EmailAddress(replayTo.Address, replayTo.DisplayName);
            }

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