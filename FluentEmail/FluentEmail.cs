using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace FluentEmail
{
    public class Email
    {
        private SmtpClient _client;

        public MailMessage Message { get; set; }
        public Exception Error { get; set; }
        public bool HasError
        {
            get
            {
                return Error != null;
            }
        }

        private Email()
        {
            Message = new MailMessage() { IsBodyHtml = true };
            _client = new SmtpClient();
        }

        public static Email From(string emailAddress, string name = "")
        {
            var email = new Email();
            email.Message.From = new MailAddress(emailAddress, name);
            return email;
        }
        
        public Email To(string emailAddress, string name = "")
        {
            Message.To.Add(new MailAddress(emailAddress, name));
            return this;
        }

        public Email To(IList<MailAddress> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Message.To.Add(address);
            }
            return this;
        }

        public Email Subject(string subject)
        {
            Message.Subject = subject;
            return this;
        }

        public Email Body(string body)
        {
            Message.Body = body;
            return this;
        }


        public Email UsingClient(SmtpClient client)
        {
            _client = client;
            return this;
        }

        public Email Send()
        {
            try
            {
                _client.Send(Message);
            }
            catch (Exception ex)
            {
                Error = ex;
            }

            return this;
        }

        public Email SendAsync(SendCompletedEventHandler callback, object token = null)
        {
            _client.SendCompleted += callback;
            _client.SendAsync(Message, token);

            return this;
        }

        public Email Cancel()
        {
            _client.SendAsyncCancel();
            return this;
        }
    }
}
