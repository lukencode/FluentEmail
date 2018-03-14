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

        public SmtpSender() : this(new SmtpClient())
        {
        }

        public SmtpSender(SmtpClient client)
        {
            _client = client;
            UseSsl = client.EnableSsl;
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
            _client?.Dispose();
        }

        private MailMessage CreateMailMessage(Email email)
        {
            var data = email.Data;
            var message = new MailMessage
            {
                Subject = data.Subject,
                Body = data.Body,
                IsBodyHtml = data.IsHtml,
                From = new MailAddress(data.FromAddress.EmailAddress, data.FromAddress.Name)
            };

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

            switch (data.Priority)
            {
                case Priority.Low:
                    message.Priority = MailPriority.Low;
                    break;
                case Priority.Normal:
                    message.Priority = MailPriority.Normal;
                    break;
                case Priority.High:
                    message.Priority = MailPriority.High;
                    break;
            }

            data.Attachments.ForEach(x =>
            {
                message.Attachments.Add(new System.Net.Mail.Attachment(x.Data, x.Filename, x.ContentType));
            });                       

            return message;
        }
    }
}
