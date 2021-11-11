namespace Email.NET
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// the email sending result
    /// </summary>
    public class EmailSendingResult
    {
        private readonly HashSet<EmailSendingError> _errors;

        /// <summary>
        /// create an instance of <see cref="EmailSendingResult"/>.
        /// </summary>
        /// <param name="isSuccess">true if the sending was successfully</param>
        /// <param name="edpName">the name of the edp used to sent the email.</param>
        /// <param name="errors">the errors associated with the sending.</param>
        public EmailSendingResult(bool isSuccess, string edpName, params EmailSendingError[] errors)
        {
            IsSuccess = isSuccess;
            MetaData = new Dictionary<string, object>();
            _errors = new HashSet<EmailSendingError>(errors);
            EdpName = edpName ?? throw new ArgumentNullException(nameof(edpName));
        }

        /// <summary>
        /// Get if the email has been sent successfully.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Get the name of the edp used to send the email.
        /// </summary>
        public string EdpName { get; }

        /// <summary>
        /// Get the errors associated with the sending failure.
        /// </summary>
        public IEnumerable<EmailSendingError> Errors => _errors;

        /// <summary>
        /// method data associated with this result if any.
        /// </summary>
        public IDictionary<string, object> MetaData { get; }

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
        /// <param name="error">the error to add.</param>
        /// <returns>the <see cref="EmailSendingResult"/> instance to enable methods chaining.</returns>
        /// <exception cref="ArgumentNullException">the key is null</exception>
        /// <exception cref="ArgumentException">key is empty, or An element with the same key already exists</exception>
        public EmailSendingResult AddMetaData(string key, object value)
        {
            MetaData.Add(key, value);
            return this;
        }

        /// <summary>
        /// create an instance of <see cref="EmailSendingResult"/> with a success state.
        /// </summary>
        /// <param name="edpName">the name of the edp used to send the email.</param>
        /// <returns>instance of <see cref="EmailSendingResult"/></returns>
        public static EmailSendingResult Success(string edpName)
            => new EmailSendingResult(true, edpName);

        /// <summary>
        /// create an instance of <see cref="EmailSendingResult"/> with a failure state.
        /// </summary>
        /// <param name="edpName">the name of the edp used to send the email.</param>
        /// <param name="errors">errors associated with the failure if any.</param>
        /// <returns>instance of <see cref="EmailSendingResult"/></returns>
        public static EmailSendingResult Failure(string edpName, params EmailSendingError[] errors)
            => new EmailSendingResult(false, edpName);
    }
}
