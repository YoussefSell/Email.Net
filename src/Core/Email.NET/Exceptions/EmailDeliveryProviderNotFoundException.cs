namespace Email.NET.Exceptions
{
    using System;

    /// <summary>
    /// exception thrown when no email delivery provider has been found
    /// </summary>
    [Serializable]
    public class EmailDeliveryProviderNotFoundException : Exception
    {
        private static readonly string message = "there is no email delivery provider with the name {{name}}, make sure you have registered the EDP with the email service";

        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public string EmailDeliveryProviderName { get; set; }

        /// <inheritdoc/>
        public EmailDeliveryProviderNotFoundException(string emailDeliveryProviderName)
            : base(message.Replace("{{name}}", emailDeliveryProviderName)) { }

        /// <inheritdoc/>
        public EmailDeliveryProviderNotFoundException(string message, string emailDeliveryProviderName)
            : base(message)
        {
            EmailDeliveryProviderName = emailDeliveryProviderName;
        }

        /// <inheritdoc/>
        protected EmailDeliveryProviderNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
