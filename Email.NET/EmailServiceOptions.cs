namespace Email.NET
{
    using Providers.SmtpClient;

    /// <summary>
    /// the options for configuring the email service
    /// </summary>
    public class EmailServiceOptions
    {
        /// <summary>
        /// the name of the default email delivery provider, by default is set to <see cref="SmtpClientEmailDeliveryProvider.Name"/>
        /// </summary>
        public string DefaultEmailDeliveryProvider { get; set; } = SmtpClientEmailDeliveryProvider.Name;
    }
}
