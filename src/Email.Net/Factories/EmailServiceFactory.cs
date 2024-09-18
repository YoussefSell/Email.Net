namespace Email.Net.Factories
{
    using Email.Net.Channel;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// the email service factory used to generate an instance of <see cref="EmailService"/>
    /// </summary>
    public partial class EmailServiceFactory
    {
        /// <summary>
        /// get an instance of the <see cref="EmailServiceFactory"/>
        /// </summary>
        public static readonly EmailServiceFactory Instance = new EmailServiceFactory();

        /// <summary>
        /// set the options of the email service.
        /// </summary>
        /// <param name="options">the email option initializer.</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable method chaining.</returns>
        public EmailServiceFactory UseOptions(Action<EmailServiceOptions> options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // set the email options and validate it.
            options(_options);
            _options.Validate();

            return this;
        }

        /// <summary>
        /// set the <see cref="IEmailDeliveryChannel"/> to be used by the email service.
        /// </summary>
        /// <param name="Channel">the <see cref="IEmailDeliveryChannel"/> instance</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable method chaining.</returns>
        public EmailServiceFactory UseChannel(IEmailDeliveryChannel Channel)
        {
            if (Channel is null)
                throw new ArgumentNullException(nameof(Channel));

            _channels.Add(Channel);

            return this;
        }

        /// <summary>
        /// create the email service instance.
        /// </summary>
        /// <returns>instance of <see cref="EmailService"/></returns>
        public IEmailService Create() => new EmailService(_channels, _options);
    }

    /// <summary>
    /// partial part of <see cref="EmailServiceFactory"/>
    /// </summary>
    public partial class EmailServiceFactory
    {
        private readonly EmailServiceOptions _options = new EmailServiceOptions();
        private readonly HashSet<IEmailDeliveryChannel> _channels = new HashSet<IEmailDeliveryChannel>();

        private EmailServiceFactory() { }
    }
}
