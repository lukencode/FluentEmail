using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Core.Defaults
{
    public class SaveToDiskSender : ISender
    {
        private readonly string _directory;

        public SaveToDiskSender(string directory)
        {
            _directory = directory;
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            var response = new SendResponse();
            await SaveEmailToDisk(email);
            return response;
        }

        private async Task<bool> SaveEmailToDisk(IFluentEmail email)
        {
            var random = new Random();
            var filename = Path.Combine(_directory, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{random.Next(1000)}");

            using (var sw = new StreamWriter(File.OpenWrite(filename)))
            {
                await sw.WriteLineAsync($"From: {email.Data.FromAddress.Name} <{email.Data.FromAddress.EmailAddress}>");
                await sw.WriteLineAsync($"To: {string.Join(",", email.Data.ToAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                await sw.WriteLineAsync($"Cc: {string.Join(",", email.Data.CcAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                await sw.WriteLineAsync($"Bcc: {string.Join(",", email.Data.BccAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                await sw.WriteLineAsync($"ReplyTo: {string.Join(",", email.Data.ReplyToAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                await sw.WriteLineAsync($"Subject: {email.Data.Subject}");
                foreach (var dataHeader in email.Data.Headers)
                {
                    await sw.WriteLineAsync($"{dataHeader.Key}:{dataHeader.Value}");
                }
                await sw.WriteLineAsync();
                await sw.WriteAsync(email.Data.Body);
            }

            return true;
        }
    }
}
