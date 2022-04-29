using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentEmail.Graph
{
    public static class FluentEmailGraphBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddGraphSender(
            this FluentEmailServicesBuilder builder,
            string GraphEmailAppId,
            string GraphEmailTenantId,
            string GraphEmailSecret,
            bool saveSentItems = false)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new GraphSender(GraphEmailAppId, GraphEmailTenantId, GraphEmailSecret, saveSentItems)));
            return builder;
        }
    }
}
