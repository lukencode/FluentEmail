using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.Graph
{
    public class GraphSender : ISender
    {
        public GraphSender(
            string GraphEmailAppId,
            string GraphEmailTenantId,
            string GraphEmailSecret)
        {

        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            throw new NotImplementedException();
        }

        public Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            throw new NotImplementedException();
        }
    }
}
