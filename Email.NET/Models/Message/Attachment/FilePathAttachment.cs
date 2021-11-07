namespace Email.NET
{
    using System;
    using System.IO;

    /// <summary>
    /// attachment represented by the file path
    /// </summary>
    public class FilePathAttachment : Attachment
    {
        /// <summary>
        /// create an instance of <see cref="FilePathAttachment"/>
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="ArgumentNullException">file path is null.</exception>
        /// <exception cref="FileNotFoundException">the file at the specified path doesn't exist.</exception>
        /// <exception cref="ArgumentException">path contains one or more of the invalid characters defined in System.IO.Path.GetInvalidPathChars, or the value is empty.</exception>
        public FilePathAttachment(string filePath)
            : base(Path.GetFileName(filePath))
        {
            if (filePath is null)
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("the file path is empty, you must supply a valid file path.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("the given file doesn't exist.", filePath);

            FilePath = filePath;
        }

        /// <summary>
        /// the path to the file this attachment is referring to
        /// </summary>
        public string FilePath { get; }
    }
}
