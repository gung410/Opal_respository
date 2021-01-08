using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetListNewCourseQuery : BaseThunderQuery<PagedResultDto<CourseModel>>
    {
        public PagedResultRequestDto PageInfo { get; set; }
    }
}
