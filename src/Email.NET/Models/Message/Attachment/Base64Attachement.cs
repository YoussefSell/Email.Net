namespace Email.NET
{
    using System;

    /// <summary>
    /// attachment represented by the file as a base64 string
    /// </summary>
    public class Base64Attachement : ByteArrayAttachment
    {
        /// <summary>
        /// create an instance of <see cref="ByteArrayAttachment"/>.
        /// </summary>
        /// <param name="fileName">the name of the file with extension.</param>
        /// <param name="base64">the file content as a base64.</param>
        /// <exception cref="ArgumentNullException">base64 is null.</exception>
        /// <exception cref="FormatException">
        /// The length of s, ignoring white-space characters, is not zero or a multiple of
        /// 4. -or- The format of s is invalid. s contains a non-base-64 character, more
        /// than two padding characters, or a non-white space-character among the padding
        /// characters.
        /// </exception>
        public Base64Attachement(string fileName, string base64) 
            : base(fileName, Convert.FromBase64String(base64)) { }
    }
}
