namespace Email.NET
{
    using Email.NET.EDP;
    using Email.NET.Factories;

    /// <summary>
    /// the extensions methods over the <see cref="MessageComposer"/> factory.
    /// </summary>
    public static class MailgunMessageComposerExtensions
    {
        /// <summary>
        /// use the test mode when sending the email with Mailgun.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer UseTestMode(this MessageComposer messageComposer)
            => messageComposer.PassEdpData(EdpData.New("test_mode", true));

        /// <summary>
        /// enable email tracking when sending the email with Mailgun.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer UseEnableTracking(this MessageComposer messageComposer)
            => messageComposer.PassEdpData(EdpData.New("enable_tracking", true));
    }
}
