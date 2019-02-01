using System;
using FluentEmail.Core.Interfaces;
using FluentEmail.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class FluentEmailRazorBuilderExtensions
    {
	    /// <summary>
	    /// Add razor renderer with project views and layouts
	    /// </summary>
	    /// <param name="builder"></param>
	    /// <param name="templateRootFolder"></param>
	    /// <returns></returns>
	    public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder, string templateRootFolder = null)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ITemplateRenderer, RazorRenderer>(sp => new RazorRenderer(templateRootFolder)));
            return builder;
        }

		/// <summary>
		/// Add razor renderer with embedded views and layouts
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="embeddedResRootType"></param>
		/// <returns></returns>
	    public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder, Type embeddedResRootType)
	    {
		    builder.Services.TryAdd(ServiceDescriptor.Singleton<ITemplateRenderer, RazorRenderer>(sp => new RazorRenderer( embeddedResRootType)));
		    return builder;
	    }
    }
}
