namespace Email.NET.EDP
{
    using System;
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// the email delivery provider data
    /// </summary>
    public partial struct EdpData
    {
        /// <summary>
        /// Gets the key of the data.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the value of the data.
        /// </summary>
        public object Value { get; }
    }

    /// <summary>
    /// partial part for <see cref="EdpData"/>
    /// </summary>
    public partial struct EdpData : IEquatable<EdpData>
    {
        /// <summary>
        /// the email delivery provider data
        /// </summary>
        /// <param name="key">the data key</param>
        /// <param name="value">the data value</param>
        /// <exception cref="ArgumentException">Key is empty</exception>
        /// <exception cref="ArgumentNullException">Key or value are null</exception>
        public EdpData(string key, object value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));

            if (!Key.IsValid())
                throw new ArgumentException("the key value is empty.");
        }

        /// <summary>
        /// create a new instance of <see cref="EdpData"/>
        /// </summary>
        /// <param name="key">the data key</param>
        /// <param name="value">the data value</param>
        /// <returns>instance of <see cref="EdpData"/></returns>
        public static EdpData New(string key, object value)
            => new EdpData(key, value);

        /// <summary>
        /// get the value
        /// </summary>
        /// <typeparam name="TValue">the type of the value</typeparam>
        /// <returns>the value instance</returns>
        public TValue GetValue<TValue>() => (TValue)Value;

        /// <summary>
        /// Indicates whether the specified edp is empty.
        /// </summary>
        /// <returns>true if the value is empty; otherwise, false.</returns>
        public bool IsEmpty() => !Key.IsValid() && Value == default;

        /// <inheritdoc/>
        public override string ToString() => $"key: {Key} | value: {Value}";

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != typeof(EdpData)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return base.Equals((EdpData)obj);
        }

        /// <summary>
        /// check if the given <see cref="EdpData"/> equals the current instance,
        /// the equality is based on the key only.
        /// </summary>
        /// <param name="other">the other <see cref="EdpData"/> instance</param>
        /// <returns>true if equals, false if not.</returns>
        public bool Equals(EdpData other) => other.Key == Key;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 144377059;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Key);
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public static bool operator ==(EdpData left, EdpData right) => EqualityComparer<EdpData>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(EdpData left, EdpData right) => !(left == right);

        /// <summary>
        /// holds some predefined <see cref="EdpData"/> keys
        /// </summary>
        public static class Keys
        {
            /// <summary>
            /// edp data for smtp options.
            /// </summary>
            public const string SmtpOptions = "smtp_options";
            
            /// <summary>
            /// edp data for ApiKey.
            /// </summary>
            public const string ApiKey = "api_key";

            /// <summary>
            /// edp data for ServerId.
            /// </summary>
            public const string ServerId = "server_id";
        }
    }
}
