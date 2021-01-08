using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.Interfaces;

namespace Microservice.Course.Domain.Entities
{
    public class CourseUser : UserEntity, IFullTextSearchable
    {
        private IEnumerable<string> _track = new List<string>();
        private IEnumerable<string> _systemRoles = new List<string>();
        private IEnumerable<string> _teachingLevel = new List<string>();
        private IEnumerable<string> _teachingCourseOfStudy = new List<string>();
        private IEnumerable<string> _teachingSubject = new List<string>();
        private IEnumerable<string> _cocurricularActivity = new List<string>();
        private IEnumerable<string> _developmentalRole = new List<string>();
        private IEnumerable<string> _jobFamily = new List<string>();
        private IEnumerable<string> _easSubstantiveGradeBanding = new List<string>();
        private IEnumerable<string> _serviceScheme = new List<string>();
        private IEnumerable<string> _designation = new List<string>();
        private IEnumerable<string> _learningFramework = new List<string>();

        public CourseUserStatus Status { get; set; }

        public CourseUserAccountType AccountType { get; set; }

        public Guid PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserMetadata> UserMetadatas { get; set; } = new List<UserMetadata>();

        [JsonIgnore]
        public virtual ICollection<UserSystemRole> UserSystemRoles { get; set; } = new List<UserSystemRole>();

        // Technical Columns

        /// <summary>
        /// This column to support search by text function in UI. This is a computed data column from FirstName, LastName, Email.
        /// </summary>
        public string FullTextSearch
        {
            get => $"{FirstName ?? string.Empty}  {LastName ?? string.Empty}  {Email ?? string.Empty}";
            set { }
        }

        /// <summary>
        /// This column is a unique full-text search key column. It also support OrderBy. For now we order by Email.
        /// </summary>
        public string FullTextSearchKey
        {
            get => $"{Email?.ToString(CultureInfo.InvariantCulture) ?? string.Empty}_{Id}";
            set { }
        }

