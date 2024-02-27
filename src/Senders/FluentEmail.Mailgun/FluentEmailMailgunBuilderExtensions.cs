using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailMailgunBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailGunSender(this FluentEmailServicesBuilder builder, string domainName, string apiKey, MailGunRegion mailGunRegion = MailGunRegion.USA)
        {
            builder.Services.AddHttpClient(nameof(MailgunSender),
                client =>
                {
                    string url = string.Empty;
                    switch (mailGunRegion)
                    {                        
                        case MailGunRegion.USA:
                            url = $"https://api.mailgun.net/v3/{domainName}/";
                            break;
                        case MailGunRegion.EU:
                            url = $"https://api.eu.mailgun.net/v3/{domainName}/";
                            break;

                        default:
                            throw new ArgumentException($"'{mailGunRegion}' is not a valid value for {nameof(mailGunRegion)}");
                    }

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}")));
                });

            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender, MailgunSender>());
            return builder;
        }
    }
}
