namespace Email.NET.EDP.Smtp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Threading.Tasks;

    /// <summary>
    /// the smtp client email delivery provider
    /// </summary>
    public partial class SmtpEmailDeliveryProvider : ISmtpEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, params EdpData[] data)
        {
            try
            {
                using (var client = CreateSmtpClient(GetSmtpOptions(data)))
                using (var mailMessage = CreateMailMessage(message))
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
        public async Task<EmailSendingResult> SendAsync(Message message, params EdpData[] data)
        {
            try
            {
                using (var client = CreateSmtpClient(GetSmtpOptions(data)))
                using (var mailMessage = CreateMailMessage(message))
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
    /// partial part for <see cref="SmtpEmailDeliveryProvider"/>
    /// </summary>
    public partial class SmtpEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "smtp_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly SmtpEmailDeliveryProviderOptions _options;

        /// <summary>
        /// create an instance of <see cref="SmtpEmailDeliveryProvider"/>.
        /// </summary>
        /// <param name="options">the email service options.</param>
        /// <exception cref="ArgumentNullException">if emailDeliveryProviders or options are null.</exception>
        public SmtpEmailDeliveryProvider(SmtpEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        /// <summary>
        /// get the smtp options, if the data list has any custom we will use that, if not we will return the smtp options specified in the EdpOptions
        /// </summary>
        /// <param name="data">the list of edp data</param>
        /// <returns><see cref="SmtpOptions"/> instance</returns>
        private SmtpOptions GetSmtpOptions(EdpData[] data)
        {
            // no data return the default options
            if (data is null || !data.Any())
                return _options.SmtpOptions;

            // check if we have custom smtp options
            var customSmptOptions = data.FirstOrDefault(e => e.Key == EdpData.Keys.SmtpOptions);
            if (customSmptOptions.IsEmpty())
                return _options.SmtpOptions;

            // get the smtp options
            var smptOptions = customSmptOptions.GetValue<SmtpOptions>();

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
        public SmtpClient CreateSmtpClient(SmtpOptions options)
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
        /// create a <see cref="MailMessage"/> instance from the <see cref="Message"/> instance
        /// </summary>
        /// <param name="message">the <see cref="Message"/> instance</param>
        /// <returns>an instance of <see cref="MailMessage"/></returns>
        /// <exception cref="ArgumentNullException">if the <paramref name="message"/>is null</exception>
        public MailMessage CreateMailMessage(Message message)
        {
            var mailMessage = new MailMessage { From = message.From, };

            if (!(message.Subject is null))
            {
                mailMessage.Subject = message.Subject.Content;
                mailMessage.BodyEncoding = message.Subject.Encoding;
            }

            if (!(message.HtmlBody is null))
            {
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message.HtmlBody.Content;
                mailMessage.BodyEncoding = message.HtmlBody.Encoding;
            }
            else if (!(message.PlainTextBody is null))
            {
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = message.PlainTextBody.Content;
                mailMessage.BodyEncoding = message.PlainTextBody.Encoding;
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

            SetAttachments(mailMessage, message.Attachments);

            return mailMessage;
        }

        /// <summary>
        /// add the given list of attachments to the <see cref="MailMessage"/> instance
        /// </summary>
        /// <param name="message">the <see cref="MailMessage"/> instance</param>
        /// <param name="attachments">the list of attachments to add</param>
        public void SetAttachments(MailMessage message, IEnumerable<NET.Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
                return;

            foreach (var file in attachments)
            {
                if (file is ByteArrayAttachment byteArrayAttachment)
                {
                    var imageToInline = new LinkedResource(new MemoryStream(byteArrayAttachment.File))
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
                else if (file is FilePathAttachment filePathAttachment)
                {
                    message.Attachments.Add(new Attachment(filePathAttachment.FilePath));
                }
            }
        }
    }
}