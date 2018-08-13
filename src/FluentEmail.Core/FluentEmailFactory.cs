using Microsoft.Extensions.DependencyInjection;
using System;

namespace FluentEmail.Core
{
    public class FluentEmailFactory : IFluentEmailFactory
    {
        private IServiceProvider services;

        public FluentEmailFactory(IServiceProvider services) => this.services = services;

        public IFluentEmail Create() => services.GetService<IFluentEmail>();
    }
}
