using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;
using NUnit.Framework;

namespace FluentEmail.Core.Tests
{
    [TestFixture]
    public class TemplateEmailTests
    {
        private Assembly ThisAssembly() => this.GetType().GetTypeInfo().Assembly;
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
				.UsingTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" });

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
				.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL", culture }, culture);

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
				.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" }, culture);

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
	            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new {Test = "FLUENTEMAIL"}, culture);

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
				.UsingTemplateFromEmbedded("FluentEmail.Core.Tests.test-embedded.txt", new { Test = "EMBEDDEDTEST" }, ThisAssembly());

			Assert.AreEqual("yo email EMBEDDEDTEST", email.Data.Body);
		}
		
		[Test]
		public void New_Anonymous_Model_Template_From_File_Matches()
		{
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" });

			Assert.AreEqual("yo email FLUENTEMAIL", email.Data.Body);
		}

		[Test]
		public void New_Using_Template_From_Not_Existing_Culture_File_Using_Defualt_Template()
		{
			var culture = new CultureInfo("fr-FR");
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL", culture }, culture);

			Assert.AreEqual("yo email FLUENTEMAIL", email.Data.Body);
		}

		[Test]
		public void New_Using_Template_From_Culture_File()
		{
			var culture = new CultureInfo("he-IL");
			var email = new Email(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new { Test = "FLUENTEMAIL" }, culture);

			Assert.AreEqual("hebrew email FLUENTEMAIL", email.Data.Body);
		}

	    [Test]
	    public void New_Using_Template_From_Current_Culture_File()
	    {
	        var culture = new CultureInfo("he-IL");
	        var email = new Email(fromEmail)
	            .To(toEmail)
	            .Subject(subject)
	            .UsingCultureTemplateFromFile($"{Path.Combine(Directory.GetCurrentDirectory(), "test.txt")}", new {Test = "FLUENTEMAIL"}, culture);

	        Assert.AreEqual("hebrew email FLUENTEMAIL", email.Data.Body);
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
				.UsingTemplateFromEmbedded("FluentEmail.Core.Tests.test-embedded.txt", new { Test = "EMBEDDEDTEST" }, ThisAssembly());

			Assert.AreEqual("yo email EMBEDDEDTEST", email.Data.Body);
		}		
	}

	public class TestTemplate : ITemplateRenderer
	{
		public string Parse<T>(string template, T model, bool isHtml = true)
		{
			return "custom template";
		}

	    public Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
	    {
	        return Task.FromResult(Parse(template, model, isHtml));
	    }
	}
}
