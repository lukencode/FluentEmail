using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentEmail.TemplateParsers.Parsers;

namespace FluentEmail.TemplateParsers {
	/// <summary>
	/// Parser factory creates an instance of the specified parser type
	/// </summary>
	public class ParserFactory {
		/// <summary>
		/// Creates the parser.
		/// </summary>
		/// <param name="templateImplementation">The template implementation.</param>
		/// <returns></returns>
		public static ITemplateParser CreateParser(Type templateImplementation) {
			var output = Activator.CreateInstance(templateImplementation);

			return (ITemplateParser)output;
		}
	}
}
