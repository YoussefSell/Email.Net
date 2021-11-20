namespace Email.NET.EDP.MailKit
{
    using global::MailKit.Net.Smtp;
    using global::MailKit.Security;
    using MimeKit;
    using System;
    using System.Collections.Generic;
using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// the MailKit client email delivery provider
    /// </summary>
    public partial class MailKitEmailDeliveryProvider : IMailKitEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, params EdpData[] data)
        {
            try
            {
                // get the smtp options & build message
                var smtpOptions = GetSmtpOptions(data);
                var mailMessage = CreateMessage(message);

                if (smtpOptions.DeliveryMethod == MailKitDeliveryMethod.SpecifiedPickupDirectory)
                {
                    using (var stream = new FileStream(Path.Combine(smtpOptions.PickupDirectoryLocation, $"{Guid.NewGuid()}.eml"), FileMode.OpenOrCreate))
                    {
                        mailMessage.WriteTo(stream);
                    }

                    return EmailSendingResult.Success(Name);
                }

                using (var client = new SmtpClient())
                {
                    // set the smtp connection options
                    if (smtpOptions.SecureSocketOptions == SecureSocketOptions.None)
                    {
                        client.Connect(
                            smtpOptions.Host,
                            smtpOptions.Port,
                            smtpOptions.SecureSocketOptions);
                    }
                    else
                    {
                        client.Connect(
                            smtpOptions.Host,
                            smtpOptions.Port,
                            smtpOptions.EnableSsl);
                    }

                    // add authentication
                    if (!string.IsNullOrEmpty(smtpOptions.UserName) && !string.IsNullOrEmpty(smtpOptions.Password))
                        client.Authenticate(smtpOptions.UserName, smtpOptions.Password);

                    client.Send(mailMessage);
                    client.Disconnect(true);
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
                // get the smtp options & build message
                var smtpOptions = GetSmtpOptions(data);
                var mailMessage = CreateMessage(message);

                if (smtpOptions.DeliveryMethod == MailKitDeliveryMethod.SpecifiedPickupDirectory)
                {
                    using (var stream = new FileStream(Path.Combine(smtpOptions.PickupDirectoryLocation, $"{Guid.NewGuid()}.eml"), FileMode.OpenOrCreate))
                    {
                        await mailMessage.WriteToAsync(stream);
                    }

                    return EmailSendingResult.Success(Name);
                }

                using (var client = new SmtpClient())
                {
                    // set the smtp connection options
                    if (smtpOptions.SecureSocketOptions == SecureSocketOptions.None)
                    {
                        await client.ConnectAsync(
                            smtpOptions.Host,
                            smtpOptions.Port,
                            smtpOptions.SecureSocketOptions);
                    }
                    else
                    {
                        await client.ConnectAsync(
                            smtpOptions.Host,
                            smtpOptions.Port,
                            smtpOptions.EnableSsl);
                    }

                    // add authentication
                    if (!string.IsNullOrEmpty(smtpOptions.UserName) && !string.IsNullOrEmpty(smtpOptions.Password))
                        await client.AuthenticateAsync(smtpOptions.UserName, smtpOptions.Password);

                    await client.SendAsync(mailMessage);
                    await client.DisconnectAsync(true);
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
    /// partial part for <see cref="MailKitEmailDeliveryProvider"/>
    /// </summary>
    public partial class MailKitEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "mailkit_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly MailKitEmailDeliveryProviderOptions _options;

        public MailKitEmailDeliveryProvider(MailKitEmailDeliveryProviderOptions options)
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
        /// create a <see cref="MimeMessage"/> instance from the <see cref="Message"/> instance
        /// </summary>
        /// <param name="message">the <see cref="Message"/> instance</param>
        /// <returns>an instance of <see cref="MimeMessage"/></returns>
        /// <exception cref="ArgumentNullException">if the <paramref name="message"/>is null</exception>
        public MimeMessage CreateMessage(Message message)
        {
            var mailMessage = new MimeMessage();

            mailMessage.From.Add(new MailboxAddress(message.From.DisplayName, message.From.Address));

            if (!(message.Subject is null))
            {
                mailMessage.Subject = message.Subject;
            }

            var bodyBuilder = new BodyBuilder();

            if (!(message.HtmlBody is null))
            {
                bodyBuilder.HtmlBody = message.HtmlBody;
            }
            else if (!(message.PlainTextBody is null))
            {
                bodyBuilder.TextBody = message.PlainTextBody;
            }

            if (!(message.ReplyTo is null) && message.ReplyTo.Any())
            {
                foreach (var email in message.ReplyTo)
                    mailMessage.ReplyTo.Add(new MailboxAddress(email.DisplayName, email.Address));
            }

            foreach (var email in message.To)
                mailMessage.To.Add(new MailboxAddress(email.DisplayName, email.Address));

            if (!(message.Bcc is null) && message.Bcc.Any())
            {
                foreach (var email in message.Bcc)
                    mailMessage.Bcc.Add(new MailboxAddress(email.DisplayName, email.Address));
            }

            if (!(message.Cc is null) && message.Cc.Any())
            {
                foreach (var email in message.Cc)
                    mailMessage.Cc.Add(new MailboxAddress(email.DisplayName, email.Address));
            }

            if (!(message.Headers is null && message.Headers.Any()))
            {
                foreach (var header in message.Headers)
                    mailMessage.Headers.Add(header.Key, header.Value);
            }

            switch (message.Priority)
            {
                case Priority.Low:
                    mailMessage.Priority = MessagePriority.NonUrgent; break;
                case Priority.High:
                    mailMessage.Priority = MessagePriority.Urgent; break;
                default:
                    mailMessage.Priority = MessagePriority.Normal; break;
            }

            SetAttachments(bodyBuilder, message.Attachments);

            return mailMessage;
        }

        /// <summary>
        /// add the given list of attachments to the <see cref="MailMessage"/> instance
        /// </summary>
        /// <param name="message">the <see cref="MailMessage"/> instance</param>
        /// <param name="attachments">the list of attachments to add</param>
        public void SetAttachments(BodyBuilder message, IEnumerable<NET.Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
                return;

            foreach (var attachment in attachments)
            {
                if (attachment is FilePathAttachment filePathAttachment)
                {
                    message.Attachments.Add(attachment.FileName);
                    continue;
                }

                message.Attachments.Add(attachment.FileName, attachment.GetAsByteArray(), ContentType.Parse(attachment.FileType));
            }
        }
    }
}