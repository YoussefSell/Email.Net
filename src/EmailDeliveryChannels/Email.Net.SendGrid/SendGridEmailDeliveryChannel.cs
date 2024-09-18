namespace Email.Net.Channel.SendGrid
{
    using global::SendGrid;
    using global::SendGrid.Helpers.Mail;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// the SendGrid client email delivery channel
    /// </summary>
    public partial class SendgridEmailDeliveryChannel : ISendgridEmailDeliveryChannel
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message)
            => SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public async Task<EmailSendingResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = CreateClient(message.ChannelData);
                var mailMessage = CreateMessage(message);

                var response = await client.SendEmailAsync(mailMessage, cancellationToken)
                    .ConfigureAwait(false);

                // build the result object and return
                return await BuildResultObjectAsync(response)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="SendgridEmailDeliveryChannel"/>
    /// </summary>
    public partial class SendgridEmailDeliveryChannel
    {
        /// <summary>
        /// the name of the email delivery channel
        /// </summary>
        public const string Name = "sendgrid_channel";

        /// <inheritdoc/>
        string IEmailDeliveryChannel.Name => Name;

        private readonly SendgridEmailDeliveryChannelOptions _options;

        /// <summary>
        /// create an instance of <see cref="SendgridEmailDeliveryChannel"/>
        /// </summary>
        /// <param name="options">the channel options instance</param>
        /// <exception cref="ArgumentNullException">if the given channel options is null</exception>
        public SendgridEmailDeliveryChannel(SendgridEmailDeliveryChannelOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        private SendGridClient CreateClient(IEnumerable<ChannelData> data)
        {
            var apiKey = _options.ApiKey;

            // get the apiKey & serverId from the data list if any.
            var apikeyChannelData = data.GetData(ChannelData.Keys.ApiKey);

            if (!apikeyChannelData.IsEmpty())
                apiKey = apikeyChannelData.GetValue<string>();

            return new SendGridClient(apiKey);
        }

        private static async Task<EmailSendingResult> BuildResultObjectAsync(Response result)
        {
            // check if we have success operations
            if (result.IsSuccessStatusCode)
            {
                // return the result
                return EmailSendingResult.Success(Name);
            }

            // create the failure result & return the result
            var emailSendingResult = EmailSendingResult.Failure(Name)
                .AddMetaData("status_code", result.StatusCode.ToString());

            var content = await result.Body.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<SendGridResponse>(content);

            foreach (var error in response.Errors)
            {
                emailSendingResult.AddError(new EmailSendingError(
                    code: error.Field,
                    message: error.Message));
            }

            return emailSendingResult;
        }

        /// <summary>
        /// create an instance of <see cref="SendGridMessage"/> from the given <see cref="EmailMessage"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="SendGridMessage"/></returns>
        public SendGridMessage CreateMessage(EmailMessage message)
        {
            var mailMessage = new SendGridMessage
            {
                Subject = message.Subject,
                HtmlContent = message.HtmlBody,
                PlainTextContent = message.PlainTextBody,
                From = new EmailAddress(message.From.Address, message.From.DisplayName),
            };

            var trackingSettingsChannel = message.ChannelData.GetData(CustomChannelData.TrackingSettings);
            if (!trackingSettingsChannel.IsEmpty())
                mailMessage.TrackingSettings = trackingSettingsChannel.GetValue<TrackingSettings>();

            if (!(message.ReplyTo is null) && message.ReplyTo.Any())
            {
                var replayTo = message.ReplyTo.First();
                mailMessage.ReplyTo = new EmailAddress(replayTo.Address, replayTo.DisplayName);
            }

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
                    mailMessage.AddCc(email.Address, email.DisplayName);
            }

            if (!(message.Headers is null && message.Headers.Any()))
            {
                mailMessage.Headers = (Dictionary<string, string>)message.Headers;
            }

            SetAttachments(mailMessage, message.Attachments);

            return mailMessage;
        }

        /// <summary>
        /// add the given list of attachments to the <see cref="SendGridMessage"/> instance
        /// </summary>
        /// <param name="message">the <see cref="SendGridMessage"/> instance</param>
        /// <param name="attachments">the list of attachments to add</param>
        private static void SetAttachments(SendGridMessage message, IEnumerable<Net.Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
                return;

            foreach (var attachment in attachments)
            {
                message.AddAttachment(attachment.FileName, attachment.GetAsBase64(), type: attachment.FileType);
            }
        }

        private sealed class SendGridResponse
        {
            [JsonProperty("errors")]
            public List<Error> Errors { get; set; }
        }

        private sealed class Error
        {
            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("field")]
            public string Field { get; set; }

            [JsonProperty("help")]
            public object Help { get; set; }
        }
    }
}