namespace Email.NET.Providers.SmtpClient
{
    using ResultNet;
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
    public partial class SmtpEmailDeliveryProvider : IEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public Result Send(Message message, params EmailDeliveryProviderData[] data)
        {
            try
            {
                using (var client = CreateSmtpClient(_options.SmtpClientOptions))
                using (var mailMessage = CreateMailMessage(message))
                {
                    client.Send(mailMessage);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure()
                    .WithMessage("failed to send the email, and exception has been raised.")
                    .WithCode(ResultCode.InternalException)
                    .WithErrors(ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Result> SendAsync(Message message, params EmailDeliveryProviderData[] data)
        {
            try
            {
                using (var client = CreateSmtpClient(_options.SmtpClientOptions))
                using (var mailMessage = CreateMailMessage(message))
                {
                    await client.SendMailAsync(mailMessage);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure()
                    .WithMessage("failed to send the email, and exception has been raised.")
                    .WithCode(ResultCode.InternalException)
                    .WithErrors(ex);
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
            this._options = options;
        }

        /// <summary>
        /// create an <see cref="SmtpClient"/> instance using the given <see cref="SmtpClientOptions"/>, 
        /// </summary>
        /// <param name="options">the <see cref="SmtpClientOptions"/> instance</param>
        /// <returns><see cref="SmtpClient"/> instance</returns>
        public SmtpClient CreateSmtpClient(SmtpClientOptions options)
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
        /// create a <see cref="MailMessage"/> instance from the <see cref="SendEmailOptions"/> instance
        /// </summary>
        /// <param name="message">the <see cref="Message"/> instance</param>
        /// <param name="emailSettings">the <see cref="EmailSettings"/> instance</param>
        /// <returns>an instance of <see cref="MailMessage"/></returns>
        /// <exception cref="ArgumentNullException">if the <paramref name="emailSettings"/>is null</exception>
        public MailMessage CreateMailMessage(Message message)
        {
            var mailMessage = new MailMessage
            {
                From = message.From,
                Body = message.Content.GetBody(),
                Subject = message.Content.GetSubject(),
                BodyEncoding = message.Content.GetBodyEncoding(),
                SubjectEncoding = message.Content.GetSubjectEncoding(),
                IsBodyHtml = message.Content.GetBodyType() == MessageBodyType.Html,
            };

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
                    using (var streamBitmap = new MemoryStream(byteArrayAttachment.File))
                    {
                        var imageToInline = new LinkedResource(streamBitmap)
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
                else if (file is FilePathAttachment filePathAttachment)
                {
                    message.Attachments.Add(new Attachment(filePathAttachment.FilePath));
                }
            }
        }
    }
}