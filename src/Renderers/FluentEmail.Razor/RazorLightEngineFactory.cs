using RazorLight;
using RazorLight.Razor;
using System;
using System.IO;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("FluentEmail.Razor.Tests")]
namespace FluentEmail.Razor
{
    internal static class RazorLightEngineFactory
    {
        public static IRazorLightEngine Create()
        {
            return new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();
        }

        public static IRazorLightEngine Create(string root)
        {
            return new RazorLightEngineBuilder()
                .UseFilesystemProject(root ?? Directory.GetCurrentDirectory())
                .UseMemoryCachingProvider()
                .Build();
        }

        public static IRazorLightEngine Create(RazorLightProject project)
        {
            return new RazorLightEngineBuilder()
                .UseProject(project)
                .UseMemoryCachingProvider()
                .Build();
        }

        public static IRazorLightEngine Create(Type embeddedResRootType)
        {
            return new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(embeddedResRootType)
                .UseMemoryCachingProvider()
                .Build();
        }
    }
}
