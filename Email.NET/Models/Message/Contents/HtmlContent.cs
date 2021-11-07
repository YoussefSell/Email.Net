namespace Email.NET
{
    using System.Text;

    /// <summary>
    /// defines an HTML message content type
    /// </summary>
    public partial class HtmlContent : IMessageContent
    {
        private readonly string _body;
        private readonly string _subject;
        private readonly Encoding _bodyEncoding;
        private readonly Encoding _subjectEncoding;

        /// <summary>
        /// create an instance of <see cref="PlainTextContent"/> with a subject and a body set to string.Empty.
        /// </summary>
        public HtmlContent() : this(string.Empty, string.Empty, null, null) { }

        /// <summary>
        /// create an instance of <see cref="HtmlContent"/> with a subject and a body.
        /// </summary>
        /// <param name="subject">the subject content.</param>
        /// <param name="body">the body content.</param>
        public HtmlContent(string subject, string body) : this(subject, body, null, null) { }

        /// <summary>
        /// create an instance of <see cref="PlainTextContent"/> with a subject and a body.
        /// </summary>
        /// <param name="subject">the subject content.</param>
        /// <param name="body">the body content.</param>
        /// <param name="subjectEncoding">the body content encoding.</param>
        /// <param name="bodyEncoding">the body content encoding.</param>
        public HtmlContent(string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding)
        {
            _body = body ?? string.Empty;
            _subject = subject ?? string.Empty;
            _bodyEncoding = bodyEncoding;
            _subjectEncoding = subjectEncoding;
        }

        /// <inheritdoc/>
        public MessageBodyType GetBodyType() => MessageBodyType.Html;

        /// <inheritdoc/>
        public string GetBody() => _body;

        /// <inheritdoc/>
        public string GetSubject() => _subject;

        /// <inheritdoc/>
        public Encoding GetBodyEncoding() => _bodyEncoding;

        /// <inheritdoc/>
        public Encoding GetSubjectEncoding() => _subjectEncoding;
    }
}
