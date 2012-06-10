using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentEmail
{
    public interface ITemplateRenderer
    {
        string Parse<T>(string template, T model);
    }
}
