namespace Email.NET
{
    using Email.NET.EDP;
    using Email.NET.Factories;

    /// <summary>
    /// the extensions methods over the <see cref="MessageComposer"/> factory.
    /// </summary>
    public static class SendGridMessageComposerExtensions
    {
        /// <summary>
        /// set the tracking settings to be used by SendGrid.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="trackingSettings">the trackingSettings to be used.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer UseCustomServerId(this MessageComposer messageComposer, SendGrid.Helpers.Mail.TrackingSettings trackingSettings)
            => messageComposer.PassEdpData(EdpData.New("sendgrid_tracking_settings", trackingSettings));
    }
}
