using System.IO;
using System.Threading.Tasks;
using FluentEmail.Liquid;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEmail.Core.Tests
{
    [TestFixture]
    public class Snippets
    {
        class MyType
        {
        }

        void EmbeddedTemplateFile()
        {
            #region EmbeddedTemplateFile

            var email = new Email("bob@hotmail.com")
                .To("benwholikesbeer@twitter.com")
                .Subject("Hey cool name!")
                .UsingTemplateFromEmbedded(
                    "Example.Project.Namespace.template-name.cshtml",
                    new {Name = "Bob"},
                    typeof(MyType).Assembly);

            #endregion
        }

        void TemplateFileFromDisk()
        {
            #region TemplateFileFromDisk

            var email = Email
                .From("bob@hotmail.com")
                .To("somedude@gmail.com")
                .Subject("woo nuget")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Mytemplate.cshtml", new {Name = "Rad Dude"});

            #endregion
        }

        #region ConfigureServices

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddFluentEmail("fromemail@test.test")
                .AddRazorRenderer()
                .AddSmtpSender("localhost", 25);
        }

        #endregion

        async Task SendingEmails(Email email)
        {
            #region SendingEmails

            // Using Smtp Sender package (or set using AddSmtpSender in services)
            Email.DefaultSender = new SmtpSender();

            //send normally
            email.Send();

            //send asynchronously
            await email.SendAsync();

            #endregion
        }

        async Task BasicUsage()
        {
            #region BasicUsage

            var email = await Email
                .From("john@email.com")
                .To("bob@email.com", "bob")
                .Subject("hows it going bob")
                .Body("yo bob, long time no see!")
                .SendAsync();

            #endregion
        }

        void RazorTemplate()
        {
            #region RazorTemplate

            // Using Razor templating package (or set using AddRazorRenderer in services)
            Email.DefaultRenderer = new RazorRenderer();

            var template = "Dear @Model.Name, You are totally @Model.Compliment.";

            var email = Email
                .From("bob@hotmail.com")
                .To("somedude@gmail.com")
                .Subject("woo nuget")
                .UsingTemplate(template, new {Name = "Luke", Compliment = "Awesome"});

            #endregion
        }

        void LiquidTemplate(string someRootPath)
        {
            #region LiquidTemplate

            // Using Liquid templating package (or set using AddLiquidRenderer in services)

            // file provider is used to resolve layout files if they are in use
            var fileProvider = new PhysicalFileProvider(Path.Combine(someRootPath, "EmailTemplates"));
            var options = new LiquidRendererOptions
            {
                FileProvider = fileProvider
            };

            Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));

            // template which utilizes layout
            var template = @"
            {% layout '_layout.liquid' %}
            Dear {{ Name }}, You are totally {{ Compliment }}.";

            var email = Email
                .From("bob@hotmail.com")
                .To("somedude@gmail.com")
                .Subject("woo nuget")
                .UsingTemplate(template, new ViewModel {Name = "Luke", Compliment = "Awesome"});

            #endregion
        }
    }

    internal class ViewModel
    {
        public string Name { get; set; }
        public string Compliment { get; set; }
    }
}