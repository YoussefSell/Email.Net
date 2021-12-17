namespace Microsoft.Extensions.DependencyInjection
{
    using Email.Net;

    /// <summary>
    /// the email.net DI builder.
    /// </summary>
    public class EmailNetBuilder
    {
        /// <summary>
        /// create an instance of <see cref="EmailNetBuilder"/>.
        /// </summary>
        /// <param name="serviceCollection">the services collection instance.</param>
        /// <param name="configuration">the email service option</param>
        internal EmailNetBuilder(IServiceCollection serviceCollection, EmailServiceOptions configuration)
        {
            Configuration = configuration;
            ServiceCollection = serviceCollection;
        }

        /// <summary>
        /// Get the service collection.
        /// </summary>
        public IServiceCollection ServiceCollection { get; }
        
        /// <summary>
        /// Get the Email service options.
        /// </summary>
        public EmailServiceOptions Configuration { get; }
    }
}
