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

		[TestMethod]
		public void Can_Add_Multiple_CCRecipients_From_List()
		{
			var emails = new List<MailAddress>();
			emails.Add(new MailAddress("email1@email.com"));
			emails.Add(new MailAddress("email2@email.com"));

			var email = Email
				.From(fromEmail)
				.CC(emails);

			Assert.AreEqual(2, email.Message.CC.Count);
		}

		[TestMethod]
		public void Can_Add_Multiple_BCCRecipients_From_List()
		{
			var emails = new List<MailAddress>();
			emails.Add(new MailAddress("email1@email.com"));
			emails.Add(new MailAddress("email2@email.com"));

			var email = Email
				.From(fromEmail)
				.BCC(emails);

			Assert.AreEqual(2, email.Message.Bcc.Count);
		}

		[TestMethod]
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

		[TestMethod]
		public void ReplyTo_Address_Is_Set()
		{
			var replyEmail = "reply@email.com";

			var email = Email.From(fromEmail)
				.ReplyTo(replyEmail);

			Assert.AreEqual(replyEmail, email.Message.ReplyToList.First().Address);
		}
		
		#region Refactored tests using setup through constructors.
		[TestMethod]
		public void New_To_Address_Is_Set()
		{
			var email = new Email(fromEmail)
				.To(toEmail);

			Assert.AreEqual(toEmail, email.Message.To[0].Address);
		}

		[TestMethod]
		public void New_From_Address_Is_Set()
		{
			var email = new Email(fromEmail);

			Assert.AreEqual(fromEmail, email.Message.From.Address);
		}

		[TestMethod]
		public void New_Subject_Is_Set()
		{
			var email = new Email(fromEmail)
				.Subject(subject);

			Assert.AreEqual(subject, email.Message.Subject);
		}

		[TestMethod]
		public void New_Body_Is_Set()
		{
			var email = new Email(fromEmail)
				.Body(body);

			Assert.AreEqual(body, email.Message.Body);
		}

		[TestMethod]
		public void New_Can_Add_Multiple_Recipients()
		{
			string toEmail1 = "bob@test.com";
			string toEmail2 = "ratface@test.com";

			var email = new Email(fromEmail)
				.To(toEmail1)
				.To(toEmail2);

			Assert.AreEqual(2, email.Message.To.Count);
		}

		[TestMethod]
		public void New_Can_Add_Multiple_Recipients_From_List()
		{
			var emails = new List<MailAddress>();
			emails.Add(new MailAddress("email1@email.com"));
			emails.Add(new MailAddress("email2@email.com"));

			var email = new Email(fromEmail)
				.To(emails);

			Assert.AreEqual(2, email.Message.To.Count);
		}

		[TestMethod]
		public void New_Can_Add_Multiple_CCRecipients_From_List()
		{
			var emails = new List<MailAddress>();
			emails.Add(new MailAddress("email1@email.com"));
			emails.Add(new MailAddress("email2@email.com"));

			var email = new Email(fromEmail)
				.CC(emails);

			Assert.AreEqual(2, email.Message.CC.Count);
		}

		[TestMethod]
		public void New_Can_Add_Multiple_BCCRecipients_From_List()
		{
			var emails = new List<MailAddress>();
			emails.Add(new MailAddress("email1@email.com"));
			emails.Add(new MailAddress("email2@email.com"));

			var email = new Email(fromEmail)
				.BCC(emails);

			Assert.AreEqual(2, email.Message.Bcc.Count);
		}

		[TestMethod]
		public void New_Is_Valid_With_Properties_Set()
		{
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.Body(body);

			Assert.AreEqual(body, email.Message.Body);
			Assert.AreEqual(subject, email.Message.Subject);
			Assert.AreEqual(fromEmail, email.Message.From.Address);
			Assert.AreEqual(toEmail, email.Message.To[0].Address);
		}

		[TestMethod]
		public void New_ReplyTo_Address_Is_Set()
		{
			var replyEmail = "reply@email.com";

			var email = new Email(fromEmail)
				.ReplyTo(replyEmail);

			Assert.AreEqual(replyEmail, email.Message.ReplyToList.First().Address);
		}
		#endregion
	}
}
