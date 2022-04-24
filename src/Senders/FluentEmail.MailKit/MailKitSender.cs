using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.MailKitSmtp
{
    /// <summary>
    /// Send emails with the MailKit Library.
    /// </summary>
    public class MailKitSender : ISender
    {
        private readonly SmtpClientOptions _smtpClientOptions;
        private readonly bool _isAmazonSes;

        /// <summary>
        /// Creates a sender that uses the given SmtpClientOptions when sending with MailKit. Since the client is internal this will dispose of the client.
        /// </summary>
        /// <param name="smtpClientOptions">The SmtpClientOptions to use to create the MailKit client</param>
        public MailKitSender(SmtpClientOptions smtpClientOptions)
        {
            _smtpClientOptions = smtpClientOptions;
            _isAmazonSes = smtpClientOptions.Server.EndsWith("amazonaws.com");
        }

        /// <summary>
        /// Send the specified email.
        /// </summary>
        /// <returns>A response with any errors and a success boolean.</returns>
        /// <param name="email">Email.</param>
        /// <param name="token">Cancellation Token.</param>
        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            var message = CreateMailMessage(email);
            var response = new SendResponse() { MessageId = message.MessageId };

            if (token?.IsCancellationRequested ?? false)
            {
                response.ErrorMessages.Add("Message was cancelled by cancellation token.");
                return response;
            }

            try
            {
                if (_smtpClientOptions.UsePickupDirectory)
                {
                    this.SaveToPickupDirectory(message, _smtpClientOptions.MailPickupDirectory).Wait();
                    return response;
                }

                using (var client = new SmtpClient())
                {
                    if (_smtpClientOptions.SocketOptions.HasValue)
                    {
                        client.Connect(
                            _smtpClientOptions.Server,
                            _smtpClientOptions.Port,
                            _smtpClientOptions.SocketOptions.Value,
                            token.GetValueOrDefault());
                    }
                    else
                    {
                        client.Connect(
                            _smtpClientOptions.Server,
                            _smtpClientOptions.Port,
                            _smtpClientOptions.UseSsl,
                            token.GetValueOrDefault());
                    }

                    // Note: only needed if the SMTP server requires authentication
                    if (_smtpClientOptions.RequiresAuthentication)
                    {
                        client.Authenticate(_smtpClientOptions.User, _smtpClientOptions.Password, token.GetValueOrDefault());
                    }

                    var mre = new ManualResetEventSlim(false);
                    if (_isAmazonSes)
                    {
                        // If using Amazon SES, subscribe to the MessageSent event where we can retrieve the overwritten MessageId, then signal ManualResetEventSlim
                        client.MessageSent += (s, e) => SmtpClient_MessageSent(s, e, mre);
                    }
                    else
                    {
                        // Otherwise signal ManualResetEventSlim right away
                        mre.Set();
                    }

                    client.Send(message, token.GetValueOrDefault());
                    client.Disconnect(true, token.GetValueOrDefault());

                    // Block until ManualResetEventSlim is signaled
                    mre.Wait();
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Send the specified email.
        /// </summary>
        /// <returns>A response with any errors and a success boolean.</returns>
        /// <param name="email">Email.</param>
        /// <param name="token">Cancellation Token.</param>
        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            var message = CreateMailMessage(email);
            var response = new SendResponse() { MessageId = message.MessageId };

            if (token?.IsCancellationRequested ?? false)
            {
                response.ErrorMessages.Add("Message was cancelled by cancellation token.");
                return response;
            }

            try
            {
                if (_smtpClientOptions.UsePickupDirectory)
                {
                    await this.SaveToPickupDirectory(message, _smtpClientOptions.MailPickupDirectory);
                    return response;
                }

                using (var client = new SmtpClient())
                {
                    if (_smtpClientOptions.SocketOptions.HasValue)
                    {
                        await client.ConnectAsync(
                            _smtpClientOptions.Server,
                            _smtpClientOptions.Port,
                            _smtpClientOptions.SocketOptions.Value,
                            token.GetValueOrDefault());
                    }
                    else
                    {
                        await client.ConnectAsync(
                            _smtpClientOptions.Server,
                            _smtpClientOptions.Port,
                            _smtpClientOptions.UseSsl,
                            token.GetValueOrDefault());
                    }

                    // Note: only needed if the SMTP server requires authentication
                    if (_smtpClientOptions.RequiresAuthentication)
                    {
                        await client.AuthenticateAsync(_smtpClientOptions.User, _smtpClientOptions.Password, token.GetValueOrDefault());
                    }

                    var mre = new ManualResetEventSlim(false);
                    if (_isAmazonSes)
                    {
                        // If using Amazon SES, subscribe to the MessageSent event where we can retrieve the overwritten MessageId, then signal ManualResetEventSlim
                        client.MessageSent += (s, e) => SmtpClient_MessageSent(s, e, mre);
                    }
                    else
                    {
                        // Otherwise signal ManualResetEventSlim right away
                        mre.Set();
                    }

                    await client.SendAsync(message, token.GetValueOrDefault());
                    await client.DisconnectAsync(true, token.GetValueOrDefault());

                    // Block until ManualResetEventSlim is signaled
                    mre.Wait();
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
            }

            return response;
        }

        private void SmtpClient_MessageSent(object sender, MessageSentEventArgs e, ManualResetEventSlim mre)
        {
            // Example response format: "Ok 010701805bea386d-8411ef2a-5a8b-46bc-9cbb-585ace484c24-000000"
            var match = Regex.Match(e.Response, @"Ok ([0-9a-z\-]+)");
            if (match.Success)
            {
                // Strip "email-smtp" and similar prefixes from SMTP hostname: https://docs.aws.amazon.com/general/latest/gr/ses.html
                var domain = _smtpClientOptions.Server.Substring(_smtpClientOptions.Server.IndexOf('.') + 1);
                var id = $"<{match.Groups[1].Value}@{domain}>";
                e.Message.MessageId = id;
            }

            // Trigger ManualResetEventSlim to signal that the processing is now complete
            mre.Set();
        }

        /// <summary>
        /// Saves email to a pickup directory.
        /// </summary>
        /// <param name="message">Message to save for pickup.</param>
        /// <param name="pickupDirectory">Pickup directory.</param>
        private async Task SaveToPickupDirectory(MimeMessage message, string pickupDirectory)
        {
            // Note: this will require that you know where the specified pickup directory is.
            var path = Path.Combine(pickupDirectory, Guid.NewGuid().ToString() + ".eml");

            if (File.Exists(path))
                return;

            try
            {
                using (var stream = new FileStream(path, FileMode.CreateNew))
                {
                    await message.WriteToAsync(stream);
                    return;
                }
            }
            catch (IOException)
            {
                // The file may have been created between our File.Exists() check and
                // our attempt to create the stream.
                throw;
            }
        }

        /// <summary>
        /// Create a MimMessage so MailKit can send it
        /// </summary>
        /// <returns>The mail message.</returns>
        /// <param name="email">Email data.</param>
        private MimeMessage CreateMailMessage(IFluentEmail email)
        {
            var data = email.Data;

            var message = new MimeMessage();

            // fixes https://github.com/lukencode/FluentEmail/issues/228
            // if for any reason, subject header is not added, add it else update it.
            if (!message.Headers.Contains(HeaderId.Subject))
                message.Headers.Add(HeaderId.Subject, Encoding.UTF8, data.Subject ?? string.Empty);
            else
                message.Headers[HeaderId.Subject] = data.Subject ?? string.Empty;

            message.Headers.Add(HeaderId.Encoding, Encoding.UTF8.EncodingName);

            message.From.Add(new MailboxAddress(data.FromAddress.Name, data.FromAddress.EmailAddress));

            var builder = new BodyBuilder();
            if (!string.IsNullOrEmpty(data.PlaintextAlternativeBody))
            {
                builder.TextBody = data.PlaintextAlternativeBody;
                builder.HtmlBody = data.Body;
            }
            else if (!data.IsHtml)
            {
                builder.TextBody = data.Body;
            }
            else
            {
                builder.HtmlBody = data.Body;
            }

            data.Attachments.ForEach(x =>
            {
                var attachment = builder.Attachments.Add(x.Filename, x.Data, ContentType.Parse(x.ContentType));
                attachment.ContentId = x.ContentId;
            });


            message.Body = builder.ToMessageBody();

            foreach (var header in data.Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

            data.ToAddresses.ForEach(x =>
            {
                message.To.Add(new MailboxAddress(x.Name, x.EmailAddress));
            });

            data.CcAddresses.ForEach(x =>
            {
                message.Cc.Add(new MailboxAddress(x.Name, x.EmailAddress));
            });

            data.BccAddresses.ForEach(x =>
            {
                message.Bcc.Add(new MailboxAddress(x.Name, x.EmailAddress));
            });

            data.ReplyToAddresses.ForEach(x =>
            {
                message.ReplyTo.Add(new MailboxAddress(x.Name, x.EmailAddress));
            });

            switch (data.Priority)
            {
                case Priority.Low:
                    message.Priority = MessagePriority.NonUrgent;
                    break;
                case Priority.Normal:
                    message.Priority = MessagePriority.Normal;
                    break;
                case Priority.High:
                    message.Priority = MessagePriority.Urgent;
                    break;
            }

            return message;
        }
    }
}
