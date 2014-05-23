using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using RazorEngine;
using RazorEngine.Templating;

namespace FluentEmail
{
    public class RazorRenderer : ITemplateRenderer
    {
        public RazorRenderer()
        {
        }

        public string Parse<T>(string template, T model, DynamicViewBag viewbag, bool isHtml = true)
        {
            return Razor.Parse(template, model, viewbag, template.GetHashCode().ToString());
        }
    }
}
