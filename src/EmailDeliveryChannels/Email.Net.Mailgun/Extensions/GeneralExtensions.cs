namespace Email.Net.Channel.Mailgun
{
    internal static class GeneralExtensions
    {
        /// <summary>
        /// Get a boolean value as a string in the "yes" or "no" format
        /// </summary>
        /// <param name="value">the boolean value</param>
        /// <returns>string of est if true, no if false</returns>
        internal static string ToYesNoString(this bool value) => value ? "yes" : "no";
    }
}
