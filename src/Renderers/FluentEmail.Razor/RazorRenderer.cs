using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using RazorLight;

namespace FluentEmail.Razor
{
    public class RazorRenderer : ITemplateRenderer
    {
        public async Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
        {            
            var engine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();            

            return await engine.CompileRenderAsync<T>(GetHashString(template), template, model);
        }

        string ITemplateRenderer.Parse<T>(string template, T model, bool isHtml)
        {
            return ParseAsync(template, model, isHtml).GetAwaiter().GetResult();
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            var hashbytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
            foreach (byte b in hashbytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
