namespace Email.NET
{
    using System.Text;

    /// <summary>
    /// this class defines the message subject
    /// </summary>
    public partial class MessageSubject
    {
        /// <summary>
        /// create an instance of <see cref="MessageSubject"/>
        /// </summary>
        /// <param name="content">the message subject content</param>
        /// <param name="encoding">the message subject encoding</param>
        public MessageSubject(string content, Encoding encoding = null)
        {
            Content = content;
            Encoding = encoding;
        }

        /// <summary>
        /// Get the message subject content 
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Get the message subject encoding
        /// </summary>
        public Encoding Encoding { get; }
    }
}
