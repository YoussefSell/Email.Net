namespace Email.Net.EDP.AmazonSES
{
    using Amazon.SimpleEmail;
    using Amazon.SimpleEmail.Model;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// the AmazonSES client email delivery provider
    /// </summary>
    public partial class AmazonSESEmailDeliveryProvider : IAmazonSESEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message)
            => SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public async Task<EmailSendingResult> SendAsync(EmailMessage message)
        {
            try
            {
                using (var client = CreateClient())
                {
                    var response = await client.SendEmailAsync(CreateMessage(message))
                        .ConfigureAwait(false);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return EmailSendingResult.Success(Name)
                            .AddMetaData("message_id", response.MessageId)
                            .AddMetaData("request_id", response.ResponseMetadata?.RequestId);
                    }

                    var result = EmailSendingResult.Failure(Name)
                        .AddMetaData("message_id", response.MessageId)
                        .AddMetaData("request_id", response.ResponseMetadata?.RequestId);

                    if (response.ResponseMetadata is not null && response.ResponseMetadata.Metadata.Any())
                    {
                        foreach (var metaData in response.ResponseMetadata.Metadata)
                            result.AddMetaData(metaData.Key, metaData.Value);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="AmazonSESEmailDeliveryProvider"/>
    /// </summary>
    public partial class AmazonSESEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "amazon_ses_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly AmazonSESEmailDeliveryProviderOptions _options;

        /// <summary>
        /// create an instance of <see cref="AmazonSESEmailDeliveryProvider"/>
        /// </summary>
        /// <param name="options">the options instance</param>
        /// <exception cref="ArgumentNullException">the given options instance is null</exception>
        public AmazonSESEmailDeliveryProvider(AmazonSESEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        private AmazonSimpleEmailServiceClient CreateClient()
        {
            if (!string.IsNullOrEmpty(_options.AwsAccessKeyId)
                && !string.IsNullOrEmpty(_options.AwsSecretAccessKey)
                && !string.IsNullOrEmpty(_options.AwsSessionToken))
            {
                return new AmazonSimpleEmailServiceClient(_options.AwsAccessKeyId, _options.AwsSecretAccessKey, _options.AwsSessionToken, _options.RegionEndpoint);
            }

            if (!string.IsNullOrEmpty(_options.AwsAccessKeyId)
                && !string.IsNullOrEmpty(_options.AwsSecretAccessKey))
            {
                return new AmazonSimpleEmailServiceClient(_options.AwsAccessKeyId, _options.AwsSecretAccessKey, _options.RegionEndpoint);
            }

            return new AmazonSimpleEmailServiceClient(_options.RegionEndpoint);
        }

        /// <summary>
        /// create an instance of <see cref="SendEmailRequest"/> from the given <see cref="Message"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="SendEmailRequest"/></returns>
        public SendEmailRequest CreateMessage(EmailMessage message)
        {
            var mailMessage = new SendEmailRequest
            {
                Message = new Message(),
                Source = message.From.Address,
                Destination = new Destination(),
            };

            if (!(message.Subject is null))
                mailMessage.Message.Subject = new Content(message.Subject);

            mailMessage.Message.Body = new Body();

            if (!(message.HtmlBody is null))
                mailMessage.Message.Body.Html = new Content(message.HtmlBody);

            if (!(message.PlainTextBody is null))
                mailMessage.Message.Body.Text = new Content(message.PlainTextBody);

            if (!string.IsNullOrEmpty(message.Charset))
            {
                mailMessage.Message.Subject.Charset = message.Charset;
                mailMessage.Message.Body.Html.Charset = message.Charset;
                mailMessage.Message.Body.Text.Charset = message.Charset;
            }

            if (!(message.ReplyTo is null) && message.ReplyTo.Any())
            {
                foreach (var email in message.ReplyTo)
                    mailMessage.ReplyToAddresses.Add(email.Address);
            }

            foreach (var email in message.To)
                mailMessage.Destination.ToAddresses.Add(email.Address);

            if (!(message.Bcc is null) && message.Bcc.Any())
            {
                foreach (var email in message.Bcc)
                    mailMessage.Destination.BccAddresses.Add(email.Address);
            }

            if (!(message.Cc is null) && message.Cc.Any())
            {
                foreach (var email in message.Cc)
                    mailMessage.Destination.CcAddresses.Add(email.Address);
            }

            return mailMessage;
        }
    }
}