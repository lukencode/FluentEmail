using FluentEmail.Core.Interfaces;
using FluentEmail.ElasticEmail;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailElasticEmailBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddElasticEmailSender(this FluentEmailServicesBuilder builder, string apiKey, bool useSubAccount = false)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new ElasticEmailSender(apiKey, useSubAccount: useSubAccount)));
            return builder;
        }
    }
}
