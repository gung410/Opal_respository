using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    public class LearningPath : FullAuditedEntity, ISoftDelete
    {
        // Overview Information
        public string Title { get; set; }

        public LearningPathStatus Status { get; set; } = LearningPathStatus.Unpublished;

        public Guid CreatedBy { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool IsDeleted { get; set; }

        // Metadata
        public IEnumerable<string> CourseLevelIds { get; set; }

        public IEnumerable<string> PDAreaThemeIds { get; set; }

        public IEnumerable<string> ServiceSchemeIds { get; set; }

        public IEnumerable<string> SubjectAreaIds { get; set; }

        public IEnumerable<string> LearningFrameworkIds { get; set; }

        public IEnumerable<string> LearningDimensionIds { get; set; }

        public IEnumerable<string> LearningAreaIds { get; set; }

        public IEnumerable<string> LearningSubAreaIds { get; set; }

        public IEnumerable<string> MetadataKeys { get; set; }

        // Built-in expression
        // Permission on Course expression
        public static Expression<Func<LearningPath, bool>> HasOwnerPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null || UserRoles.IsSysAdministrator(userRoles) || x.CreatedBy == userId;
        }

        public static Expression<Func<LearningPath, bool>> IsEditableExpr()
        {
            return learningPath => learningPath.Status != LearningPathStatus.Published;
        }

        public static bool HasCreateEditPublishUnpublishPermission(Guid? userId, List<string> userRoles, Func<string, bool> checkHasPermissionFn)
        {
            return userId == null
                   || UserRoles.IsAdministrator(userRoles)
                   || (checkHasPermissionFn(LearningManagementPermissionKeys.CreateEditPublishUnpublishLP)
                       && userRoles.Any(r => r == UserRoles.CourseContentCreator));
        }

        public bool HasOwnerPermission(Guid? userId, IEnumerable<string> userRoles, Func<LearningPath, bool> haveFullRight)
        {
            return HasOwnerPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight(this);
        }

        public bool IsEditable()
        {
            return IsEditableExpr().Compile()(this);
        }
    }
}
