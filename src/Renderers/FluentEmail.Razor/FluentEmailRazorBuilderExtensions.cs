using FluentEmail.Core.Interfaces;
using FluentEmail.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class FluentEmailRazorBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder, string templateRootFolder = null)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ITemplateRenderer, RazorRenderer>(sp => new RazorRenderer(templateRootFolder)));
            return builder;
        }
    }
}
