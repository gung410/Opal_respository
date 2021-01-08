using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendClassRunNotificationLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public SendClassRunNotificationLogic(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(userContext)
        {
            _readCourseRepository = readCourseRepository;
            _thunderCqrs = thunderCqrs;
        }

        public async Task ByClassRuns<TMailEvent>(
            List<ClassRun> classRuns,
            Func<ClassRun, CourseEntity, TMailEvent> emailEventFn,
            CancellationToken cancellationToken = default)
            where TMailEvent : BaseThunderEvent
        {
            var courseIds = classRuns.Select(x => x.CourseId);
            var courseDict = await _readCourseRepository.GetDictionaryByIdsAsync(courseIds);

            if (emailEventFn != null)
            {
                await _thunderCqrs.SendEvents(
                    classRuns.Select(p => emailEventFn(p, courseDict[p.CourseId])),
                    cancellationToken);
            }
        }
    }
}
