using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.BusinessLogic
{
    public class WriteMyCourseLogic : BaseBusinessLogic<MyCourse>, IWriteMyCourseLogic
    {
        private readonly IReadOnlyRepository<UserReview> _readUserReviewRepository;

        public WriteMyCourseLogic(
            IThunderCqrs thunderCqrs,
            IWriteOnlyRepository<MyCourse> writeMyCourseRepository,
            IReadOnlyRepository<UserReview> readUserReviewRepository)
            : base(thunderCqrs, writeMyCourseRepository)
        {
            _readUserReviewRepository = readUserReviewRepository;
        }

        public async Task Insert(MyCourse myCourse)
        {
            await SendLearningMessage(myCourse);

            await WriteRepository.InsertAsync(myCourse);
        }

        public async Task Update(MyCourse myCourse)
        {
            await SendLearningMessage(myCourse);

            await WriteRepository.UpdateAsync(myCourse);
        }

        private async Task SendLearningMessage(MyCourse myCourse)
        {
            if (myCourse.IsMicroLearning())
            {
                var userReview = await _readUserReviewRepository
                    .GetAll()
                    .Where(p => p.UserId == myCourse.UserId)
                    .Where(p => p.ItemId == myCourse.CourseId)
                    .FirstOrDefaultAsync();

                // Support for report module
                await ThunderCqrs.SendEvent(new LearningRecordEvent(myCourse, userReview));
            }
            else if (myCourse.RegistrationId.HasValue)
            {
                var learningProcess = new LearningProcessModel
                {
                    LearningStatus = myCourse.Status,
                    ProgressMeasure = myCourse.ProgressMeasure,
                    RegistrationId = myCourse.RegistrationId.Value
                };

                await ThunderCqrs.SendEvent(new LearningProcessChangeEvent(learningProcess, LearningProcessType.Updated));
            }

            await ThunderCqrs.SendEvent(new MyCourseChangeEvent(myCourse, myCourse.Status));
        }
    }
}
