using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using Azure.Core;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Azure.Email;

// Read more about Azure Email Communication Services here:
// https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/send-email?pivots=programming-language-csharp
public class AzureEmailSender : ISender
{
    private EmailClient _emailClient;

    /// <summary>
    /// Initializes a new instance of <see cref="AzureEmailSender"/>
    /// </summary>
    /// <param name="connectionString">Connection string acquired from the Azure Communication Services resource.</param>
    public AzureEmailSender(string connectionString)
    {
        _emailClient = new EmailClient(connectionString);
    }

    /// <summary> Initializes a new instance of <see cref="AzureEmailSender"/>.</summary>
    /// <param name="connectionString">Connection string acquired from the Azure Communication Services resource.</param>
    /// <param name="options">Client option exposing <see cref="ClientOptions.Diagnostics"/>, <see cref="ClientOptions.Retry"/>, <see cref="ClientOptions.Transport"/>, etc.</param>
    public AzureEmailSender(string connectionString, EmailClientOptions options)
    {
        _emailClient = new EmailClient(connectionString, options);
    }

    /// <summary> Initializes a new instance of <see cref="AzureEmailSender"/>.</summary>
    /// <param name="endpoint">The URI of the Azure Communication Services resource.</param>
    /// <param name="keyCredential">The <see cref="AzureKeyCredential"/> used to authenticate requests.</param>
    /// <param name="options">Client option exposing <see cref="ClientOptions.Diagnostics"/>, <see cref="ClientOptions.Retry"/>, <see cref="ClientOptions.Transport"/>, etc.</param>
    public AzureEmailSender(Uri endpoint, AzureKeyCredential keyCredential, EmailClientOptions options = default)
    {
        _emailClient = new EmailClient(endpoint, keyCredential, options);
    }

    /// <summary> Initializes a new instance of <see cref="AzureEmailSender"/>.</summary>
    /// <param name="endpoint">The URI of the Azure Communication Services resource.</param>
    /// <param name="tokenCredential">The TokenCredential used to authenticate requests, such as DefaultAzureCredential.</param>
    /// <param name="options">Client option exposing <see cref="ClientOptions.Diagnostics"/>, <see cref="ClientOptions.Retry"/>, <see cref="ClientOptions.Transport"/>, etc.</param>
    public AzureEmailSender(Uri endpoint, TokenCredential tokenCredential, EmailClientOptions options = default)
    {
        _emailClient = new EmailClient(endpoint, tokenCredential, options);
    }

    public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
    {
        return SendAsync(email, token).GetAwaiter().GetResult();
    }

    public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
    {
        var emailContent = new EmailContent(email.Data.Subject);
        
        if (email.Data.IsHtml)
        {
            emailContent.Html = email.Data.Body;
        }
        else
        {
            emailContent.PlainText = email.Data.Body;
        }

        var toRecipients = new List<EmailAddress>();
        
        if(email.Data.ToAddresses.Any())
        {
            email.Data.ToAddresses.ForEach(r => toRecipients.Add(new EmailAddress(r.EmailAddress, r.Name)));
        }

        var ccRecipients = new List<EmailAddress>();
        
        if(email.Data.CcAddresses.Any())
        {
            email.Data.CcAddresses.ForEach(r => ccRecipients.Add(new EmailAddress($"cc{r.EmailAddress}", r.Name)));
        }
        
        var bccRecipients = new List<EmailAddress>();
        
        if(email.Data.BccAddresses.Any())
        {
            email.Data.BccAddresses.ForEach(r => bccRecipients.Add(new EmailAddress($"bcc{r.EmailAddress}", r.Name)));
        }
        
        var emailRecipients = new EmailRecipients(toRecipients, ccRecipients, bccRecipients);

        var sender = $"{email.Data.FromAddress.Name} <{email.Data.FromAddress.EmailAddress}>";
        var emailMessage = new EmailMessage(sender, emailContent, emailRecipients);
        
        if (email.Data.ReplyToAddresses.Any(a => !string.IsNullOrWhiteSpace(a.EmailAddress)))
        {
            foreach (var emailAddress in email.Data.ReplyToAddresses)
            {
                emailMessage.ReplyTo.Add(new EmailAddress(emailAddress.EmailAddress, emailAddress.Name));
            }
        }
        
        if (email.Data.Headers.Any())
        {
            foreach (var header in email.Data.Headers)
            {
                emailMessage.CustomHeaders.Add(new EmailCustomHeader(header.Key, header.Value));
            }
        }

        if(email.Data.Attachments.Any())
        {
            foreach (var attachment in email.Data.Attachments)
            {
                emailMessage.Attachments.Add(await ConvertAttachment(attachment));
            }
        }

        emailMessage.Importance = email.Data.Priority switch
        {
            Priority.High => EmailImportance.High,
            Priority.Normal => EmailImportance.Normal,
            Priority.Low => EmailImportance.Low,
            _ => EmailImportance.Normal
        };

        try
        {
            var sendEmailResult = (await _emailClient.SendAsync(emailMessage, token ?? CancellationToken.None)).Value;
            
            var messageId = sendEmailResult.MessageId;
            if (string.IsNullOrWhiteSpace(messageId))
            {
                return new SendResponse
                {
                    ErrorMessages = new List<string> { "Failed to send email." }
                };
            }
            
            // We want to verify that the email was sent.
            // The maximum time we will wait for the message status to be sent/delivered is 2 minutes.
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            SendStatusResult sendStatusResult;
            do
            {
                sendStatusResult = await _emailClient.GetSendStatusAsync(messageId, cancellationToken.Token);
                
                if (sendStatusResult.Status != SendStatus.Queued)
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken.Token);
            } while (!cancellationToken.IsCancellationRequested);

            if (cancellationToken.IsCancellationRequested)
            {
                return new SendResponse
                {
                    ErrorMessages = new List<string> { "Failed to send email, timed out while getting status." }
                };
            }

            if (sendStatusResult.Status == SendStatus.OutForDelivery)
            {
                return new SendResponse
                {
                    MessageId = messageId
                };
            }
            
            return new SendResponse
            {
                ErrorMessages = new List<string> { "Failed to send email." }
            };
        }
        catch (Exception ex)
        {
            return new SendResponse
            {
                ErrorMessages = new List<string> { ex.Message }
            };
        }
    }
    
    private async Task<EmailAttachment> ConvertAttachment(Attachment attachment) =>
        new(attachment.Filename, attachment.ContentType,
            await GetAttachmentAsBase64String(attachment.Data));

    private async Task<string> GetAttachmentAsBase64String(Stream stream)
    {
        using var ms = new MemoryStream();
        
        await stream.CopyToAsync(ms);
        
        return Convert.ToBase64String(ms.ToArray());
    }
}