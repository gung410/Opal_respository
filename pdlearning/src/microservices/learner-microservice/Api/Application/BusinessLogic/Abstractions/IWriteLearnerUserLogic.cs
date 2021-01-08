using System.Threading.Tasks;
using Microservice.Learner.Application.RequestDtos;

namespace Microservice.Learner.Application.BusinessLogic.Abstractions
{
    public interface IWriteLearnerUserLogic : IBusinessLogic
    {
        Task SaveLearnerUser(SaveLearnerUserRequestDto request);
    }
}
