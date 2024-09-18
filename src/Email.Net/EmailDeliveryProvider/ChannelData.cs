namespace Email.Net.Channel
{
    using System;
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// the email delivery channel data
    /// </summary>
    public readonly partial struct ChannelData
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
    /// partial part for <see cref="ChannelData"/>
    /// </summary>
    public partial struct ChannelData : IEquatable<ChannelData>
    {
        /// <summary>
        /// the email delivery channel data
        /// </summary>
        /// <param name="key">the data key</param>
        /// <param name="value">the data value</param>
        /// <exception cref="ArgumentException">Key is empty</exception>
        /// <exception cref="ArgumentNullException">Key or value are null</exception>
        public ChannelData(string key, object value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));

            if (!Key.IsValid())
                throw new ArgumentException("the key value is empty.");
        }

        /// <summary>
        /// create a new instance of <see cref="ChannelData"/>
        /// </summary>
        /// <param name="key">the data key</param>
        /// <param name="value">the data value</param>
        /// <returns>instance of <see cref="ChannelData"/></returns>
        public static ChannelData New(string key, object value)
            => new(key, value);

        /// <summary>
        /// get the value
        /// </summary>
        /// <typeparam name="TValue">the type of the value</typeparam>
        /// <returns>the value instance</returns>
        public TValue GetValue<TValue>() => (TValue)Value;

        /// <summary>
        /// Indicates whether the specified channel is empty.
        /// </summary>
        /// <returns>true if the value is empty; otherwise, false.</returns>
        public bool IsEmpty() => !Key.IsValid() && Value == default;

        /// <inheritdoc/>
        public override string ToString() => $"key: {Key} | value: {Value}";

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is ChannelData channelData) return Equals(channelData);
            return false;
        }

        /// <inheritdoc/>
        public bool Equals(ChannelData other) => other.Key == Key;

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
        public static bool operator ==(ChannelData left, ChannelData right) => EqualityComparer<ChannelData>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(ChannelData left, ChannelData right) => !(left == right);

        /// <summary>
        /// holds some predefined <see cref="ChannelData"/> keys
        /// </summary>
        public static class Keys
        {
            /// <summary>
            /// channel data for smtp options.
            /// </summary>
            public const string SmtpOptions = "smtp_options";

            /// <summary>
            /// channel data for ApiKey.
            /// </summary>
            public const string ApiKey = "api_key";

            /// <summary>
            /// channel data for messageId.
            /// </summary>
            public const string MessageId = "message_id";

            /// <summary>
            /// channel data for mailingId.
            /// </summary>
            public const string MailingId = "mailing_id";

            /// <summary>
            /// channel data for campainId.
            /// </summary>
            public const string CampaignId = "campaign_id";
        }
    }
}
