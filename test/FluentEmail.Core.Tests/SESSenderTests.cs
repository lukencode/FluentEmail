using FluentEmail.Core;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace FluentEmail.SES.Tests
{
    public class SESSenderTests
    {
        // TODO: Put your SES details here.
        const string accessKeyId = "";
        const string regionEndpoint = "";
        const string secretAccessKey = "";

        const string toEmail = "fluentEmail@mailinator.com";
        const string toName = "FluentEmail Mailinator";
        const string fromEmail = "test@fluentmail.com";
        const string fromName = "SESSender Test";

        [SetUp]
        public void SetUp()
        {
            var sesClientOptions = Options.Create(new FluenEmailSESOptions()
            {
                AccessKeyId = accessKeyId,
                RegionEndpoint = regionEndpoint,
                SecretAccessKey = secretAccessKey
            });

            var sender = new SESSender(sesClientOptions);
            Email.DefaultSender = sender;
        }

        [Test]
        [Ignore("No SES credentials")]
        public async Task CanSendEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of SES Sender.";

            var email = Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }


        [Test]
        [Ignore("No SES credentials")]
        public async Task CanSendEmailWithAttachments()
        {
            const string subject = "SendMail With Attachments Test";
            const string body = "This email is testing the attachment functionality of SES Sender.";

            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.WriteLine("Hey this is some text in an attachment");
            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Core.Models.Attachment
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

        [Test]
        [Ignore("No SES credentials")]
        public async Task CanSendHighPriorityEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of SES Sender.";

            var email = Email
                .From(fromEmail, fromName)
                .To(toEmail, toName)
                .Subject(subject)
                .Body(body)
                .HighPriority();

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test]
        [Ignore("No SES credentials")]
        public async Task CanSendLowPriorityEmail()
        {
            const string subject = "SendMail Test";
            const string body = "This email is testing send mail functionality of SES Sender.";

            var email = Email
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
