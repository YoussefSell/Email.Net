﻿namespace Microsoft.Extensions.DependencyInjection
{
    using Email.NET.EDP;
    using Email.NET.EDP.SocketLabs;
    using System;

    /// <summary>
    /// the Configurations class
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// add the Socketlabs EDP to be used with your email service.
        /// </summary>
        /// <param name="builder">the emailNet builder instance.</param>
        /// <param name="config">the configuration builder instance.</param>
        /// <returns>instance of <see cref="EmailNetBuilder"/> to enable methods chaining.</returns>
        public static EmailNetBuilder UseSocketlabs(this EmailNetBuilder builder, Action<SocketLabsEmailDeliveryProviderOptions> config)
        {
            // load the configuration
            var configuration = new SocketLabsEmailDeliveryProviderOptions();
            config(configuration);

            // validate the configuration
            configuration.Validate();

            builder.ServiceCollection.AddSingleton((s) => configuration);
            builder.ServiceCollection.AddScoped<IEmailDeliveryProvider, SocketLabsEmailDeliveryProvider>();
            builder.ServiceCollection.AddScoped<ISocketLabsEmailDeliveryProvider, SocketLabsEmailDeliveryProvider>();

            return builder;
        }
    }
}
