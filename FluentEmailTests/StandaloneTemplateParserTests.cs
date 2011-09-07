using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentEmail;
using FluentEmail.TemplateParsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentEmailTests {
	[TestClass]
	[DeploymentItem(@"FluentEmailTests\\test.txt")]
	public class StandaloneTemplateParserTests {
		[TestMethod]
		public void Anonymous_Model_Template_Matches() {
			const string template = "sup @Model.Name";

			var templateParser = ParserFactory.CreateParser(ParserType.Razor);
			var parserOutput = templateParser.ParseFromString(template, new{Name = "LUKE"});

			Assert.AreEqual("sup LUKE", parserOutput);
		}

		[TestMethod]
		public void Anonymous_Model_Template_From_File_Matches() {

			var templateParser = ParserFactory.CreateParser(ParserType.Razor);
			var parserOutput = templateParser.ParseFromFile(@"~/Test.txt", new { Test = "FLUENTEMAIL" });

			Assert.AreEqual("yo email FLUENTEMAIL", parserOutput);
		}

		[TestMethod]
		public void Anonymous_Model_With_List_Template_Matches() {
			const string template = "sup @Model.Name here is a list @foreach(var i in Model.Numbers) { @i }";

			var templateParser = ParserFactory.CreateParser(ParserType.Razor);
			var parserOutput = templateParser.ParseFromString(template, new { Name = "LUKE", Numbers = new string[] { "1", "2", "3" } });

			Assert.AreEqual("sup LUKE here is a list 123", parserOutput);
		}
	}
}
