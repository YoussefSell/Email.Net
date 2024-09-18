namespace Email.Net
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// the email sending result
    /// </summary>
    public partial class EmailSendingResult
    {
        /// <summary>
        /// Get if the email has been sent successfully.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Get the name of the channel used to send the email.
        /// </summary>
        public string ChannelName { get; }

        /// <summary>
        /// method data associated with this result if any.
        /// </summary>
        public IDictionary<string, object> MetaData { get; }

        /// <summary>
        /// Get the errors associated with the sending failure.
        /// </summary>
        public IEnumerable<EmailSendingError> Errors => _errors;
    }

    /// <summary>
    /// the partial part for <see cref="EmailSendingResult"/>
    /// </summary>
    public partial class EmailSendingResult
    {
        private readonly HashSet<EmailSendingError> _errors;

        /// <summary>
        /// create an instance of <see cref="EmailSendingResult"/>.
        /// </summary>
        /// <param name="isSuccess">true if the sending was successfully</param>
        /// <param name="channelName">the name of the channel used to sent the email.</param>
        /// <param name="errors">the errors associated with the sending.</param>
        public EmailSendingResult(bool isSuccess, string channelName, params EmailSendingError[] errors)
        {
            IsSuccess = isSuccess;
            MetaData = new Dictionary<string, object>();
            _errors = new HashSet<EmailSendingError>(errors);
            ChannelName = channelName ?? throw new ArgumentNullException(nameof(channelName));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var stringbuilder = new StringBuilder($"{ChannelName} -> sending: ");

            if (IsSuccess)
                stringbuilder.Append("Succeeded");
            else
                stringbuilder.Append("Failed");

            if (!(_errors is null) && _errors.Count != 0)
                stringbuilder.Append($" | {_errors.Count} errors");

            if (MetaData.Count != 0)
                stringbuilder.Append($" | {MetaData.Count} meta-data");

            return stringbuilder.ToString();
        }

        /// <summary>
        /// add a new error to the errors list.
        /// </summary>
        /// <param name="error">the error to add.</param>
        /// <returns>the <see cref="EmailSendingResult"/> instance to enable methods chaining.</returns>
        /// <exception cref="ArgumentNullException">the error is null</exception>
        public EmailSendingResult AddError(EmailSendingError error)
        {
            if (error is null)
                throw new ArgumentNullException(nameof(error));

            _errors.Add(error);
            return this;
        }

        /// <summary>
        /// add a new error to the errors list.
        /// </summary>
        /// <param name="exception">the exception to add as error.</param>
        /// <returns>the <see cref="EmailSendingResult"/> instance to enable methods chaining.</returns>
        /// <exception cref="ArgumentNullException">the error is null</exception>
        public EmailSendingResult AddError(Exception exception)
            => AddError(new EmailSendingError(exception));

        /// <summary>
        /// add a new error to the errors list.
        /// </summary>
        /// <param name="key">the key of the meta-data.</param>
        /// <param name="value">the value of the meta-data.</param>
        /// <returns>the <see cref="EmailSendingResult"/> instance to enable methods chaining.</returns>
        /// <exception cref="ArgumentNullException">the key is null</exception>
        /// <exception cref="ArgumentException">key is empty, or An element with the same key already exists</exception>
        public EmailSendingResult AddMetaData(string key, object value)
        {
            MetaData.Add(key, value);
            return this;
        }

        /// <summary>
        /// get the meta-data with the given key, if not found default will be returned
        /// </summary>
        /// <typeparam name="TValue">the type of the value</typeparam>
        /// <param name="key">the meta data key</param>
        /// <param name="defaultValue">the default value to return if noting found, by default is it set to "default"</param>
        /// <returns>instance of the value for the given key, or default if not found</returns>
        public TValue GetMetaData<TValue>(string key, TValue defaultValue = default)
        {
            if (MetaData.TryGetValue(key, out object value))
                return (TValue)value;

            return defaultValue;
        }

        /// <summary>
        /// create an instance of <see cref="EmailSendingResult"/> with a success state.
        /// </summary>
        /// <param name="channelName">the name of the channel used to send the email.</param>
        /// <returns>instance of <see cref="EmailSendingResult"/></returns>
        public static EmailSendingResult Success(string channelName)
            => new EmailSendingResult(true, channelName);

        /// <summary>
        /// create an instance of <see cref="EmailSendingResult"/> with a failure state.
        /// </summary>
        /// <param name="channelName">the name of the channel used to send the email.</param>
        /// <param name="errors">errors associated with the failure if any.</param>
        /// <returns>instance of <see cref="EmailSendingResult"/></returns>
        public static EmailSendingResult Failure(string channelName, params EmailSendingError[] errors)
            => new EmailSendingResult(false, channelName);

        /// <summary>
        /// this static class holds the keys names used in the email sending meta-data
        /// </summary>
        public static class MetaDataKeys
        {
            /// <summary>
            /// key to indicate whether the sending is paused.
            /// </summary>
            public const string SendingPaused = "sending_paused";
        }
    }
}
