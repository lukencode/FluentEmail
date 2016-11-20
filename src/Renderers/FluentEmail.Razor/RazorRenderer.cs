using FluentEmail.Core.Interfaces;
using RazorLight;

namespace FluentEmail.Razor
{
    public class RazorRenderer : ITemplateRenderer
    {
        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            var engine = EngineFactory.CreatePhysical("/");
            return engine.Parse(template, model);
        }
    }
}
