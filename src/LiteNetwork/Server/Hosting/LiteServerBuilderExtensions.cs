using LiteNetwork.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LiteNetwork.Server.Hosting
{
    public static class LiteServerBuilderExtensions
    {
        /// <summary>
        /// Initializes a <typeparamref name="TLiteServer"/> with the specified <typeparamref name="TLiteServerUser"/>.
        /// </summary>
        /// <typeparam name="TLiteServer">Server type.</typeparam>
        /// <typeparam name="TLiteServerUser">Server's user type.</typeparam>
        /// <param name="builder">A <see cref="ILiteBuilder"/> to add server.</param>
        /// <param name="configure">Delegate to configure a <see cref="LiteServerOptions"/>.</param>
        /// <returns>The <see cref="ILiteBuilder"/>.</returns>
        public static ILiteBuilder AddLiteServer<TLiteServer, TLiteServerUser>(this ILiteBuilder builder, Action<LiteServerOptions> configure)
            where TLiteServer : LiteServer<TLiteServerUser>
            where TLiteServerUser : LiteServerUser
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<TLiteServer>(serviceProvider =>
            {
                LiteServerOptions options = new();
                configure(options);
                
                TLiteServer server = ActivatorUtilities.CreateInstance<TLiteServer>(serviceProvider, options);

                if (server is not null)
                {
                    server.SetServiceProvider(serviceProvider);
                }
                
                return server!;
            });

            builder.Services.AddLiteServerHostedService<TLiteServerUser>();

            return builder;
        }

        /// <summary>
        /// Initializes a <typeparamref name="TLiteServerImplementation"/> with the specified <typeparamref name="TLiteServerUser"/>.
        /// </summary>
        /// <typeparam name="TLiteServer">LiteServer abstraction.</typeparam>
        /// <typeparam name="TLiteServerImplementation">LiteServer implementation.</typeparam>
        /// <typeparam name="TLiteServerUser">LiteServer user.</typeparam>
        /// <param name="builder">A <see cref="ILiteBuilder"/> to add server.</param>
        /// <param name="configure">Delegate to configure a <see cref="LiteServerOptions"/>.</param>
        /// <returns>The <see cref="ILiteBuilder"/>.</returns>
        public static ILiteBuilder AddLiteServer<TLiteServer, TLiteServerImplementation, TLiteServerUser>(this ILiteBuilder builder, Action<LiteServerOptions> configure)
            where TLiteServer : class
            where TLiteServerImplementation : LiteServer<TLiteServerUser>, TLiteServer
            where TLiteServerUser : LiteServerUser
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<TLiteServer, TLiteServerImplementation>(serviceProvider =>
            {
                LiteServerOptions options = new();
                configure(options);

                TLiteServerImplementation server = ActivatorUtilities.CreateInstance<TLiteServerImplementation>(serviceProvider, options);

                if (server is not null)
                {
                    server.SetServiceProvider(serviceProvider);
                }

                return server!;
            });

            builder.Services.AddLiteServerHostedService<TLiteServerUser>();

            return builder;
        }

        private static void AddLiteServerHostedService<TLiteServerUser>(this IServiceCollection services)
            where TLiteServerUser : LiteServerUser
        {
            services.AddHostedService(serviceProvider =>
            {
                return new LiteServerHostedService<TLiteServerUser>(serviceProvider.GetRequiredService<LiteServer<TLiteServerUser>>());
            });
        }
    }
}
