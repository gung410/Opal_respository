namespace Microservice.StandaloneSurvey.Application.Consumers
{
    public class UserDepartmentChangeMessage
    {
        public int UserId { get; set; }

        public int DepartmentId { get; set; }
    }
}
