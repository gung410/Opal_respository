using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.classrun.created")]
    public class ClassRunCreatedConsumer : ScopedOpalMessageConsumer<ClassRunChangeMessage>
    {
        private readonly IRepository<ClassRun> _classRunRepository;

        public ClassRunCreatedConsumer(IRepository<ClassRun> classRunRepository)
        {
            _classRunRepository = classRunRepository;
        }

        public async Task InternalHandleAsync(ClassRunChangeMessage message)
        {
            var anyExistingClassRun = await _classRunRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .AnyAsync();

            if (anyExistingClassRun)
            {
                // Implement Idempotent to avoid duplicate data come when the message can redeliver in RabbitMQ
                return;
            }

            var classRun = new ClassRun
            {
                Id = message.Id,
                ClassTitle = message.ClassTitle,
                ClassRunCode = message.ClassRunCode,
                ApplicationEndDate = message.ApplicationEndDate,
                ApplicationStartDate = message.ApplicationStartDate,
                FacilitatorIds = message.FacilitatorIds,
                CoFacilitatorIds = message.CoFacilitatorIds,
                CourseId = message.CourseId,
                EndDateTime = message.EndDateTime,
                StartDateTime = message.StartDateTime,
                PlanningStartTime = message.PlanningStartTime,
                PlanningEndTime = message.PlanningEndTime,
                MaxClassSize = message.MaxClassSize,
                MinClassSize = message.MinClassSize,
                CreatedBy = message.CreatedBy,
                CreatedDate = message.CreatedDate,
                Status = message.Status
            };

            await _classRunRepository.InsertAsync(classRun);
        }
    }
}
