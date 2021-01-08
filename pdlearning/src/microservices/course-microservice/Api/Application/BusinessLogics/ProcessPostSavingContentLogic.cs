using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ProcessPostSavingContentLogic : BaseBusinessLogic
    {
        private readonly CourseCudLogic _courseCudLogic;
        private readonly ClassRunCudLogic _classRunCudLogic;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public ProcessPostSavingContentLogic(
            CourseCudLogic courseCudLogic,
            ClassRunCudLogic classRunCudLogic,
            IReadOnlyRepository<Session> readSessionRepository,
            IUserContext userContext) : base(userContext)
        {
            _courseCudLogic = courseCudLogic;
            _classRunCudLogic = classRunCudLogic;
            _readSessionRepository = readSessionRepository;
        }

        public async Task Execute(CourseEntity course, ClassRun classRun, Guid userId, CancellationToken cancellationToken)
        {
            if (classRun != null)
            {
                classRun.ChangedDate = Clock.Now;
                classRun.ChangedBy = userId;
                classRun.ContentStatus = ContentStatus.Draft;

                var sessions = await _readSessionRepository.GetAll().Where(p => p.ClassRunId == classRun.Id).ToListAsync(cancellationToken);
                await _classRunCudLogic.Update(ClassRunAggregatedEntityModel.Create(classRun, course, sessions), cancellationToken);
            }
            else
            {
                course.ChangedDate = Clock.Now;
                course.ChangedBy = userId;
                course.ContentStatus = ContentStatus.Draft;
                await _courseCudLogic.Update(course, cancellationToken);
            }
        }
    }
}
