using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using FluentEmail;
using System.ComponentModel;
using System.Net.Mail;
using System.Threading;

namespace FluentEmailTests
{
	[TestFixture]
	public class AsyncTests
	{
		static bool callbackCalled = false;
		static private ManualResetEvent updatedEvent = new ManualResetEvent(false);
		
		[TestFixtureTearDownAttribute]
		public void CleanUp()
		{
			Array.ForEach(Directory.GetFiles(Path.Combine(CurrentProjectPath(), "Emails")), File.Delete);
		}

		[Test]
		public void Callback_Method_Is_Called_On_Cancel()
		{
			string toEmail = "bob@test.com";
			string fromEmail = "johno@test.com";
			string subject = "sup dawg";
			string body = "what be the hipitity hap?";
			
			string tempEmailPath = Path.Combine(CurrentProjectPath(), "Emails");
			
			var smtpClient = new SmtpClient("localhost", 25);
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
			smtpClient.PickupDirectoryLocation = tempEmailPath;
			

			var email = new Email(smtpClient)
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.Body(body);
			
			email.SendAsync(MailDeliveryComplete);

			email.Cancel();

			updatedEvent.WaitOne();

			Assert.IsTrue(callbackCalled);
		}

		private static void MailDeliveryComplete(object sender, AsyncCompletedEventArgs e)
		{
			callbackCalled = true;
			updatedEvent.Set();
		}
		
		public string CurrentProjectPath()
		{
			return Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
		}
		
		
		
	}
}
