using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.Graph
{
    /// <summary>
    /// Implementation of <see cref="ISender"/> for the Microsoft Graph API.
    /// See <see cref="FluentEmailServicesBuilderExtensions"/>.
    /// </summary>
    public class GraphSender : ISender
    {
        private bool _saveSent;

        private GraphServiceClient _graphClient;

        public GraphSender(GraphSenderOptions options)
        {
            _saveSent = options.SaveSentItems ?? true;

            var clientApp = ConfidentialClientApplicationBuilder
                .Create(options.ClientId)
                .WithTenantId(options.TenantId)
                .WithClientSecret(options.Secret)
                .Build();

            var authProvider = new ClientCredentialProvider(clientApp);
            _graphClient = new GraphServiceClient(authProvider);
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            try
            {
                var message = CreateMessage(email);
                await _graphClient.Users[email.Data.FromAddress.EmailAddress]
                    .SendMail(message, _saveSent)
                    .Request()
                    .PostAsync();

                return new SendResponse
                {
                    MessageId = message.Id,
                };
            }
            catch (Exception ex)
            {
                return new SendResponse
                {
                    ErrorMessages = new List<string> { ex.Message },
                };
            }
        }

        private static Message CreateMessage(IFluentEmail email)
        {
            var messageBody = new ItemBody
            {
                Content = email.Data.Body,
                ContentType = email.Data.IsHtml ? BodyType.Html : BodyType.Text,
            };

            var message = new Message();
            message.Subject = email.Data.Subject;
            message.Body = messageBody;
            message.From = ConvertToRecipient(email.Data.FromAddress);
            message.ReplyTo = CreateRecipientList(email.Data.ReplyToAddresses);
            message.ToRecipients = CreateRecipientList(email.Data.ToAddresses);
            message.CcRecipients = CreateRecipientList(email.Data.CcAddresses);
            message.BccRecipients = CreateRecipientList(email.Data.BccAddresses);

            if (email.Data.Attachments != null && email.Data.Attachments.Count > 0)
            {
                message.Attachments = new MessageAttachmentsCollectionPage();

                email.Data.Attachments.ForEach(a =>
                {
                    var attachment = new FileAttachment
                    {
                        Name = a.Filename,
                        ContentType = a.ContentType,
                        ContentBytes = GetAttachmentBytes(a.Data)
                    };

                    message.Attachments.Add(attachment);
                });
            }

            switch (email.Data.Priority)
            {
                case Priority.High:
                    message.Importance = Importance.High;
                    break;
                case Priority.Normal:
                    message.Importance = Importance.Normal;
                    break;
                case Priority.Low:
                    message.Importance = Importance.Low;
                    break;
                default:
                    message.Importance = Importance.Normal;
                    break;
            }

            return message;
        }

        private static IList<Recipient> CreateRecipientList(IEnumerable<Address> addressList)
        {
            if (addressList == null)
            {
                return new List<Recipient>();
            }

            return addressList
                .Select(ConvertToRecipient)
                .ToList();
        }

        private static Recipient ConvertToRecipient(Address address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = address.EmailAddress,
                    Name = address.Name,
                },
            };
        }

        private static byte[] GetAttachmentBytes(Stream stream)
        {
            using var m = new MemoryStream();
            stream.CopyTo(m);
            return m.ToArray();
        }
    }
}
