namespace Microsoft.Extensions.DependencyInjection
{
    using Email.NET.EDP;
    using Email.NET.EDP.SendGrid;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the SendGrid EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSendGrid(this EmailNetBuilder builder, Action<SendgridEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new SendgridEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryProvider, SendgridEmailDeliveryProvider>();
            builder.ServiceCollection.AddScoped<ISendgridEmailDeliveryProvider, SendgridEmailDeliveryProvider>();

            return builder;
        }
    }
}
