using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentEmail.Core
{
    public class FluentEmailFactory : IFluentEmailFactory
    {
        private IServiceProvider services;

        public FluentEmailFactory(IServiceProvider services) => this.services = services;

        public IFluentEmail Create() => services.GetService<IFluentEmail>();
    }
}
