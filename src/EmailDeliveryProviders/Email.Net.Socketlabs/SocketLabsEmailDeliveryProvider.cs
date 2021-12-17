namespace Email.Net.EDP.SocketLabs
{
    using global::SocketLabs.InjectionApi;
    using global::SocketLabs.InjectionApi.Message;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// the SocketLabs client email delivery provider
    /// </summary>
    public partial class SocketLabsEmailDeliveryProvider : ISocketLabsEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message)
        {
            try
            {
                // create the basic message
                var basicMessage = CreateMessage(message);

                // create the client
                using (var client = CreateClient(message.EdpData))
                {
                    // send the message
                    var result = client.Send(basicMessage);

                    // build the result object and return
                    return BuildResultObject(result);
                }
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }

        /// <inheritdoc/>
        public async Task<EmailSendingResult> SendAsync(Message message)
        {
            try
            {
                // create the basic message
                var basicMessage = CreateMessage(message);

                // create the client
                using (var client = CreateClient(message.EdpData))
                {
                    // send the message
                    var result = await client.SendAsync(basicMessage, default);

                    // build the result object and return
                    return BuildResultObject(result);
                }
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="SocketLabsEmailDeliveryProvider"/>
    /// </summary>
    public partial class SocketLabsEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "socketlabs_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly SocketLabsEmailDeliveryProviderOptions _options;

        /// <summary>
        /// create an instance of <see cref="SocketLabsEmailDeliveryProvider"/>
        /// </summary>
        /// <param name="options">the edp options instance</param>
        /// <exception cref="ArgumentNullException">if the given provider options is null</exception>
        public SocketLabsEmailDeliveryProvider(SocketLabsEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        private SocketLabsClient CreateClient(IEnumerable<EdpData> data)
        {
            var apiKey = _options.ApiKey;
            var serverId = _options.DefaultServerId;

            // get the apiKey & serverId from the data list if any.
            var apikeyEdpData = data.GetData(EdpData.Keys.ApiKey);
            var serverIdEdpData = data.GetData(CustomEdpData.ServerId);

            if (!apikeyEdpData.IsEmpty())
                apiKey = apikeyEdpData.GetValue<string>();

            if (!serverIdEdpData.IsEmpty())
                serverId = serverIdEdpData.GetValue<int>();

            return new SocketLabsClient(serverId, apiKey);
        }

        private static EmailSendingResult BuildResultObject(SendResponse result)
        {
            // check if we have success operations
            if (result.Result == SendResult.Success)
            {
                // return the result
                return EmailSendingResult.Success(Name)
                    .AddMetaData("transaction_receipt_key", result.TransactionReceipt);
            }

            // create the failure result & return the result
            return EmailSendingResult.Failure(Name)
                .AddMetaData("transaction_receipt_key", result.TransactionReceipt)
                .AddError(new EmailSendingError(
                    code: result.Result.ToString(),
                    message: result.ResponseMessage)
                );
        }

        /// <summary>
        /// create an instance of <see cref="BasicMessage"/> from the given <see cref="Message"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="BasicMessage"/></returns>
        public BasicMessage CreateMessage(Message message)
        {
            var messageIdEdpData = message.EdpData.GetData(EdpData.Keys.MessageId);
            var mailingIdEdpData = message.EdpData.GetData(EdpData.Keys.MailingId);

            var mailMessage = new BasicMessage
            {
                Subject = message.Subject,
                HtmlBody = message.HtmlBody,
                PlainTextBody = message.PlainTextBody,
                From = new EmailAddress(message.From.Address, message.From.DisplayName),
                MessageId = !messageIdEdpData.IsEmpty() ? messageIdEdpData.GetValue<string>() : string.Empty,
                MailingId = !mailingIdEdpData.IsEmpty() ? mailingIdEdpData.GetValue<string>() : string.Empty,
            };

            if (!string.IsNullOrEmpty(message.Charset))
                mailMessage.CharSet = message.Charset;

            if (!(message.ReplyTo is null) && message.ReplyTo.Any())
            {
                var replayTo = message.ReplyTo.First();
                mailMessage.ReplyTo = new EmailAddress(replayTo.Address, replayTo.DisplayName);
            }

            foreach (var email in message.To)
                mailMessage.To.Add(new EmailAddress(email.Address, email.DisplayName));

            if (!(message.Bcc is null) && message.Bcc.Any())
            {
                foreach (var email in message.Bcc)
                    mailMessage.Bcc.Add(new EmailAddress(email.Address, email.DisplayName));
            }

            if (!(message.Cc is null) && message.Cc.Any())
            {
                foreach (var email in message.Cc)
                    mailMessage.Cc.Add(new EmailAddress(email.Address, email.DisplayName));
            }

            if (!(message.Headers is null && message.Headers.Any()))
            {
                foreach (var header in message.Headers)
                    mailMessage.CustomHeaders.Add(header.Key, header.Value);
            }

            SetAttachments(mailMessage, message.Attachments);

            return mailMessage;
        }

        /// <summary>
        /// add the given list of attachments to the <see cref="BasicMessage"/> instance
        /// </summary>
        /// <param name="message">the <see cref="BasicMessage"/> instance</param>
        /// <param name="attachments">the list of attachments to add</param>
        private void SetAttachments(BasicMessage message, IEnumerable<Net.Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
                return;

            foreach (var attachment in attachments)
            {
                if (attachment is FilePathAttachment filePathAttachment)
                {
                    message.Attachments.Add(attachment.FileName, attachment.FileType, filePathAttachment.FilePath);
                    continue;
                }

                message.Attachments.Add(attachment.FileName, attachment.FileType, attachment.GetAsByteArray());
            }
        }
    }
}