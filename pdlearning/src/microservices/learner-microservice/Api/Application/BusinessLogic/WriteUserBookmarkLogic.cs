using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.BusinessLogic
{
    public class WriteUserBookmarkLogic : BaseBusinessLogic<UserBookmark>, IWriteUserBookmarkLogic
    {
        public WriteUserBookmarkLogic(
            IThunderCqrs thunderCqrs,
            IWriteOnlyRepository<UserBookmark> writeUserBookmarkRepository) : base(thunderCqrs, writeUserBookmarkRepository)
        {
        }

        public async Task Insert(UserBookmark userBookmark)
        {
            await ThunderCqrs.SendEvent(new UserBookChangeEvent(userBookmark, UserBookmarkEventChangeType.Created));

            await WriteRepository.InsertAsync(userBookmark);
        }

        public async Task Delete(UserBookmark userBookmark)
        {
            await ThunderCqrs.SendEvent(new UserBookChangeEvent(userBookmark, UserBookmarkEventChangeType.Deleted));

            await WriteRepository.DeleteAsync(userBookmark);
        }
    }
}
