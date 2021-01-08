using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class FollowUnfollowedMessage
    {
        public int Id { get; set; }

        public TargetType TargetType { get; set; }
    }
}
