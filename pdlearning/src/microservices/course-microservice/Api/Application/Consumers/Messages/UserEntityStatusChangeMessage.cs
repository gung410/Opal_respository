#pragma warning disable SA1402 // File may only contain a single type
using System;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Consumers
{
    public class UserEntityStatusChangeMessage
    {
        public UserData UserData { get; set; }

        public int UserId { get; set; }

        public Guid UserCxId { get; set; }
    }

    public class UserData
    {
        public CourseUserStatus ToEntityStatusId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
