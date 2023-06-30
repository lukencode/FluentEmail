using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Mailgun;

public interface IMailgunSender : ISender
{
    /// <summary>
    /// Mailgun specific extension method that allows you to use a template instead of a message body.
    /// For more information, see: https://documentation.mailgun.com/en/latest/api-sending.html#sending.
    /// </summary>
    /// <param name="email">Fluent email.</param>
    /// <param name="templateName">Mailgun template name.</param>
    /// <param name="templateData">Mailgun template data.</param>
    /// <param name="token">Optional cancellation token.</param>
    /// <returns>A SendResponse object.</returns>
    Task<SendResponse> SendWithTemplateAsync(IFluentEmail email, string templateName, object templateData, CancellationToken? token = null);
}