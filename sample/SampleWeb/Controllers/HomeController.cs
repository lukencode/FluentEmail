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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
