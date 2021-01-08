using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class GetMyCoursesSummaryRequestDto
    {
        public MyCourseStatus[] StatusFilter { get; set; }
    }
}
