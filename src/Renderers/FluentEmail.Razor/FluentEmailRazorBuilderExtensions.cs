using FluentEmail.Core.Interfaces;
using FluentEmail.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailRazorBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ITemplateRenderer, RazorRenderer>());
            return builder;
        }
    }
}
