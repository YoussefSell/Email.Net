namespace Email.NET
{
    using Email.NET.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// the email service used to abstract the email sending
    /// </summary>
    public partial class EmailService
    {
        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryProvider"/>.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        public EmailSendingResult Send(Message message, params EmailDeliveryProviderData[] data)
            => Send(message, _defaultProvider, data);

        /// <summary>
        /// Sends the specified email message using the default <see cref="IEmailDeliveryProvider"/>.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        public Task<EmailSendingResult> SendAsync(Message message, params EmailDeliveryProviderData[] data)
            => SendAsync(message, _defaultProvider, data);

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="providerName">the name of the email delivery provider used for sending the email message.</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        public EmailSendingResult Send(Message message, string providerName, params EmailDeliveryProviderData[] data)
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

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="providerName">the name of the email delivery provider used for sending the email message.</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        public Task<EmailSendingResult> SendAsync(Message message, string providerName, params EmailDeliveryProviderData[] data)
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

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="provider">the email delivery provider used for sending the email message.</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        public EmailSendingResult Send(Message message, IEmailDeliveryProvider provider, params EmailDeliveryProviderData[] data)
        {
            // check if given params are not null.
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            // validate the email message 
            message.Validate();

            // send the email message
            return provider.Send(message, data);
        }

        /// <summary>
        /// Sends the specified email message.
        /// </summary>
        /// <param name="message">the email message to be send</param>
        /// <param name="provider">the email delivery provider used for sending the email message.</param>
        /// <param name="data">any additional data need to be passed to the email provider for further configuration</param>
        public Task<EmailSendingResult> SendAsync(Message message, IEmailDeliveryProvider provider, params EmailDeliveryProviderData[] data)
        {
            // check if given params are not null.
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            // validate the email message 
            message.Validate();

            // send the email message
            return provider.SendAsync(message, data);
        }
    }

    /// <summary>
    /// partial part for <see cref="EmailService"/>
    /// </summary>
    public partial class EmailService
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

            Options = options;

            // init the providers dictionary
            _providers = emailDeliveryProviders.ToDictionary(provider => provider.Name);

            // check if the default email delivery provider exist
            if (_providers.ContainsKey(options.DefaultEmailDeliveryProvider))
                throw new EmailDeliveryProviderNotFoundException(options.DefaultEmailDeliveryProvider);

            // set the default provider
            _defaultProvider = _providers[options.DefaultEmailDeliveryProvider];
        }

        /// <summary>
        /// the email service options instance
        /// </summary>
        public EmailServiceOptions Options { get; }
    }
}
