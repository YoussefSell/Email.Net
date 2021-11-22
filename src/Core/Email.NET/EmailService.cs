﻿namespace Email.NET
{
    using EDP;
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// the email service used to abstract the email sending
    /// </summary>
    public partial class EmailService
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, params EdpData[] data)
            => Send(message, _defaultProvider, data);

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(Message message, params EdpData[] data)
            => SendAsync(message, _defaultProvider, data);

        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, string providerName, params EdpData[] data)
        {
            // check if the provider name is valid
            if (providerName is null)
                throw new ArgumentNullException(nameof(providerName));

            // check if the provider exist
            if (!_providers.TryGetValue(providerName, out IEmailDeliveryProvider provider))
                throw new EmailDeliveryProviderNotFoundException(providerName);

            // send the email message
            return Send(message, provider, data);
        }

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(Message message, string providerName, params EdpData[] data)
        {
            // check if the provider name is valid
            if (providerName is null)
                throw new ArgumentNullException(nameof(providerName));

            // check if the provider exist
            if (!_providers.TryGetValue(providerName, out IEmailDeliveryProvider provider))
                throw new EmailDeliveryProviderNotFoundException(providerName);

            // send the email message
            return SendAsync(message, provider, data);
        }

        /// <inheritdoc/>
        public EmailSendingResult Send(Message message, IEmailDeliveryProvider provider, params EdpData[] data)
        {
            // check if given params are not null.
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            // check if the from is null
            CheckMessageFromValue(message);

            // check if the sending is paused
            if (Options.PauseSending)
            {
                return EmailSendingResult.Success(provider.Name)
                    .AddMetaData(EmailSendingResult.MetaDataKeys.SendingPaused, true);
            }

            // send the email message
            return provider.Send(message, data);
        }

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(Message message, IEmailDeliveryProvider provider, params EdpData[] data)
        {
            // check if given params are not null.
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            // check if the from is null
            CheckMessageFromValue(message);

            // check if the sending is paused
            if (Options.PauseSending)
            {
                return Task.FromResult(EmailSendingResult.Success(provider.Name)
                    .AddMetaData("sending_paused", true));
            }

            // send the email message
            return provider.SendAsync(message, data);
        }
    }

    /// <summary>
    /// partial part for <see cref="EmailService"/>
    /// </summary>
    public partial class EmailService : IEmailService
    {
        private readonly IDictionary<string, IEmailDeliveryProvider> _providers;
        private readonly IEmailDeliveryProvider _defaultProvider;

        /// <summary>
        /// create an instance of <see cref="EmailService"/>.
        /// </summary>
        /// <param name="emailDeliveryProviders">the list of supported email delivery providers.</param>
        /// <param name="options">the email service options.</param>
        /// <exception cref="ArgumentNullException">if emailDeliveryProviders or options are null.</exception>
        /// <exception cref="ArgumentException">if emailDeliveryProviders list is empty.</exception>
        /// <exception cref="EmailDeliveryProviderNotFoundException">if the default email delivery provider cannot be found.</exception>
        public EmailService(IEnumerable<IEmailDeliveryProvider> emailDeliveryProviders, EmailServiceOptions options)
        {
            if (emailDeliveryProviders is null)
                throw new ArgumentNullException(nameof(emailDeliveryProviders));

            if (!emailDeliveryProviders.Any())
                throw new ArgumentException("you must specify at least one email delivery provider, the list is empty.", nameof(emailDeliveryProviders));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();

            Options = options;

            // init the providers dictionary
            _providers = emailDeliveryProviders.ToDictionary(provider => provider.Name);

            // check if the default email delivery provider exist
            if (!_providers.ContainsKey(options.DefaultEmailDeliveryProvider))
                throw new EmailDeliveryProviderNotFoundException(options.DefaultEmailDeliveryProvider);

            // set the default provider
            _defaultProvider = _providers[options.DefaultEmailDeliveryProvider];
        }

        /// <summary>
        /// the email service options instance
        /// </summary>
        public EmailServiceOptions Options { get; }

        /// <summary>
        /// Get the list of email delivery providers attached to this email service.
        /// </summary>
        public IEnumerable<IEmailDeliveryProvider> Edps => _providers.Values;

        /// <summary>
        /// Get the default email delivery provider attached to this email service.
        /// </summary>
        public IEmailDeliveryProvider DefaultEdp => _defaultProvider;

        /// <summary>
        /// check if the message from value is supplied
        /// </summary>
        /// <param name="message">the message instance</param>
        private void CheckMessageFromValue(Message message)
        {
            if (message.From is null)
            {
                if (Options.DefaultFrom is null)
                    throw new ArgumentException($"the {typeof(Message).FullName} [From] value is null, either supply a from value in the message, or set a default [From] value in {typeof(EmailServiceOptions).FullName}");

                message.SetFrom(Options.DefaultFrom);
            }
        }
    }
}