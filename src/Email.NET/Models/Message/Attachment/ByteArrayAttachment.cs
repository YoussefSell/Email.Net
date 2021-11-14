namespace Email.NET
{
    using System;
    using System.Linq;

    /// <summary>
    /// attachment represented by the file as a byte array
    /// </summary>
    public class ByteArrayAttachment : Attachment
    {
        /// <summary>
        /// create an instance of <see cref="ByteArrayAttachment"/>.
        /// </summary>
        /// <param name="fileName">the name of the file with extension.</param>
        /// <param name="file">file content as a byte[].</param>
        public ByteArrayAttachment(string fileName, byte[] file)
            : base(fileName)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            if (!file.Any())
                throw new ArgumentException("the file array is empty", nameof(file));

            File = file;
        }

        /// <summary>
        /// the file as a byte[]
        /// </summary>
        public byte[] File { get; }
    }
}
