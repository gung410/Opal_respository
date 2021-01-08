using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class MyCoursesSummaryModel
    {
        public MyCoursesSummaryModel()
        {
        }

        public MyCoursesSummaryModel(
            MyCourseStatus status,
            int total = 0)
        {
            StatusFilter = status;
            Total = total;
        }

        public MyCourseStatus StatusFilter { get; set; }

        public int Total { get; set; }
    }
}
