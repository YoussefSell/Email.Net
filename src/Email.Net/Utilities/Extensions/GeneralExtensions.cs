namespace Email.Net
{
    using Email.Net.Channel;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// general extension methods
    /// </summary>
    public static class GeneralExtensions
    {
        /// <summary>
        /// check if the given string is not null or empty or white space
        /// </summary>
        /// <param name="value">the string value to be checked</param>
        /// <returns>true if valid, false if not</returns>
        internal static bool IsValid(this string value)
            => !(string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value));

        /// <summary>
        /// get the data with the given key.
        /// </summary>
        /// <param name="data">the source list.</param>
        /// <param name="key">the data key</param>
        /// <returns>the <see cref="ChannelData"/> instance</returns>
        public static ChannelData GetData(this IEnumerable<ChannelData> data, string key)
            => data.FirstOrDefault(e => e.Key == key);
    }
}
