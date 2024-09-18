namespace Email.Net.Exceptions
{
    using System;

    /// <summary>
    /// exception thrown when no email delivery channel has been found
    /// </summary>
    [Serializable]
    public class EmailDeliveryChannelNotFoundException : Exception
    {
        private static readonly string message = "there is no email delivery channel with the name {{name}}, make sure you have registered the Channel with the email service";

        /// <summary>
        /// the name of the email delivery channel
        /// </summary>
        public string EmailDeliveryChannelName { get; set; }

        /// <inheritdoc/>
        public EmailDeliveryChannelNotFoundException(string emailDeliveryChannelName)
            : base(message.Replace("{{name}}", emailDeliveryChannelName)) { }

        /// <inheritdoc/>
        public EmailDeliveryChannelNotFoundException(string message, string emailDeliveryChannelName)
            : base(message)
        {
            EmailDeliveryChannelName = emailDeliveryChannelName;
        }

        /// <inheritdoc/>
        protected EmailDeliveryChannelNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
