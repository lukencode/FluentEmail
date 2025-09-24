using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.SES
{
    public class SESSender : ISender
    {
        private readonly AmazonSimpleEmailServiceV2Client _emailClient;

        public SESSender(IOptions<FluenEmailSESOptions> options)
        {
            var config = new AmazonSimpleEmailServiceV2Config()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint)
            };

            _emailClient = new AmazonSimpleEmailServiceV2Client(
                options.Value.AccessKeyId,
                options.Value.SecretAccessKey,
                config);
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            var response = new SendResponse();

            if (token?.IsCancellationRequested ?? false)
            {
                response.ErrorMessages.Add("Message was cancelled by cancellation token.");
                return response;
            }

            try
            {
                var message = new MimeMessage();

                if (string.IsNullOrWhiteSpace(email.Data.Subject))
                {
                    response.ErrorMessages.Add("Subject is missing.");
                }

                if (email.Data.ToAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
                {
                    message.To.AddRange(email.Data.ToAddresses.Select(ConvertAddress));
                }

                if (email.Data.CcAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
                {
                    message.Cc.AddRange(email.Data.CcAddresses.Select(ConvertAddress));
                }

                if (email.Data.BccAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
                {
                    message.Bcc.AddRange(email.Data.BccAddresses.Select(ConvertAddress));
                }

                if (email.Data.ReplyToAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
                {
                    message.ReplyTo.AddRange(email.Data.ReplyToAddresses.Select(ConvertAddress));
                }

                switch (email.Data.Priority)
                {
                    case Priority.High:
                        message.Priority = MessagePriority.Urgent;
                        break;

                    case Priority.Normal:
                        // Do not set anything.
                        // Leave default values. It means Normal Priority.
                        break;

                    case Priority.Low:
                        message.Priority = MessagePriority.NonUrgent;
                        break;
                }

                message.From.Add(ConvertAddress(email.Data.FromAddress));
                message.Subject = email.Data.Subject;

                var bodyBuilder = new BodyBuilder();
                if (!string.IsNullOrEmpty(email.Data.PlaintextAlternativeBody))
                {
                    bodyBuilder.TextBody = email.Data.PlaintextAlternativeBody;
                    bodyBuilder.HtmlBody = email.Data.Body;
                }
                else if (!email.Data.IsHtml)
                {
                    bodyBuilder.TextBody = email.Data.Body;
                }
                else
                {
                    bodyBuilder.HtmlBody = email.Data.Body;
                }

                foreach (var attachment in email.Data.Attachments ?? new List<Attachment>())
                {
                    if (attachment.Data != null)
                    {
                        bodyBuilder.Attachments.Add(
                            attachment.Filename,
                            attachment.Data,
                            MimeKit.ContentType.Parse(attachment.ContentType ?? "application/octet-stream")
                        );
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var messageStream = new MemoryStream();
                await message.WriteToAsync(messageStream);
                messageStream.Seek(0, SeekOrigin.Begin);

                var emailResponse = await _emailClient.SendEmailAsync(new SendEmailRequest
                {
                    Content = new EmailContent
                    {
                        Raw = new RawMessage()
                        {
                            Data = messageStream
                        }
                    }
                });

                response.MessageId = emailResponse.MessageId;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
            }

            return response;
        }

        private MailboxAddress ConvertAddress(Address address) => new MailboxAddress(address.Name, address.EmailAddress);
    }
}
