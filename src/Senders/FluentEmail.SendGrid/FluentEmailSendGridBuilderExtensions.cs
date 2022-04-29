using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentEmail.SendGrid
{
    public static class FluentEmailSendGridBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddSendGridSender(this FluentEmailServicesBuilder builder, string apiKey, bool sandBoxMode = false)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ISender>(_ => new SendGridSender(apiKey, sandBoxMode)));
            return builder;
        }
    }
}
