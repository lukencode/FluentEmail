using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core.Models;

namespace FluentEmail.Core.Interfaces
{
    public interface ISender
    {
        SendResponse Send(IFluentEmail email, CancellationToken? token = null);
        Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null);
    }
}
