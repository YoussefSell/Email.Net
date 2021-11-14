namespace Email.NET
{
    using Factories;
    using System.Text;

    /// <summary>
    /// extensions methods over the MessageComposser factory
    /// </summary>
    public static class MessageComposerExtensions
    {
        /// <summary>
        /// set the message HTML content.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="subject">the message subject</param>
        /// <param name="htmlBody">the message HTML content</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer WithHtmlContent(this MessageComposer messageComposer, string subject, string htmlBody)
            => messageComposer.Content(new HtmlContent(subject, htmlBody));

        /// <summary>
        /// set the message HTML content.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="subject">the message subject</param>
        /// <param name="htmlBody">the message HTML content</param>
        /// <param name="subjectEncoding">the body content encoding.</param>
        /// <param name="bodyEncoding">the body content encoding.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer WithHtmlContent(this MessageComposer messageComposer, string subject, string htmlBody, Encoding subjectEncoding, Encoding bodyEncoding)
            => messageComposer.Content(new HtmlContent(subject, htmlBody, subjectEncoding, bodyEncoding));

        /// <summary>
        /// set the message plain text content.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="subject">the message subject</param>
        /// <param name="plainTextBody">the message HTML content</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer WithPlainTextContent(this MessageComposer messageComposer, string subject, string plainTextBody)
            => messageComposer.Content(new PlainTextContent(subject, plainTextBody));

        /// <summary>
        /// set the message plain text content.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="subject">the message subject</param>
        /// <param name="plainTextBody">the message HTML content</param>
        /// <param name="subjectEncoding">the body content encoding.</param>
        /// <param name="bodyEncoding">the body content encoding.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer WithPlainTextContent(this MessageComposer messageComposer, string subject, string plainTextBody, Encoding subjectEncoding, Encoding bodyEncoding)
            => messageComposer.Content(new PlainTextContent(subject, plainTextBody, subjectEncoding, bodyEncoding));
    }
}
