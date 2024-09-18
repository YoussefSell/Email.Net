namespace Email.Net.Channel.Channel_name
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// the SocketLabs client email delivery channel
    /// </summary>
    public partial class ChannelEmailDeliveryChannel : IChannelEmailDeliveryChannel
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return Task.FromResult(EmailSendingResult.Failure(Name).AddError(ex));
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="ChannelEmailDeliveryChannel"/>
    /// </summary>
    public partial class ChannelEmailDeliveryChannel
    {
        /// <summary>
        /// the name of the email delivery channel
        /// </summary>
        public const string Name = "name_channel";

        /// <inheritdoc/>
        string IEmailDeliveryChannel.Name => Name;

        private readonly ChannelEmailDeliveryChannelOptions _options;

        /// <summary>
        /// create an instance of <see cref="ChannelEmailDeliveryChannel"/>
        /// </summary>
        /// <param name="options">the channel options instance</param>
        /// <exception cref="ArgumentNullException">if the given channel options is null</exception>
        public ChannelEmailDeliveryChannel(ChannelEmailDeliveryChannelOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        private object CreateClient(IEnumerable<ChannelData> data)
        {
            throw new NotImplementedException();
        }

        private static EmailSendingResult BuildResultObject(object result)
        {
            // create the failure result & return the result
            return EmailSendingResult.Success(Name);
        }

        /// <summary>
        /// create an instance of <see cref="BasicMessage"/> from the given <see cref="EmailMessage"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="BasicMessage"/></returns>
        public object CreateMessage(EmailMessage message)
        {
            throw new NotImplementedException();
        }
    }
}