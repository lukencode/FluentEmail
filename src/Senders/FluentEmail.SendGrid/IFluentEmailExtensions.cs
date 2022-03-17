using FluentEmail.Core;
using FluentEmail.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentEmail.SendGrid
{
    public static class IFluentEmailExtensions
    {
        public static async Task<SendResponse> SendWithTemplateAsync(this IFluentEmail email, string templateId, object templateData)
        {
            var sendGridSender = email.Sender as ISendGridSender;
            return await sendGridSender.SendWithTemplateAsync(email, templateId, templateData);
        }
    }
}
