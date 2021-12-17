namespace Email.Net
{
    using Email.Net.EDP;
    using Email.Net.Factories;

    /// <summary>
    /// the extensions methods over the <see cref="MessageComposer"/> factory.
    /// </summary>
    public static class MessageComposerExtensions
    {
        /// <summary>
        /// pass a custom edp data to configure a custom apiKey to be used when sending the email with an edp that supports the ApiKey configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="apiKey">the apiKey to be used.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer UseCustomApiKey(this MessageComposer messageComposer, string apiKey)
            => messageComposer.PassEdpData(EdpData.New(EdpData.Keys.ApiKey, apiKey));

        /// <summary>
        /// pass a custom edp data to configure a messageId to be sent with the email if the edp supports the messageId configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="messageId">the messageId to be used.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer SetMessageId(this MessageComposer messageComposer, string messageId)
            => messageComposer.PassEdpData(EdpData.New(EdpData.Keys.MessageId, messageId));

        /// <summary>
        /// pass a custom edp data to configure a mailingId to be sent with the email if the edp supports the mailingId configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="mailingId">the mailing to be used.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer SetMailingId(this MessageComposer messageComposer, string mailingId)
            => messageComposer.PassEdpData(EdpData.New(EdpData.Keys.MailingId, mailingId));

        /// <summary>
        /// pass a custom edp data to configure a campaignId to be sent with the email if the edp supports the campaignId configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="campaignId">the campaignId to be used.</param>
        /// <returns>Instance of <see cref="MessageComposer"/> to enable fluent chaining.</returns>
        public static MessageComposer SetCampaignId(this MessageComposer messageComposer, string campaignId)
            => messageComposer.PassEdpData(EdpData.New(EdpData.Keys.CampaignId, campaignId));
    }
}
