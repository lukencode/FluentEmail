using System;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentEmail.Core
{
    public static class FluentEmailServiceCollectionExtensions
    {
        public static FluentEmailServicesBuilder AddFluentEmail(this IServiceCollection services, string defaultFromEmail, string defaultFromName = "")
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var builder = new FluentEmailServicesBuilder(services);
            services.TryAdd(ServiceDescriptor.Transient<IFluentEmail>(x => 
                new Email(x.GetService<ITemplateRenderer>(), x.GetService<ISender>(), defaultFromEmail, defaultFromName)
            ));

            services.TryAddTransient<IFluentEmailFactory, FluentEmailFactory>();

            return builder;
        }
    }

    public class FluentEmailServicesBuilder
    {
        public IServiceCollection Services { get; private set; }

        internal FluentEmailServicesBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
