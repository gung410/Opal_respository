using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Badge.Application.BusinessLogic
{
    public interface ICommunityStatisticLogic
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }
}
