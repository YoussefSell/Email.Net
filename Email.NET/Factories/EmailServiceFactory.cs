namespace Email.NET.Factories
{
    using EDP;
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
        /// set the <see cref="IEmailDeliveryProvider"/> to be used by the email service.
        /// </summary>
        /// <param name="edp">the <see cref="IEmailDeliveryProvider"/> instance</param>
        /// <returns>instance of <see cref="EmailServiceFactory"/> to enable method chaining.</returns>
        public EmailServiceFactory UseEDP(IEmailDeliveryProvider edp)
        {
            if (edp is null)
                throw new ArgumentNullException(nameof(edp));

            _edps.Add(edp);

            return this;
        }

        /// <summary>
        /// create the email service instance.
        /// </summary>
        /// <returns>instance of <see cref="EmailService"/></returns>
        public EmailService Create() => new EmailService(_edps, _options);
    }

    /// <summary>
    /// partial part of <see cref="EmailServiceFactory"/>
    /// </summary>
    public partial class EmailServiceFactory
    {
        private readonly EmailServiceOptions _options = new EmailServiceOptions();
        private readonly HashSet<IEmailDeliveryProvider> _edps = new HashSet<IEmailDeliveryProvider>();

        private EmailServiceFactory() { }
    }
}
