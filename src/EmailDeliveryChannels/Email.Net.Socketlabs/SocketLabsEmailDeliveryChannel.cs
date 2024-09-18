namespace Email.Net.Channel.SocketLabs
{
    using global::SocketLabs.InjectionApi;
    using global::SocketLabs.InjectionApi.Message;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// the SocketLabs client email delivery channel
    /// </summary>
    public partial class SocketLabsEmailDeliveryChannel : ISocketLabsEmailDeliveryChannel
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message)
        {
            try
            {
                // create the basic message
                var basicMessage = CreateMessage(message);

                // create the client
                using (var client = CreateClient(message.ChannelData))
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
        public async Task<EmailSendingResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                // create the basic message
                var basicMessage = CreateMessage(message);

                // create the client
                using var client = CreateClient(message.ChannelData);

                // send the message
                var result = await client.SendAsync(basicMessage, cancellationToken)
                    .ConfigureAwait(false);

                // build the result object and return
                return BuildResultObject(result);
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="SocketLabsEmailDeliveryChannel"/>
    /// </summary>
    public partial class SocketLabsEmailDeliveryChannel
    {
        /// <summary>
        /// the name of the email delivery channel
        /// </summary>
        public const string Name = "socketlabs_channel";

        /// <inheritdoc/>
        string IEmailDeliveryChannel.Name => Name;

        private readonly SocketLabsEmailDeliveryChannelOptions _options;

        /// <summary>
        /// create an instance of <see cref="SocketLabsEmailDeliveryChannel"/>
        /// </summary>
        /// <param name="options">the channel options instance</param>
        /// <exception cref="ArgumentNullException">if the given channel options is null</exception>
        public SocketLabsEmailDeliveryChannel(SocketLabsEmailDeliveryChannelOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        private SocketLabsClient CreateClient(IEnumerable<ChannelData> data)
        {
            var apiKey = _options.ApiKey;
            var serverId = _options.DefaultServerId;

            // get the apiKey & serverId from the data list if any.
            var apikeyChannelData = data.GetData(ChannelData.Keys.ApiKey);
            var serverIdChannelData = data.GetData(CustomChannelData.ServerId);

            if (!apikeyChannelData.IsEmpty())
                apiKey = apikeyChannelData.GetValue<string>();

            if (!serverIdChannelData.IsEmpty())
                serverId = serverIdChannelData.GetValue<int>();

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
        /// create an instance of <see cref="BasicMessage"/> from the given <see cref="EmailMessage"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="BasicMessage"/></returns>
        public BasicMessage CreateMessage(EmailMessage message)
        {
            var messageIdChannelData = message.ChannelData.GetData(ChannelData.Keys.MessageId);
            var mailingIdChannelData = message.ChannelData.GetData(ChannelData.Keys.MailingId);

            var mailMessage = new BasicMessage
            {
                Subject = message.Subject,
                HtmlBody = message.HtmlBody,
                PlainTextBody = message.PlainTextBody,
                From = new EmailAddress(message.From.Address, message.From.DisplayName),
                MessageId = !messageIdChannelData.IsEmpty() ? messageIdChannelData.GetValue<string>() : string.Empty,
                MailingId = !mailingIdChannelData.IsEmpty() ? mailingIdChannelData.GetValue<string>() : string.Empty,
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
        private static void SetAttachments(BasicMessage message, IEnumerable<Net.Attachment> attachments)
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