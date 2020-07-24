﻿using FluentEmail.Core;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.SendGrid.Tests
{
    public class SendGridSenderTests
    {
        const string apiKey = "missing-credentials"; // TODO: Put your API key here

        const string toEmail = "fluentEmail@mailinator.com";
        const string toName = "FluentEmail Mailinator";
        const string fromEmail = "test@fluentmail.com";
        const string fromName = "SendGridSender Test";

        private const string templateId = ""; // TODO: PUT YOUR TEMPLATE ID HERE

        [SetUp]
        public void SetUp()
        {
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentException("SendGrid Api Key needs to be supplied");

            var sender = new SendGridSender(apiKey, true);
            Email.DefaultSender = sender;
        }

        [Test, Ignore("No sendgrid credentials")]
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

        [Test, Ignore("No sendgrid credentials")]
        public async Task CanSendEmailWithAttachments()
        {
            const string subject = "SendMail With Attachments Test";
            const string body = "This email is testing the attachment functionality of SendGrid Sender.";

            using (var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/test-binary.xlsx"))
            {
                var attachment = new Attachment()
                {
                    Data = stream,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    Filename = "test-binary.xlsx"
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

        [Test, Ignore("No sendgrid credentials")]
        public async Task CanSendHighPriorityEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of SendGrid Sender.";

            var email = Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body)
                .HighPriority();

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test, Ignore("No sendgrid credentials")]
        public async Task CanSendLowPriorityEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of SendGrid Sender.";

            var email = Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body)
                .LowPriority();

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test]
        public async Task CanSendEmailWithTemplate()
        {
            var email = Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .SetTemplateData(templateId, new
                {
                    //put your template data here
                })
                .LowPriority();

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }
    }
}