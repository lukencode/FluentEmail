using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentEmail.Core;
using NUnit.Framework;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.Smtp.Tests
{
    [NonParallelizable]
    public class SmtpSenderTests
    {
        // Warning: To pass, an smtp listener must be running on localhost:25.

        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";
        const string body = "what be the hipitity hap?";

        private readonly string tempDirectory;

        public SmtpSenderTests()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "EmailTest");
        }

        [SetUp]
        public void SetUp()
        {
            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = tempDirectory
            });

            Email.DefaultSender = sender;
            Directory.CreateDirectory(tempDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(tempDirectory, true);
        }

        [Test]
        public void CanSendEmail()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Body("<h2>Test</h2>", true);

            var response = email.Send();

            var files = Directory.EnumerateFiles(tempDirectory, "*.eml");
            Assert.IsTrue(response.Successful);
            Assert.IsNotEmpty(files);
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

            var files = Directory.EnumerateFiles(tempDirectory, "*.eml");
            Assert.IsTrue(response.Successful);
            Assert.IsNotEmpty(files);
        }

        [Test]
        public async Task CanSendAsyncHtmlAndPlaintextTogether()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Body("<h2>Test</h2><p>some body text</p>", true)
                .PlaintextAlternativeBody("Test - Some body text");

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test]
        public void CanSendHtmlAndPlaintextTogether()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Body("<h2>Test</h2><p>some body text</p>", true)
                .PlaintextAlternativeBody("Test - Some body text");

            var response = email.Send();

            Assert.IsTrue(response.Successful);
        }
    }
}
