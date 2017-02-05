using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core.Models;

namespace FluentEmail.Core.Interfaces
{
    public interface ISender
    {
        SendResponse Send(Email email, CancellationToken? token = null);
        Task<SendResponse> SendAsync(Email email, CancellationToken? token = null);
    }
}
