using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace FluentEmail.Mailgun
{
    public static class IFluentEmailExtensions
    {
        public static async Task<SendResponse> SendWithTemplateAsync(this IFluentEmail email, string templateName, object templateData)
        {
            var mailgunSender = email.Sender as IMailgunSender;
            return await mailgunSender.SendWithTemplateAsync(email, templateName, templateData);
        }
    }
}
