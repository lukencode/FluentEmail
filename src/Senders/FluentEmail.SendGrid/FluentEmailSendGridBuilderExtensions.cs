using FluentEmail.Core.Interfaces;
using FluentEmail.SendGrid;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailSendGridBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddSendGridSender(this FluentEmailServicesBuilder builder, string apiKey, bool sandBoxMode = false)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new SendGridSender(apiKey, sandBoxMode)));
            return builder;
        }
    }
}
