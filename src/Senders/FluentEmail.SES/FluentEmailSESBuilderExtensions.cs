using FluentEmail.Core.Interfaces;
using FluentEmail.SES;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailSESBuilderExtensions
    {
        /// <summary>
        /// Adds the SES sender services.
        /// </summary>
        /// <param name="builder">The Fluent Email Service builder.</param>
        /// <param name="sesClientOptions">The options to configure the SES client.</param>
        /// <returns><see cref="FluentEmailServicesBuilder"/></returns>
        public static FluentEmailServicesBuilder AddSESSender(this FluentEmailServicesBuilder builder, FluenEmailSESOptions sesClientOptions)
        {
            builder.Services.Configure<FluenEmailSESOptions>(options =>
            {
                options.AccessKeyId = sesClientOptions.AccessKeyId;
                options.SecretAccessKey = sesClientOptions.SecretAccessKey;
                options.RegionEndpoint = sesClientOptions.RegionEndpoint;
            });

            builder.Services.TryAddSingleton<ISender, SESSender>();
            return builder;
        }

        /// <summary>
        /// Adds the SES sender services.
        /// </summary>
        /// <param name="builder">The Fluent Email Service builder.</param>
        /// <param name="sesClientOptions">The options to configure the SES client.</param>
        /// <returns><see cref="FluentEmailServicesBuilder"/></returns>
        public static FluentEmailServicesBuilder AddSESSender(this FluentEmailServicesBuilder builder, Action<IServiceProvider, FluenEmailSESOptions> sesClientOptions)
        {
            builder.Services.AddOptions<FluenEmailSESOptions>()
                .Configure<IServiceProvider>((options, provider) =>
                {
                    sesClientOptions(provider, options);
                });

            builder.Services.TryAddSingleton<ISender, SESSender>();
            return builder;
        }
    }

}
