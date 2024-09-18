namespace Email.Net
{
    using Email.Net.Channel;
    using Email.Net.Factories;

    /// <summary>
    /// the extensions methods over the <see cref="EmailMessageComposer"/> factory.
    /// </summary>
    public static class MessageComposerExtensions
    {
        /// <summary>
        /// pass a custom channel data to configure a custom apiKey to be used when sending the email with an channel that supports the ApiKey configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="apiKey">the apiKey to be used.</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining.</returns>
        public static EmailMessageComposer UseCustomApiKey(this EmailMessageComposer messageComposer, string apiKey)
            => messageComposer.PassChannelData(ChannelData.New(ChannelData.Keys.ApiKey, apiKey));

        /// <summary>
        /// pass a custom channel data to configure a messageId to be sent with the email if the channel supports the messageId configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="messageId">the messageId to be used.</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining.</returns>
        public static EmailMessageComposer SetMessageId(this EmailMessageComposer messageComposer, string messageId)
            => messageComposer.PassChannelData(ChannelData.New(ChannelData.Keys.MessageId, messageId));

        /// <summary>
        /// pass a custom channel data to configure a mailingId to be sent with the email if the channel supports the mailingId configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="mailingId">the mailing to be used.</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining.</returns>
        public static EmailMessageComposer SetMailingId(this EmailMessageComposer messageComposer, string mailingId)
            => messageComposer.PassChannelData(ChannelData.New(ChannelData.Keys.MailingId, mailingId));

        /// <summary>
        /// pass a custom channel data to configure a campaignId to be sent with the email if the channel supports the campaignId configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="campaignId">the campaignId to be used.</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining.</returns>
        public static EmailMessageComposer SetCampaignId(this EmailMessageComposer messageComposer, string campaignId)
            => messageComposer.PassChannelData(ChannelData.New(ChannelData.Keys.CampaignId, campaignId));
    }
}
