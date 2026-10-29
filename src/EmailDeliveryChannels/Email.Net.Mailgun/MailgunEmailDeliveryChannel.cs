namespace Email.Net.Channel.Mailgun
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// the Mailgun client email delivery channel
    /// </summary>
    public partial class MailgunEmailDeliveryChannel : IMailgunEmailDeliveryChannel
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message)
            => SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public async Task<EmailSendingResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            // only dispose the HttpClient if we own it
            var client = _httpClient ?? new HttpClient();

            try
            {
                var buildUri = new UriBuilder()
                {
                    Host = _options.BaseUrl,
                    Scheme = _options.UseSSL ? "https" : "http",
                    Path = string.Format("{0}/messages", _options.Domain)
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, buildUri.ToString())
                {
                    Content = CreateMessage(message),
                };

                // set the authorization on the request, not on the client, since the client could be shared
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{_options.ApiKey}")));

                using var response = await client.SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return EmailSendingResult.Success(Name);

                return EmailSendingResult.Failure(Name)
                    .AddError(new EmailSendingError(((int)response.StatusCode).ToString(), response.StatusCode.ToString()))
                    .AddMetaData("api_response", await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
            finally
            {
                if (_httpClient is null)
                    client.Dispose();
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="MailgunEmailDeliveryChannel"/>
    /// </summary>
    public partial class MailgunEmailDeliveryChannel
    {
        /// <summary>
        /// the name of the email delivery channel
        /// </summary>
        public const string Name = "mailgun_channel";

        /// <inheritdoc/>
        string IEmailDeliveryChannel.Name => Name;

        private readonly MailgunEmailDeliveryChannelOptions _options;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// create an instance of <see cref="MailgunEmailDeliveryChannel"/>
        /// </summary>
        /// <param name="options">the channel options instance</param>
        /// <exception cref="ArgumentNullException">if the given channel options is null</exception>
        public MailgunEmailDeliveryChannel(MailgunEmailDeliveryChannelOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        /// <summary>
        /// create an instance of <see cref="MailgunEmailDeliveryChannel"/> that uses the given <see cref="HttpClient"/> to send the requests.
        /// </summary>
        /// <param name="options">the channel options instance</param>
        /// <param name="httpClient">the <see cref="HttpClient"/> instance used to call the Mailgun api</param>
        /// <exception cref="ArgumentNullException">if the given channel options or httpClient is null</exception>
        public MailgunEmailDeliveryChannel(MailgunEmailDeliveryChannelOptions options, HttpClient httpClient)
            : this(options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// create an instance of <see cref="HttpClient"/> from the given <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="HttpClient"/></returns>
        public HttpContent CreateMessage(EmailMessage message)
        {
            var content = new MultipartFormDataContent();

            if (!(message.Subject is null))
                content.Add(new StringContent(message.Subject), "subject");

            if (!(message.HtmlBody is null))
                content.Add(new StringContent(message.HtmlBody), "html");

            if (!(message.PlainTextBody is null))
                content.Add(new StringContent(message.PlainTextBody), "text");

            var campaignId = message.ChannelData.GetData(ChannelData.Keys.CampaignId);
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

            var enableTrackingChannel = message.ChannelData.GetData(CustomChannelData.EnableTracking);
            if (!enableTrackingChannel.IsEmpty())
                content.Add(new StringContent(enableTrackingChannel.GetValue<bool>().ToYesNoString()), "o:tracking");

            var testModeChannel = message.ChannelData.GetData(CustomChannelData.TestMode);
            if (!testModeChannel.IsEmpty())
                content.Add(new StringContent(testModeChannel.GetValue<bool>().ToYesNoString()), "o:testmode");

            return content;
        }
    }
}