using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailMailgunBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailGunSender(this FluentEmailServicesBuilder builder, string domainName, string apiKey, MailGunRegion mailGunRegion = MailGunRegion.USA)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new MailgunSender(domainName, apiKey, mailGunRegion)));
            return builder;
        }
    }
}
