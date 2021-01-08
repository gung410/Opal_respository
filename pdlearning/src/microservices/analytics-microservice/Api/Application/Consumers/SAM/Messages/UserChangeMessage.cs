using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microservice.Analytics.Application.Common.Converters;
using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.SAM.Messages
{
#pragma warning disable SA1402 // File may only contain a single type
    public class UserChangeMessage
    {
        public int DepartmentId { get; set; }

        public int DepartmentArcheTypeId { get; set; }

        public UserInfo UserData { get; set; }
    }

    public class UserInfo
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public int MobileCountryCode { get; set; }

        public short Gender { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int DepartmentId { get; set; }

        public DateTime? Created { get; set; }

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
        public UserIdentityInfo<Guid> Identity { get; set; }
    }

    public class CareerPath
    {
        public UserIdentityInfo<string> Identity { get; set; }
    }

    public class DevelopmentalRole
    {
        public UserIdentityInfo<Guid> Identity { get; set; }
    }

    public class LearningFramework
    {
        public UserIdentityInfo<string> Identity { get; set; }
    }

    public class UserSystemRole
    {
        public UserIdentityInfo<string> Identity { get; set; }

        public List<LocalizedData> LocalizedData { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }

        public string LocalizedText { get; set; }
    }

    public class LocalizedData
    {
        public int Id { get; set; }

        public string LanguageCode { get; set; }

        public List<Field> Fields { get; set; }
    }

    public class JsonDynamicAttribute
    {
        public string Designation { get; set; }

        public List<string> TeachingLevels { get; set; } = new List<string>();

        public List<string> TeachingCourseOfStudy { get; set; } = new List<string>();

        public List<string> TeachingSubjects { get; set; } = new List<string>();

        public List<string> CocurricularActivities { get; set; } = new List<string>();

        public List<string> JobFamily { get; set; } = new List<string>();

        public bool FinishOnBoarding { get; set; }

        public DateTime? FinishedOnboardingDate { get; set; }

        public DateTime? StartedOnboardingDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? FirstLoginDate { get; set; }

        public bool? HoldsSupervisoryRole { get; set; }

        public bool? IdpLocked { get; set; }

        public bool? SentWelcomeEmail { get; set; }

        public DateTime? SentWelcomeEmailDate { get; set; }

        public int? LastEntityStatusId { get; set; }

        public AnalyticSAMNotificationType[] NotificationPreference { get; set; } = Array.Empty<AnalyticSAMNotificationType>();

        public DateTime? DateAppointedToService { get; set; }

        public DateTime? DateToCurrentScheme { get; set; }

        public DateTime? DateAppointedToTrainedGrade { get; set; }
    }

    public class UserIdentityInfo<T>
    {
        [JsonConverter(typeof(NullableToIntConverter))]
        public int Id { get; set; }

        public T ExtId { get; set; }

        [JsonConverter(typeof(NullableToEnumConverter<AnalyticSAMArchetype>))]
        public AnalyticSAMArchetype Archetype { get; set; }
    }

    public class UserEntityStatus
    {
        public bool ExternallyMastered { get; set; }

        public AnalyticSAMStatus StatusId { get; set; }

        public AnalyticEntityStatusReason StatusReasonId { get; set; }

        public bool Deleted { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? LastUpdated { get; set; }
    }

    public class Group
    {
        [JsonConverter(typeof(NullableToEnumConverter<AnalyticSAMUserGroupType>))]
        public AnalyticSAMUserGroupType Type { get; set; }

        public UserIdentityInfo<Guid?> UserIdentity { get; set; }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
