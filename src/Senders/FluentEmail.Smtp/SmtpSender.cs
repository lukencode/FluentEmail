using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Smtp
{
    public class SmtpSender : ISender
    {
        private SmtpClient _client;
        public bool UseSsl { get; set; }

        public MailMessage Message { get; set; }

        public SmtpSender() : this(new SmtpClient())
        {
        }

        public SmtpSender(SmtpClient client)
        {
            _client = client;
            Message = new MailMessage();
            UseSsl = true;
        }

        public SendResponse Send(Email email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(Email email, CancellationToken? token = null)
        {
            var response = new SendResponse();
            _client.EnableSsl = UseSsl;

            var message = CreateMailMessage(email);

            if (token?.IsCancellationRequested ?? false)
            {
                response.ErrorMessages.Add("Message was cancelled by cancellation token.");
                return response;
            }

            await _client.SendMailAsync(message);

            Dispose();
            return response;
        }

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();

            if (Message != null)
                Message.Dispose();
        }

        private MailMessage CreateMailMessage(Email email)
        {
            var data = email.Data;
            var message = new MailMessage();
            message.Subject = data.Subject;
            message.Body = data.Body;
            Message.IsBodyHtml = data.IsHtml;
            message.From = new MailAddress(data.FromAddress.EmailAddress, data.FromAddress.Name);
            data.ToAddresses.ForEach(x =>
            {
                message.To.Add(new MailAddress(x.EmailAddress, x.Name));
            });
            data.CcAddresses.ForEach(x =>
            {
                message.CC.Add(new MailAddress(x.EmailAddress, x.Name));
            });
            data.BccAddresses.ForEach(x =>
            {
                message.Bcc.Add(new MailAddress(x.EmailAddress, x.Name));
            });
            data.ReplyToAddresses.ForEach(x =>
            {
                message.ReplyToList.Add(new MailAddress(x.EmailAddress, x.Name));
            });

            if (data.Priority == Priority.Low)
            {
                message.Priority = MailPriority.Low;
            }
            else if (data.Priority == Priority.Normal)
            {
                message.Priority = MailPriority.Normal;                
            }
            else if (data.Priority == Priority.High)
            {
                message.Priority = MailPriority.High;
            }

            data.Attachments.ForEach(x =>
            {
                message.Attachments.Add(new System.Net.Mail.Attachment(x.Data, x.Filename, x.ContentType));
            });                       

            return message;
        }
    }
}
