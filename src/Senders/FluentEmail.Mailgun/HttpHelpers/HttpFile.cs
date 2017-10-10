using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FluentEmail.Mailgun.HttpHelpers
{
    public class HttpFile
    {
        public string ParameterName { get; set; }
        public string Filename { get; set; }
        public Stream Data { get; set; }
        public string ContentType { get; set; }
    }
}
