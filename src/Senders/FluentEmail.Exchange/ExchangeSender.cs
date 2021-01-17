using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Microsoft.Exchange.WebServices.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.Exchange
{
    public class ExchangeSender : ISender
    {
        private readonly ExchangeService meExchangeClient;

        public ExchangeSender(ExchangeService paExchangeClient)
        {
            meExchangeClient = paExchangeClient;
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return System.Threading.Tasks.Task.Run(() => SendAsync(email, token)).Result;
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

            message.Save();
            message.SendAndSaveCopy();

            return response;
        }

        private EmailMessage CreateMailMessage(IFluentEmail paEmail)
        {
            var paData = paEmail.Data;

            EmailMessage loExchangeMessage = new EmailMessage(meExchangeClient)
            {
                Subject = paData.Subject,
                Body = paData.Body,
            };

            if (!string.IsNullOrEmpty(paData.FromAddress?.EmailAddress))
                loExchangeMessage.From = new EmailAddress(paData.FromAddress.Name, paData.FromAddress.EmailAddress);

            paData.ToAddresses.ForEach(x =>
            {
                loExchangeMessage.ToRecipients.Add(new EmailAddress(x.Name, x.EmailAddress));
            });

            paData.CcAddresses.ForEach(x =>
            {
                loExchangeMessage.CcRecipients.Add(new EmailAddress(x.Name, x.EmailAddress));
            });

            paData.BccAddresses.ForEach(x =>
            {
                loExchangeMessage.BccRecipients.Add(new EmailAddress(x.EmailAddress, x.Name));
            });

            switch (paData.Priority)
            {
                case Priority.Low:
                    loExchangeMessage.Importance = Importance.Low;
                    break;

                case Priority.Normal:
                    loExchangeMessage.Importance = Importance.Normal;
                    break;

                case Priority.High:
                    loExchangeMessage.Importance = Importance.High;
                    break;
            }

            paData.Attachments.ForEach(x =>
            {
                // System.Net.Mail.Attachment a = new System.Net.Mail.Attachment(x.Data, x.Filename, x.ContentType);
                // a.ContentId = x.ContentId;
                loExchangeMessage.Attachments.AddFileAttachment(x.Filename, x.Data);
            });

            return loExchangeMessage;
        }
    }
}