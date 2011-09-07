using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using FluentEmail.TemplateParsers;
using RazorEngine;
using RazorEngine.Templating;
using System.Dynamic;

namespace FluentEmail
{
    public class Email : IHideObjectMembers
    {
        private SmtpClient _client;
        private bool _useSsl;

        public MailMessage Message { get; set; }

        private Email()
        {
            Message = new MailMessage();
            _client = new SmtpClient();
        }

        /// <summary>
        /// Creates a new email instance using the default from
        /// address from smtp config settings
        /// </summary>
        /// <returns>Instance of the Email class</returns>
        public static Email FromDefault()
        {
            var email = new Email
            {
                Message = new MailMessage()
            };

            return email;
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
            var email = new Email
                            {
                                Message = { From = new MailAddress(emailAddress, name) }
                            };
            return email;
        }

        /// <summary>
		/// Adds a reciepient to the email. Supports semi colon and comma seperation of email addresses
		/// If name is supplied it should be formatted in the same order as the email addresses
        /// </summary>
        /// <param name="emailAddress">Email address of recipeient</param>
        /// <param name="name">Name of recipient</param>
        /// <returns>Instance of the Email class</returns>
        public Email To(string emailAddress, string name = ""){
        	AssignToAddresses(GetMailAddresses(emailAddress, name), AddressType.To);
        	return this;
        }

        /// <summary>
        /// Adds all reciepients in list to email
        /// </summary>
        /// <param name="mailAddresses">List of recipients</param>
        /// <returns>Instance of the Email class</returns>
        public Email To(IList<MailAddress> mailAddresses)
        {
            AssignToAddresses(mailAddresses, AddressType.To);
            return this;
        }

        /// <summary>
		/// Adds a Carbon Copy to the email. Supports semi colon and comma seperation of email addresses
		/// If name is supplied it should be formatted in the same order as the email addresses
        /// </summary>
        /// <param name="emailAddress">Email address to cc</param>
        /// <param name="name">Name to cc</param>
        /// <returns>Instance of the Email class</returns>
        public Email CC(string emailAddress, string name = "")
        {
			AssignToAddresses(GetMailAddresses(emailAddress, name), AddressType.Cc);
            return this;
        }

		/// <summary>
		/// Adds a blind carbon copy to the email. Supports semi colon and comma seperation of email addresses
		/// If name is supplied it should be formatted in the same order as the email addresses
		/// </summary>
		/// <param name="emailAddress">Email address of bcc</param>
		/// <param name="name">Name of bcc</param>
		/// <returns>
		/// Instance of the Email class
		/// </returns>
        public Email BCC(string emailAddress, string name = "")
        {
			AssignToAddresses(GetMailAddresses(emailAddress, name), AddressType.Bcc);
            return this;
        }

		/// <summary>
		/// Sets the ReplyTo address on the email. Supports semi colon and comma seperation of email addresses
		/// If name is supplied it should be formatted in the same order as the email addresses
		/// </summary>
		/// <param name="address">The ReplyTo Address</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public Email ReplyTo(string address, string name = "")
        {
			AssignToAddresses(GetMailAddresses(address, name), AddressType.ReplyTo);
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
        /// Marks the email as High Priority
        /// </summary>
        /// <returns></returns>
        public Email HighPriority()
        {
            Message.Priority = MailPriority.High;
            return this;
        }

        /// <summary>
        /// Marks the email as Low Priority
        /// </summary>
        /// <returns></returns>
        public Email LowPriority()
        {
            Message.Priority = MailPriority.Low;
            return this;
        }

		/// <summary>
		/// Adds the template file to the email
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filename">The path to the file to load</param>
		/// <param name="model">The model.</param>
		/// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
		/// <param name="parserType">Type of the parser.</param>
		/// <returns>
		/// Instance of the Email class
		/// </returns>
		public Email UsingTemplateFromFile<T>(string filename, T model, bool isHtml = true, ParserType parserType = ParserType.Razor){
			AssignBody(parserType, filename, model, isHtml, true);
			return this;
		}

    	/// <summary>
		/// Adds razor template to the email
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="template">The template.</param>
		/// <param name="model">The model.</param>
		/// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
		/// <param name="parserType">Type of the parser.</param>
		/// <returns>
		/// Instance of the Email class
		/// </returns>
        public Email UsingTemplate<T>(string template, T model, bool isHtml = true, ParserType parserType = ParserType.Razor)
        {
			AssignBody(parserType, template, model, isHtml, false);
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
            foreach (var attachment in attachments.Where(attachment => !Message.Attachments.Contains(attachment)))
            {
                Message.Attachments.Add(attachment);
            }
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

		/// <summary>
		/// Uses the SSL transport layer.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Assigns the body to the message.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parserType">Type of the parser.</param>
		/// <param name="content">The content.</param>
		/// <param name="model">The model.</param>
		/// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
		/// <param name="fromFile">if set to <c>true</c> [from file].</param>
		private void AssignBody<T>(ParserType parserType, string content, T model, bool isHtml, bool fromFile) {
			var body = fromFile ? ParserFactory.CreateParser(parserType).ParseFromFile(content, model) 
					: ParserFactory.CreateParser(parserType).ParseFromString(content, model);

			Message.Body = body;
			Message.IsBodyHtml = isHtml;
		}

		/// <summary>
		/// Gets the mail addresses from the email address string. Supports semi colon and comma seperation
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private static IEnumerable<MailAddress> GetMailAddresses(string emailAddress, string name){

			const char semiColon = ';';
			const char comma = ',';
			var splitArray = new[]{semiColon, comma};

			IList<MailAddress> output = new List<MailAddress>();

			if (emailAddress.Contains(semiColon) || emailAddress.Contains(comma)) {
				var nameSplit = name.Split(splitArray);
				var addressSplit = emailAddress.Split(splitArray);
				for (int i = 0; i < addressSplit.Length; i++) {
					var currentName = string.Empty;
					if ((nameSplit.Length - 1) >= i) {
						currentName = nameSplit[i];
					}
					output.Add(new MailAddress(addressSplit[i].Trim(), currentName.Trim()));
				}
			} else {
				output.Add(new MailAddress(emailAddress, name));
			}

			return output;
		}

		/// <summary>
		/// Assigns to addresses to the message. 
		/// </summary>
		/// <param name="addressList">The address list.</param>
		/// <param name="type">The type.</param>
		private void AssignToAddresses(IEnumerable<MailAddress> addressList, AddressType type) {
			foreach (var address in addressList) {
				switch (type) {
					case AddressType.To:
						Message.To.Add(address);
						break;
					case AddressType.Cc:
						Message.CC.Add(address);
						break;
					case AddressType.Bcc:
						Message.Bcc.Add(address);
						break;
					case AddressType.ReplyTo:
						Message.ReplyToList.Add(address);
						break;
				}
			}
		}
    }
}
