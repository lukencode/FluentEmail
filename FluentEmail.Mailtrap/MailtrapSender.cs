using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Mailtrap
{
    public class MailtrapSender : ISender
    {
        private readonly string _username;
        private readonly string _password;
        private HttpClient _httpClient;

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            throw new NotImplementedException();
        }

        public Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            throw new NotImplementedException();
        }
    }
}