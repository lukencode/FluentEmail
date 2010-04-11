using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentEmail;
using System.Net.Mail;

namespace FluentEmailTests
{
    [TestClass]
    public class FluentEmailTests
    {
        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";
        const string body = "what be the hipitity hap?";

        [TestMethod]
        public void To_Address_Is_Set()
        {
            var email = Email
                        .From(fromEmail)
                        .To(toEmail);

            Assert.AreEqual(toEmail, email.Message.To[0].Address);
        }

        [TestMethod]
        public void From_Address_Is_Set()
        {
            var email = Email.From(fromEmail);

            Assert.AreEqual(fromEmail, email.Message.From.Address);
        }

        [TestMethod]
        public void Subject_Is_Set()
        {
            var email = Email
                        .From(fromEmail)
                        .Subject(subject);

            Assert.AreEqual(subject, email.Message.Subject);
        }

        [TestMethod]
        public void Body_Is_Set()
        {
            var email = Email.From(fromEmail)
                                .Body(body);

            Assert.AreEqual(body, email.Message.Body);
        }

        [TestMethod]
        public void Can_Add_Multiple_Recipients()
        {
            string toEmail1 = "bob@test.com";
            string toEmail2 = "ratface@test.com";

            var email = Email
                        .From(fromEmail)
                        .To(toEmail1)
                        .To(toEmail2);

            Assert.AreEqual(2, email.Message.To.Count);
        }

        [TestMethod]
        public void Can_Add_Multiple_Recipients_From_List()
        {
            var emails = new List<MailAddress>();
            emails.Add(new MailAddress("email1@email.com"));
            emails.Add(new MailAddress("email2@email.com"));

            var email = Email
                        .From(fromEmail)
                        .To(emails);

            Assert.AreEqual(2, email.Message.To.Count);
        }

        public void Is_Valid_With_Properties_Set()
        {
            var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .Body(body);

            Assert.AreEqual(body, email.Message.Body);
            Assert.AreEqual(subject, email.Message.Subject);
            Assert.AreEqual(fromEmail, email.Message.From.Address);
            Assert.AreEqual(toEmail, email.Message.To[0].Address);
        }
    }
}
