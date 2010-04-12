using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace FluentEmail
{
    public class Email : IHideObjectMembers
    {
        private SmtpClient _client;
        public MailMessage Message { get; set; }

        private Email()
        {
            Message = new MailMessage();
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

        public Email CC(string emailAddress, string name = "")
        {
            Message.CC.Add(new MailAddress(emailAddress, name));
            return this;
        }

        public Email BCC(string emailAddress, string name = "")
        {
            Message.Bcc.Add(new MailAddress(emailAddress, name));
            return this;
        }

        public Email Subject(string subject)
        {
            Message.Subject = subject;
            return this;
        }

        /// <summary>
        /// Adds a Body to the Email
        /// </summary>
        /// <param name="body">The content of the body</param>
        /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns></returns>
        public Email Body(string body, bool isHtml = true)
        {
            Message.Body = body;
            Message.IsBodyHtml = isHtml;
            return this;
        }

        /// <summary>
        /// Adds an Attachment to the Email
        /// </summary>
        /// <param name="attachment">The Attachment to add</param>
        /// <returns></returns>
        public Email Attach(Attachment attachment)
        {
            if (!Message.Attachments.Contains(attachment))
                Message.Attachments.Add(attachment);
            return this;
        }

        /// <summary>
        /// Adds Multiple Attachments to the Email
        /// </summary>
        /// <param name="attachments">The List of Attachments to add</param>
        /// <returns></returns>
        public Email Attach(IList<Attachment> attachments)
        {
            foreach (var attachment in attachments)
            {
                if (!Message.Attachments.Contains(attachment))
                    Message.Attachments.Add(attachment);
            }
            return this;
        }
        
        public Email UsingClient(SmtpClient client)
        {
            _client = client;
            return this;
        }

        public Email Send()
        {
            _client.Send(Message);
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
