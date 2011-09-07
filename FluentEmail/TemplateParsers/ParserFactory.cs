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
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static ITemplateParser CreateParser(ParserType type){
			switch (type){
				case ParserType.Razor:
					return new RazorTemplateParserImpl();
				default:
					throw new ArgumentException("Unknown parser type");
			}
		}
	}
}
