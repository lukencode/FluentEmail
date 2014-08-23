namespace FluentEmail
{
	
	#region using
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	#endregion
	
	internal static class EmbeddedResourceHelper
	{
		internal static string GetResourceAsString(Assembly assembly, string path)
		{
			string result;

			using (var stream = assembly.GetManifestResourceStream(path))
				using (var reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}

			return result;
		}
	}
}
