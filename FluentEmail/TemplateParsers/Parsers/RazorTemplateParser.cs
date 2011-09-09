using System;
using System.Dynamic;
using System.IO;
using RazorEngine;

namespace FluentEmail.TemplateParsers.Parsers {
	public class RazorTemplateParser : ITemplateParser {

		public RazorTemplateParser() {
			InitializeRazorParser();
		}

		/// <summary>
		/// Parses from file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public string ParseFromFile<T>(string fileName, T model) {
			var path = GetPath(fileName);

			using (var textReader = new StreamReader(path)) {
				var template = textReader.ReadToEnd();
				return ParseFromString(template, model);
			}
		}

		/// <summary>
		/// Parses from string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="template">The template.</param>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public string ParseFromString<T>(string template, T model) {
			var result = Razor.Parse(template, model);
			return result;
		}

		//Some weirdness pointed out by lukencode. Will be validating this further when I get a chance
		/// <summary>
		/// Initializes the razor parser.
		/// </summary>
		private static void InitializeRazorParser() {
			dynamic temp = new ExpandoObject();
			temp.PlaceHolder = String.Empty;
		}

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		private static String GetPath(string fileName) {
			const string tilde = "~";
			var baseDir = string.Empty;

			if (fileName.StartsWith(tilde)) {
				baseDir = AppDomain.CurrentDomain.BaseDirectory;
				fileName = fileName.Replace(tilde, String.Empty);
			}

			var output = Path.GetFullPath(baseDir + fileName);

			return output;
		}
	}
}
