using System;
using FluentEmail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentEmailTests
{
    [TestClass]
    public class CachedTemplateTests
    {
        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";
        const string body = "what be the hipitity hap?";

        [TestMethod]
        public void Reuse_Template()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";
            string template2 = "sup @Model.Name this is the second template";

            for (var i = 0; i < 10; i++)
            {
                var email = Email
                            .From(fromEmail)
                            .To(toEmail)
                            .Subject(subject)
                            .UsingTemplate(template, new { Name = i, Numbers = new string[] { "1", "2", "3" } });

                Assert.AreEqual("sup " + i + " here is a list 123", email.Message.Body);

                var email2 = Email
                            .From(fromEmail)
                            .To(toEmail)
                            .Subject(subject)
                            .UsingTemplate(template2, new { Name = i });

                Assert.AreEqual("sup " + i + " this is the second template", email2.Message.Body);
            }
        }
    }
}
