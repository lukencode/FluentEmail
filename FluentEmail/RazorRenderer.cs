using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Xipton.Razor;

namespace FluentEmail
{
    public class RazorRenderer : ITemplateRenderer
    {
        private RazorMachine _razor;

        public RazorRenderer()
        {
            initializeRazorParser();
        }

        public string Parse<T>(string template, T model)
        {
            var razorTemplate = _razor.ExecuteContent(template, model);
            return razorTemplate.Result;
        }

        private void initializeRazorParser()
        {
            _razor = new RazorMachine();
        }
    }
}
