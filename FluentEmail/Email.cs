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
        private bool _useSsl;

        public FluentMailMessage Message { get; set; }

        private Email()
        {
            Message = new FluentMailMessage();
            _client = new SmtpClient();
        }

        /// <summary>
        /// Creates a new Email instance and sets the from
        /// property
        /// </summary>
        /// <param name="emailAddress">Email address to send from</param>
        /// <param name="name">Name to send from</param>
        /// <returns>Instance of the Email class</returns>
        public static Email From(string emailAddress, string name = "")
        {
            var email = new Email();
            email.Message.From = new MailAddress(emailAddress, name);
            return email;
        }

        /// <summary>
        /// Adds a reciepient to the email
        /// </summary>
        /// <param name="emailAddress">Email address of recipeient</param>
        /// <param name="name">Name of recipient</param>
        /// <returns>Instance of the Email class</returns>
        public Email To(string emailAddress, string name = "")
        {
            Message.To.Add(new MailAddress(emailAddress, name));
            return this;
        }

        /// <summary>
        /// Adds all reciepients in list to email
        /// </summary>
        /// <param name="mailAddresses">List of recipients</param>
        /// <returns>Instance of the Email class</returns>
        public Email To(IList<MailAddress> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Message.To.Add(address);
            }
            return this;
        }

        /// <summary>
        /// Adds a Carbon Copy to the email
        /// </summary>
        /// <param name="emailAddress">Email address to cc</param>
        /// <param name="name">Name to cc</param>
        /// <returns>Instance of the Email class</returns>
        public Email CC(string emailAddress, string name = "")
        {
            Message.CC.Add(new MailAddress(emailAddress, name));
            return this;
        }

        /// <summary>
        /// Adds a blind carbon copy to the email
        /// </summary>
        /// <param name="emailAddress">Email address of bcc</param>
        /// <param name="name">Name of bcc</param>
        /// <returns>Instance of the Email class</returns>
        public Email BCC(string emailAddress, string name = "")
        {
            Message.Bcc.Add(new MailAddress(emailAddress, name));
            return this;
        }

        /// <summary>
        /// Sets the subject of the email
        /// </summary>
        /// <param name="subject">email subject</param>
        /// <returns>Instance of the Email class</returns>
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
        /// Adds the template file to the email
        /// </summary>
        /// <param name="filename">The path to the file to load</param>
        /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns>Instance of the Email class</returns>
        public Email UsingTemplate(string filename, bool isHtml = true)
        {
            Message.BodyFileName = filename;
            Message.IsBodyHtml = isHtml;
            return this;
        }

        /// <summary>
        /// Adds an Attachment to the Email
        /// </summary>
        /// <param name="attachment">The Attachment to add</param>
        /// <returns>Instance of the Email class</returns>
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
        /// <returns>Instance of the Email class</returns>
        public Email Attach(IList<Attachment> attachments)
        {
            foreach (var attachment in attachments)
            {
                if (!Message.Attachments.Contains(attachment))
                    Message.Attachments.Add(attachment);
            }
            return this;
        }

        /// <summary>
        /// Adds a replace field for the template
        /// </summary>
        /// <param name="key">The Template text to replace</param>
        /// <param name="value">The value to replace the key with</param>
        /// <returns>Instance of the TemplateEmail class</returns>
        public Email Replace(string key, string value)
        {
            Message.Replacements.Add(key, value);
            return this;
        }

        /// <summary>
        /// Over rides the default client from .config file
        /// </summary>
        /// <param name="client">Smtp client to send from</param>
        /// <returns>Instance of the Email class</returns>
        public Email UsingClient(SmtpClient client)
        {
            _client = client;
            return this;
        }

        public Email UseSSL()
        {
            _useSsl = true;
            return this;
        }

        /// <summary>
        /// Sends email synchronously
        /// </summary>
        /// <returns>Instance of the Email class</returns>
        public Email Send()
        {
            if (!String.IsNullOrEmpty(Message.BodyFileName) && String.IsNullOrEmpty(Message.Body))
            {
                //Using HTML template
                Message.GenerateBody();
            }
            _client.EnableSsl = _useSsl;
            _client.Send(Message);
            return this;
        }

        /// <summary>
        /// Sends message asynchronously with a callback
        /// handler
        /// </summary>
        /// <param name="callback">Method to call on complete</param>
        /// <param name="token">User token to pass to callback</param>
        /// <returns>Instance of the Email class</returns>
        public Email SendAsync(SendCompletedEventHandler callback, object token = null)
        {
            if (!String.IsNullOrEmpty(Message.BodyFileName) && String.IsNullOrEmpty(Message.Body))
            {
                //Using HTML template
                Message.GenerateBody();
            }
            _client.EnableSsl = _useSsl;
            _client.SendCompleted += callback;
            _client.SendAsync(Message, token);

            return this;
        }

        /// <summary>
        /// Cancels async message sending
        /// </summary>
        /// <returns>Instance of the Email class</returns>
        public Email Cancel()
        {
            _client.SendAsyncCancel();
            return this;
        }
    }
}
