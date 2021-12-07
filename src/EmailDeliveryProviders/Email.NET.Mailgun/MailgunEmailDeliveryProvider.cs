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
        public EmailSendingResult Send(Message message)
            => SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public async Task<EmailSendingResult> SendAsync(Message message)
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

                    var response = await client.PostAsync(buildUri.ToString(), CreateMessage(message))
                        .ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return EmailSendingResult.Success(Name);

                    return EmailSendingResult.Failure(Name)
                        .AddError(new EmailSendingError(((int)response.StatusCode).ToString(), response.StatusCode.ToString()))
                        .AddMetaData("api_response", await response.Content.ReadAsStringAsync());
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

        /// <summary>
        /// create an instance of <see cref="MailgunEmailDeliveryProvider"/>
        /// </summary>
        /// <param name="options">the edp options instance</param>
        /// <exception cref="ArgumentNullException">if the given provider options is null</exception>
        public MailgunEmailDeliveryProvider(MailgunEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        /// <summary>
        /// create an instance of <see cref="HttpClient"/> from the given <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="HttpClient"/></returns>
        public HttpContent CreateMessage(Message message)
        {
            var content = new MultipartFormDataContent();

            if (!(message.Subject is null))
                content.Add(new StringContent(message.Subject), "subject");

            if (!(message.HtmlBody is null))
                content.Add(new StringContent(message.HtmlBody), "html");

            if (!(message.PlainTextBody is null))
                content.Add(new StringContent(message.PlainTextBody), "text");

            var campaignId = message.EdpData.GetData(EdpData.Keys.CampaignId);
            if (!campaignId.IsEmpty())
                content.Add(new StringContent(campaignId.GetValue<string>()), "o:campaign");

            if (!string.IsNullOrEmpty(message.From.DisplayName))
                content.Add(new StringContent($"{message.From.DisplayName} <{message.From.Address}>"), "from");
            else
                content.Add(new StringContent(message.From.Address), "from");

            if (!(message.ReplyTo is null) && message.ReplyTo.Any())
            {
                foreach (var email in message.ReplyTo)
                {
                    if (!string.IsNullOrEmpty(email.DisplayName))
                        content.Add(new StringContent($"{email.DisplayName} <{email.Address}>"), "h:Reply-To");
                    else
                        content.Add(new StringContent(email.Address), "h:Reply-To");
                }
            }

            foreach (var email in message.To)
            {
                if (!string.IsNullOrEmpty(email.DisplayName))
                    content.Add(new StringContent($"{email.DisplayName} <{email.Address}>"), "to");
                else
                    content.Add(new StringContent(email.Address), "to");
            }

            if (!(message.Bcc is null) && message.Bcc.Any())
            {
                foreach (var email in message.Bcc)
                {
                    if (!string.IsNullOrEmpty(email.DisplayName))
                        content.Add(new StringContent($"{email.DisplayName} <{email.Address}>"), "bcc");
                    else
                        content.Add(new StringContent(email.Address), "bcc");
                }
            }

            if (!(message.Cc is null) && message.Cc.Any())
            {
                foreach (var email in message.Cc)
                {
                    if (!string.IsNullOrEmpty(email.DisplayName))
                        content.Add(new StringContent($"{email.DisplayName} <{email.Address}>"), "cc");
                    else
                        content.Add(new StringContent(email.Address), "cc");
                }
            }

            if (!(message.Headers is null && message.Headers.Any()))
            {
                foreach (var header in message.Headers)
                    content.Add(new StringContent(header.Value), $"h:{header.Key}");
            }

            if (!(message.Attachments is null) && message.Attachments.Any())
            {
                foreach (var attachment in message.Attachments)
                {
                    content.Add(new ByteArrayContent(attachment.GetAsByteArray()), "attachment", attachment.FileName);
                }
            }

            var enableTrackingEdp = message.EdpData.GetData("enable_tracking");
            if (!enableTrackingEdp.IsEmpty())
                content.Add(new StringContent(enableTrackingEdp.GetValue<bool>().ToYesNoString()), "o:tracking");

            var testModeEdp = message.EdpData.GetData("test_mode");
            if (!testModeEdp.IsEmpty())
                content.Add(new StringContent(testModeEdp.GetValue<bool>().ToYesNoString()), "o:testmode");

            return content;
        }
    }
}