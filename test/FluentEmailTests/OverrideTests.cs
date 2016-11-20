using System.ComponentModel;
using System.Net.Mail;
using System.Threading;
using FluentEmail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentEmailTests
{
    public class MockEmail : Email
    {
        public override IFluentEmail Send()
        {
            Message.Body = "The Send() method has been overridden";
            return this;
        }

        public override IFluentEmail SendAsync(SendCompletedEventHandler callback, object token = null)
        {
            Message.Body = "The SendAsync() method has been overridden";
            return this;
        }
    }

    [TestClass]
    public class OverrideTests
    {
        private IFluentEmail email;

        static private ManualResetEvent updatedEvent = new ManualResetEvent(false);

        const string toEmail = "to@test.com";
        const string subject = "this is the subject";
        const string body = "This is the body";

        [TestInitialize]
        public void Initialize()
        {
            email = new MockEmail();
            email.To(toEmail).Subject(subject).Body(body);
        }

        [TestMethod]
        public void Send_Method_Can_Be_Overridden()
        {
            Assert.AreEqual(email.Message.Body, body);
            Assert.AreEqual(email.Message.Subject, subject);

            email.Send();

            Assert.AreEqual(email.Message.Body, "The Send() method has been overridden");
        }

        [TestMethod]
        public void SendAsync_Method_Can_Be_Overridden()
        {
            Assert.AreEqual(email.Message.Body, body);
            Assert.AreEqual(email.Message.Subject, subject);

            email.SendAsync(MailDeliveryComplete);
            email.Cancel();

            Assert.AreEqual(email.Message.Body, "The SendAsync() method has been overridden");
        }

        private static void MailDeliveryComplete(object sender, AsyncCompletedEventArgs e)
        {
            updatedEvent.Set();
        }
    }
}