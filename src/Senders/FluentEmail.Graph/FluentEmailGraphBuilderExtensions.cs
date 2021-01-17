using FluentEmail.Core.Interfaces;
using FluentEmail.Graph;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
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
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new GraphSender(GraphEmailAppId, GraphEmailTenantId, GraphEmailSecret, saveSentItems)));
            return builder;
        }
    }
}
