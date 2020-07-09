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
            using (var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}/Test.txt"))
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
                .AttachFromFilename($"{Directory.GetCurrentDirectory()}/Test.txt", "text/plain");

            Assert.AreEqual(20, email.Data.Attachments.First().Data.Length);
        }

        [Test]
        public void Attachment_from_filename_AttachmentName_Is_set()
        {

            // THis is the first failing tests - the test is unable to locat the text.txt file.
            System.Console.WriteLine($" MMH : {Directory.GetCurrentDirectory()}" );
            System.Console.WriteLine($" MH : { Directory.GetCurrentDirectory()}/Test.txt");
            var attachmentExists = File.Exists($"{Directory.GetCurrentDirectory()}/Test.txt");
            System.Console.WriteLine($" MH : File exists : {attachmentExists}");
            System.Console.WriteLine($" MH : { Directory.GetCurrentDirectory()}/Test.txt");
            System.Console.WriteLine($" MH : File exists : {File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Test.txt"))}");
            System.Console.WriteLine($" MH : File exists : {File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "test.txt"))}");



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
