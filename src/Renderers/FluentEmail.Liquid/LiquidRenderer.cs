using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using Fluid;
using Fluid.Ast;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace FluentEmail.Liquid
{
    public class LiquidRenderer : ITemplateRenderer
    {
        private readonly IOptions<LiquidRendererOptions> _options;
        private readonly LiquidParser _parser;

        public LiquidRenderer(IOptions<LiquidRendererOptions> options)
        {
            _options = options;
            _parser = new LiquidParser();
        }

        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            return ParseAsync(template, model, isHtml).GetAwaiter().GetResult();
        }

        public async Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
        {
            var rendererOptions = _options.Value;

            // Check for a custom file provider
            var fileProvider = rendererOptions.FileProvider;
            var viewTemplate = ParseTemplate(template);

            var context = new TemplateContext(model, rendererOptions.TemplateOptions)
            {
                // provide some services to all statements
                AmbientValues =
                {
                    ["FileProvider"] = fileProvider,
                    ["Sections"] = new Dictionary<string, List<Statement>>()
                },
                Options =
                {
                    FileProvider = fileProvider
                }
            };

            rendererOptions.ConfigureTemplateContext?.Invoke(context, model!);

            var body = await viewTemplate.RenderAsync(context, rendererOptions.TextEncoder);

            // if a layout is specified while rendering a view, execute it
            if (context.AmbientValues.TryGetValue("Layout", out var layoutPath))
            {
                context.AmbientValues["Body"] = body;
                var layoutTemplate = ParseLiquidFile((string)layoutPath, fileProvider!);

                return await layoutTemplate.RenderAsync(context, rendererOptions.TextEncoder);
            }

            return body;
        }

        private IFluidTemplate ParseLiquidFile(
            string path,
            IFileProvider? fileProvider)
        {
            static void ThrowMissingFileProviderException()
            {
                const string message = "Cannot parse external file, file provider missing";
                throw new ArgumentNullException(nameof(LiquidRendererOptions.FileProvider), message);
            }

            if (fileProvider is null)
            {
                ThrowMissingFileProviderException();
            }

            var fileInfo = fileProvider!.GetFileInfo(path);
            using var stream = fileInfo.CreateReadStream();
            using var sr = new StreamReader(stream);

            return ParseTemplate(sr.ReadToEnd());
        }

        private IFluidTemplate ParseTemplate(string content)
        {
            if (!_parser.TryParse(content, out var template, out var errors))
            {
                throw new Exception(string.Join(Environment.NewLine, errors));
            }

            return template;
        }
    }
}