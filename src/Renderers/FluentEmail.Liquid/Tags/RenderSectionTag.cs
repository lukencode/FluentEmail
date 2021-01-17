using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Fluid.Tags;

namespace FluentEmail.Liquid.Tags
{
    internal sealed class RenderSectionTag : IdentifierTag
    {
        public override async ValueTask<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context, string sectionName)
        {
            if (context.AmbientValues.TryGetValue("Sections", out var sections))
            {
                var dictionary = (Dictionary<string, List<Statement>>) sections;
                if (dictionary.TryGetValue(sectionName, out var section))
                {
                    foreach(var statement in section)
                    {
                        await statement.WriteToAsync(writer, encoder, context);
                    }
                }
            }

            return Completion.Normal;
        }
    }
}
