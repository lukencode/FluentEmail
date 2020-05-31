using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net;
using System.Net.Mail;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailSmtpBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddSmtpSender(this FluentEmailServicesBuilder builder, SmtpClient smtpClient)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ISender>(x => new SmtpSender(smtpClient)));
            return builder;
        }

        public static FluentEmailServicesBuilder AddSmtpSender(this FluentEmailServicesBuilder builder, string host, int port) => AddSmtpSender(builder, () => new SmtpClient(host, port));

        public static FluentEmailServicesBuilder AddSmtpSender(this FluentEmailServicesBuilder builder, string host, int port, string username, string password) => AddSmtpSender(builder,
             () => new SmtpClient(host, port) { EnableSsl = true, Credentials = new NetworkCredential (username, password) });
        
        public static FluentEmailServicesBuilder AddSmtpSender(this FluentEmailServicesBuilder builder, Func<SmtpClient> clientFactory)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(x => new SmtpSender(clientFactory)));
            return builder;
        }
    }
}
