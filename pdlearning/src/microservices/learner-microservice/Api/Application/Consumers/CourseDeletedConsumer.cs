using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.deleted")]
    public class CourseDeletedConsumer : ScopedOpalMessageConsumer<CourseChangeMessage>
    {
        private readonly IRepository<Course> _courseRepository;

        public CourseDeletedConsumer(IRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task InternalHandleAsync(CourseChangeMessage message)
        {
            var existingCourse = await _courseRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .FirstOrDefaultAsync();

            if (existingCourse == null)
            {
                return;
            }

            await _courseRepository.DeleteAsync(existingCourse);
        }
    }
}
