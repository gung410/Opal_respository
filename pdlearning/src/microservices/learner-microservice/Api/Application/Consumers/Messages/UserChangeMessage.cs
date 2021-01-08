#pragma warning disable SA1402 // File may only contain a single type
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
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

        public UserIdentityInfo<Guid?> Identity { get; set; }

        public UserEntityStatus EntityStatus { get; set; }

        public List<PersonnelGroups> PersonnelGroups { get; set; }

        public List<Group> Groups { get; set; }
    }

    public class UserIdentityInfo<T>
    {
        [JsonConverter(typeof(NullableToIntConverter))]
        public int Id { get; set; }

        public T ExtId { get; set; }
    }

    public class PersonnelGroups
    {
        public UserIdentityInfo<Guid?> Identity { get; set; }
    }

    public class UserEntityStatus
    {
        public bool ExternallyMastered { get; set; }

        public LearnerUserStatus StatusId { get; set; }
    }

    public class Group
    {
        [JsonConverter(typeof(NullableToEnumConverter<LearnerUserGroupType>))]
        public LearnerUserGroupType Type { get; set; }

        public UserIdentityInfo<Guid?> UserIdentity { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
