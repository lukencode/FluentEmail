using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using RazorEngine;

namespace FluentEmail
{
    public class RazorRenderer : ITemplateRenderer
    {
        public RazorRenderer()
        {
            initializeRazorParser();
        }

        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            return Razor.Parse<T>(template, model);
        }

        private void initializeRazorParser()
        {
            // HACK: this is required to get the Razor Parser to work, no idea why, something to with dynamic objects i guess, tracked this down as the test worked sometimes, turned out
            // it was when the ViewBag was touched from the controller tests, if that happened before the Razor.Parse in ShoudSpikeTheSillyError() then it ran fine.
            dynamic x2 = new ExpandoObject();
            x2.Dummy = "";
        }
    }
}
