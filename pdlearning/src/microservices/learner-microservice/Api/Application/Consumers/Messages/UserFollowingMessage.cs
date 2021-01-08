namespace Microservice.Learner.Application.Consumers
{
    public class UserFollowingMessage
    {
        public UserMessage User { get; set; }

        public UserMessage Target { get; set; }

        public string TargetType { get; set; }
    }
}
