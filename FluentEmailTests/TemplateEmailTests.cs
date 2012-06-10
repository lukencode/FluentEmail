using System;
using System.IO;
using FluentEmail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentEmailTests
{
    [TestClass]
    [DeploymentItem(@"FluentEmailTests\\test.txt")]
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
        public void Set_Custom_Template()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

            var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .UsingTemplateEngine(new TestTemplate())
                        .UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

            Assert.AreEqual("custom template", email.Message.Body);
        }
    }

    public class TestTemplate : ITemplateRenderer
    {
        public string Parse<T>(string template, T model)
        {
            return "custom template";
        }
    }

}
