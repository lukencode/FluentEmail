using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGridAttachment = SendGrid.Helpers.Mail.Attachment;

namespace FluentEmail.SendGrid
{
    public class SendGridSender : ISender
    {
        private readonly string _apiKey;
        private readonly bool _sandBoxMode;

        public SendGridSender(string apiKey, bool sandBoxMode = false)
        {
            _apiKey = apiKey;
            _sandBoxMode = sandBoxMode;
        }
        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            var sendGridClient = new SendGridClient(_apiKey);

            var mailMessage = new SendGridMessage();
            mailMessage.SetSandBoxMode(_sandBoxMode);

            mailMessage.SetFrom(ConvertAddress(email.Data.FromAddress));

            if (email.Data.ToAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
                mailMessage.AddTos(email.Data.ToAddresses.Select(ConvertAddress).ToList());

            if (email.Data.CcAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
                mailMessage.AddCcs(email.Data.CcAddresses.Select(ConvertAddress).ToList());

            if (email.Data.BccAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
                mailMessage.AddBccs(email.Data.BccAddresses.Select(ConvertAddress).ToList());

            if (email.Data.ReplyToAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
                // SendGrid does not support multiple ReplyTo addresses
                mailMessage.SetReplyTo(email.Data.ReplyToAddresses.Select(ConvertAddress).First());

            mailMessage.SetSubject(email.Data.Subject);

            if (email.Data.Headers.Any())
            {
                mailMessage.AddHeaders(email.Data.Headers);
            }

            if (email.Data.IsHtml)
            {
                mailMessage.HtmlContent = email.Data.Body;
            }
            else
            {
                mailMessage.PlainTextContent = email.Data.Body;
            }

            switch (email.Data.Priority)
            {
                case Priority.High:
                    // https://stackoverflow.com/questions/23230250/set-email-priority-with-sendgrid-api
                    mailMessage.AddHeader("Priority", "Urgent");
                    mailMessage.AddHeader("Importance", "High");
                    // https://docs.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmail/2bb19f1b-b35e-4966-b1cb-1afd044e83ab
                    mailMessage.AddHeader("X-Priority", "1");
                    mailMessage.AddHeader("X-MSMail-Priority", "High");
                    break;

                case Priority.Normal:
                    // Do not set anything.
                    // Leave default values. It means Normal Priority.
                    break;

                case Priority.Low:
                    // https://stackoverflow.com/questions/23230250/set-email-priority-with-sendgrid-api
                    mailMessage.AddHeader("Priority", "Non-Urgent");
                    mailMessage.AddHeader("Importance", "Low");
                    // https://docs.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmail/2bb19f1b-b35e-4966-b1cb-1afd044e83ab
                    mailMessage.AddHeader("X-Priority", "5");
                    mailMessage.AddHeader("X-MSMail-Priority", "Low");
                    break;
            }

            if (!string.IsNullOrEmpty(email.Data.PlaintextAlternativeBody))
            {
                mailMessage.PlainTextContent = email.Data.PlaintextAlternativeBody;
            }

            if (email.Data.Attachments.Any())
            {
                foreach (var attachment in email.Data.Attachments)
                {
                    var sendGridAttachment = await ConvertAttachment(attachment);
                    mailMessage.AddAttachment(sendGridAttachment.Filename, sendGridAttachment.Content,
                        sendGridAttachment.Type, sendGridAttachment.Disposition, sendGridAttachment.ContentId);
                }
            }

            var sendGridResponse = await sendGridClient.SendEmailAsync(mailMessage, token.GetValueOrDefault());

            var sendResponse = new SendResponse();

            if (sendGridResponse.Headers.TryGetValues(
                "X-Message-ID",
                out IEnumerable<string> messageIds))
            {
                sendResponse.MessageId = messageIds.FirstOrDefault();
            }

            if (IsHttpSuccess((int)sendGridResponse.StatusCode)) return sendResponse;

            sendResponse.ErrorMessages.Add($"{sendGridResponse.StatusCode}");
            var messageBodyDictionary = await sendGridResponse.DeserializeResponseBodyAsync(sendGridResponse.Body);

            if (messageBodyDictionary.ContainsKey("errors"))
            {
                var errors = messageBodyDictionary["errors"];

                foreach (var error in errors)
                {
                    sendResponse.ErrorMessages.Add($"{error}");
                }
            }

            return sendResponse;
        }

        private EmailAddress ConvertAddress(Address address) => new EmailAddress(address.EmailAddress, address.Name);

        private async Task<SendGridAttachment> ConvertAttachment(Core.Models.Attachment attachment) => new SendGridAttachment
        {
            Content = await GetAttachmentBase64String(attachment.Data),
            Filename = attachment.Filename,
            Type = attachment.ContentType
        };

        private async Task<string> GetAttachmentBase64String(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private bool IsHttpSuccess(int statusCode)
        {
            return statusCode >= 200 && statusCode < 300;
        }
    }
}
