using System;

using FluentEmail.Core.Interfaces;
using FluentEmail.Liquid;

using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailFluidBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddLiquidRenderer(
            this FluentEmailServicesBuilder builder,
            Action<LiquidRendererOptions>? configure = null)
        {
            builder.Services.AddOptions<LiquidRendererOptions>();
            if (configure != null)
            {
                builder.Services.Configure(configure);
            }

            builder.Services.TryAddSingleton<ITemplateRenderer, LiquidRenderer>();
            return builder;
        }
    }
}