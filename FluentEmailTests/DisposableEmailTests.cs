using System;
using System.Net.Mail;
using NUnit.Framework;
using FluentEmail;

namespace FluentEmailTests
{
    [TestFixture]
    public class DisposableEmailTests
    {
        const string FromEmail = "johno@test.com";

        private class FakeEmailMessage : MailMessage
        {
            public bool DisposeHasBeenCalled { get; private set; }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                DisposeHasBeenCalled = true;
            }
        }

        private class FakeSmtpClient : SmtpClient
        {
            public FakeSmtpClient() : base("localhost", 25)
            {
            }

            public bool DisposeHasBeenCalled { get; private set; }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                DisposeHasBeenCalled = true;
            }
        }
        
        [Test]
        public void Email_Implements_IDisposable()
        {
        	var email = new Email().From("johno@test.com");
        	
            Assert.IsInstanceOf<IDisposable>(email);

            //Assert.AreEqual(replyEmail, email.Message.ReplyToList.First().Address);
   
        }

        [Test]
        public void Disposes_Of_The_Message()
        {
      
        	var email = new Email(new FakeSmtpClient()).From(FromEmail);
//            var email = Email.From(FromEmail).UsingClient(new FakeSmtpClient());

            var mailMessage = new FakeEmailMessage();
           
            email.Message = mailMessage;

            email.Dispose();

            Assert.IsTrue(mailMessage.DisposeHasBeenCalled);
        }

        [Test]
        public void Disposes_Of_The_SmtpClient()
        {
        	var smtpClient = new FakeSmtpClient();
        	var email = new Email(smtpClient).From(FromEmail);

//            email.UsingClient(smtpClient);

            email.Dispose();

            Assert.IsTrue(smtpClient.DisposeHasBeenCalled);
        }

        [Test]
        public void Does_Not_Cause_An_Exception_If_Message_Is_Null()
        {
        	var email = new Email(new FakeSmtpClient()).From(FromEmail);

            email.Message = null;

            email.Dispose();
        }

        [Test]
        public void Does_Not_Cause_An_Exception_If_SmtpClient_Is_Null()
        {
        	var email = new Email(null).From(FromEmail);

//            email.UsingClient(null);

            email.Dispose();
        }
    }
}
