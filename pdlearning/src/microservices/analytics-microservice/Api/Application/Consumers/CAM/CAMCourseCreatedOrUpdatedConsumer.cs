using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.CAM.Mappers;
using Microservice.Analytics.Application.Consumers.CAM.Messages;
using Microservice.Analytics.Common.Helpers;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.CAM
{
    [OpalConsumer("microservice.events.course.created")]
    [OpalConsumer("microservice.events.course.updated")]
    public class CAMCourseCreatedOrUpdatedConsumer : ScopedOpalMessageConsumer<CAMCourseChangeMessage>
    {
        private readonly ILogger<CAMCourseCreatedOrUpdatedConsumer> _logger;
        private readonly IRepository<CAM_CourseHistory> _camCourseHistoryRepository;
        private readonly IRepository<CSL_Space> _cslSpaceRepository;

        public CAMCourseCreatedOrUpdatedConsumer(
            ILoggerFactory loggerFactory,
            IRepository<CAM_CourseHistory> camCourseHistoryRepository,
            IRepository<CSL_Space> cslSpaceRepository)
        {
            _logger = loggerFactory.CreateLogger<CAMCourseCreatedOrUpdatedConsumer>();
            _camCourseHistoryRepository = camCourseHistoryRepository;
            _cslSpaceRepository = cslSpaceRepository;
        }

        public async Task InternalHandleAsync(CAMCourseChangeMessage message)
        {
            var courseHistories = await _camCourseHistoryRepository
                .GetAllListAsync(t =>
                    t.CourseId == message.Id &&
                    t.ToDate == null);

            var now = Clock.Now;

            var numOfHistory = await _camCourseHistoryRepository.CountAsync(t => t.CourseId == message.Id);
            var cslSpace = await _cslSpaceRepository.FirstOrDefaultAsync(t => t.CoursesId.ToLower() == message.Id.ToString().ToLower());
            var newCourseHistory = message.MapToCAMCourseHistoryEntity(numOfHistory + 1, null, cslSpace?.Id);

            if (courseHistories.Any())
            {
                foreach (var item in courseHistories)
                {
                    item.ToDate = now;
                }

                await _camCourseHistoryRepository.UpdateManyAsync(courseHistories);
                var oldCourseHistory = courseHistories.OrderByDescending(x => x.CreatedDate).Last();

                if (CheckDifferentHelper.HasDifferent(oldCourseHistory, newCourseHistory, IgnoreFieldCheckDiffPredicate))
                {
                    await _camCourseHistoryRepository.InsertAsync(newCourseHistory);
                }
            }
            else
            {
                await _camCourseHistoryRepository.InsertAsync(newCourseHistory);
            }
        }

        private static bool IgnoreFieldCheckDiffPredicate(PropertyInfo x)
        {
            return x.Name != nameof(CAM_CourseHistory.CreatedDate) &&
                   x.Name != nameof(CAM_CourseHistory.ChangedDate) &&
                   x.Name != nameof(CAM_CourseHistory.FromDate) &&
                   x.Name != nameof(CAM_CourseHistory.ToDate) &&
                   x.Name != nameof(CAM_CourseHistory.No) &&
                   x.Name != nameof(CAM_CourseHistory.Id) &&
                   !typeof(IList).IsAssignableTo(x.PropertyType);
        }
    }
}
