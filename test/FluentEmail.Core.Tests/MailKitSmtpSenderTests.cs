﻿using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.MailKitSmtp;
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
        [TestCase]
        [TestCase("logotest.png")]
        public async Task CanSendEmailWithInlineImages(string contentId = null)
        {
            using (var stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}"))
            {
                var attachment = new Attachment
                {
                    IsInline = true,
                    Data = stream,
                    ContentType = "image/png",
                    Filename = "logotest.png",
                    ContentId = contentId
                };

                var email = Email
                    .From(fromEmail)
                    .To(toEmail)
                    .Subject(subject)
                    .Body("<html>Inline image here: <img src=\"cid:logotest.png\">" +
                          "<p>You should see an image without an attachment, or without a download prompt, depending on the email client.</p></html>", true)
                    .Attach(attachment);

                var response = await email.SendAsync();

                var files = Directory.EnumerateFiles(tempDirectory, "*.eml");
                Assert.IsTrue(response.Successful);
                Assert.IsNotEmpty(files);
            }
        }

        [Test]
        public async Task CanSendEmailWithInlineImagesAndAttachmentTogether()
        {
            var attachmentStream = new MemoryStream();
            var sw = new StreamWriter(attachmentStream);
            sw.WriteLine("Hey this is some text in an attachment");
            sw.Flush();
            attachmentStream.Seek(0, SeekOrigin.Begin);

            var attachment = new Attachment
            {
                Data = attachmentStream,
                ContentType = "text/plain",
                Filename = "MailKitAttachment.txt",
            };

            using var inlineStream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "logotest.png")}");

            var attachmentInline = new Attachment
            {
                IsInline = true,
                Data = inlineStream,
                ContentType = "image/png",
                Filename = "logotest.png",
            };

            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body("<html>Inline image here: <img src=\"cid:logotest.png\">" +
                      "<p>You should see an image inline without a picture attachment.</p>" +
                      "<p>A single .txt file should also be attached.</p></html>", true)
                .Attach(attachment)
                .Attach(attachmentInline);

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
