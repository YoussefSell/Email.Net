namespace Email.NET
{
    using System;

    /// <summary>
    /// the email delivery provider data
    /// </summary>
    public struct EdpData
    {
        /// <summary>
        /// the email delivery provider data
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public EdpData(string key, string value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the key of the data.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the value of the data.
        /// </summary>
        public object Value { get; }
    }
}
