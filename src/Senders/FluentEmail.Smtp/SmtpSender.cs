using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.Smtp
{
    public class SmtpSender : ISender
    {
        private readonly Func<SmtpClient> _clientFactory;
        private readonly SmtpClient _smtpClient;

        /// <summary>
        /// Creates a sender using the default SMTP settings.
        /// </summary>
        public SmtpSender() : this(() => new SmtpClient())
        {
        }

        /// <summary>
        /// Creates a sender that uses the factory to create and dispose an SmtpClient with each email sent.
        /// </summary>
        /// <param name="clientFactory"></param>
        public SmtpSender(Func<SmtpClient> clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Creates a sender that uses the given SmtpClient, but does not dispose it.
        /// </summary>
        /// <param name="smtpClient"></param>
        public SmtpSender(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return Task.Run(() => SendAsync(email, token)).Result;
        }

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            var response = new SendResponse();

            var message = CreateMailMessage(email);

            if (token?.IsCancellationRequested ?? false)
            {
                response.ErrorMessages.Add("Message was cancelled by cancellation token.");
                return response;
            }

            if (_smtpClient == null)
            {
                using (var client = _clientFactory())
                {
                    await client.SendMailAsync(message);
                }
            }
            else
            {
                await _smtpClient.SendMailAsync(message);
            }

            return response;
        }

        private MailMessage CreateMailMessage(IFluentEmail email)
        {
            var data = email.Data;
            MailMessage message = null;

            // Smtp seems to require the HTML version as the alternative.
            if (!string.IsNullOrEmpty(data.PlaintextAlternativeBody))
            {
                message = new MailMessage
                {
                    Subject = data.Subject,
                    Body = data.PlaintextAlternativeBody,
                    IsBodyHtml = false,
                    From = new MailAddress(data.FromAddress.EmailAddress, data.FromAddress.Name)
                };

                var mimeType = new System.Net.Mime.ContentType("text/html");
                AlternateView alternate = AlternateView.CreateAlternateViewFromString(data.Body, mimeType);
                message.AlternateViews.Add(alternate);
            }
            else
            {
                message = new MailMessage
                {
                    Subject = data.Subject,
                    Body = data.Body,
                    IsBodyHtml = data.IsHtml,
                    From = new MailAddress(data.FromAddress.EmailAddress, data.FromAddress.Name)
                };
            }

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
