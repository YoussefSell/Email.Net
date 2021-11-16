namespace Email.NET
{
    using EDP;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// extensions over list data types
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// get the data with the given key.
        /// </summary>
        /// <param name="data">the source list.</param>
        /// <param name="key">the data key</param>
        /// <returns>the <see cref="EdpData"/> instance</returns>
        public static EdpData GetData(this IEnumerable<EdpData> data, string key)
            => data.FirstOrDefault(e => e.Key == key);
    }
}
