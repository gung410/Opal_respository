#pragma warning disable SA1402 // File may only contain a single type
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microservice.Course.Common;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Consumers
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

        public string MobileNumber { get; set; }

        public UserIdentityInfo<Guid?> Identity { get; set; }

        public UserEntityStatus EntityStatus { get; set; }

        public JsonDynamicAttribute JsonDynamicAttributes { get; set; }

        public List<CareerPath> CareerPaths { get; set; } = new List<CareerPath>();

        public List<DevelopmentalRole> DevelopmentalRoles { get; set; } = new List<DevelopmentalRole>();

        public List<LearningFramework> LearningFrameworks { get; set; } = new List<LearningFramework>();

        public List<PersonnelGroups> PersonnelGroups { get; set; } = new List<PersonnelGroups>();

        public List<Group> Groups { get; set; } = new List<Group>();

        public List<UserSystemRole> SystemRoles { get; set; } = new List<UserSystemRole>();
    }

    public class PersonnelGroups
    {
        public UserIdentityInfo<string> Identity { get; set; }
    }

    public class CareerPath
    {
        public UserIdentityInfo<string> Identity { get; set; }
    }

    public class DevelopmentalRole
    {
        public UserIdentityInfo<string> Identity { get; set; }
    }

    public class LearningFramework
    {
        public UserIdentityInfo<string> Identity { get; set; }
    }

    public class UserSystemRole
    {
        public UserIdentityInfo<string> Identity { get; set; }
    }

    public class JsonDynamicAttribute
    {
        public string Designation { get; set; }

        public List<string> TeachingLevels { get; set; } = new List<string>();

        public List<string> TeachingCourseOfStudy { get; set; } = new List<string>();

        public List<string> TeachingSubjects { get; set; } = new List<string>();

        public List<string> CocurricularActivities { get; set; } = new List<string>();

        public List<string> JobFamily { get; set; } = new List<string>();
    }

    public class UserIdentityInfo<T>
    {
        [JsonConverter(typeof(NullableToIntConverter))]
        public int Id { get; set; }

        public T ExtId { get; set; }
    }

    public class UserEntityStatus
    {
        public bool ExternallyMastered { get; set; }

        public CourseUserStatus StatusId { get; set; }
    }

    public class Group
    {
        [JsonConverter(typeof(NullableToEnumConverter<CourseUserGroupType>))]
        public CourseUserGroupType Type { get; set; }

        public UserIdentityInfo<Guid?> UserIdentity { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
