using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.BusinessLogic.Abstractions
{
    public interface IWriteUserReviewLogic : IBusinessLogic
    {
        Task Insert(UserReview userReview);

        Task Update(UserReview userReview);
    }
}
