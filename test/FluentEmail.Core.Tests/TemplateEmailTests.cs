using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;
using NUnit.Framework;

namespace FluentEmail.Core.Tests
{
    [TestFixture]
    public class TemplateEmailTests
	{
		const string toEmail = "bob@test.com";
		const string fromEmail = "johno@test.com";
		const string subject = "sup dawg";

		[Test]
		public void Anonymous_Model_Template_From_File_Matches()
		{
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Test.txt", new { Test = "FLUENTEMAIL" });

			Assert.AreEqual("yo email FLUENTEMAIL", email.Data.Body);
		}

		[Test]
		public void Using_Template_From_Not_Existing_Culture_File_Using_Defualt_Template()
		{
			var culture = new CultureInfo("fr-FR");
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Directory.GetCurrentDirectory()}/Test.txt", new { Test = "FLUENTEMAIL", culture }, culture);

			Assert.AreEqual("yo email FLUENTEMAIL", email.Data.Body);
		}

		[Test]
		public void Using_Template_From_Culture_File()
		{
			var culture = new CultureInfo("he-IL");
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Directory.GetCurrentDirectory()}/Test.txt", new { Test = "FLUENTEMAIL" }, culture);

			Assert.AreEqual("hebrew email FLUENTEMAIL", email.Data.Body);
		}

	    [Test]
	    public void Using_Template_From_Current_Culture_File()
	    {
	        var culture = new CultureInfo("he-IL");
	        var email = Email
	            .From(fromEmail)
	            .To(toEmail)
	            .Subject(subject)
	            .UsingCultureTemplateFromFile($"{Directory.GetCurrentDirectory()}/Test.txt", new {Test = "FLUENTEMAIL"}, culture);

	        Assert.AreEqual("hebrew email FLUENTEMAIL", email.Data.Body);
	    }

	    [Test]
		public void Anonymous_Model_Template_Matches()
		{
			string template = "sup ##Name##";

			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplate(template, new { Name = "LUKE" });

			Assert.AreEqual("sup LUKE", email.Data.Body);
		}

		[Test]
		public void Anonymous_Model_With_List_Template_Matches()
		{
			string template = "sup ##Name## here is a list @foreach(var i in Model.Numbers) { @i }";

			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

			Assert.AreEqual("sup LUKE here is a list 123", email.Data.Body);
		}

		[Test]
		public void Set_Custom_Template()
		{
			string template = "sup ##Name## here is a list @foreach(var i in Model.Numbers) { @i }";

			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateEngine(new TestTemplate())
				.UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

			Assert.AreEqual("custom template", email.Data.Body);
		}

		[Test]
		public void Using_Template_From_Embedded_Resource()
		{
			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromEmbedded("FluentEmailTests.test-embedded.txt", new { Test = "EMBEDDEDTEST" }, Assembly.GetEntryAssembly());

			Assert.AreEqual("yo email EMBEDDEDTEST", email.Data.Body);
		}

		[Test]
		public void Reuse_Cached_Templates()
		{
			string template = "sup ##Name## here is a list @foreach(var i in Model.Numbers) { @i }";
			string template2 = "sup ##Name## this is the second template";

			for (var i = 0; i < 10; i++)
			{
				var email = Email
					.From(fromEmail)
					.To(toEmail)
					.Subject(subject)
					.UsingTemplate(template, new { Name = i, Numbers = new string[] { "1", "2", "3" } });

				Assert.AreEqual("sup " + i + " here is a list 123", email.Data.Body);

				var email2 = Email
					.From(fromEmail)
					.To(toEmail)
					.Subject(subject)
					.UsingTemplate(template2, new { Name = i });

				Assert.AreEqual("sup " + i + " this is the second template", email2.Data.Body);
			}
		}
		
		#region Refactored tests using setup through constructors.
		[Test]
		public void New_Anonymous_Model_Template_From_File_Matches()
		{
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Test.txt", new { Test = "FLUENTEMAIL" });

			Assert.AreEqual("yo email FLUENTEMAIL", email.Data.Body);
		}

		[Test]
		public void New_Using_Template_From_Not_Existing_Culture_File_Using_Defualt_Template()
		{
			var culture = new CultureInfo("fr-FR");
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Directory.GetCurrentDirectory()}/Test.txt", new { Test = "FLUENTEMAIL", culture }, culture);

			Assert.AreEqual("yo email FLUENTEMAIL", email.Data.Body);
		}

		[Test]
		public void New_Using_Template_From_Culture_File()
		{
			var culture = new CultureInfo("he-IL");
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Directory.GetCurrentDirectory()}/Test.txt", new { Test = "FLUENTEMAIL" }, culture);

			Assert.AreEqual("hebrew email FLUENTEMAIL", email.Data.Body);
		}

	    [Test]
	    public void New_Using_Template_From_Current_Culture_File()
	    {
	        var culture = new CultureInfo("he-IL");
	        var email = new Email(fromEmail)
	            .To(toEmail)
	            .Subject(subject)
	            .UsingCultureTemplateFromFile($"{Directory.GetCurrentDirectory()}/Test.txt", new {Test = "FLUENTEMAIL"}, culture);

	        Assert.AreEqual("hebrew email FLUENTEMAIL", email.Data.Body);
	    }

	    [Test]
		public void New_Anonymous_Model_Template_Matches()
		{
			string template = "sup @Model.Name";

			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplate(template, new { Name = "LUKE" });

			Assert.AreEqual("sup LUKE", email.Data.Body);
		}

		[Test]
		public void New_Anonymous_Model_With_List_Template_Matches()
		{
			string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

			Assert.AreEqual("sup LUKE here is a list 123", email.Data.Body);
		}

		[Test]
		public void New_Set_Custom_Template()
		{
			string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

			var email = new Email(new TestTemplate(), new SaveToDiskSender("/"), fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

			Assert.AreEqual("custom template", email.Data.Body);
		}

		[Test]
		public void New_Using_Template_From_Embedded_Resource()
		{
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromEmbedded("FluentEmailTests.test-embedded.txt", new { Test = "EMBEDDEDTEST" }, Assembly.GetEntryAssembly());

			Assert.AreEqual("yo email EMBEDDEDTEST", email.Data.Body);
		}

		[Test]
		public void New_Reuse_Cached_Templates()
		{
			string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";
			string template2 = "sup @Model.Name this is the second template";

			for (var i = 0; i < 10; i++)
			{
				var email = new Email(fromEmail)
					.To(toEmail)
					.Subject(subject)
					.UsingTemplate(template, new { Name = i, Numbers = new string[] { "1", "2", "3" } });

				Assert.AreEqual("sup " + i + " here is a list 123", email.Data.Body);

				var email2 = new Email(fromEmail)
					.To(toEmail)
					.Subject(subject)
					.UsingTemplate(template2, new { Name = i });

				Assert.AreEqual("sup " + i + " this is the second template", email2.Data.Body);
			}
		}
		#endregion
	}

	public class TestTemplate : ITemplateRenderer
	{
		public string Parse<T>(string template, T model, bool isHtml = true)
		{
			return "custom template";
		}
	}

}
