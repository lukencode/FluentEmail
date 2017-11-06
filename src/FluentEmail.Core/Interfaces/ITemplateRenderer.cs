using System.Threading.Tasks;

namespace FluentEmail.Core.Interfaces
{
    public interface ITemplateRenderer
    {
        string Parse<T>(string template, T model, bool isHtml = true);
        Task<string> ParseAsync<T>(string template, T model, bool isHtml = true);
    }
}
