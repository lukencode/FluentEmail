using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.Azure.Email.Tests
{
    [NonParallelizable]
    public class AzureEmailSenderTests
    {
        const string connectionString = ""; // TODO: Put your ConnectionString here

        const string toEmail = "fluentEmail@mailinator.com";
        const string toName = "FluentEmail Mailinator";
        const string fromEmail = "test@fluentmail.com"; // TODO: Put a valid/verified sender here
        const string fromName = "AzureEmailSender Test";

        [SetUp]
        public void SetUp()
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Azure Communication Services Connection String needs to be supplied");

            var sender = new AzureEmailSender(connectionString);
            Core.Email.DefaultSender = sender;
        }

        [Test, Ignore("No azure credentials")]
        public async Task CanSendEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of Azure Email Sender.";

            var email = Core.Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }
        
        [Test, Ignore("No azure credentials")]
        public async Task CanSendEmailWithReplyTo()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail with ReplyTo functionality of Azure Email Sender.";

            var email = Core.Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .ReplyTo(toEmail, toName)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test, Ignore("No azure credentials")]
        public async Task CanSendEmailWithAttachments()
        {
            const string subject = "SendMail With Attachments Test";
            const string body = "This email is testing the attachment functionality of Azure Email Sender.";

            await using var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/test-binary.xlsx");
            var attachment = new Attachment
            {
                Data = stream,
                ContentType = "xlsx",
                Filename = "test-binary.xlsx"
            };

            var email = Core.Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body)
                .Attach(attachment);


            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test, Ignore("No azure credentials")]
        public async Task CanSendHighPriorityEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of Azure Email Sender.";

            var email = Core.Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body)
                .HighPriority();

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test, Ignore("No azure credentials")]
        public async Task CanSendLowPriorityEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of Azure Email Sender.";

            var email = Core.Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body)
                .LowPriority();

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }
    }
}
