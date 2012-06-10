﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using RazorEngine;
using RazorEngine.Templating;
using System.Dynamic;

namespace FluentEmail
{
    public class Email : IHideObjectMembers, IDisposable
    {
        private SmtpClient _client;
        private bool? _useSsl;

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
        /// Adds a reciepient to the email, Splits name and address on ';'
        /// </summary>
        /// <param name="emailAddress">Email address of recipeient</param>
        /// <param name="name">Name of recipient</param>
        /// <returns>Instance of the Email class</returns>
        public Email To(string emailAddress, string name)
        {
            if (emailAddress.Contains(";"))
            {
                //email address has semi-colon, try split
                var nameSplit = name.Split(';');
                var addressSplit = emailAddress.Split(';');
                for (int i = 0; i < addressSplit.Length; i++)
                {
                    var currentName = string.Empty;
                    if ((nameSplit.Length - 1) >= i)
                    {
                        currentName = nameSplit[i];
                    }
                    Message.To.Add(new MailAddress(addressSplit[i], currentName));
                }
            }
            else
            {
                Message.To.Add(new MailAddress(emailAddress, name));
            }
            return this;
        }

        /// <summary>
        /// Adds a reciepient to the email
        /// </summary>
        /// <param name="emailAddress">Email address of recipeient (allows multiple splitting on ';')</param>
        /// <returns></returns>
        public Email To(string emailAddress)
        {
            if (emailAddress.Contains(";"))
            {
                foreach (string address in emailAddress.Split(';'))
                {
                    Message.To.Add(new MailAddress(address));
                }
            }
            else
            {
                Message.To.Add(new MailAddress(emailAddress));
            }

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
        /// Adds all Carbon Copy in list to an email
        /// </summary>
        /// <param name="mailAddresses">List of recipients to CC</param>
        /// <returns>Instance of the Email class</returns>
        public Email CC(IList<MailAddress> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Message.CC.Add(address);
            }
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
        /// Adds all blind carbon copy in list to an email
        /// </summary>
        /// <param name="mailAddresses">List of recipients to BCC</param>
        /// <returns>Instance of the Email class</returns>
        public Email BCC(IList<MailAddress> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Message.Bcc.Add(address);
            }
            return this;
        }

        /// <summary>
        /// Sets the ReplyTo address on the email
        /// </summary>
        /// <param name="address">The ReplyTo Address</param>
        /// <returns></returns>
        public Email ReplyTo(string address)
        {
            Message.ReplyToList.Add(new MailAddress(address));

            return this;
        }

        /// <summary>
        /// Sets the ReplyTo address on the email
        /// </summary>
        /// <param name="address">The ReplyTo Address</param>
        /// <param name="name">The Display Name of the ReplyTo</param>
        /// <returns></returns>
        public Email ReplyTo(string address, string name)
        {
            Message.ReplyToList.Add(new MailAddress(address, name));

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
        /// <param name="filename">The path to the file to load</param>
        /// <param name="model">Model to pass to template</param>
        /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns>Instance of the Email class</returns>
        public Email UsingRazorTemplateFromFile<T>(string filename, T model, bool isHtml = true)
        {
            if (filename.StartsWith("~"))
            {
                var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                filename = Path.GetFullPath(baseDir + filename.Replace("~", ""));
            }

            //Generate the Template
            var path = Path.GetFullPath(filename);
            var template = "";

            TextReader reader = new StreamReader(path);
            try
            {
                template = reader.ReadToEnd();
            }
            finally
            {
                reader.Close();
            }

            //bind template
            initializeRazorParser();

            var result = Razor.Parse<T>(template, model);
            Message.Body = result;
            Message.IsBodyHtml = isHtml;

            return this;
        }

        /// <summary>
        /// Adds razor template to the email
        /// </summary>
        /// <param name="template">The path to the file to load</param>
        /// <param name="model">The view model to pass to the template </param>
        /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns>Instance of the Email class</returns>
        public Email UsingRazorTemplate<T>(string template, T model, bool isHtml = true)
        {
            //HACK YO
            initializeRazorParser();

            var result = Razor.Parse<T>(template, model);
            Message.Body = result;
            Message.IsBodyHtml = isHtml;

            return this;
        }

        /// <summary>
        /// Adds Razor template to the email
        /// </summary>
        /// <param name="template">The path to the file to load</param>
        /// <param name="model">The view model to pass to the template </param>
        /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns>Instance of the Email class</returns>
        [Obsolete("This method is depreciated. Use the UsingRazorTemplate() method instead")]
        public Email UsingTemplate<T>(string template, T model, bool isHtml = true)
        {
            return UsingRazorTemplate(template, model, isHtml);
        }

        /// <summary>
        /// Adds Razor template file to the email
        /// </summary>
        /// <param name="filename">The path to the file to load</param>
        /// <param name="model">Model to pass to template</param>
        /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns>Instance of the Email class</returns>
        [Obsolete("This method is depreciated. Use the UsingRazorTemplateFromFile() method instead")]
        public Email UsingTemplateFromFile<T>(string filename, T model, bool isHtml = true)
        {
            return UsingRazorTemplateFromFile(filename, model, isHtml);
        }

        /// <summary>
        /// Adds Markdown template file to the email
        /// </summary>
        /// <param name="filename">The path to the file to load</param>
        /// <param name="model">Model to pass to template</param>
        /// <param name="isHtml">True if Body is HTML, false for plain text (Optional)</param>
        /// <returns>Instance of the Email class</returns>
        public Email UsingMarkdownTemplateFromFile<TModel>(string filename, TModel model, bool isHtml = true)
        {
            if (filename.StartsWith("~"))
            {
                var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                filename = Path.GetFullPath(baseDir + filename.Replace("~", ""));
            }

            var template = File.ReadAllText(filename);

            var result = MarkdownParser.Parse<TModel>(template, model, isHtml);
            Message.Body = result;
            Message.IsBodyHtml = isHtml;

            return this;
        }

        /// <summary>
        /// Adds Markdown template to the email
        /// </summary>
        /// <param name="template">Template markdown string</param>
        /// <param name="model">The view model to pass to the template</param>
        /// <param name="isHtml">True is Boyd is Html, false for plain text (Optional)</param>
        /// <returns></returns>
        public Email UsingMarkdownTempate<TModel>(string template, TModel model, bool isHtml = true)
        {
            var result = MarkdownParser.Parse<TModel>(template, model, isHtml);
            Message.Body = result;
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
            if (_useSsl.HasValue)
                _client.EnableSsl = _useSsl.Value;

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
            if (_useSsl.HasValue)
                _client.EnableSsl = _useSsl.Value;

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

        private void initializeRazorParser()
        {
            // HACK: this is required to get the Razor Parser to work, no idea why, something to with dynamic objects i guess, tracked this down as the test worked sometimes, turned out
            // it was when the ViewBag was touched from the controller tests, if that happened before the Razor.Parse in ShoudSpikeTheSillyError() then it ran fine.
            dynamic x2 = new ExpandoObject();
            x2.Dummy = "";
        }

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();

            if (Message != null)
                Message.Dispose();
        }
    }
}
