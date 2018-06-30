using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using SampleWeb.Models;

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

        public async Task<IActionResult> SendMultiple([FromServices] FluentEmailFactory emailFactory)
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
                .UsingTemplate(@"hi @Model.Name this is the second email @(5 + 5)!", model1)
                .SendAsync();

            return Content("ok");
        }
    }
}
