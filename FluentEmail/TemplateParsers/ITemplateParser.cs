using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentEmail.TemplateParsers {
	/// <summary>
	/// Template Parser definition
	/// </summary>
	public interface ITemplateParser {
		string ParseFromFile<T>(string fileName, T model);
		string ParseFromString<T>(string template, T model);
	}
}
