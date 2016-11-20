using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;

namespace FluentEmail.Core.Defaults
{
    public class ReplaceRenderer : ITemplateRenderer
    {
        public Dictionary<string, string> Replacements { get; set; }

        public ReplaceRenderer()
        {
            Replacements = new Dictionary<string, string>();
        }

        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            foreach (var item in Replacements)
            {
                template = template.Replace(item.Key, item.Value);
            }            

            return template;            
        }
    }
}
