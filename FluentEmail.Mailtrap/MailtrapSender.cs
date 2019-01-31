using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;

namespace FluentEmail.Mailtrap
{
    public class MailtrapSender : ISender
    {
        private readonly SmtpClient _smtpClient;

        public MailtrapSender(string host, int port, string userName, string password)
        {
            _smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true
            };
        }
        
        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            var smtpSender = new SmtpSender(_smtpClient);
            return smtpSender.Send(email, token);
        }

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            var smtpSender = new SmtpSender(_smtpClient);
            return await smtpSender.SendAsync(email, token);
        }
    }
}