using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using NUnit.Framework;

namespace FluentEmail.Mailtrap.Tests
{
    public class MailtrapSenderTests
    {
        const string toEmail = "testto.fluentemail@mailinator.com";
        const string fromEmail = "testfrom.fluentemail@mailinator.com";
        const string subject = "Mailtrap Email Test";
        const string body = "This email is testing the functionality of mailtrap.";
        const string username = ""; // Mailtrap SMTP inbox username
        const string password = ""; // Mailtrap SMTP inbox password

        [SetUp]
        public void SetUp()
        {
            var sender = new MailtrapSender(username, password);
            Email.DefaultSender = sender;
        }

        [Test, Ignore("No mailtrap credentials")]
        public void CanSendEmail()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

            var response = email.Send();

            Assert.IsTrue(response.Successful);
        }


        [Test, Ignore("No mailtrap credentials")]
        public async Task CanSendEmailAsync()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test, Ignore("No mailtrap credentials")]
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
                Filename = "mailtrapTest.txt"
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

        [Test, Ignore("No mailtrap credentials")]
        public async Task CanSendEmailWithInlineImages()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".logotest.png");
            stream.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment()
            {
                IsInline = true,
                Data = stream,
                ContentType = "image/png",
                Filename = "logotest.png"
            };

            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body("<html>Inline image here: <img src=\"cid:logotest.png\">" +
                "<p>You should see an image without an attachment, or without a download prompt, depending on the email client.</p></html>", true)
                .Attach(attachment);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }
    }
}
