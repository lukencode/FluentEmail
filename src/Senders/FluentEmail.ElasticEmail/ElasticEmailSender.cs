using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using static ElasticEmailClient.Api;
using static ElasticEmailClient.ApiTypes;


namespace FluentEmail.ElasticEmail
{
    public class ElasticEmailSender : ISender
    {
        private readonly string _apiKey;
        private readonly bool _useSubAccount;

        /// <summary>
        /// Creates a sender when sending with ElasticEmail
        /// </summary>
        /// <param name="apiKey">ApiKey for ElasticEmail client</param>
        /// <param name="useSubAccount">If ApiKey is for subaccount of ElasticEmail service</param>
        public ElasticEmailSender(string apiKey, bool useSubAccount = false)
        {
            _apiKey = apiKey;
            _useSubAccount = useSubAccount;
        }

        /// <summary>
        /// Send the specified email.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="token">Cancellation Token.</param>
        /// <returns>A response with any errors and a success boolean.</returns>
        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            return Task.Run(() => SendAsync(email, token)).Result;
        }

        /// <summary>
        /// Send the specified email.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="token">Cancellation Token.</param>
        /// <returns>A response with any errors and a success boolean.</returns>
        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            ElasticEmailClient.Api.ApiKey = _apiKey;

            var sendResponse = new SendResponse();

            if (token?.IsCancellationRequested ?? false)
            {
                sendResponse.ErrorMessages.Add("Message was cancelled by cancellation token.");
                return sendResponse;
            }

            if (_useSubAccount)
            {
                string subAccountApiKey = await ElasticEmailClient.Api.Account.GetSubAccountApiKeyAsync(email.Data.FromAddress.EmailAddress);
                ElasticEmailClient.Api.ApiKey = subAccountApiKey;
            }

            try
            {
                EmailSend elasticEmailResponse = await ElasticEmailClient.Api.Email.SendAsync
                ( 
                    subject: email.Data.Subject,
                    from: email.Data.FromAddress.EmailAddress,
                    fromName: email.Data.FromAddress.Name,
                    msgTo: email.Data.ToAddresses.Select(t => t.EmailAddress),
                    msgCC: email.Data.CcAddresses.Select(t => t.EmailAddress),
                    msgBcc: email.Data.BccAddresses.Select(t => t.EmailAddress),
                    bodyHtml: email.Data.IsHtml ? email.Data.Body : null,
                    bodyText: email.Data.IsHtml ? email.Data.PlaintextAlternativeBody : email.Data.Body,
                    headers: email.Data.Headers,
                    attachmentFiles: await Task.WhenAll(email.Data.Attachments.Select(a => ConvertAttachment(a)))
                );
            }
            catch (Exception ex)
            {
                sendResponse.ErrorMessages.Add($"{ex.Message}");
            }

            return sendResponse;
        }

        /// <summary>
        /// Converts FluentEmail Attachment to ElasticEmail FileData object
        /// </summary>
        /// <param name="attachment">FluentEmail Attachment</param>
        /// <returns>ElasticEmail FileData object</returns>
        private async Task<FileData> ConvertAttachment(Attachment attachment) => new FileData
        {
            Content = await GetAttachmentByteArray(attachment.Data),
            FileName = attachment.Filename,
            ContentType = attachment.ContentType
        };

        /// <summary>
        /// Converts Stream data from FluentEmail Attachment to byte array
        /// </summary>
        /// <param name="stream">Stream with attachment data</param>
        /// <returns>byte array with data from stream</returns>
        private async Task<byte[]> GetAttachmentByteArray(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
