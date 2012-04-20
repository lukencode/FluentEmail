using System;
using System.IO;
using System.Text;
using FluentEmail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentEmailTests
{
    [TestClass]
    [DeploymentItem(@"FluentEmailTests\\test.txt")]
    [DeploymentItem(@"FluentEmailTests\\test.md")]
    public class TemplateEmailTests
    {
        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";

        [TestMethod]
        public void Anonymous_Model_Template_From_File_Matches()
        {
            var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .UsingTemplateFromFile(@"~/Test.txt", new { Test = "FLUENTEMAIL" });

            Assert.AreEqual("yo email FLUENTEMAIL", email.Message.Body);
        }

        [TestMethod]
        public void Anonymous_Model_Template_Matches()
        {
            string template = "sup @Model.Name";

            var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .UsingTemplate(template, new { Name = "LUKE" });

            Assert.AreEqual("sup LUKE", email.Message.Body);
        }

        [TestMethod]
        public void Anonymous_Model_With_List_Template_Matches()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

            var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

            Assert.AreEqual("sup LUKE here is a list 123", email.Message.Body);
        }

        [TestMethod]
        public void Anonymous_Model_Markdown_Template_Matches()
        {
            var template = "# sup @Model.Name";

            var email = Email.From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .UsingMarkdownTempate(template, new { Name = "LUKE" });

            Assert.AreEqual("<h1>sup LUKE</h1>\n", email.Message.Body);
        }

        [TestMethod]
        public void Anonymous_Model_Markdown_Template_Plain_Matches()
        {
            var template = "# sup @Model.Name";

            var email = Email.From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .UsingMarkdownTempate(template, new { Name = "LUKE" }, false);

            Assert.AreEqual("# sup LUKE", email.Message.Body);
        }

        [TestMethod]
        public void Anonymous_Model_With_List_Markdown_Template_Matches()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Hello @Model.Name");
            sb.AppendLine();
            sb.AppendLine("@foreach n in Model.Numbers) {");
            sb.AppendLine("- @n");
            sb.AppendLine("}");
            sb.AppendLine();

            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .UsingMarkdownTempate(sb.ToString(), new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

            Console.Write(email.Message.Body);

            Assert.IsTrue(email.Message.Body.Length > 0);
            Assert.IsTrue(email.Message.Body.Contains("<li>3</li>"));
        }

        [TestMethod]
        public void Anonymous_Model_With_List_Markdown_Template_From_File_Matches()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .UsingMarkdownTemplateFromFile(@"~/test.md", new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

            Console.Write(email.Message.Body);

            Assert.IsTrue(email.Message.Body.Length > 0);
            Assert.IsTrue(email.Message.Body.Contains("<h1>"));
            Assert.IsTrue(email.Message.Body.Contains("<li>Number: 3</li>"));
        }
    }
}
