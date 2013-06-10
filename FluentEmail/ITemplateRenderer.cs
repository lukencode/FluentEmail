using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RazorEngine.Templating;

namespace FluentEmail
{
    public interface ITemplateRenderer
    {
        string Parse<T>(string template, T model, DynamicViewBag viewBag, bool isHtml = true);
    }
}
