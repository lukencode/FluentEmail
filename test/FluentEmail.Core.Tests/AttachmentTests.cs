using System.IO;
using System.Linq;
using System.Reflection;
using FluentEmail.Core.Models;
using NUnit.Framework;

namespace FluentEmail.Core.Tests
{
    [TestFixture]
    public class AttachmentTests
    {
        private Assembly ThisAssembly() => this.GetType().GetTypeInfo().Assembly;
        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";

        [Test]
        public void Attachment_from_stream_Is_set()
        {
            using (var stream = File.OpenRead($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}"))
            {
                var attachment = new Attachment()
                {
                    Data = stream,
                    Filename = "Test.txt",
                    ContentType = "text/plain"
                };

                var email = Email.From(fromEmail)
                    .To(toEmail)
                    .Subject(subject)
                    .Attach(attachment);

                Assert.AreEqual(20, email.Data.Attachments.First().Data.Length);
            }
        }

        [Test]
        public void Attachment_from_filename_Is_set()
        {
            var email = Email.From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .AttachFromFilename($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", "text/plain");

            Assert.AreEqual(20, email.Data.Attachments.First().Data.Length);
        }

        [Test]
        public void Attachment_from_filename_AttachmentName_Is_set()
        {
            var attachmentName = "attachment.txt";
            var email = Email.From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .AttachFromFilename($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", "text/plain", attachmentName);

            Assert.AreEqual(20, email.Data.Attachments.First().Data.Length);
            Assert.AreEqual(attachmentName, email.Data.Attachments.First().Filename);
        }
    }
}
