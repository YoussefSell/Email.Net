namespace Email.Net
{
    using Email.Net.EDP;
    using Email.Net.Factories;

    /// <summary>
    /// the extensions methods over the <see cref="EmailMessageComposer"/> factory.
    /// </summary>
    public static class SmtpProviderMessageComposerExtensions
    {
        /// <summary>
        /// pass a custom edp data to configure a custom smtp options to override the base smtp configuration
        /// </summary>
        /// <param name="messageComposer">the message composer instance</param>
        /// <param name="smtpOptions">the smtp options instance</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining</returns>
        public static EmailMessageComposer UseCustomSmtpOptions(this EmailMessageComposer messageComposer, EDP.Smtp.SmtpOptions smtpOptions)
            => messageComposer.PassEdpData(EdpData.Keys.SmtpOptions, smtpOptions);
    }
}
