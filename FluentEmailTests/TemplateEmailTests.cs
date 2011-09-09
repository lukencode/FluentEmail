using System;
using System.Dynamic;
using System.IO;
using FluentEmail;
using FluentEmail.TemplateParsers.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentEmailTests {
	[TestClass]
	[DeploymentItem(@"FluentEmailTests\\test.txt")]
	public class TemplateEmailTests {
		const string toEmail = "bob@test.com";
		const string fromEmail = "johno@test.com";
		const string subject = "sup dawg";

		[TestMethod]
		public void Anonymous_Model_Template_From_File_Matches(){

			var email = Email
				.From(fromEmail)
				.To(toEmail)
				.Subject(subject)
				.UsingTemplateFromFile(@"~/Test.txt", new { Test = "FLUENTEMAIL" }, typeof(RazorTemplateParser));
						//.UsingTemplateFromFile<RazorTemplateParser>(@"~/Test.txt", new { Test = "FLUENTEMAIL" });

			Assert.AreEqual("yo email FLUENTEMAIL", email.Message.Body);
		}

		[TestMethod]
		public void Anonymous_Model_Template_Matches() {
			string template = "sup @Model.Name";

			var email = Email
						.From(fromEmail)
						.To(toEmail)
						.Subject(subject)
						.UsingTemplate(template, new { Name = "LUKE" }, typeof(RazorTemplateParser));

			Assert.AreEqual("sup LUKE", email.Message.Body);
		}

		[TestMethod]
		public void Anonymous_Model_With_List_Template_Matches() {
			string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

			var email = Email
						.From(fromEmail)
						.To(toEmail)
						.Subject(subject)
						.UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } }, typeof(RazorTemplateParser));

			Assert.AreEqual("sup LUKE here is a list 123", email.Message.Body);
		}

	}
}
