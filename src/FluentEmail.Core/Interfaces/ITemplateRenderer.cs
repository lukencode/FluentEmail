namespace FluentEmail.Core.Interfaces
{
    public interface ITemplateRenderer
    {
        string Parse<T>(string template, T model, bool isHtml = true);
    }
}
