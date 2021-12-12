namespace Email.NET.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;

    /// <summary>
    /// a factory for creating the mail message.
    /// </summary>
    public partial class MessageComposer
    {
        private Priority _priority;
        
        private string _charset;
        private string _subjectContent;
        private string _htmlBodyContent;
        private string _plainTextBodyContent;

        private readonly HashSet<MailAddress> _to;
        private readonly HashSet<MailAddress> _bcc;
        private readonly HashSet<MailAddress> _cc;
        private readonly HashSet<MailAddress> _replyTo;
        private readonly HashSet<NET.Attachment> _attachments;

        private MailAddress _from;
        private Dictionary<string, string> _headers;
        private readonly HashSet<EDP.EdpData> _edpData;

        internal MessageComposer()
        {
            _priority = Priority.Normal;
            _to = new HashSet<MailAddress>();
            _cc = new HashSet<MailAddress>();
            _bcc = new HashSet<MailAddress>();
            _replyTo = new HashSet<MailAddress>();
            _edpData = new HashSet<EDP.EdpData>();
            _attachments = new HashSet<NET.Attachment>();
            _headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// set the message subject content.
        /// </summary>
        /// <param name="subject">the message subject content</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public MessageComposer WithSubject(string subject)
        {
            _subjectContent = subject;
            return this;
        }

        /// <summary>
        /// set the message HTML content.
        /// </summary>
        /// <param name="htmlBody">the message HTML content</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public MessageComposer WithHtmlContent(string htmlBody)
        {
            _htmlBodyContent = htmlBody;
            return this;
        }

        /// <summary>
        /// set the message plain text content.
        /// </summary>
        /// <param name="plainTextBody">the message plain text content</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public MessageComposer WithPlainTextContent(string plainTextBody)
        {
            _plainTextBodyContent = plainTextBody;
            return this;
        }

        /// <summary>
        /// set the character set for your message.
        /// </summary>
        /// <param name="charset">the charset.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public MessageComposer SetCharsetTo(string charset)
        {
            _charset = charset;
            return this;
        }

        /// <summary>
        /// add the send From email address.
        /// </summary>
        /// <param name="emailAddress">sender email address.</param>
        /// <param name="displayName">the name of the sender.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public MessageComposer From(string emailAddress, string displayName = "")
            => From(new MailAddress(emailAddress, displayName));

        /// <summary>
        /// add the send From email address.
        /// </summary>
        /// <param name="mailAddress">the sender email address.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public MessageComposer From(MailAddress mailAddress)
        {
            _from = mailAddress;
            return this;
        }

        /// <summary>
        /// add the send To email address.
        /// </summary>
        /// <param name="emailAddress">recipient email address.</param>
        /// <param name="displayName">name of the recipient, only supply this value if you are passing one email address, if you're passing multiple email this value will be ignored.</param>
        /// <param name="delimiter">if the email address is a list of email you can supply the delimiter of emails., by default is set to ";"</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer To(string emailAddress, string displayName = "", char delimiter = ';')
        {
            if (emailAddress is null)
                throw new ArgumentNullException(nameof(emailAddress));

            if (emailAddress == string.Empty)
                throw new AggregateException("the given email address is empty.");

            if (delimiter == default)
                delimiter = ';';

            // try to split the email address
            var emails = emailAddress.Split(delimiter);

            // if we have only one email
            if (emails.Length == 1)
            {
                _to.Add(new MailAddress(emailAddress, displayName));
                return this;
            }

            // add the email address
            foreach (var email in emails)
                _to.Add(new MailAddress(email));

            return this;
        }

        /// <summary>
        /// add the send To email address.
        /// </summary>
        /// <param name="mailAddress">recipient email address.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer To(params MailAddress[] mailAddress)
        {
            foreach (var address in mailAddress)
                _to.Add(address);

            return this;
        }

        /// <summary>
        /// add the ReplyTo To email address.
        /// </summary>
        /// <param name="emailAddress">replayTo email address.</param>
        /// <param name="displayName">name of the replayTo user, only supply this value if you are passing one email address, if you're passing multiple email this value will be ignored.</param>
        /// <param name="delimiter">if the email address is a list of email you can supply the delimiter of emails., by default is set to ";"</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer ReplyTo(string emailAddress, string displayName = "", char delimiter = ';')
        {
            if (emailAddress is null)
                throw new ArgumentNullException(nameof(emailAddress));

            if (emailAddress == string.Empty)
                throw new AggregateException("the given email address is empty.");

            if (delimiter == default)
                delimiter = ';';

            // try to split the email address
            var emails = emailAddress.Split(delimiter);

            // if we have only one email
            if (emails.Length == 1)
            {
                _replyTo.Add(new MailAddress(emailAddress, displayName));
                return this;
            }

            // add the email address
            foreach (var email in emails)
                _replyTo.Add(new MailAddress(email));

            return this;
        }

        /// <summary>
        /// add the ReplyTo email address.
        /// </summary>
        /// <param name="mailAddress">replyTo email address.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer ReplyTo(params MailAddress[] mailAddress)
        {
            foreach (var address in mailAddress)
                _replyTo.Add(address);

            return this;
        }

        /// <summary>
        /// add the Bcc To email address.
        /// </summary>
        /// <param name="emailAddress">Bcc email address.</param>
        /// <param name="displayName">name of the Bcc user.</param>
        /// <param name="delimiter">if the email address is a list of email you can supply the delimiter of emails., by default is set to ";"</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithBcc(string emailAddress, string displayName = "", char delimiter = ';')
        {
            if (emailAddress is null)
                throw new ArgumentNullException(nameof(emailAddress));

            if (emailAddress == string.Empty)
                throw new AggregateException("the given email address is empty.");

            if (delimiter == default)
                delimiter = ';';

            // try to split the email address
            var emails = emailAddress.Split(delimiter);

            // if we have only one email
            if (emails.Length == 1)
            {
                _bcc.Add(new MailAddress(emailAddress, displayName));
                return this;
            }

            // add the email address
            foreach (var email in emails)
                _bcc.Add(new MailAddress(email));

            return this;
        }

        /// <summary>
        /// add the Bcc email address.
        /// </summary>
        /// <param name="mailAddress">Bcc email address.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithBcc(params MailAddress[] mailAddress)
        {
            foreach (var address in mailAddress)
                _bcc.Add(address);

            return this;
        }

        /// <summary>
        /// add the Cc To email address.
        /// </summary>
        /// <param name="emailAddress">Cc email address.</param>
        /// <param name="displayName">name of the Cc user.</param>
        /// <param name="delimiter">if the email address is a list of email you can supply the delimiter of emails., by default is set to ";"</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithCc(string emailAddress, string displayName = "", char delimiter = ';')
        {
            if (emailAddress is null)
                throw new ArgumentNullException(nameof(emailAddress));

            if (emailAddress == string.Empty)
                throw new AggregateException("the given email address is empty.");

            if (delimiter == default)
                delimiter = ';';

            // try to split the email address
            var emails = emailAddress.Split(delimiter);

            // if we have only one email
            if (emails.Length == 1)
            {
                _cc.Add(new MailAddress(emailAddress, displayName));
                return this;
            }

            // add the email address
            foreach (var email in emails)
                _cc.Add(new MailAddress(email));

            return this;
        }

        /// <summary>
        /// add the Cc email address.
        /// </summary>
        /// <param name="mailAddress">Cc email address.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithCc(params MailAddress[] mailAddress)
        {
            foreach (var address in mailAddress)
                _cc.Add(address);

            return this;
        }

        /// <summary>
        /// add a new header value.
        /// </summary>
        /// <param name="key">the header key.</param>
        /// <param name="value">the header value.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithHeader(string key, string value)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _headers.Add(key, value);
            return this;
        }

        /// <summary>
        /// set the headers value, this will overwrite the internal list with provided list.
        /// </summary>
        /// <param name="headers">the headers list.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            if (headers is null)
                throw new ArgumentNullException(nameof(headers));

            foreach (var header in headers)
            {
                // if the header exist update the value
                if (_headers.ContainsKey(header.Key))
                {
                    _headers[header.Key] = header.Value;
                    continue;
                }

                _headers.Add(header.Key, header.Value);
            }

            return this;
        }

        /// <summary>
        /// set the message priority to High.
        /// </summary>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithHighPriority()
        {
            _priority = Priority.High;
            return this;
        }

        /// <summary>
        /// set the message priority to Low.
        /// </summary>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithLowPriority()
        {
            _priority = Priority.Low;
            return this;
        }

        /// <summary>
        /// set the message priority to Normal.
        /// </summary>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer WithNormalPriority()
        {
            _priority = Priority.Normal;
            return this;
        }

        /// <summary>
        /// add the data to be passed to the edp.
        /// </summary>
        /// <param name="key">the data key.</param>
        /// <param name="value">the data value.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer PassEdpData(string key, object value)
            => PassEdpData(EDP.EdpData.New(key, value));

        /// <summary>
        /// add the data to be passed to the edp.
        /// </summary>
        /// <param name="data">the data instance.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer PassEdpData(params EDP.EdpData[] data)
        {
            foreach (var item in data)
                _edpData.Add(item);

            return this;
        }

        /// <summary>
        /// add the attachments to the message.
        /// </summary>
        /// <param name="attachments">the list of attachments</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining</returns>
        public MessageComposer IncludeAttachment(params NET.Attachment[] attachments)
        {
            foreach (var attachment in attachments)
                _attachments.Add(attachment);

            return this;
        }

        /// <summary>
        /// build the <see cref="Message"/> instance.
        /// </summary>
        /// <returns>Instance of <see cref="Message"/>.</returns>
        public Message Build()
            => new Message(
                _subjectContent, _plainTextBodyContent, _htmlBodyContent, 
                _charset, _from, _to, _priority, _replyTo, _bcc, _cc, 
                _attachments, _headers, _edpData
            );
    }
}
