using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;

namespace FluentEmail.Mailtrap
{
    /// <summary>
    /// Send emails to a Mailtrap.io inbox
    /// </summary>
    public class MailtrapSender : ISender
    {
        private readonly SmtpClient _smtpClient;
        private static readonly int[] ValidPorts = {25, 465, 2525};

        /// <summary>
        /// Creates a sender that uses the given Mailtrap credentials, but does not dispose it.
        /// </summary>
        /// <param name="userName">Username of your mailtrap.io SMTP inbox</param>
        /// <param name="password">Password of your mailtrap.io SMTP inbox</param>
        /// <param name="host">Host address for the Mailtrap.io SMTP inbox</param>
        /// <param name="port">Port for the Mailtrap.io SMTP server. Accepted values are 25, 465 or 2525.</param>
        public MailtrapSender(string userName, string password, string host = "smtp.mailtrap.io", int? port = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Mailtrap UserName needs to be supplied", nameof(userName));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Mailtrap Password needs to be supplied", nameof(password));

            if (port.HasValue && !ValidPorts.Contains(port.Value))
                throw new ArgumentException("Mailtrap Port needs to be either 25, 465 or 2525", nameof(port));
            
            _smtpClient = new SmtpClient(host, port.GetValueOrDefault(2525))
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