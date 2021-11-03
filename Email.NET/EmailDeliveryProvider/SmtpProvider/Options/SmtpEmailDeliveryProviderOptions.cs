namespace Email.NET.Providers.SmtpClient
{
    /// <summary>
    /// the options for configuring the Smtp email delivery provider
    /// </summary>
    public class SmtpEmailDeliveryProviderOptions
    {
        /// <summary>
        /// the smtp configuration
        /// </summary>
        public SmtpClientOptions SmtpClientOptions { get; set; }

        /// <summary>
        /// validate if the options are all set correctly
        /// </summary>
        internal void Validate()
        {

        }
    }
}
