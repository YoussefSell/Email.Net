namespace Email.NET.EDP.Mailgun
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// the Mailgun client email delivery provider
    /// </summary>
    public partial class MailgunEmailDeliveryProvider : IMailgunEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, params EdpData[] data)
            => SendAsync(message, data).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public async Task<EmailSendingResult> SendAsync(Message message, params EdpData[] data)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var buildUri = new UriBuilder()
                    {
                        Host = _options.BaseUrl,
                        Scheme = _options.UseSSL ? "https" : "http",
                        Path = string.Format("{0}/messages", _options.Domain)
                    };

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{_options.ApiKey}")));

                    var response = await client.PostAsync(buildUri.ToString(), CreateMessage(message, data))
                        .ConfigureAwait(false);

                    return EmailSendingResult.Success(Name);
                }
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="MailgunEmailDeliveryProvider"/>
    /// </summary>
    public partial class MailgunEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "mailgun_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly MailgunEmailDeliveryProviderOptions _options;

        public MailgunEmailDeliveryProvider(MailgunEmailDeliveryProviderOptions options)
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
        public HttpContent CreateMessage(Message message, EdpData[] data)
        {
            var content = new MultipartFormDataContent();

            if (!(message.Subject is null))
                content.Add(new StringContent("subject"), message.Subject.Content);

            if (!(message.HtmlBody is null))
                content.Add(new StringContent("html"), message.HtmlBody.Content);

            if (!(message.PlainTextBody is null))
                content.Add(new StringContent("text"), message.PlainTextBody.Content);

            var campaignId = data.GetData(EdpData.Keys.CampaignId);
            if (!campaignId.IsEmpty())
                content.Add(new StringContent("o:campaign"), campaignId.GetValue<string>());

            if (!string.IsNullOrEmpty(message.From.DisplayName))
                content.Add(new StringContent("from"), $"{message.From.DisplayName} <{message.From.Address}>");
            else
                content.Add(new StringContent("from"), message.From.Address);

            if (!(message.Headers is null && message.Headers.Any()))
            {
                foreach (var header in message.Headers)
                    content.Add(new StringContent($"h:{header.Key}"), header.Value);
            }

            foreach (var email in message.To)
            {
                if (!string.IsNullOrEmpty(email.DisplayName))
                    content.Add(new StringContent("to"), $"{email.DisplayName} <{email.Address}>");
                else
                    content.Add(new StringContent("to"), email.Address);
            }

            if (!(message.ReplyTo is null) && message.ReplyTo.Any())
            {
                foreach (var email in message.ReplyTo)
                {
                    if (!string.IsNullOrEmpty(email.DisplayName))
                        content.Add(new StringContent("h:Reply-To"), $"{email.DisplayName} <{email.Address}>");
                    else
                        content.Add(new StringContent("h:Reply-To"), email.Address);
                }
            }

            if (!(message.Bcc is null) && message.Bcc.Any())
            {
                foreach (var email in message.Bcc)
                {
                    if (!string.IsNullOrEmpty(email.DisplayName))
                        content.Add(new StringContent("bcc"), $"{email.DisplayName} <{email.Address}>");
                    else
                        content.Add(new StringContent("bcc"), email.Address);
                }
            }

            if (!(message.Cc is null) && message.Cc.Any())
            {
                foreach (var email in message.Cc)
                {
                    if (!string.IsNullOrEmpty(email.DisplayName))
                        content.Add(new StringContent("cc"), $"{email.DisplayName} <{email.Address}>");
                    else
                        content.Add(new StringContent("cc"), email.Address);
                }
            }

            if (!(message.Attachments is null) && message.Attachments.Any())
            {
                foreach (var attachment in message.Attachments)
                {
                    content.Add(new ByteArrayContent(attachment.GetAsByteArray()), "attachment", attachment.FileName);
                }
            }

            return content;
        }
    }
}