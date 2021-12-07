namespace Email.NET.EDP.SendGrid
{
    using global::SendGrid;
    using global::SendGrid.Helpers.Mail;
    using Newtonsoft.Json;
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
        public EmailSendingResult Send(Message message)
            => SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public async Task<EmailSendingResult> SendAsync(Message message)
        {
            try
            {
                var client = CreateClient(message.EdpData);
                var mailMessage = CreateMessage(message);

                var response = await client.SendEmailAsync(mailMessage)
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

        /// <summary>
        /// create an instance of <see cref="SendgridEmailDeliveryProvider"/>
        /// </summary>
        /// <param name="options">the edp options instance</param>
        /// <exception cref="ArgumentNullException">if the given provider options is null</exception>
        public SendgridEmailDeliveryProvider(SendgridEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        private SendGridClient CreateClient(IEnumerable<EdpData> data)
        {
            var apiKey = _options.ApiKey;

            // get the apiKey & serverId from the data list if any.
            var apikeyEdpData = data.GetData(EdpData.Keys.ApiKey);

            if (!apikeyEdpData.IsEmpty())
                apiKey = apikeyEdpData.GetValue<string>();

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
        /// create an instance of <see cref="SendGridMessage"/> from the given <see cref="Message"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="SendGridMessage"/></returns>
        public SendGridMessage CreateMessage(Message message)
        {
            var mailMessage = new SendGridMessage
            {
                Subject = message.Subject,
                HtmlContent = message.HtmlBody,
                PlainTextContent = message.PlainTextBody,
                From = new EmailAddress(message.From.Address, message.From.DisplayName),
            };

            var trackingSettingsEDP = message.EdpData.GetData("sendgrid_tracking_settings");
            if (!trackingSettingsEDP.IsEmpty())
                mailMessage.TrackingSettings = trackingSettingsEDP.GetValue<TrackingSettings>();

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
        private void SetAttachments(SendGridMessage message, IEnumerable<NET.Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
                return;

            foreach (var attachment in attachments)
            {
                message.AddAttachment(attachment.FileName, attachment.GetAsBase64(), type: attachment.FileType);
            }
        }

        private class SendGridResponse
        {
            [JsonProperty("errors")]
            public List<Error> Errors { get; set; }
        }

        private class Error
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