using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentEmail.Mailgun
{
    public static class FluentEmailMailgunBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailGunSender(this FluentEmailServicesBuilder builder, string domainName, string apiKey, MailGunRegion mailGunRegion = MailGunRegion.USA)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailgunSender(domainName, apiKey, mailGunRegion)));
            return builder;
        }
    }
}
