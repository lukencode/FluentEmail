using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentEmail.Core;
using NUnit.Framework;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.Smtp.Tests
{
    public class SmtpSenderTests
    {
        // Warning: To pass, an smtp listener must be running on localhost:25.

        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";
        const string body = "what be the hipitity hap?";

        [SetUp]
        public void SetUp()
        {
            var sender = new SmtpSender(new SmtpClient("localhost"))
            {
                UseSsl = false
            };
            Email.DefaultSender = sender;
        }

        [Test]
        public void CanSendEmail()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Body("Test");

            var response = email.Send();

            Assert.IsTrue(response.Successful);
        }

        [Test]
        public async Task CanSendEmailWithAttachments()
        {
            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.WriteLine("Hey this is some text in an attachment");
            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment()
            {
                Data = stream,
                ContentType = "text/plain",
                Filename = "mailgunTest.txt"
            };

            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .Attach(attachment);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }
    }
}
