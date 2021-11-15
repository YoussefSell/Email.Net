namespace Email.NET
{
    using System.Text;

    /// <summary>
    /// this class defines the message body
    /// </summary>
    public partial class MessageBody
    {
        /// <summary>
        /// create an instance of <see cref="MessageBody"/>
        /// </summary>
        /// <param name="content">the message body content</param>
        /// <param name="encoding">the message body encoding</param>
        public MessageBody(string content, Encoding encoding = null)
        {
            Content = content;
            Encoding = encoding;
        }

        /// <summary>
        /// Get the message body content 
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Get the message body encoding
        /// </summary>
        public Encoding Encoding { get; }
    }
}
