namespace FluentEmail
{
	
	#region using
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	#endregion

	public interface ITemplateRenderer
	{
		string Parse<T>(string template, T model, bool isHtml = true);
	}
}
