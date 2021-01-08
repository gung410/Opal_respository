using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.assignment.created")]
    public class AssignmentCreatedConsumer : ScopedOpalMessageConsumer<AssignmentChangeMessage>
    {
        private readonly IRepository<Assignment> _assignmentRepository;

        public AssignmentCreatedConsumer(IRepository<Assignment> assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task InternalHandleAsync(AssignmentChangeMessage message)
        {
            var anyExistingAssignment = await _assignmentRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .AnyAsync();

            if (anyExistingAssignment)
            {
                return;
            }

            var assignment = new Assignment
            {
                Id = message.Id,
                CourseId = message.CourseId,
                ClassRunId = message.ClassRunId,
                Type = message.Type,
                Title = message.Title,
                CreatedBy = message.CreatedBy,
                CreatedDate = message.CreatedDate
            };

            await _assignmentRepository.InsertAsync(assignment);
        }
    }
}
