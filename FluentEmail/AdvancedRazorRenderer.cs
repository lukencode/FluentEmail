using System;
using System.IO;
using Xipton.Razor;

namespace FluentEmail
{
    public class AdvancedRazorRenderer : ITemplateRenderer
    {
        private RazorMachine _razorMachine { get; set; }

        public AdvancedRazorRenderer()
        {
            _razorMachine = new RazorMachine();
        }

        public void AddLayout(string virtualPath, string content)
        {
            if (!virtualPath.StartsWith("~"))
                virtualPath = string.Format("~/shared/{0}.cshtml", virtualPath);

            _razorMachine.RegisterTemplate(virtualPath, content);
        }

        public void AddLayoutFromFile(string virtualPath, string filename)
        {
            if (filename.StartsWith("~"))
                filename = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + filename.Replace("~", ""));
            string fullPath = Path.GetFullPath(filename);
            string content;
            TextReader textReader = (TextReader)new StreamReader(fullPath);
            try
            {
                content = textReader.ReadToEnd();
            }
            finally
            {
                textReader.Close();
            }

            AddLayout(virtualPath, content);
        }

        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            return _razorMachine.ExecuteContent(template, model).Result;
        }
    }
}