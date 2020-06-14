using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEmail.Graph
{
    public class GraphSender : ISender
    {
        private readonly string _appId;
        private readonly string _tenantId;
        private readonly string _graphSecret;

        private ClientCredentialProvider authProvider;
        private GraphServiceClient _graphClient;

        public GraphSender(
            string GraphEmailAppId,
            string GraphEmailTenantId,
            string GraphEmailSecret)
        {
            _appId = GraphEmailAppId;
            _tenantId = GraphEmailTenantId;
            _graphSecret = GraphEmailSecret;
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
