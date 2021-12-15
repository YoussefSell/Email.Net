namespace Email.Net
{
    using System;
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// base model for all attachment types
    /// </summary>
    public abstract partial class Attachment
    {
        /// <summary>
        /// create an instance of <see cref="Attachment"/>
        /// </summary>
        /// <param name="fileName">the name of the file with extension</param>
        protected Attachment(string fileName)
        {
            if (fileName is null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("the given file name is empty");

            FileName = fileName;
            Extension = FileHelper.GetFileExtension(fileName);

            if (string.IsNullOrEmpty(Extension))
                throw new ArgumentException("the given file name doesn't contains an extension");

            FileType = FileHelper.GetExtensionMimeType(Extension);
        }

        /// <summary>
        /// the original name of the file including the extension
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// the file extension.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// the mime-type of the file
        /// </summary>
        public string FileType { get; }

        /// <summary>
        /// get the file content as a byte array
        /// </summary>
        /// <returns>the file content as a byte array</returns>
        public abstract byte[] GetAsByteArray();

        /// <summary>
        /// get the file content as base64
        /// </summary>
        /// <returns>the file content as base64</returns>
        public abstract string GetAsBase64();
    }

    /// <summary>
    /// partial part for <see cref="Attachment"/>
    /// </summary>
    public partial class Attachment : System.IEquatable<Attachment>
    {
        /// <inheritdoc/>
        public override string ToString()
            => $"name: {FileName}, type: {FileType}";

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != typeof(Attachment)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return base.Equals(obj as Attachment);
        }

        /// <inheritdoc/>
        public bool Equals(Attachment other)
            => !(other is null) && FileName.Equals(other.FileName, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return 144377059 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
            }
        }

        /// <inheritdoc/>
        public static bool operator ==(Attachment left, Attachment right) => EqualityComparer<Attachment>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(Attachment left, Attachment right) => !(left == right);
    }
}
