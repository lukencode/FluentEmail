using System;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using RazorLight;

namespace FluentEmail.Razor
{
    public class RazorRenderer : ITemplateRenderer
    {
        public async Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
        {
            var project = new InMemoryRazorLightProject();
            var engine = new EngineFactory().Create(project);
            
            return await engine.CompileRenderAsync<T>(Guid.NewGuid().ToString(), template, model);
        }

        string ITemplateRenderer.Parse<T>(string template, T model, bool isHtml)
        {
            return ParseAsync(template, model, isHtml).GetAwaiter().GetResult();
        }
    }
}
