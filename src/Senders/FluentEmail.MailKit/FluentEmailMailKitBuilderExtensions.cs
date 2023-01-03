using FluentEmail.Core.Interfaces;
using FluentEmail.MailKitSmtp;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MailKit;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailMailKitBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailKitSender(this FluentEmailServicesBuilder builder, SmtpClientOptions smtpClientOptions)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailKitSender(smtpClientOptions)));
            return builder;
        }

        public static FluentEmailServicesBuilder AddMailKitSender(this FluentEmailServicesBuilder builder, SmtpClientOptions smtpClientOptions, IProtocolLogger protocolLogger)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailKitSender(smtpClientOptions, protocolLogger)));
            return builder;
        }
    }
}
