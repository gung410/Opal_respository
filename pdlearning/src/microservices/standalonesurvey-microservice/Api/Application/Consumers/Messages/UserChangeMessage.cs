#pragma warning disable SA1402 // File may only contain a single type
using System;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    public class UserUpdatedMessage : UserChangeMessage
    {
    }

    public class UserCreatedMessage : UserChangeMessage
    {
    }

    public class UserChangeMessage
    {
        public int DepartmentId { get; set; }

        public UserInfo UserData { get; set; }
    }

    public class UserInfo
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string DepartmentName { get; set; }

        public UserIdentityInfo Identity { get; set; }
    }

    public class UserIdentityInfo
    {
        public int Id { get; set; }

        public Guid ExtId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
