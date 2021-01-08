using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.Interfaces;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    public class ECertificateTemplate : FullAuditedEntity, IHasDepartment, ISoftDelete, IFullTextSearchable
    {
        public string Title { get; set; }

        public ECertificateTemplateStatus Status { get; set; }

        public Guid ECertificateLayoutId { get; set; }

        public List<ECertificateTemplateParam> Params { get; set; } = new List<ECertificateTemplateParam>();

        public int DepartmentId { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public bool IsSystem { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// This column is a unique full-text search key column. It also support OrderBy. For now we order by CreatedDate.
        /// </summary>
        public string FullTextSearchKey
        {
            get => $"{Title?.TakeFirst(100).TrimAllSpaces() ?? string.Empty}_{Id.ToString().ToUpperInvariant()}";
            set { }
        }

        public string FullTextSearch
        {
            get => $"{Title ?? string.Empty}";
            set { }
        }

        public static Expression<Func<ECertificateTemplate, bool>> HasUpdatePermissionExpr(Guid? userId, List<string> userRoles)
        {
            return x => userId == null
                    || UserRoles.IsAdministrator(userRoles);
        }

        public static Expression<Func<ECertificateTemplate, bool>> HasOwnerPermissionExpr(Guid? userId)
        {
            return x => userId == null || x.CreatedBy == userId;
        }

        public static Expression<Func<ECertificateTemplate, bool>> HasViewECertificateTemplatePermissionQueryExpr(Guid? userId)
        {
            Expression<Func<ECertificateTemplate, bool>> expr1 = x => x.IsSystem == true;

            return expr1.Or(HasOwnerPermissionExpr(userId));
        }

        public static bool HasCreatePermission(Guid? userId, List<string> userRoles)
        {
            return userId == null || UserRoles.IsAdministrator(userRoles) || userRoles.Any(r => r == UserRoles.CourseContentCreator);
        }

        public bool HasOwnerPermission(Guid? userId, Func<ECertificateTemplate, bool> haveFullRight)
        {
            return HasOwnerPermissionExpr(userId).Compile()(this) || haveFullRight(this);
        }
    }
}
