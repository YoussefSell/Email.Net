namespace Email.Net
{
    using Email.Net.Channel;
    using Email.Net.Factories;

    /// <summary>
    /// the extensions methods over the <see cref="EmailMessageComposer"/> factory.
    /// </summary>
    public static class SmtpChannelMessageComposerExtensions
    {
        /// <summary>
        /// pass a custom channel data to configure a custom smtp options to override the base smtp configuration
        /// </summary>
        /// <param name="messageComposer">the message composer instance</param>
        /// <param name="smtpOptions">the smtp options instance</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining</returns>
        public static EmailMessageComposer UseCustomSmtpOptions(this EmailMessageComposer messageComposer, Channel.Smtp.SmtpOptions smtpOptions)
            => messageComposer.PassChannelData(ChannelData.Keys.SmtpOptions, smtpOptions);
    }
}
