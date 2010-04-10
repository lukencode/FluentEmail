using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentEmail;
using System.ComponentModel;
using System.Net.Mail;
using System.Threading;

namespace FluentEmailTests
{
    [TestClass]
    public class AsyncTests
    {
        static bool callbackCalled = false;
        static private ManualResetEvent updatedEvent = new ManualResetEvent(false);

        [TestMethod]
        public void Callback_Method_Is_Called_On_Cancel()
        {
            string toEmail = "bob@test.com";
            string fromEmail = "johno@test.com";
            string subject = "sup dawg";
            string body = "what be the hipitity hap?";

            var email = Email.New(new SmtpClient("localhost", 25))
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .From(fromEmail)
                .SendAsync(MailDeliveryComplete);

            email.Cancel();

            updatedEvent.WaitOne();

            Assert.IsTrue(callbackCalled);
        }

        private static void MailDeliveryComplete(object sender, AsyncCompletedEventArgs e)
        {
            callbackCalled = true;
            updatedEvent.Set();
        }
    }
}
