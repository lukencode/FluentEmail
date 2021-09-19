using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentEmail.Core;
using Fluid;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEmail.Liquid.Tests.ComplexModel
{
    public class ComplexModelRenderTests
    {
        public ComplexModelRenderTests()
        {
            SetupRenderer();
        }

        [Test]
        public void Can_Render_Complex_Model_Properties()
        {
            var model = new ParentModel
            {
                ParentName = new NameDetails { Firstname = "Luke", Surname = "Dinosaur" },
                ChildrenNames = new List<NameDetails>
                {
                    new NameDetails { Firstname = "ChildFirstA", Surname = "ChildLastA" },
                    new NameDetails { Firstname = "ChildFirstB", Surname = "ChildLastB" }
                }
            };

            var expected = @"
Parent: Luke
Children:

* ChildFirstA ChildLastA
* ChildFirstB ChildLastB
";

            var email = Email
                .From(TestData.FromEmail)
                .To(TestData.ToEmail)
                .Subject(TestData.Subject)
                .UsingTemplate(Template(), model);
            email.Data.Body.Should().Be(expected);
        }

        private string Template()
        {
            return @"
Parent: {{ ParentName.Firstname }}
Children:
{% for Child in ChildrenNames %}
* {{ Child.Firstname }} {{ Child.Surname }}{% endfor %}
";
        }

        private static void SetupRenderer(
            IFileProvider fileProvider = null,
            Action<TemplateContext, object> configureTemplateContext = null)
        {
            var options = new LiquidRendererOptions
            {
                FileProvider = fileProvider,
                ConfigureTemplateContext = configureTemplateContext,
                TemplateOptions = new TemplateOptions { MemberAccessStrategy = new UnsafeMemberAccessStrategy() }
            };
            Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));
        }
    }
}