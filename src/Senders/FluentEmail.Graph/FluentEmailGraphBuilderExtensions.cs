using FluentEmail.Core.Interfaces;
using FluentEmail.Graph;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to register the <see cref="GraphSender"/> with the <c>FluentEmailServicesBuilder</c> from <c>FluentEmail.Core</c>.
    /// </summary>
    public static class FluentEmailServicesBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddGraphSender(
            this FluentEmailServicesBuilder builder,
            GraphSenderOptions options)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new GraphSender(options)));
            return builder;
        }

        public static FluentEmailServicesBuilder AddGraphSender(
            this FluentEmailServicesBuilder builder,
            string graphEmailClientId,
            string graphEmailTenantId,
            string graphEmailSecret,
            bool saveSentItems = false)
        {
            var options = new GraphSenderOptions
            {
                ClientId = graphEmailClientId,
                TenantId = graphEmailTenantId,
                Secret = graphEmailSecret,
                SaveSentItems = saveSentItems,
            };
            return builder.AddGraphSender(options);
        }
    }
}
