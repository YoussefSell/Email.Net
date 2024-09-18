namespace Email.Net
{
    using Email.Net.Channel;
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// the email service used to abstract the email sending
    /// </summary>
    public partial class EmailService
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message)
            => Send(message, _defaultChannel);

        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message, string channel_name)
        {
            // check if the channel name is valid
            if (channel_name is null)
                throw new ArgumentNullException(nameof(channel_name));

            // check if the channel exist
            if (!_channels.TryGetValue(channel_name, out IEmailDeliveryChannel channel))
                throw new EmailDeliveryChannelNotFoundException(channel_name);

            // send the email message
            return Send(message, channel);
        }

        /// <inheritdoc/>
        public EmailSendingResult Send(EmailMessage message, IEmailDeliveryChannel channel)
        {
            // check if given params are not null.
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            // check if the from is null
            CheckMessageFromValue(message);

            // check if the sending is paused
            if (Options.PauseSending)
            {
                return EmailSendingResult.Success(channel.Name)
                    .AddMetaData(EmailSendingResult.MetaDataKeys.SendingPaused, true);
            }

            // send the email message
            return channel.Send(message);
        }

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
            => SendAsync(message, _defaultChannel, cancellationToken);

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(EmailMessage message, string channel_name, CancellationToken cancellationToken = default)
        {
            // check if the channel name is valid
            if (channel_name is null)
                throw new ArgumentNullException(nameof(channel_name));

            // check if the channel exist
            if (!_channels.TryGetValue(channel_name, out IEmailDeliveryChannel channel))
                throw new EmailDeliveryChannelNotFoundException(channel_name);

            // send the email message
            return SendAsync(message, channel, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(EmailMessage message, IEmailDeliveryChannel channel, CancellationToken cancellationToken = default)
        {
            // check if given params are not null.
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            // check if the from is null
            CheckMessageFromValue(message);

            // check if the sending is paused
            if (Options.PauseSending)
            {
                return Task.FromResult(EmailSendingResult.Success(channel.Name)
                    .AddMetaData("sending_paused", true));
            }

            // send the email message
            return channel.SendAsync(message, cancellationToken);
        }
    }

    /// <summary>
    /// partial part for <see cref="EmailService"/>
    /// </summary>
    public partial class EmailService : IEmailService
    {
        private readonly IDictionary<string, IEmailDeliveryChannel> _channels;
        private readonly IEmailDeliveryChannel _defaultChannel;

        /// <summary>
        /// create an instance of <see cref="EmailService"/>.
        /// </summary>
        /// <param name="EmailDeliveryChannels">the list of supported email delivery channels.</param>
        /// <param name="options">the email service options.</param>
        /// <exception cref="ArgumentNullException">if EmailDeliveryChannels or options are null.</exception>
        /// <exception cref="ArgumentException">if EmailDeliveryChannels list is empty.</exception>
        /// <exception cref="EmailDeliveryChannelNotFoundException">if the default email delivery channel cannot be found.</exception>
        public EmailService(IEnumerable<IEmailDeliveryChannel> EmailDeliveryChannels, EmailServiceOptions options)
        {
            if (EmailDeliveryChannels is null)
                throw new ArgumentNullException(nameof(EmailDeliveryChannels));

            if (!EmailDeliveryChannels.Any())
                throw new ArgumentException("you must specify at least one email delivery channel, the list is empty.", nameof(EmailDeliveryChannels));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();

            Options = options;

            // init the channels dictionary
            _channels = EmailDeliveryChannels.ToDictionary(channel => channel.Name);

            // check if the default email delivery channel exist
            if (!_channels.ContainsKey(options.DefaultEmailDeliveryChannel))
                throw new EmailDeliveryChannelNotFoundException(options.DefaultEmailDeliveryChannel);

            // set the default channel
            _defaultChannel = _channels[options.DefaultEmailDeliveryChannel];
        }

        /// <summary>
        /// Get the email service options instance
        /// </summary>
        public EmailServiceOptions Options { get; }

        /// <summary>
        /// Get the list of email delivery channels attached to this email service.
        /// </summary>
        public IEnumerable<IEmailDeliveryChannel> Channels => _channels.Values;

        /// <summary>
        /// Get the default email delivery channel attached to this email service.
        /// </summary>
        public IEmailDeliveryChannel DefaultChannel => _defaultChannel;

        /// <summary>
        /// check if the message from value is supplied
        /// </summary>
        /// <param name="message">the message instance</param>
        private void CheckMessageFromValue(EmailMessage message)
        {
            if (message.From is null)
            {
                if (Options.DefaultFrom is null)
                    throw new ArgumentException($"the {typeof(EmailMessage).FullName} [From] value is null, either supply a from value in the message, or set a default [From] value in {typeof(EmailServiceOptions).FullName}");

                message.SetFrom(Options.DefaultFrom);
            }
        }
    }
}
