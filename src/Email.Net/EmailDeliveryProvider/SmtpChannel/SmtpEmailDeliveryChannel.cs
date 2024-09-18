namespace Email.Net.Channel.Smtp
{
    using Email.Net.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// the smtp client email delivery channel
    /// </summary>
    public partial class SmtpEmailDeliveryChannel : ISmtpEmailDeliveryChannel
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message)
        {
            try
            {
                using (var client = CreateClient(GetSmtpOptions(message.ChannelData)))
                using (var mailMessage = CreateMessage(message))
                {
                    client.Send(mailMessage);
                }

                return EmailSendingResult.Success(Name);
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
                using (var client = CreateClient(GetSmtpOptions(message.ChannelData)))
                using (var mailMessage = CreateMessage(message))
                {
                    await client.SendMailAsync(mailMessage);
                }

                return EmailSendingResult.Success(Name);
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="SmtpEmailDeliveryChannel"/>
    /// </summary>
    public partial class SmtpEmailDeliveryChannel
    {
        /// <summary>
        /// the name of the email delivery channel
        /// </summary>
        public const string Name = "smtp_channel";

        /// <inheritdoc/>
        string IEmailDeliveryChannel.Name => Name;

        private readonly SmtpEmailDeliveryChannelOptions _options;

        /// <summary>
        /// create an instance of <see cref="SmtpEmailDeliveryChannel"/>.
        /// </summary>
        /// <param name="options">the email service options.</param>
        /// <exception cref="ArgumentNullException">if EmailDeliveryChannels or options are null.</exception>
        public SmtpEmailDeliveryChannel(SmtpEmailDeliveryChannelOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        /// <summary>
        /// get the smtp options, if the data list has any custom we will use that, if not we will return the smtp options specified in the ChannelOptions
        /// </summary>
        /// <param name="data">the list of channel data</param>
        /// <returns><see cref="SmtpOptions"/> instance</returns>
        private SmtpOptions GetSmtpOptions(IEnumerable<ChannelData> data)
        {
            // no data return the default options
            if (data is null || !data.Any())
                return _options.SmtpOptions;

            // check if we have custom smtp options
            var customSmptOptionsChannel = data.GetData(ChannelData.Keys.SmtpOptions);
            if (customSmptOptionsChannel.IsEmpty())
                return _options.SmtpOptions;

            // get the smtp options
            var smptOptions = customSmptOptionsChannel.GetValue<SmtpOptions>();

            // check if valid
            smptOptions.Validate();

            // all done
            return smptOptions;
        }

        /// <summary>
        /// create an <see cref="SmtpClient"/> instance using the given <see cref="SmtpOptions"/>, 
        /// </summary>
        /// <param name="options">the <see cref="SmtpOptions"/> instance</param>
        /// <returns><see cref="SmtpClient"/> instance</returns>
        public SmtpClient CreateClient(SmtpOptions options)
        {
            return new SmtpClient(options.Host, options.Port)
            {
                Timeout = options.Timeout,
                EnableSsl = options.EnableSsl,
                TargetName = options.TargetName,
                Credentials = options.Credentials,
                DeliveryFormat = options.DeliveryFormat,
                DeliveryMethod = options.DeliveryMethod,
                UseDefaultCredentials = options.UseDefaultCredentials,
                PickupDirectoryLocation = options.PickupDirectoryLocation,
            };
        }

        /// <summary>
        /// create a <see cref="MailMessage"/> instance from the <see cref="EmailMessage"/> instance
        /// </summary>
        /// <param name="message">the <see cref="EmailMessage"/> instance</param>
        /// <returns>an instance of <see cref="MailMessage"/></returns>
        /// <exception cref="ArgumentNullException">if the <paramref name="message"/>is null</exception>
        public MailMessage CreateMessage(EmailMessage message)
        {
            var mailMessage = new MailMessage
            {
                From = message.From,
                Subject = message.Subject
            };

            if (!string.IsNullOrEmpty(message.HtmlBody) && !string.IsNullOrEmpty(message.PlainTextBody))
            {
                mailMessage.Body = message.PlainTextBody;
                mailMessage.IsBodyHtml = false;

                mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
                    message.HtmlBody, new ContentType("text/html; charset=UTF-8")));
            }
            else if (!(message.HtmlBody is null))
            {
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message.HtmlBody;
            }
            else if (!(message.PlainTextBody is null))
            {
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = message.PlainTextBody;
            }

            var encoding = message.GetEncodingFromCharset();
            if (encoding is not null)
            {
                mailMessage.BodyEncoding = encoding;
                mailMessage.SubjectEncoding = encoding;
            }

            if (!(message.ReplyTo is null) && message.ReplyTo.Any())
            {
                foreach (var email in message.ReplyTo)
                    mailMessage.ReplyToList.Add(email);
            }

            foreach (var email in message.To)
                mailMessage.To.Add(email);

            if (!(message.Bcc is null) && message.Bcc.Any())
            {
                foreach (var email in message.Bcc)
                    mailMessage.Bcc.Add(email);
            }

            if (!(message.Cc is null) && message.Cc.Any())
            {
                foreach (var email in message.Cc)
                    mailMessage.CC.Add(email);
            }

            if (!(message.Headers is null && message.Headers.Any()))
            {
                foreach (var header in message.Headers)
                    mailMessage.Headers.Add(header.Key, header.Value);
            }

            switch (message.Priority)
            {
                case Priority.Low:
                    mailMessage.Priority = MailPriority.Low; break;
                case Priority.High:
                    mailMessage.Priority = MailPriority.High; break;
                default:
                    mailMessage.Priority = MailPriority.Normal; break;
            }

            SetAttachments(mailMessage, message.Attachments);

            return mailMessage;
        }

        /// <summary>
        /// add the given list of attachments to the <see cref="MailMessage"/> instance
        /// </summary>
        /// <param name="message">the <see cref="MailMessage"/> instance</param>
        /// <param name="attachments">the list of attachments to add</param>
        private void SetAttachments(MailMessage message, IEnumerable<Net.Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
                return;

            foreach (var file in attachments)
            {
                if (file is FilePathAttachment filePathAttachment)
                {
                    message.Attachments.Add(new Attachment(filePathAttachment.FilePath));
                    continue;
                }

                var imageToInline = new LinkedResource(new MemoryStream(file.GetAsByteArray()))
                {
                    ContentId = file.FileName,
                    ContentType = new ContentType(file.FileType),
                };

                var attachment = new Attachment(imageToInline.ContentStream, imageToInline.ContentType)
                {
                    Name = file.FileName,
                    TransferEncoding = TransferEncoding.Base64
                };

                message.Attachments.Add(attachment);
            }
        }
    }
}