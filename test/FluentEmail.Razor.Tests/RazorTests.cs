﻿using FluentEmail.Core;
using NUnit.Framework;
using System.Dynamic;
using System.IO;
using FluentEmail.Razor;

namespace FluentEmail.Razor.Tests
{
	public class RazorTests
    {
        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";

        [SetUp]
        public void SetUp()
        {
            var engine = RazorLightEngineFactory.Create();
            Email.DefaultRenderer = new RazorRenderer(engine);
        }

        [Test]
        public void Anonymous_Model_With_List_Template_Matches()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .UsingTemplate(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

            Assert.AreEqual("sup LUKE here is a list 123", email.Data.Body);
        }

        [Test]
        public void Reuse_Cached_Templates()
        {
            string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";
            string template2 = "sup @Model.Name this is the second template";

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


	    [Test]
	    public void Should_be_able_to_use_project_layout_with_viewbag()
	    {
		    var projectRoot = Directory.GetCurrentDirectory();
            var engine = RazorLightEngineFactory.Create(projectRoot);
            Email.DefaultRenderer = new RazorRenderer(engine);

		    string template = @"
@{
	Layout = ""./Shared/_Layout.cshtml"";
}
sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

			dynamic viewBag = new ExpandoObject();
			viewBag.Title = "Hello!";
		    var email = new Email(fromEmail)
			    .To(toEmail)
			    .Subject(subject)
			    .UsingTemplate(template, new ViewModelWithViewBag{ Name = "LUKE", Numbers = new[] { "1", "2", "3" }, ViewBag = viewBag});

		    Assert.AreEqual("<h1>Hello!</h1>\r\n<div>\r\nsup LUKE here is a list 123</div>", email.Data.Body);
	    }

	    [Test]
	    public void Should_be_able_to_use_embedded_layout_with_viewbag()
	    {
            var engine = RazorLightEngineFactory.Create(typeof(RazorTests));
            Email.DefaultRenderer = new RazorRenderer(engine);
		    string template = @"
@{
	Layout = ""_EmbeddedLayout.cshtml"";
}
sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

		    dynamic viewBag = new ExpandoObject();
		    viewBag.Title = "Hello!";
		    var email = new Email(fromEmail)
			    .To(toEmail)
			    .Subject(subject)
			    .UsingTemplate(template, new ViewModelWithViewBag{ Name = "LUKE", Numbers = new[] { "1", "2", "3" }, ViewBag = viewBag});

		    Assert.AreEqual("<h2>Hello!</h2>\r\n<div>\r\nsup LUKE here is a list 123</div>", email.Data.Body);
	    }
    }

	public class ViewModelWithViewBag : IViewBagModel
	{
		public ExpandoObject ViewBag { get; set;}
		public string Name {get;set;}
		public string[] Numbers {get;set;}
	}
}
