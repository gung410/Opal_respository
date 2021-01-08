using Microservice.Calendar.Application.Consumers.Messages.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class UserGroupItem
    {
        public UserIdentityModel UserIdentity { get; set; }

        public UserGroupType Type { get; set; }
    }
}
