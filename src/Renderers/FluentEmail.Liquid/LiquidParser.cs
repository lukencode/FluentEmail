using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;

namespace FluentEmail.Liquid;

public class LiquidParser : FluidParser
{
    public LiquidParser()
    {
        RegisterExpressionTag("layout", OnRegisterLayoutTag);
        RegisterEmptyTag("renderbody", OnRegisterRenderBodyTag);
        RegisterIdentifierBlock("section", OnRegisterSectionBlock);
        RegisterIdentifierTag("rendersection", OnRegisterSectionTag);
    }

    private async ValueTask<Completion> OnRegisterLayoutTag(Expression expression, TextWriter writer, TextEncoder encoder, TemplateContext context)
    {
        const string viewExtension = ".liquid";

        var relativeLayoutPath = (await expression.EvaluateAsync(context)).ToStringValue();

        if (!relativeLayoutPath.EndsWith(viewExtension, StringComparison.OrdinalIgnoreCase))
        {
            relativeLayoutPath += viewExtension;
        }

        context.AmbientValues["Layout"] = relativeLayoutPath;

        return Completion.Normal;
    }

    private async ValueTask<Completion> OnRegisterRenderBodyTag(TextWriter writer, TextEncoder encoder, TemplateContext context)
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

    private ValueTask<Completion> OnRegisterSectionBlock(string sectionName, IReadOnlyList<Statement> statements, TextWriter writer, TextEncoder encoder, TemplateContext context)
    {
        if (context.AmbientValues.TryGetValue("Sections", out var sections))
        {
            var dictionary = (Dictionary<string, List<Statement>>) sections;
              
            dictionary[sectionName] = statements.ToList();
        }

        return new ValueTask<Completion>(Completion.Normal);
    }

    private async ValueTask<Completion> OnRegisterSectionTag(string sectionName, TextWriter writer, TextEncoder encoder, TemplateContext context)
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