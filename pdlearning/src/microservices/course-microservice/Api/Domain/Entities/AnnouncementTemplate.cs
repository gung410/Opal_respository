using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microservice.Course.Domain.Interfaces;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    public class AnnouncementTemplate : FullAuditedEntity, ISoftDelete, IFullTextSearchable
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsDeleted { get; set; }

        // Technical Columns

        /// <summary>
        /// This column to support search by text function in UI. This is a computed data column from Title.
        /// </summary>
        public string FullTextSearch
        {
            get => $"{Title ?? string.Empty}";
            set { }
        }

        /// <summary>
        /// This column is a unique full-text search key column. It also support OrderBy. For now we order by CreatedDate.
        /// </summary>
        public string FullTextSearchKey
        {
            get => $"{CreatedDate:yyyy-MM-dd hh:mm:ss.fffffff}_{Id.ToString().ToUpperInvariant()}";
            set { }
        }

        public static Expression<Func<AnnouncementTemplate, bool>> HasOwnerPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null || UserRoles.IsSysAdministrator(userRoles) || x.CreatedBy == userId;
        }

        public static bool HasCreatePermission(Guid? userId, List<string> userRoles)
        {
            return userId == null
                   || UserRoles.IsAdministrator(userRoles)
                   || userRoles.Any(r => r == UserRoles.CourseContentCreator || r == UserRoles.CourseFacilitator);
        }

        public bool HasOwnerPermission(Guid? userId, IEnumerable<string> userRoles)
        {
            return HasOwnerPermissionExpr(userId, userRoles).Compile()(this);
        }
    }
}
