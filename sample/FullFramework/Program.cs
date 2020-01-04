using FluentEmail.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentEmail.Razor;
using FluentEmail.Smtp;

namespace FullFramework
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Email.DefaultSender = new SmtpSender(new SmtpClient("localhost", 25));
            Email.DefaultRenderer = new RazorRenderer();

            var model = new
            {
                Name = "Ben"
            };

            var assembly = Assembly.GetAssembly(typeof(FullFramework.Program));

            await Email.From("fullframework@test.test")
                .To("test3@test.test")
                .Subject("console full framework")
                .UsingTemplateFromEmbedded("FullFramework.EmailTemplates.Test1.cshtml", model, assembly)
                .SendAsync();
        }
    }
}
