using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.CAM.Mappers;
using Microservice.Analytics.Application.Consumers.CAM.Messages;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.CAM
{
    [OpalConsumer("microservice.events.course.deleted")]
    public class CAMCourseDeletedConsumer : ScopedOpalMessageConsumer<CAMCourseChangeMessage>
    {
        private readonly ILogger<CAMCourseDeletedConsumer> _logger;
        private readonly IRepository<CAM_CourseHistory> _camCourseHistoryRepository;

        public CAMCourseDeletedConsumer(ILoggerFactory loggerFactory, IRepository<CAM_CourseHistory> camCourseHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<CAMCourseDeletedConsumer>();
            _camCourseHistoryRepository = camCourseHistoryRepository;
        }

        public async Task InternalHandleAsync(CAMCourseChangeMessage message)
        {
            var courseHistories = await _camCourseHistoryRepository.GetAllListAsync(t => t.CourseId == message.Id && t.ToDate == null);

            var now = Clock.Now;

            foreach (var item in courseHistories)
            {
                item.ToDate = now;
            }

            await _camCourseHistoryRepository.UpdateManyAsync(courseHistories);

            var numOfHistory = await _camCourseHistoryRepository.CountAsync(t => t.CourseId == message.Id);

            await _camCourseHistoryRepository.InsertAsync(message.MapToCAMCourseHistoryEntity(numOfHistory + 1, now));
        }
    }
}
