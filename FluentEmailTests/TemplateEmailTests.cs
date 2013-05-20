﻿using System;
using System.Globalization;
using System.IO;
using System.Threading;
using FluentEmail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentEmailTests
{
    [TestClass]
    [DeploymentItem(@"FluentEmailTests\\test.txt")]
    [DeploymentItem(@"FluentEmailTests\\test.he-IL.txt")]
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
        public void Using_Template_From_Not_Existing_Culture_File_Using_Defualt_Template()
        {
            var culture = new CultureInfo("fr-FR");
            var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .UsingCultureTemplateFromFile(@"~/Test.txt", new { Test = "FLUENTEMAIL", culture });

            Assert.AreEqual("yo email FLUENTEMAIL", email.Message.Body);
        }

        [TestMethod]
        public void Using_Template_From_Culture_File()
        {
            var culture = new CultureInfo("he-IL");
            var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .UsingCultureTemplateFromFile(@"~/Test.txt", new { Test = "FLUENTEMAIL" }, culture);

            Assert.AreEqual("hebrew email FLUENTEMAIL", email.Message.Body);
        }

        [TestMethod]
        public void Using_Template_From_Current_Culture_File()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("he-IL");
            try
            {
                var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .UsingCultureTemplateFromFile(@"~/Test.txt", new { Test = "FLUENTEMAIL" });

                Assert.AreEqual("hebrew email FLUENTEMAIL", email.Message.Body);
            }
            finally
            {

                Thread.CurrentThread.CurrentUICulture = currentCulture;
            }
            
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
        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            return "custom template";
        }
    }

}
