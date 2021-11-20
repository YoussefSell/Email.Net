namespace Email.NET
{
    using System.Text;

    /// <summary>
    /// defines the message content
    /// </summary>
    public interface IMessageContent
    {
        /// <summary>
        /// get the message subject
        /// </summary>
        /// <returns>the message subject as a string.</returns>
        string GetSubject();

        /// <summary>
        /// get the encoding of the subject
        /// </summary>
        /// <returns>the subject encoding</returns>
        Encoding GetSubjectEncoding();

        /// <summary>
        /// get the message body content.
        /// </summary>
        /// <returns>the message body as a string.</returns>
        string GetBody();

        /// <summary>
        /// get the encoding of the body
        /// </summary>
        /// <returns>the body encoding</returns>
        Encoding GetBodyEncoding();

        /// <summary>
        /// get the message body content type
        /// </summary>
        /// <returns>type of the message body</returns>
        MessageBodyType GetBodyType();
    }
}
