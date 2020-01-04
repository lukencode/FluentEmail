using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Mvc;

namespace SampleWeb.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index([FromServices]IFluentEmail email)
        {
            var model = new
            {
                Name = "test name"
            };

            await email
                .To("test@test.test")
                .Subject("test email subject")
                .UsingTemplate(@"hi @Model.Name this is a razor template @(5 + 5)!", model)
                .SendAsync();

            return View();
        }

        public async Task<IActionResult> SendMultiple([FromServices] IFluentEmailFactory emailFactory)
        {
            var model1 = new
            {
                Name = "test name"
            };

            await emailFactory.Create()
                .To("test1@test.test")
                .SetFrom("sender1@test.test")
                .Subject("test email subject")
                .UsingTemplate(@"hi @Model.Name this is the first email @(5 + 5)!", model1)
                .SendAsync();

            var model2 = new
            {
                Name = "test2 name"
            };

            await emailFactory.Create()
                .To("test1@test.test")
                .SetFrom("sender2@test.test")
                .Subject("test email subject")
                .UsingTemplate(@"hi @Model.Name this is the second email @(5 + 5)!", model2)
                .SendAsync();

            return Content("ok");
        }

        public async Task<IActionResult> NonDependencyInjectionTest()
        {
            Email.DefaultSender = new SmtpSender(new SmtpClient("localhost", 25));
            Email.DefaultRenderer = new RazorRenderer();

            var model = new
            {
                Name = "Ben"
            };

            var assembly = Assembly.GetAssembly(typeof(SampleWeb.Program));

            await Email.From("nondependencyinjection@test.test")
                .To("test2@test.test")
                .Subject("old way")
                .UsingTemplateFromEmbedded("SampleWeb.EmailTemplates.Test1.cshtml", model, assembly)
                .SendAsync();
            
            return Ok();
        }
    }
}
