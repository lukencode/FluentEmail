using FluentEmail.Core.Interfaces;
using RazorLight;
using RazorLight.Extensions;

namespace FluentEmail.Razor
{
    public class RazorRenderer : ITemplateRenderer
    {
        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            var engine = EngineFactory.CreatePhysical("/");
            return engine.ParseString(template, model);
        }
    }
}
