using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentEmailSmtpBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddSmtpSender(this FluentEmailServicesBuilder builder, string host, int port)
        {
            var smtpClient = new SmtpClient(host, port);
            builder.Services.TryAdd(ServiceDescriptor.Transient<ISender>(x => new SmtpSender(smtpClient)));
            return builder;
        }

        public static FluentEmailServicesBuilder AddSmtpSender(this FluentEmailServicesBuilder builder, SmtpClient smtpClient)
        {
            builder.Services.TryAdd(ServiceDescriptor.Transient<ISender>(x => new SmtpSender(smtpClient)));
            return builder;
        }

        public static FluentEmailServicesBuilder AddSmtpSender(this FluentEmailServicesBuilder builder, Func<SmtpClient> clientFactory)
        {
            builder.Services.TryAdd(ServiceDescriptor.Transient<ISender>(x => new SmtpSender(clientFactory)));
            return builder;
        }
    }
}
