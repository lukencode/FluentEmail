using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.Smtp
{
    public class SmtpSender : ISender
    {
        private readonly Func<SmtpClient> _clientFactory;
        private readonly SmtpClient _smtpClient;

        /// <summary>
        /// Creates a sender using the default SMTP settings.
        /// </summary>
        public SmtpSender() : this(() => new SmtpClient())
        {
        }

        /// <summary>
        /// Creates a sender that uses the factory to create and dispose an SmtpClient with each email sent.
        /// </summary>
        /// <param name="clientFactory"></param>
        public SmtpSender(Func<SmtpClient> clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Creates a sender that uses the given SmtpClient, but does not dispose it.
        /// </summary>
        /// <param name="smtpClient"></param>
        public SmtpSender(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            // Uses task.run to negate Synchronisation Context
            // see: https://stackoverflow.com/questions/28333396/smtpclient-sendmailasync-causes-deadlock-when-throwing-a-specific-exception/28445791#28445791
            return Task.Run(() => SendAsync(email, token)).Result;
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

            if (_smtpClient == null)
            {
                using (var client = _clientFactory())
                {
                    await client.SendMailExAsync(message);                    
                }
            }
            else
            {
                await _smtpClient.SendMailExAsync(message);
            }

            return response;
        }

        private MailMessage CreateMailMessage(IFluentEmail email)
        {
            var data = email.Data;
            MailMessage message = null;

            // Smtp seems to require the HTML version as the alternative.
            if (!string.IsNullOrEmpty(data.PlaintextAlternativeBody))
            {
                message = new MailMessage
                {
                    Subject = data.Subject,
                    Body = data.PlaintextAlternativeBody,
                    IsBodyHtml = false,
                    From = new MailAddress(data.FromAddress.EmailAddress, data.FromAddress.Name)
                };

                var mimeType = new System.Net.Mime.ContentType("text/html; charset=UTF-8");
                AlternateView alternate = AlternateView.CreateAlternateViewFromString(data.Body, mimeType);
                message.AlternateViews.Add(alternate);
            }
            else
            {
                message = new MailMessage
                {
                    Subject = data.Subject,
                    Body = data.Body,
                    IsBodyHtml = data.IsHtml,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8,
                    From = new MailAddress(data.FromAddress.EmailAddress, data.FromAddress.Name)
                };
            }

            foreach (var header in data.Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

            data.ToAddresses.ForEach(x =>
            {
                message.To.Add(new MailAddress(x.EmailAddress, x.Name));
            });

            data.CcAddresses.ForEach(x =>
            {
                message.CC.Add(new MailAddress(x.EmailAddress, x.Name));
            });

            data.BccAddresses.ForEach(x =>
            {
                message.Bcc.Add(new MailAddress(x.EmailAddress, x.Name));
            });

            data.ReplyToAddresses.ForEach(x =>
            {
                message.ReplyToList.Add(new MailAddress(x.EmailAddress, x.Name));
            });

            switch (data.Priority)
            {
                case Priority.Low:
                    message.Priority = MailPriority.Low;
                    break;
                case Priority.Normal:
                    message.Priority = MailPriority.Normal;
                    break;
                case Priority.High:
                    message.Priority = MailPriority.High;
                    break;
            }

            data.Attachments.ForEach(x =>
            {
                System.Net.Mail.Attachment a = new System.Net.Mail.Attachment(x.Data, x.Filename, x.ContentType);

                a.ContentId = x.ContentId;

                message.Attachments.Add(a);
            });

            return message;
        }
    }

    // Taken from https://stackoverflow.com/questions/28333396/smtpclient-sendmailasync-causes-deadlock-when-throwing-a-specific-exception/28445791#28445791
    // SmtpClient causes deadlock when throwing exceptions. This fixes that.
    public static class SendMailEx
    {
        public static Task SendMailExAsync(
            this System.Net.Mail.SmtpClient @this,
            System.Net.Mail.MailMessage message,
            CancellationToken token = default(CancellationToken))
        {
            // use Task.Run to negate SynchronizationContext
            return Task.Run(() => SendMailExImplAsync(@this, message, token));
        }

        private static async Task SendMailExImplAsync(
            System.Net.Mail.SmtpClient client, 
            System.Net.Mail.MailMessage message, 
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var tcs = new TaskCompletionSource<bool>();
            System.Net.Mail.SendCompletedEventHandler handler = null;
            Action unsubscribe = () => client.SendCompleted -= handler;

            handler = async (s, e) =>
            {
                unsubscribe();

                // a hack to complete the handler asynchronously
                await Task.Yield(); 

                if (e.UserState != tcs)
                    tcs.TrySetException(new InvalidOperationException("Unexpected UserState"));
                else if (e.Cancelled)
                    tcs.TrySetCanceled();
                else if (e.Error != null)
                    tcs.TrySetException(e.Error);
                else
                    tcs.TrySetResult(true);
            };

            client.SendCompleted += handler;
            try
            {
                client.SendAsync(message, tcs);
                using (token.Register(() => client.SendAsyncCancel(), useSynchronizationContext: false))
                {
                    await tcs.Task;
                }
            }
            finally
            {
                unsubscribe();
            }
        }
    }
}
