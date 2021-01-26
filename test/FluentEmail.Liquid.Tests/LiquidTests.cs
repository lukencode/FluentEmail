using System;
using System.IO;
using System.Reflection;
using FluentEmail.Core;

using Fluid;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

using NUnit.Framework;

namespace FluentEmail.Liquid.Tests
{
	public class LiquidTests
    {
	    private const string ToEmail = "bob@test.com";
        private const string FromEmail = "johno@test.com";
        private const string Subject = "sup dawg";

        [SetUp]
        public void SetUp()
        {
            // default to have no file provider, only required when layout files are in use
            SetupRenderer(null);
        }

        private static void SetupRenderer(
            IFileProvider fileProvider = null,
            Action<TemplateContext, object> configureTemplateContext = null)
        {
            var options = new LiquidRendererOptions
            {
                FileProvider = fileProvider,
                ConfigureTemplateContext = configureTemplateContext,
            };
            Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));
        }

        [Test]
        public void Model_With_List_Template_Matches()
        {
            const string template = "sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";

            var email = Email
                .From(FromEmail)
                .To(ToEmail)
                .Subject(Subject)
                .UsingTemplate(template, new ViewModel { Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

            Assert.AreEqual("sup LUKE here is a list 123", email.Data.Body);
        }


        [Test]
        public void Custom_Context_Values()
        {
            SetupRenderer(new NullFileProvider(), (context, model) =>
            {
                context.SetValue("FirstName", "Samantha");
                context.SetValue("IntegerNumbers", new[] {3, 2, 1});
            });

            const string template = "sup {{ FirstName }} here is a list {% for i in IntegerNumbers %}{{ i }}{% endfor %}";

            var email = Email
                .From(FromEmail)
                .To(ToEmail)
                .Subject(Subject)
                .UsingTemplate(template, new ViewModel { Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

            Assert.AreEqual("sup Samantha here is a list 321", email.Data.Body);
        }

        // currently not cached as Fluid is so fast, but can be added later
        [Test]
        public void Reuse_Cached_Templates()
        {
            const string template = "sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";
            const string template2 = "sup {{ Name }} this is the second template";

            for (var i = 0; i < 10; i++)
            {
                var email = Email
                    .From(FromEmail)
                    .To(ToEmail)
                    .Subject(Subject)
                    .UsingTemplate(template, new ViewModel { Name = i.ToString(), Numbers = new[] { "1", "2", "3" } });

                Assert.AreEqual("sup " + i + " here is a list 123", email.Data.Body);

                var email2 = Email
                    .From(FromEmail)
                    .To(ToEmail)
                    .Subject(Subject)
                    .UsingTemplate(template2, new ViewModel { Name = i.ToString() });

                Assert.AreEqual("sup " + i + " this is the second template", email2.Data.Body);
            }
        }

        [Test]
        public void New_Model_Template_Matches()
        {
            const string template = "sup {{ Name }}";

            var email = new Email(FromEmail)
                .To(ToEmail)
                .Subject(Subject)
                .UsingTemplate(template, new ViewModel { Name = "LUKE" });

            Assert.AreEqual("sup LUKE", email.Data.Body);
        }

        [Test]
        public void New_Model_With_List_Template_Matches()
        {
            const string template = "sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";

            var email = new Email(FromEmail)
                .To(ToEmail)
                .Subject(Subject)
                .UsingTemplate(template, new ViewModel { Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

            Assert.AreEqual("sup LUKE here is a list 123", email.Data.Body);
        }

        // currently not cached as Fluid is so fast, but can be added later
        [Test]
        public void New_Reuse_Cached_Templates()
        {
            const string template = "sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";
            const string template2 = "sup {{ Name }} this is the second template";

            for (var i = 0; i < 10; i++)
            {
                var email = new Email(FromEmail)
                    .To(ToEmail)
                    .Subject(Subject)
                    .UsingTemplate(template, new ViewModel { Name = i.ToString(), Numbers = new[] { "1", "2", "3" } });

                Assert.AreEqual("sup " + i + " here is a list 123", email.Data.Body);

                var email2 = new Email(FromEmail)
                    .To(ToEmail)
                    .Subject(Subject)
                    .UsingTemplate(template2, new ViewModel { Name = i.ToString() });

                Assert.AreEqual("sup " + i + " this is the second template", email2.Data.Body);
            }
        }

	    [Test]
	    public void Should_be_able_to_use_project_layout()
	    {
            SetupRenderer(new PhysicalFileProvider(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName, "EmailTemplates")));

		    const string template = @"
{% layout '_layout.liquid' %}
sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";

			var email = new Email(FromEmail)
			    .To(ToEmail)
			    .Subject(Subject)
			    .UsingTemplate(template, new ViewModel{ Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

		    Assert.AreEqual($"<h1>Hello!</h1>{Environment.NewLine}<div>{Environment.NewLine}sup LUKE here is a list 123</div>", email.Data.Body);
	    }


        [Test]
        public void Should_be_able_to_use_embedded_layout()
        {
            SetupRenderer(new EmbeddedFileProvider(typeof(LiquidTests).Assembly, "FluentEmail.Liquid.Tests.EmailTemplates"));

            const string template = @"
{% layout '_embedded.liquid' %}
sup {{ Name }} here is a list {% for i in Numbers %}{{ i }}{% endfor %}";

            var email = new Email(FromEmail)
                .To(ToEmail)
                .Subject(Subject)
                .UsingTemplate(template, new ViewModel{ Name = "LUKE", Numbers = new[] { "1", "2", "3" } });

            Assert.AreEqual($"<h2>Hello!</h2>{Environment.NewLine}<div>{Environment.NewLine}sup LUKE here is a list 123</div>", email.Data.Body);
        }

        private class ViewModel
        {
            public string Name { get; set; }
            public string[] Numbers { get; set; }
        }
    }
}
