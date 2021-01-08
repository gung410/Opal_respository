using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.BusinessLogic.Abstractions
{
    public interface IWriteUserBookmarkLogic : IBusinessLogic
    {
        Task Insert(UserBookmark userBookmark);

        Task Delete(UserBookmark userBookmark);
    }
}
