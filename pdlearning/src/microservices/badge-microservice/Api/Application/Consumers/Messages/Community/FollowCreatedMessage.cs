using Microservice.Badge.Application.Consumers.Dtos;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class FollowCreatedMessage
    {
        public int Id { get; set; }

        public UserDto User { get; set; }

        public TargetDto Target { get; set; }

        public TargetType TargetType { get; set; }
    }
}
