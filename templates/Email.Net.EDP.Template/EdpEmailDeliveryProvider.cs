namespace Email.Net.EDP.Edp_name
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// the SocketLabs client email delivery provider
    /// </summary>
    public partial class EdpEmailDeliveryProvider : IEdpEmailDeliveryProvider
    {
        /// <inheritdoc/>
        public EmailSendingResult Send(Message message)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return EmailSendingResult.Failure(Name).AddError(ex);
            }
        }

        /// <inheritdoc/>
        public Task<EmailSendingResult> SendAsync(Message message)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return Task.FromResult(EmailSendingResult.Failure(Name).AddError(ex));
            }
        }
    }

    /// <summary>
    /// partial part for <see cref="EdpEmailDeliveryProvider"/>
    /// </summary>
    public partial class EdpEmailDeliveryProvider
    {
        /// <summary>
        /// the name of the email delivery provider
        /// </summary>
        public const string Name = "name_edp";

        /// <inheritdoc/>
        string IEmailDeliveryProvider.Name => Name;

        private readonly EdpEmailDeliveryProviderOptions _options;

        /// <summary>
        /// create an instance of <see cref="EdpEmailDeliveryProvider"/>
        /// </summary>
        /// <param name="options">the edp options instance</param>
        /// <exception cref="ArgumentNullException">if the given provider options is null</exception>
        public EdpEmailDeliveryProvider(EdpEmailDeliveryProviderOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            // validate if the options are valid
            options.Validate();
            _options = options;
        }

        private object CreateClient(IEnumerable<EdpData> data)
        {
            throw new NotImplementedException();
        }

        private static EmailSendingResult BuildResultObject(object result)
        {
            // create the failure result & return the result
            return EmailSendingResult.Success(Name);
        }

        /// <summary>
        /// create an instance of <see cref="BasicMessage"/> from the given <see cref="Message"/>.
        /// </summary>
        /// <param name="message">the message instance</param>
        /// <returns>instance of <see cref="BasicMessage"/></returns>
        public object CreateMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }
}