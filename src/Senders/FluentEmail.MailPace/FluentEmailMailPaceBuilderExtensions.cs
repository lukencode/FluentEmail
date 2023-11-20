using FluentEmail.Core.Interfaces;
using FluentEmail.MailPace;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailMailPaceBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailPaceSender(
            this FluentEmailServicesBuilder builder,
            string serverToken)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailPaceSender(serverToken)));
            return builder;
        }
    }
}