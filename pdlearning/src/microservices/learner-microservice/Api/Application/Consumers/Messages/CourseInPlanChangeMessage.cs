namespace Microservice.Learner.Application.Consumers
{
    public class CourseInPlanChangeMessage
    {
        public object Result { get; set; }

        public PlanInformation AdditionalInformation { get; set; }
    }
}
