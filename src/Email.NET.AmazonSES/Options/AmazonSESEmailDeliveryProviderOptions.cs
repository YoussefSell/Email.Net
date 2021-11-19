namespace Email.NET.EDP.AmazonSES
{
    using Amazon;

    /// <summary>
    /// the options for configuring the AmazonSES email delivery provider
    /// </summary>
    public class AmazonSESEmailDeliveryProviderOptions
    {
        /// <summary>
        /// Get or set the AWS access Key Id.
        /// </summary>
        public string AwsAccessKeyId { get; set; }
        
        /// <summary>
        /// Get or set the AWS secret access key.
        /// </summary>
        public string AwsSecretAccessKey { get; set; }
        
        /// <summary>
        /// Get or set the AWS session token.
        /// </summary>
        public string AwsSessionToken { get; set; }

        /// <summary>
        /// Get or set the AWS Region you're using for Amazon SES. 
        /// </summary>
        public RegionEndpoint RegionEndpoint { get; set; }

        /// <summary>
        /// validate if the options are all set correctly
        /// </summary>
        internal void Validate()
        {
            
        }
    }
}
