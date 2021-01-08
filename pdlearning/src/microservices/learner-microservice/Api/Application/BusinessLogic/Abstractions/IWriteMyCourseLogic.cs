using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.BusinessLogic.Abstractions
{
    public interface IWriteMyCourseLogic : IBusinessLogic
    {
        Task Insert(MyCourse myCourse);

        Task Update(MyCourse myCourse);
    }
}
