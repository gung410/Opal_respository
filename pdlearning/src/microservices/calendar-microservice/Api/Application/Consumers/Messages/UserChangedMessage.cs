#pragma warning disable SA1402 // File may only contain a single type
using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Consumers.Messages.Models;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Consumers
{
    public class UserChangedMessage
     {
        public int DepartmentId { get; set; }

        public UserInfo UserData { get; set; }
     }

    public class UserInfo
     {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public UserIdentityInfo Identity { get; set; }

        public List<UserGroupItem> Groups { get; set; }

        public UserEntityStatus EntityStatus { get; set; }
    }

    public class UserIdentityInfo
     {
        public int Id { get; set; }

        public Guid ExtId { get; set; }
     }

    public class UserEntityStatus
    {
        public UserStatus StatusId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
