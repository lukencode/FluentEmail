using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentEmail;

namespace FluentEmailTests
{
    [TestClass]
    public class FluentEmailTests
    {
        [TestMethod]
        public void To_Address_Is_Set()
        {
            string toEmail = "bob@test.com";

            var email = Email.New()
                .To(toEmail);

            Assert.AreEqual(toEmail, email.Message.To[0].Address);
        }

        [TestMethod]
        public void From_Address_Is_Set()
        {
            string fromEmail = "johno@test.com";

            var email = Email.New()
                .From(fromEmail);

            Assert.AreEqual(fromEmail, email.Message.From.Address);
        }

        [TestMethod]
        public void Subject_Is_Set()
        {
            string subject = "sup dawg";

            var email = Email.New()
                .Subject(subject);

            Assert.AreEqual(subject, email.Message.Subject);
        }

        [TestMethod]
        public void Body_Is_Set()
        {
            string body = "what be the hipitity hap?";

            var email = Email.New()
                .Body(body);

            Assert.AreEqual(body, email.Message.Body);
        }

        [TestMethod]
        public void Can_Add_Multiple_Recipients()
        {
            string toEmail1 = "bob@test.com";
            string toEmail2 = "ratface@test.com";

            var email = Email.New()
                .To(toEmail1)
                .To(toEmail2);

            Assert.AreEqual(2, email.Message.To.Count);
        }

        public void Is_Valid_With_Properties_Set()
        {        
            string toEmail = "bob@test.com";   
            string fromEmail = "johno@test.com"; 
            string subject = "sup dawg";
            string body = "what be the hipitity hap?";

            var email = Email.New()
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .From(fromEmail);

            Assert.AreEqual(body, email.Message.Body);
            Assert.AreEqual(subject, email.Message.Subject);
            Assert.AreEqual(fromEmail, email.Message.From.Address);
            Assert.AreEqual(toEmail, email.Message.To[0].Address);
        }
    }
}
