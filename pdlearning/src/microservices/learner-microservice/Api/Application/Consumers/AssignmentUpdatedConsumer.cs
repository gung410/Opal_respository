using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.assignment.updated")]
    public class AssignmentUpdatedConsumer : ScopedOpalMessageConsumer<AssignmentChangeMessage>
    {
        private readonly IRepository<Assignment> _assignmentRepository;

        public AssignmentUpdatedConsumer(
            IRepository<Assignment> assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task InternalHandleAsync(AssignmentChangeMessage message)
        {
            var assignment = await _assignmentRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .FirstOrDefaultAsync();

            if (assignment == null)
            {
                return;
            }

            assignment.Update(
                message.CourseId,
                message.ClassRunId,
                message.Title,
                message.Type,
                message.ChangedDate,
                message.ChangedBy);

            await _assignmentRepository.UpdateAsync(assignment);
        }
    }
}
