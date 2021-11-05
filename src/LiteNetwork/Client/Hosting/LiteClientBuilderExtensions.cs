using LiteNetwork.Client.Abstractions;
using LiteNetwork.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LiteNetwork.Client.Hosting
{
    public static class LiteClientBuilderExtensions
    {
        /// <summary>
        /// Initializes a basic <see cref="LiteClient"/>.
        /// </summary>
        /// <param name="builder">A <see cref="ILiteBuilder"/> to add the client.</param>
        /// <param name="configure">Delegate to configure a <see cref="LiteClientOptions"/>.</param>
        /// <returns>The <see cref="ILiteBuilder"/>.</returns>
        public static ILiteBuilder AddLiteClient(this ILiteBuilder builder, Action<LiteClientOptions> configure)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<ILiteClient, LiteClient>(serviceProvider =>
            {
                LiteClientOptions options = new();
                configure(options);

                return new LiteClient(options, serviceProvider);
            });


            builder.Services.AddHostedService(serviceProvider =>
            {
                var serverInstance = serviceProvider.GetRequiredService<ILiteClient>();
                return new LiteClientHostedService(serverInstance);
            });
            return builder;
        }

        /// <summary>
        /// Initializes a custom <see cref="ILiteClient"/>.
        /// </summary>
        /// <typeparam name="TLiteClient">Custom client type.</typeparam>
        /// <param name="builder">A <see cref="ILiteBuilder"/> to add the client.</param>
        /// <param name="configure">Delegate to configure a <see cref="LiteClientOptions"/>.</param>
        /// <returns>The <see cref="ILiteBuilder"/>.</returns>
        public static ILiteBuilder AddLiteClient<TLiteClient>(this ILiteBuilder builder, Action<LiteClientOptions> configure)
            where TLiteClient : class, ILiteClient
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton(serviceProvider =>
            {
                LiteClientOptions options = new();
                configure(options);

                return ActivatorUtilities.CreateInstance<TLiteClient>(serviceProvider, options);
            });

            builder.Services.AddHostedService(serviceProvider =>
            {
                var clientInstance = serviceProvider.GetRequiredService<TLiteClient>();

                return new LiteClientHostedService(clientInstance);
            });

            return builder;
        }

        /// <summary>
        /// Initializes a custom <see cref="ILiteClient"/> with a custom interface.
        /// </summary>
        /// <typeparam name="TLiteClient">LiteClient abstraction.</typeparam>
        /// <typeparam name="TLiteClientImplementation">LiteClient implementation.</typeparam>
        /// <param name="builder">A <see cref="ILiteBuilder"/> to add the client.</param>
        /// <param name="configure">Delegate to configure a <see cref="LiteClientOptions"/>.</param>
        /// <returns>The <see cref="ILiteBuilder"/>.</returns>
        public static ILiteBuilder AddLiteClient<TLiteClient, TLiteClientImplementation>(this ILiteBuilder builder, Action<LiteClientOptions> configure)
            where TLiteClient : class
            where TLiteClientImplementation : class, TLiteClient, ILiteClient
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<TLiteClient, TLiteClientImplementation>(serviceProvider =>
            {
                LiteClientOptions options = new();
                configure(options);

                return ActivatorUtilities.CreateInstance<TLiteClientImplementation>(serviceProvider, options);
            });

            builder.Services.AddSingleton<ILiteClient, TLiteClientImplementation>(serviceProvider =>
            {
                return (TLiteClientImplementation)serviceProvider.GetRequiredService<TLiteClient>();
            });

            builder.Services.AddHostedService(serviceProvider =>
            {
                var clientInstance = serviceProvider.GetRequiredService<ILiteClient>();
                
                return new LiteClientHostedService(clientInstance);
            });

            return builder;
        }
    }
}
