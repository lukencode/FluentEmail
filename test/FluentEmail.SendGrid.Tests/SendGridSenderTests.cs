using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using NUnit.Framework;

namespace FluentEmail.SendGrid.Tests
{
    public class SendGridSenderTests
    {
        const string apiKey = "";
        const string toEmail = "fluentEmail@mailinator.com";
        const string toName = "FluentEmail Mailinator";
        const string fromEmail = "test@fluentmail.com";
        const string fromName = "SendGridSender Test";

        [SetUp]
        public void SetUp()
        {
            var sender = new SendGridSender(apiKey, true);
            Email.DefaultSender = sender;
        }

        [Test]
        public async Task CanSendEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of SendGrid Sender.";

            var email = Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test]
        public async Task CanSendEmailWithAttachments()
        {
            const string subject = "SendMail With Attachments Test";
            const string body = "This email is testing the attachment functionality of SendGrid Sender.";

            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.WriteLine("Hey this is some text in an attachment");
            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment()
            {
                Data = stream,
                ContentType = "text/plain",
                Filename = "sendGridTest.txt"
            };

            var email = Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body)
                .Attach(attachment);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }
    }
}