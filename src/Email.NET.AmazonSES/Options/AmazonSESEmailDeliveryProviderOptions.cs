namespace Email.NET.EDP.AmazonSES
{
    using Amazon;
    using Exceptions;

    /// <summary>
    /// the options for configuring the AmazonSES email delivery provider
    /// </summary>
    public class AmazonSESEmailDeliveryProviderOptions
    {
        /// <summary>
        /// Get or set the AWS access Key Id, if not specified it will use the value from the shared credentials file.
        /// please refer to the <see href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/create-shared-credentials-file.html">Amazon docs</see> for details.
        /// </summary>
        public string AwsAccessKeyId { get; set; }

        /// <summary>
        /// Get or set the AWS secret access key, if not specified it will use the value from the shared credentials file.
        /// please refer to the <see href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/create-shared-credentials-file.html">Amazon docs</see> for details.
        /// </summary>
        public string AwsSecretAccessKey { get; set; }

        /// <summary>
        /// Get or set the AWS session token. this property will be used only if you specified the <seealso cref="AwsSecretAccessKey"/> & <seealso cref="AwsAccessKeyId"/>.
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
            if (RegionEndpoint is null)
                throw new RequiredOptionValueNotSpecifiedException<AmazonSESEmailDeliveryProviderOptions>(
                    $"{nameof(RegionEndpoint)}", "the given AmazonSESEmailDeliveryProviderOptions.RegionEndpoint value is null or empty.");
        }
    }
}
