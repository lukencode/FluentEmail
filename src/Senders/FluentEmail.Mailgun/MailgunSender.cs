using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace FluentEmail.Mailgun
{
    public class MailgunSender : ISender
    {
        private readonly string _apiKey;
        private readonly string _domainName;

        public MailgunSender(string domainName, string apiKey)
        {
            _domainName = domainName;
            _apiKey = apiKey;
        }

        public SendResponse Send(Email email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public Task<SendResponse> SendAsync(Email email, CancellationToken? token = null)
        {
            var client = new RestClient($"https://api.mailgun.net/v3/{_domainName}");
            client.Authenticator = new HttpBasicAuthenticator("api", _apiKey);

            var request = new RestRequest("messages", Method.POST);
            request.AddParameter("from", $"{email.Data.FromAddress.Name} <{email.Data.FromAddress.EmailAddress}>");
            email.Data.ToAddresses.ForEach(x => {
                request.AddParameter("to", $"{x.Name} <{x.EmailAddress}>");
            });
            email.Data.CcAddresses.ForEach(x => {
                request.AddParameter("cc", $"{x.Name} <{x.EmailAddress}>");
            });
            email.Data.BccAddresses.ForEach(x => {
                request.AddParameter("bcc", $"{x.Name} <{x.EmailAddress}>");
            });
            request.AddParameter("subject", email.Data.Subject);

            request.AddParameter(email.Data.IsHtml ? "html" : "text", email.Data.Body);

            if (email.Data.Attachments.Any())
            {
                request.AlwaysMultipartFormData = true;
            }
            email.Data.Attachments.ForEach(x =>
            {
                request.AddFile("attachment", StreamWriter(x.Data), x.Filename, x.Data.Length, x.ContentType);
            });

            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<SendResponse>();

                var handle = client.ExecuteAsync<MailgunResponse>(request, response =>
                {
                    var result = new SendResponse();
                    if (string.IsNullOrEmpty(response.Data.Id))
                    {
                        result.ErrorMessages.Add(response.Data.Message);
                    }
                    t.TrySetResult(result);
                });

                return t.Task;
            });
        }

        private Action<Stream> StreamWriter(Stream stream)
        {
            return s =>
            {
                stream.CopyTo(s);
                stream.Dispose();
            };
        }
    }
}
