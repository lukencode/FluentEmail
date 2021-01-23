using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Fluid.Tags;

namespace FluentEmail.Liquid.Tags
{
    internal sealed class RenderBodyTag : SimpleTag
    {
        public override async ValueTask<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            static void ThrowParseException()
            {
                throw new ParseException("Could not render body, Layouts can't be evaluated directly.");
            }

            if (context.AmbientValues.TryGetValue("Body", out var body))
            {
                await writer.WriteAsync((string)body);
            }
            else
            {
                ThrowParseException();
            }

            return Completion.Normal;
        }
    }
}
