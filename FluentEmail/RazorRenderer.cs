﻿namespace FluentEmail
{

	#region using
	using System;
	using System.Collections.Generic;
	using System.Dynamic;
	using System.Linq;
	using System.Text;
	using RazorEngine;
	#endregion

	public class RazorRenderer : ITemplateRenderer
	{
		public RazorRenderer()
		{
		}

		public string Parse<T>(string template, T model, bool isHtml = true)
		{
			return Razor.Parse(template, model, template.GetHashCode().ToString());
		}
	}
}
