using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.IO;

namespace FluentEmail
{
    public class FluentMailMessage : MailMessage
    {
        public Dictionary<string, string> Replacements { get; set; }
        /// <summary>
        /// Gets or sets the name of the file that contains text for the body of the e-mail message.
        /// </summary>
        public string BodyFileName { get; set; }

        public FluentMailMessage()
        {
            Replacements = new Dictionary<string, string>();
        }

        public void GenerateBody()
        {
            //If a file is set load it as the Body
            if (!String.IsNullOrEmpty(BodyFileName) && String.IsNullOrEmpty(Body))
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
