namespace Microsoft.Extensions.DependencyInjection
{
    using Email.Net.EDP;
    using Email.Net.EDP.MailKit;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the MailKit EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseMailKit(this EmailNetBuilder builder, Action<MailKitEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new MailKitEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryProvider, MailKitEmailDeliveryProvider>();
            builder.ServiceCollection.AddScoped<IMailKitEmailDeliveryProvider, MailKitEmailDeliveryProvider>();

            return builder;
        }
    }
}
