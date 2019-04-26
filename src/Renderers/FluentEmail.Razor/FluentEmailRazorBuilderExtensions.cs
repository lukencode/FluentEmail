using FluentEmail.Core.Interfaces;
using FluentEmail.Razor;
using RazorLight.Extensions;
using RazorLight.Razor;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailRazorBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder)
        {
            builder.Services.AddRazorLight(() => RazorLightEngineFactory.Create());
            builder.Services.AddSingleton<ITemplateRenderer, RazorRenderer>();
            return builder;
        }

        /// <summary>
        /// Add razor renderer with project views and layouts
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="templateRootFolder"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder, string templateRootFolder)
        {
            builder.Services.AddRazorLight(() => RazorLightEngineFactory.Create(templateRootFolder));
            builder.Services.AddSingleton<ITemplateRenderer, RazorRenderer>();
            return builder;
        }

        /// <summary>
        /// Add razor renderer with embedded views and layouts
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="embeddedResourceRootType"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder, Type embeddedResourceRootType)
        {
            builder.Services.AddRazorLight(() => RazorLightEngineFactory.Create(embeddedResourceRootType));
            builder.Services.AddSingleton<ITemplateRenderer, RazorRenderer>();
            return builder;
        }

        /// <summary>
        /// Add razor renderer with a RazorLightProject to support views and layouts
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="razorLightProject"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder, RazorLightProject razorLightProject)
        {
            builder.Services.AddRazorLight(() => RazorLightEngineFactory.Create(razorLightProject));
            builder.Services.AddSingleton<ITemplateRenderer, RazorRenderer>();
            return builder;
        }
    }
}
