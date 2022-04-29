using System;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentEmail.Liquid
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