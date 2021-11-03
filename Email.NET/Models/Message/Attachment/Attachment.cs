namespace Email.NET
{
    using Email.NET.Utilities;
    using System.Collections.Generic;

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
            FileName = fileName;
            FileType = FileHelper.GetFileMimeType(fileName);
        }

        /// <summary>
        /// the original name of the file including the extension
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// the mime-type of the file
        /// </summary>
        public string FileType { get; }
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
            => !(other is null) &&
            other.FileName == FileName &&
            other.FileType == FileType;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 144377059;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileType);
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public static bool operator ==(Attachment left, Attachment right) => EqualityComparer<Attachment>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(Attachment left, Attachment right) => !(left == right);
    }
}
