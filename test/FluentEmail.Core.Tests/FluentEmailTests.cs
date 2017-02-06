using System.Collections.Generic;
using FluentEmail.Core.Models;
using NUnit.Framework;
using System.Linq;

namespace FluentEmail.Core.Tests
{
    [TestFixture]
    public class FluentEmailTests
	{
		const string toEmail = "bob@test.com";
		const string fromEmail = "johno@test.com";
		const string subject = "sup dawg";
		const string body = "what be the hipitity hap?";

		[Test]
		public void To_Address_Is_Set()
		{
			var email = Email
				.From(fromEmail)
				.To(toEmail);

			Assert.AreEqual(toEmail, email.Data.ToAddresses[0].EmailAddress);
		}

		[Test]
		public void From_Address_Is_Set()
		{
			var email = Email.From(fromEmail);

			Assert.AreEqual(fromEmail, email.Data.FromAddress.EmailAddress);
		}

		[Test]
		public void Subject_Is_Set()
		{
			var email = Email
				.From(fromEmail)
				.Subject(subject);

			Assert.AreEqual(subject, email.Data.Subject);
		}

		[Test]
		public void Body_Is_Set()
		{
			var email = Email.From(fromEmail)
				.Body(body);

			Assert.AreEqual(body, email.Data.Body);
		}

		[Test]
		public void Can_Add_Multiple_Recipients()
		{
			string toEmail1 = "bob@test.com";
			string toEmail2 = "ratface@test.com";

			var email = Email
				.From(fromEmail)
				.To(toEmail1)
				.To(toEmail2);

			Assert.AreEqual(2, email.Data.ToAddresses.Count);
		}

		[Test]
		public void Can_Add_Multiple_Recipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = Email
				.From(fromEmail)
				.To(emails);

			Assert.AreEqual(2, email.Data.ToAddresses.Count);
		}

		[Test]
		public void Can_Add_Multiple_CCRecipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = Email
				.From(fromEmail)
				.CC(emails);

			Assert.AreEqual(2, email.Data.CcAddresses.Count);
		}

		[Test]
		public void Can_Add_Multiple_BCCRecipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = Email
				.From(fromEmail)
				.BCC(emails);

			Assert.AreEqual(2, email.Data.BccAddresses.Count);
		}

		[Test]
		public void Is_Valid_With_Properties_Set()
		{
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.Body(body);

			Assert.AreEqual(body, email.Data.Body);
			Assert.AreEqual(subject, email.Data.Subject);
			Assert.AreEqual(fromEmail, email.Data.FromAddress.EmailAddress);
			Assert.AreEqual(toEmail, email.Data.ToAddresses[0].EmailAddress);
		}

		[Test]
		public void ReplyTo_Address_Is_Set()
		{
			var replyEmail = "reply@email.com";

			var email = Email.From(fromEmail)
				.ReplyTo(replyEmail);

			Assert.AreEqual(replyEmail, email.Data.ReplyToAddresses.First().EmailAddress);
		}
		
		#region Refactored tests using setup through constructors.
		[Test]
		public void New_To_Address_Is_Set()
		{
			var email = new Email(fromEmail)
				.To(toEmail);

			Assert.AreEqual(toEmail, email.Data.ToAddresses[0].EmailAddress);
		}

		[Test]
		public void New_From_Address_Is_Set()
		{
			var email = new Email(fromEmail);

			Assert.AreEqual(fromEmail, email.Data.FromAddress.EmailAddress);
		}

		[Test]
		public void New_Subject_Is_Set()
		{
			var email = new Email(fromEmail)
				.Subject(subject);

			Assert.AreEqual(subject, email.Data.Subject);
		}

		[Test]
		public void New_Body_Is_Set()
		{
			var email = new Email(fromEmail)
				.Body(body);

			Assert.AreEqual(body, email.Data.Body);
		}

		[Test]
		public void New_Can_Add_Multiple_Recipients()
		{
			string toEmail1 = "bob@test.com";
			string toEmail2 = "ratface@test.com";

			var email = new Email(fromEmail)
				.To(toEmail1)
				.To(toEmail2);

			Assert.AreEqual(2, email.Data.ToAddresses.Count);
		}

		[Test]
		public void New_Can_Add_Multiple_Recipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = new Email(fromEmail)
				.To(emails);

			Assert.AreEqual(2, email.Data.ToAddresses.Count);
		}

		[Test]
		public void New_Can_Add_Multiple_CCRecipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = new Email(fromEmail)
				.CC(emails);

			Assert.AreEqual(2, email.Data.CcAddresses.Count);
		}

		[Test]
		public void New_Can_Add_Multiple_BCCRecipients_From_List()
		{
			var emails = new List<Address>();
			emails.Add(new Address("email1@email.com"));
			emails.Add(new Address("email2@email.com"));

			var email = new Email(fromEmail)
				.BCC(emails);

			Assert.AreEqual(2, email.Data.BccAddresses.Count);
		}

		[Test]
		public void New_Is_Valid_With_Properties_Set()
		{
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.Body(body);

			Assert.AreEqual(body, email.Data.Body);
			Assert.AreEqual(subject, email.Data.Subject);
			Assert.AreEqual(fromEmail, email.Data.FromAddress.EmailAddress);
			Assert.AreEqual(toEmail, email.Data.ToAddresses[0].EmailAddress);
		}

		[Test]
		public void New_ReplyTo_Address_Is_Set()
		{
			var replyEmail = "reply@email.com";

			var email = new Email(fromEmail)
				.ReplyTo(replyEmail);

			Assert.AreEqual(replyEmail, email.Data.ReplyToAddresses.First().EmailAddress);
		}
		#endregion
	}
}
