using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Newtonsoft.Json;

namespace FluentEmail.MailPace;

public class MailPaceSender : ISender, IDisposable
{
    private readonly HttpClient _httpClient;

    public MailPaceSender(string serverToken)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("MailPace-Server-Token", serverToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public SendResponse Send(IFluentEmail email, CancellationToken? token = null) =>
        SendAsync(email, token).GetAwaiter().GetResult();

    public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
    {
        var sendRequest = BuildSendRequestFor(email);
        
        var content = new StringContent(JsonConvert.SerializeObject(sendRequest), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("https://app.mailpace.com/api/v1/send", content)
            .ConfigureAwait(false);

        var mailPaceResponse = await TryOrNull(async () => JsonConvert.DeserializeObject<MailPaceResponse>(
            await response.Content.ReadAsStringAsync())) ?? new MailPaceResponse();
        
        if (response.IsSuccessStatusCode)
        {
            return new SendResponse { MessageId = mailPaceResponse.Id };
        }
        else
        {
            var result = new SendResponse();
            
            if (!string.IsNullOrEmpty(mailPaceResponse.Error))
            {
                result.ErrorMessages.Add(mailPaceResponse.Error);
            }

            if (mailPaceResponse.Errors != null && mailPaceResponse.Errors.Count != 0)
            {
                result.ErrorMessages.AddRange(mailPaceResponse.Errors
                    .Select(it => $"{it.Key}: {string.Join("; ", it.Value)}"));
            }

            if (!result.ErrorMessages.Any())
            {
                result.ErrorMessages.Add(response.ReasonPhrase ?? "An unknown error has occurred.");
            }

            return result;
        }
    }

    private static MailPaceSendRequest BuildSendRequestFor(IFluentEmail email)
    {
        var sendRequest = new MailPaceSendRequest
        {
            From = $"{email.Data.FromAddress.Name} <{email.Data.FromAddress.EmailAddress}>",
            To = string.Join(",", email.Data.ToAddresses.Select(it => !string.IsNullOrEmpty(it.Name) ? $"{it.Name} <{it.EmailAddress}>" : it.EmailAddress)),
            Subject = email.Data.Subject
        };

        if (email.Data.CcAddresses.Any())
        {
            sendRequest.Cc = string.Join(",", email.Data.CcAddresses.Select(it => !string.IsNullOrEmpty(it.Name) ? $"{it.Name} <{it.EmailAddress}>" : it.EmailAddress));
        }

        if (email.Data.BccAddresses.Any())
        {
            sendRequest.Bcc = string.Join(",", email.Data.BccAddresses.Select(it => !string.IsNullOrEmpty(it.Name) ? $"{it.Name} <{it.EmailAddress}>" : it.EmailAddress));
        }

        if (email.Data.ReplyToAddresses.Any())
        {
            sendRequest.ReplyTo = string.Join(",", email.Data.ReplyToAddresses.Select(it => !string.IsNullOrEmpty(it.Name) ? $"{it.Name} <{it.EmailAddress}>" : it.EmailAddress));
        }

        if (email.Data.IsHtml)
        {
            sendRequest.HtmlBody = email.Data.Body;
            if (!string.IsNullOrEmpty(email.Data.PlaintextAlternativeBody))
            {
                sendRequest.TextBody = email.Data.PlaintextAlternativeBody;
            }
        }
        else
        {
            sendRequest.TextBody = email.Data.Body;
        }

        if (email.Data.Tags.Any())
        {
            sendRequest.Tags.AddRange(email.Data.Tags);
        }

        if (email.Data.Attachments.Any())
        {
            sendRequest.Attachments.AddRange(
                email.Data.Attachments.Select(it => new MailPaceAttachment
                {
                    Name = it.Filename,
                    Content = it.Data.ConvertToBase64(),
                    ContentType = it.ContentType ?? Path.GetExtension(it.Filename), // jpeg, jpg, png, gif, txt, pdf, docx, xlsx, pptx, csv, att, ics, ical, html, zip
                    Cid = it.IsInline ? it.ContentId : null
                }));
        }
        
        return sendRequest;
    }

    private async Task<T> TryOrNull<T>(Func<Task<T>> method)
    {
        try
        {
            return await method();
        }
        catch
        {
            return default;
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}