        public IEnumerable<string> Track
        {
            get => _track ?? new List<string>();

            set
            {
                _track = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.Track);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.Track, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> SystemRoles
        {
            get => _systemRoles ?? new List<string>();

            set
            {
                _systemRoles = value;
                UserSystemRoles.RemoveAll(x => true);
                if (value != null)
                {
                    UserSystemRoles.AddRange(value
                    .Select(p => UserSystemRole.Create(Id, p))
                    .ToList());
                }
            }
        }

        public IEnumerable<string> TeachingLevel
        {
            get => _teachingLevel ?? new List<string>();

            set
            {
                _teachingLevel = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.TeachingLevel);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.TeachingLevel, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> TeachingCourseOfStudy
        {
            get => _teachingCourseOfStudy ?? new List<string>();

            set
            {
                _teachingCourseOfStudy = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.TeachingCourseOfStudy);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.TeachingCourseOfStudy, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> TeachingSubject
        {
            get => _teachingSubject ?? new List<string>();

            set
            {
                _teachingSubject = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.TeachingSubject);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.TeachingSubject, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> CocurricularActivity
        {
            get => _cocurricularActivity ?? new List<string>();

            set
            {
                _cocurricularActivity = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.CocurricularActivity);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.CocurricularActivity, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> DevelopmentalRole
        {
            get => _developmentalRole ?? new List<string>();

            set
            {
                _developmentalRole = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.DevelopmentalRole);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.DevelopmentalRole, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> JobFamily
        {
            get => _jobFamily ?? new List<string>();

            set
            {
                _jobFamily = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.JobFamily);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.JobFamily, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> EasSubstantiveGradeBanding
        {
            get => _easSubstantiveGradeBanding ?? new List<string>();

            set
            {
                _easSubstantiveGradeBanding = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.EasSubstantiveGradeBanding);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.EasSubstantiveGradeBanding, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> ServiceScheme
        {
            get => _serviceScheme ?? new List<string>();

            set
            {
                _serviceScheme = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.ServiceScheme);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.ServiceScheme, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> Designation
        {
            get => _designation ?? new List<string>();

            set
            {
                _designation = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.Designation);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.Designation, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> LearningFramework
        {
            get => _learningFramework ?? new List<string>();

            set
            {
                _learningFramework = value;
                UserMetadatas.RemoveAll(p => p.Type == UserMetadataValueType.LearningFramework);
                if (value != null)
                {
                    UserMetadatas.AddRange(value
                        .Select(p => UserMetadata.Create(Id, UserMetadataValueType.LearningFramework, p))
                        .ToList());
                }
            }
        }

        /// <summary>
        /// ONLY allow to use in memory, not use Queryable.
        /// </summary>
        [NotMapped]
        public List<Guid> ApprovingOfficerIds => F.List(PrimaryApprovingOfficerId)
            .Concat(AlternativeApprovingOfficerId.HasValue
                ? F.List(AlternativeApprovingOfficerId.Value)
                : new List<Guid>()).ToList();

        /// <summary>
        /// This column to support filter equivalent to ServiceScheme.Contain([some user id]) by using full-text search.
        /// </summary>
        public string ServiceSchemeFullTextSearch
        {
            get => ServiceScheme != null && ServiceScheme.Any() ? JsonSerializer.Serialize(ServiceScheme) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to TeachingCourseOfStudy.Contain([some user id]) by using full-text search.
        /// </summary>
        public string TeachingCourseOfStudyFullTextSearch
        {
            get => TeachingCourseOfStudy != null && TeachingCourseOfStudy.Any() ? JsonSerializer.Serialize(TeachingCourseOfStudy) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to Designation.Contain([some user id]) by using full-text search.
        /// </summary>
        public string DesignationFullTextSearch
        {
            get => Designation != null && Designation.Any() ? JsonSerializer.Serialize(Designation) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to DevelopmentalRole.Contain([some user id]) by using full-text search.
        /// </summary>
        public string DevelopmentalRoleFullTextSearch
        {
            get => DevelopmentalRole != null && DevelopmentalRole.Any() ? JsonSerializer.Serialize(DevelopmentalRole) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to TeachingLevel.Contain([some user id]) by using full-text search.
        /// </summary>
        public string TeachingLevelFullTextSearch
        {
            get => TeachingLevel != null && TeachingLevel.Any() ? JsonSerializer.Serialize(TeachingLevel) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to TeachingSubject.Contain([some user id]) by using full-text search.
        /// </summary>
        public string TeachingSubjectFullTextSearch
        {
            get => TeachingSubject != null && TeachingSubject.Any() ? JsonSerializer.Serialize(TeachingSubject) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to LearningFramework.Contain([some user id]) by using full-text search.
        /// </summary>
        public string LearningFrameworkFullTextSearch
        {
            get => LearningFramework != null && LearningFramework.Any() ? JsonSerializer.Serialize(LearningFramework) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to SystemRoles.Contain([some user id]) by using full-text search.
        /// </summary>
        public string SystemRolesFullTextSearch
        {
            get => SystemRoles != null && SystemRoles.Any() ? JsonSerializer.Serialize(SystemRoles) : null;
            set { }
        }

        public static Expression<Func<CourseUser, bool>> HasRoleExpr(List<string> userRoles)
        {
            return p => p.UserSystemRoles.Any(_ => userRoles.Contains(_.Value));
        }

        public static Expression<Func<CourseUser, bool>> IsUserApplicableInCourseMetaDataExpr(CourseEntity course)
        {
            return p => (!course.TrackIds.Any() || p.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.Track && course.TrackIds.Contains(_.Value.ToString()))) &&
                        (!course.DevelopmentalRoleIds.Any() || p.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.DevelopmentalRole && course.DevelopmentalRoleIds.Contains(_.Value.ToString()))) &&
                        (!course.TeachingLevels.Any() || p.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.TeachingLevel && course.TeachingLevels.Contains(_.Value.ToString()))) &&
                        (!course.TeachingSubjectIds.Any() || p.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.TeachingSubject && course.TeachingSubjectIds.Contains(_.Value.ToString()))) &&
                        (!course.TeachingCourseStudyIds.Any() || p.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.TeachingCourseOfStudy && course.TeachingCourseStudyIds.Contains(_.Value.ToString()))) &&
                        (!course.CocurricularActivityIds.Any() || p.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.CocurricularActivity && course.CocurricularActivityIds.Contains(_.Value.ToString()))) &&
                        (!course.JobFamily.Any() || p.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.JobFamily && course.JobFamily.Contains(_.Value.ToString()))) &&
                        (!course.EasSubstantiveGradeBandingIds.Any() || p.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.EasSubstantiveGradeBanding && course.EasSubstantiveGradeBandingIds.Contains(_.Value.ToString())));
        }

        public static Expression<Func<CourseUser, bool>> IsUserApplicableInCourseOrganisationExpr(CourseEntity course)
        {
            return user => course.ApplicableDivisionIds.Contains(user.DepartmentId) ||
                        course.ApplicableBranchIds.Contains(user.DepartmentId) ||
                        course.ApplicableZoneIds.Contains(user.DepartmentId) ||
                        course.ApplicableClusterIds.Contains(user.DepartmentId) ||
                        course.ApplicableSchoolIds.Contains(user.DepartmentId);
        }

        public bool HasRole(List<string> userRoles)
        {
            return HasRoleExpr(userRoles).Compile()(this);
        }

        public bool HasMetadata(UserMetadataValueType type, params string[] metaDataIds)
        {
            return UserMetadatas.Any(p => p.Type == type && metaDataIds.Contains(p.Value));
        }
    }
}
