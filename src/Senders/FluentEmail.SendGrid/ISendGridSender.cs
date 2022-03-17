using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.SendGrid
{
    public interface ISendGridSender : ISender
    {
        /// <summary>
        /// SendGrid specific extension method that allows you to use a template instead of a message body.
        /// For more information, see: https://sendgrid.com/docs/ui/sending-email/how-to-send-an-email-with-dynamic-transactional-templates/.
        /// </summary>
        /// <param name="email">Fluent email.</param>
        /// <param name="templateId">SendGrid template ID.</param>
        /// <param name="templateData">SendGrid template data.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A SendResponse object.</returns>
        Task<SendResponse> SendWithTemplateAsync(IFluentEmail email, string templateId, object templateData, CancellationToken? token = null);
    }
}
