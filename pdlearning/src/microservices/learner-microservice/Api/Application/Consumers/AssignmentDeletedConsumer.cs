using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.assignment.deleted")]
    public class AssignmentDeletedConsumer : ScopedOpalMessageConsumer<AssignmentChangeMessage>
    {
        private readonly IRepository<Assignment> _assignmentRepository;

        public AssignmentDeletedConsumer(IRepository<Assignment> assignmentRepository)
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

            await _assignmentRepository.DeleteAsync(assignment);
        }
    }
}
