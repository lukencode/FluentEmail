using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.MailKitSmtp;
using MimeKit;
using NUnit.Framework;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace FluentEmail.MailKit.Tests
{
    [NonParallelizable]
    public class MailKitSmtpSenderTests
    {
        // Warning: To pass, an smtp listener must be running on localhost:25.

        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";
        const string body = "what be the hipitity hap?";

        private readonly string tempDirectory;

        public MailKitSmtpSenderTests()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "EmailTest");
        }

        [SetUp]
        public void SetUp()
        {
            var sender = new MailKitSender(new SmtpClientOptions
            { 
                 Server = "localhost",
                 Port = 25,
                 UseSsl = false,
                 RequiresAuthentication = false,
                 UsePickupDirectory = true,
                 MailPickupDirectory = Path.Combine(Path.GetTempPath(), "EmailTest")
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

            var attachment = new Attachment
            {
                Data = stream,
                ContentType = "text/plain",
                Filename = "MailKitAttachment.txt"
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

        [Test]
        public void CanPreprocessMessageByOverridingCreateMailMessage()
        {
            var sender = new PreprocessingMailKitSender(new SmtpClientOptions
            {
                Server = "localhost",
                Port = 25,
                UseSsl = false,
                RequiresAuthentication = false,
                UsePickupDirectory = true,
                MailPickupDirectory = tempDirectory
            });
            Email.DefaultSender = sender;

            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

            var response = email.Send();

            Assert.IsTrue(response.Successful);
            var eml = Directory.EnumerateFiles(tempDirectory, "*.eml").First();
            var content = File.ReadAllText(eml);
            Assert.IsTrue(content.Contains("X-Preprocessed: yes"),
                "Override of CreateMailMessage should be able to modify the message before sending.");
        }

        /// <summary>
        /// Simulates e.g. DKIM-signing: preprocess the MimeMessage after creation.
        /// Possible since CreateMailMessage is protected virtual (issue #388).
        /// </summary>
        private class PreprocessingMailKitSender : MailKitSender
        {
            public PreprocessingMailKitSender(SmtpClientOptions options) : base(options) { }

            protected override MimeMessage CreateMailMessage(IFluentEmail email)
            {
                var message = base.CreateMailMessage(email);
                message.Headers.Add("X-Preprocessed", "yes");
                return message;
            }
        }
    }
}
