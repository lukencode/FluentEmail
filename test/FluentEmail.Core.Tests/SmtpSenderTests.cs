using System.IO;
using System.Net.Mail;
using System.Threading;
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

        private static IFluentEmail TestEmail => Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

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
            var email = TestEmail
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

            var attachment = new Attachment
            {
                Data = stream,
                ContentType = "text/plain",
                Filename = "mailgunTest.txt"
            };

            var email = TestEmail
                .Attach(attachment);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
            var files = Directory.EnumerateFiles(tempDirectory, "*.eml");
            Assert.IsNotEmpty(files);
        }

        [Test]
        public async Task CanSendEmailWithInlineImageAndAttachments()
        {
            var stream = File.OpenRead(@"C:\Users\wilko.vanderveen\source\repos\FluentEmail\test\FluentEmail.Core.Tests\Attachments\fluentemail_logo_64x64.png");
                    
            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment
            {
                IsInline = true,
                Data = stream,
                ContentType = "image/png",
                Filename = "fluentemail-logo.png",
                ContentId = "MyVeryCoolContentId"
            };

            var email = TestEmail
                .Attach(attachment);

            email.Body("<html><head></head><body><img src=\"cid:MyVeryCoolContentId\" /></body></html", true);
      

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
            var files = Directory.EnumerateFiles(tempDirectory, "*.eml");
            Assert.IsNotEmpty(files);
        }

        [Test]
        public async Task CanSendEmailWithInlineImage()
        {
            var stream = File.OpenRead(@"C:\Users\wilko.vanderveen\source\repos\FluentEmail\test\FluentEmail.Core.Tests\Attachments\fluentemail_logo_64x64.png");

            stream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment
            {
                IsInline = true,
                Data = stream,
                ContentType = "image/png",              
                ContentId = "MyVeryCoolContentId"
            };

            var email = TestEmail
                .Attach(attachment);

            email.Body("<html><head></head><body><img src=\"cid:MyVeryCoolContentId\" /></body></html", true);


            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
            var files = Directory.EnumerateFiles(tempDirectory, "*.eml");
            Assert.IsNotEmpty(files);
        }

        [Test]
        public async Task CanSendAsyncHtmlAndPlaintextTogether()
        {
            var email = TestEmail
                .Body("<h2>Test</h2><p>some body text</p>", true)
                .PlaintextAlternativeBody("Test - Some body text");

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }

        [Test]
        public void CanSendHtmlAndPlaintextTogether()
        {
            var email = TestEmail
                .Body("<h2>Test</h2><p>some body text</p>", true)
                .PlaintextAlternativeBody("Test - Some body text");

            var response = email.Send();

            Assert.IsTrue(response.Successful);
        }

        [Test]
        public void CancelSendIfCancelationRequested()
        {
            var email = TestEmail;

            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();

            var response = email.Send(tokenSource.Token);

            Assert.IsFalse(response.Successful);
        }
    }
}
