using FluentEmail.Core.Interfaces;
using FluentEmail.SendGrid;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailSendGridBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddSendGridSender(this FluentEmailServicesBuilder builder, string apiKey, string host = null, bool sandBoxMode = false)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ISender>(_ => new SendGridSender(apiKey, host, sandBoxMode)));
            return builder;
        }
    }
}
