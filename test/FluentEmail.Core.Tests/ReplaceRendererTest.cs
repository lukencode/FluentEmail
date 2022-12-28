using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using NUnit.Framework;

namespace FluentEmail.Core.Tests
{
    [TestFixture]
    public class ReplaceRendererTest
    {
        [Test]
        public void ModelPropertyValueIsNull_Test()
        {
            ITemplateRenderer templateRenderer = new ReplaceRenderer();

            var address = new Address("james@test.com", "james");
            Assert.True(address.Name == "james");
            var template = "this is name: ##Name##";
            Assert.True("this is name: james" == templateRenderer.Parse(template, address));

            address.Name = null;
            Assert.True("this is name: " == templateRenderer.Parse(template, address));
        }
    }
}