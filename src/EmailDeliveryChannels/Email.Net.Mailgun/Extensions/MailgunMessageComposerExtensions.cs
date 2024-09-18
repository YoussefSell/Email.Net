namespace Email.Net
{
    using Email.Net.Factories;
    using Email.Net.Channel.Mailgun;

    /// <summary>
    /// the extensions methods over the <see cref="EmailMessageComposer"/> factory.
    /// </summary>
    public static class MailgunMessageComposerExtensions
    {
        /// <summary>
        /// use the test mode when sending the email with Mailgun.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining.</returns>
        public static EmailMessageComposer UseTestMode(this EmailMessageComposer messageComposer)
            => messageComposer.PassChannelData(CustomChannelData.TestMode, true);

        /// <summary>
        /// enable email tracking when sending the email with Mailgun.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining.</returns>
        public static EmailMessageComposer UseEnableTracking(this EmailMessageComposer messageComposer)
            => messageComposer.PassChannelData(CustomChannelData.EnableTracking, true);
    }
}
