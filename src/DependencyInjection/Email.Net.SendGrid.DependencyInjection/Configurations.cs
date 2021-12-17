namespace Microsoft.Extensions.DependencyInjection
{
    using Email.Net.EDP;
    using Email.Net.EDP.SendGrid;
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
        /// <param name="apiKey">set your Twilio SendGrid Api key.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSendGrid(this EmailNetBuilder builder, string apiKey)
            => builder.UseSendGrid(op => op.ApiKey = apiKey);

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
