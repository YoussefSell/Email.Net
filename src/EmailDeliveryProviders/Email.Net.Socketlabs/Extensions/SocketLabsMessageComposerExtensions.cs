namespace Email.Net
{
    using Email.Net.EDP.SocketLabs;
    using Email.Net.Factories;

    /// <summary>
    /// the extensions methods over the <see cref="EmailMessageComposer"/> factory.
    /// </summary>
    public static class SocketLabsMessageComposerExtensions
    {
        /// <summary>
        /// pass a custom edp data to configure a custom serverId to be used when sending the email with an edp that supports the ServerId configuration.
        /// </summary>
        /// <param name="messageComposer">the message composer instance.</param>
        /// <param name="serverId">the serverId to be used.</param>
        /// <returns>Instance of <see cref="EmailMessageComposer"/> to enable fluent chaining.</returns>
        public static EmailMessageComposer UseCustomServerId(this EmailMessageComposer messageComposer, int serverId)
            => messageComposer.PassEdpData(CustomEdpData.ServerId, serverId);
    }
}
