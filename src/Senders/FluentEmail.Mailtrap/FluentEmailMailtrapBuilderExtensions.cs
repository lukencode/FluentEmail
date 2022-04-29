using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentEmail.Mailtrap
{
    public static class FluentEmailMailtrapBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddMailtrapSender(this FluentEmailServicesBuilder builder, string userName, string password, string host = null, int? port = null)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailtrapSender(userName, password, host, port)));
            return builder;
        }
    }
}
