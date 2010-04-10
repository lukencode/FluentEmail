using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace FluentEmail
{
    public class Email
    {
        public MailMessage Message { get; set; }
        public Exception Error { get; set; }
        public bool HasError
        {
            get
            {
                return Error != null;
            }
        }

        public bool IsValid
        {
            get
            {

            }
        }

        public static Email New { get { return new Email(); } }

        private Email()
        {
            Message = new MailMessage() { IsBodyHtml = true };
        }

        public Email To(string emailAddress, string name = "")
        {
            Message.To.Add(new MailAddress(emailAddress, name));
            return this;
        }

        public Email From(string emailAddress, string name = "")
        {
            Message.From = new MailAddress(emailAddress, name);
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

        public Email Send()
        {
            try
            {
                var client = new SmtpClient();
                client.Send(Message);
            }
            catch (Exception ex)
            {
                Error = ex;
            }

            return this;
        }
    }
}
