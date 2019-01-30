using FluentEmail.Core.Interfaces;
using RazorLight;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FluentEmail.Razor
{
	public class RazorRenderer : ITemplateRenderer
    {
	    private readonly RazorLightEngine _engine;
	    public RazorRenderer(string root = null)
	    {
		    _engine = new RazorLightEngineBuilder()
			    .UseFilesystemProject(root ?? Directory.GetCurrentDirectory())
			    .UseMemoryCachingProvider()
			    .Build();      
	    }

	    public async Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
	    {            
		    dynamic viewBag = (model as IHaveViewBag)?.ViewBag;
		    return await _engine.CompileRenderAsync<T>(GetHashString(template), template, model, viewBag);
	    }

	    string ITemplateRenderer.Parse<T>(string template, T model, bool isHtml)
	    {
		    return ParseAsync(template, model, isHtml).GetAwaiter().GetResult();
	    }

	    public static string GetHashString(string inputString)
	    {
		    var sb = new StringBuilder();
		    var hashbytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
		    foreach (byte b in hashbytes)
		    {
			    sb.Append(b.ToString("X2"));
		    }

		    return sb.ToString();
	    }
    }
}
