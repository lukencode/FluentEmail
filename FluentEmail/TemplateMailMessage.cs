using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Collections.Specialized;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using System.IO;

namespace FluentEmail
{
    public class TemplateMailMessage : MailMessage
    {
        public Dictionary<string, string> Replacements { get; set; }
        /// <summary>
        /// Gets or sets the e-mail address of the message sender.
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Gets or sets the subject line of the e-mail message.
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Gets or sets the name of the file that contains text for the body of the e-mail message.
        /// </summary>
        public string BodyFileName { get; set; }

        public TemplateMailMessage()
            : base()
        {
            Replacements = new Dictionary<string, string>();
        }

        public void GenerateBody()
        {
            //Generate the Body Template
            string path = Path.GetFullPath(BodyFileName);

            TextReader reader = new StreamReader(path);
            try
            {
                Body = reader.ReadToEnd();
            }
            finally
            {
                reader.Close();
            }

            //Replace the stuff that need replacing
            foreach (string key in Replacements.Keys)
            {
                string replacement = Replacements[key];
                Body = Regex.Replace(Body, key, replacement, RegexOptions.IgnoreCase);
            }
        }


    }
}
