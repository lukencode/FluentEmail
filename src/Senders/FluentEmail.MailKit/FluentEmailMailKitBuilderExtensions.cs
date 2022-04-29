using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentEmail.MailKitSmtp
{
    public static class FluentEmailMailKitBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailKitSender(this FluentEmailServicesBuilder builder, SmtpClientOptions smtpClientOptions)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailKitSender(smtpClientOptions)));
            return builder;
        }
    }
}
