using System;
using FluentEmail.Core.Interfaces;
using FluentEmail.MailKitSmtp;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailMailKitBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailKitSender(this FluentEmailServicesBuilder builder, SmtpClientOptions smtpClientOptions)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new MailKitSender(smtpClientOptions)));
            return builder;
        }
    }
}
