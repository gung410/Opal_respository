using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.BusinessLogic.Abstractions
{
    public interface IWriteMyDigitalContentLogic : IBusinessLogic
    {
        Task Insert(MyDigitalContent myDigitalContent);

        Task Update(MyDigitalContent myDigitalContent);
    }
}
