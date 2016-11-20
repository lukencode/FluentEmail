using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FluentEmail
{
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
