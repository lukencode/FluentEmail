using System;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentEmail;

namespace FluentEmailTests
{
	[TestClass]
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
		
		[TestMethod]
		public void Email_Implements_IDisposable()
		{
			var email = Email.From("johno@test.com");

			Assert.IsInstanceOfType(email, typeof(IDisposable));

			//Assert.AreEqual(replyEmail, email.Message.ReplyToList.First().Address);
		}

		[TestMethod]
		public void Disposes_Of_The_Message()
		{
			var email = Email.From(FromEmail).UsingClient(new FakeSmtpClient());

			var mailMessage = new FakeEmailMessage();

			email.Message = mailMessage;

			email.Dispose();

			Assert.IsTrue(mailMessage.DisposeHasBeenCalled);
		}

		[TestMethod]
		public void Disposes_Of_The_SmtpClient()
		{
			var email = Email.From(FromEmail);

			var smtpClient = new FakeSmtpClient();

			email.UsingClient(smtpClient);

			email.Dispose();

			Assert.IsTrue(smtpClient.DisposeHasBeenCalled);
		}

		[TestMethod]
		public void Does_Not_Cause_An_Exception_If_Message_Is_Null()
		{
			var email = Email.From(FromEmail).UsingClient(new FakeSmtpClient());

			email.Message = null;

			email.Dispose();
		}

		[TestMethod]
		public void Does_Not_Cause_An_Exception_If_SmtpClient_Is_Null()
		{
			var email = Email.From(FromEmail);

			email.UsingClient(null);

			email.Dispose();
		}
		
		#region Refactored tests using setup through constructors.
		[TestMethod]
		public void New_Email_Implements_IDisposable()
		{
			var email = new Email("johno@test.com");

			Assert.IsInstanceOfType(email, typeof(IDisposable));

			//Assert.AreEqual(replyEmail, email.Message.ReplyToList.First().Address);
		}

		[TestMethod]
		public void New_Disposes_Of_The_Message()
		{
			var email = new Email(new FakeSmtpClient(), FromEmail);
			
			var mailMessage = new FakeEmailMessage();

			email.Message = mailMessage;

			email.Dispose();

			Assert.IsTrue(mailMessage.DisposeHasBeenCalled);
		}

		[TestMethod]
		public void New_Disposes_Of_The_SmtpClient()
		{
			var smtpClient = new FakeSmtpClient();
			var email = new Email(smtpClient, FromEmail);
			
			email.Dispose();

			Assert.IsTrue(smtpClient.DisposeHasBeenCalled);
		}

		[TestMethod]
		public void New_Does_Not_Cause_An_Exception_If_Message_Is_Null()
		{
			var email = new Email(new FakeSmtpClient(), FromEmail);

			email.Message = null;

			email.Dispose();
		}

		[TestMethod]
		public void New_Does_Not_Cause_An_Exception_If_SmtpClient_Is_Null()
		{
			SmtpClient smtpClient = null;
			var email = new Email(smtpClient, FromEmail);
			email.Dispose();
		}
		
		#endregion
	}
}
