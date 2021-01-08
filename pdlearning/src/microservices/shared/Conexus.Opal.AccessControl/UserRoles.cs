using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.AccessControl.Domain.Constants;
using Conexus.Opal.AccessControl.Domain.Models;
using Thunder.Platform.Core.Context;

namespace Thunder.Service.Authentication
{
#pragma warning disable SA1402 // File may only contain a single type

    /// <summary>
    /// We must define this mapping to convert SystemRole ExtId -> SystemRole Text because SAM API and Claim info isn"t consistent.
    /// Sample: ExtId: divisionadministrator -> Division Administrator.
    /// </summary>
    public static class SystemRole
    {
        public const string DivisionAdministrator = "divisionadmin";
        public const string BranchAdministrator = "branchadmin";
        public const string SchoolAdministrator = "schooladmin";
        public const string OPJApprovingOfficer = "approvingofficer";
        public const string CourseApprovingOfficer = "reportingofficer";
        public const string Learner = "learner";
        public const string SystemAdministrator = "overallsystemadministrator";
        public const string SchoolContentApprovingOfficer = "schoolcontentapprovingofficer";
        public const string MOEHQContentApprovingOfficer = "MOEHQcontentapprovingofficer";
        public const string WebPageEditor = "webpageeditor";
        public const string CoursePlanningCoordinator = "courseplanningcoordinator";
        public const string CourseContentCreator = "coursecontentcreator";
        public const string CourseAdministrator = "courseadmin";
        public const string CourseFacilitator = "coursefacilitator";
        public const string ContentCreator = "contentcreator";
        public const string SchoolStaffDeveloper = "schooltrainingcoordinator";
        public const string UserAccount = "useraccountadministrator";

        // NOTE: In SAM module, Divisional Learning Coordinator display text 'Divisional Learning Coordinator' but value display 'divisiontrainingcoordinator'.
        // So role Divisional Learning Coordinator will be 'DivisionalTrainingCoordinator' in CAM module.
        public const string DivisionTrainingCoordinator = "divisiontrainingcoordinator";
    }

    public static class UserRoles
    {
        public const string MOEHQContentApprovingOfficer = "MOEHQ Content Approving Officer";
        public const string CoursePlanningCoordinator = "Course Planning Coordinator";
        public const string CourseApprovingOfficer = "Course Approving Officer";
        public const string ContentCreator = "Content Creator";
        public const string SystemAdministrator = "System Administrator";
        public const string SchoolAdministrator = "School Administrator";
        public const string UserAccountAdministrator = "User Account Administrator";
        public const string CourseAdministrator = "Course Administrator";
        public const string SchoolContentApprovingOfficer = "School Content Approving Officer";
        public const string WebPageEditor = "Web Page Editor";
        public const string CourseContentCreator = "Course Content Creator";
        public const string OPJApprovingOfficer = "OPJ Approving Officer";
        public const string Learner = "Learner";
        public const string SchoolStaffDeveloper = "School Staff Developer";

        // NOTE: In SAM module, Divisional Learning Coordinator display text 'Divisional Learning Coordinator' but value display 'divisiontrainingcoordinator'.
        // So role Divisional Learning Coordinator will be 'DivisionalTrainingCoordinator' in CAM module.
        public const string DivisionalTrainingCoordinator = "Divisional Training Coordinator";
        public const string DivisionAdministrator = "Division Administrator";
        public const string BranchAdministrator = "Branch Administrator";
        public const string CourseFacilitator = "Course Facilitator";

        /// <summary>
        /// We must define this mapping to convert SystemRole ExtId -> SystemRole Text because SAM API and Claim information isn"t consistent.
        /// Sample: ExtId: divisionadministrator -> Division Administrator.
        /// </summary>
        public static Dictionary<string, string> SystemRoleMapping = new Dictionary<string, string>
        {
            {
                SystemRole.DivisionAdministrator,
                DivisionAdministrator
            },
            {
                SystemRole.BranchAdministrator,
                BranchAdministrator
            },
            {
                SystemRole.SchoolAdministrator,
                SchoolAdministrator
            },
            {
                SystemRole.OPJApprovingOfficer,
                OPJApprovingOfficer
            },
            {
                SystemRole.CourseApprovingOfficer,
                CourseApprovingOfficer
            },
            {
                SystemRole.Learner,
                Learner
            },
            {
                SystemRole.SystemAdministrator,
                SystemAdministrator
            },
            {
                SystemRole.SchoolContentApprovingOfficer,
                SchoolContentApprovingOfficer
            },
            {
                SystemRole.MOEHQContentApprovingOfficer,
                MOEHQContentApprovingOfficer
            },
            {
                SystemRole.WebPageEditor,
                WebPageEditor
            },
            {
                SystemRole.CoursePlanningCoordinator,
                CoursePlanningCoordinator
            },
            {
                SystemRole.CourseContentCreator,
                CourseContentCreator
            },
            {
                SystemRole.CourseAdministrator,
                CourseAdministrator
            },
            {
                SystemRole.CourseFacilitator,
                CourseFacilitator
            },
            {
                SystemRole.ContentCreator,
                ContentCreator
            },
            {
                SystemRole.SchoolStaffDeveloper,
                SchoolStaffDeveloper
            },
            {
                SystemRole.UserAccount,
                UserAccountAdministrator
            },
            {
                SystemRole.DivisionTrainingCoordinator,
                DivisionalTrainingCoordinator
            }
        };

        public static bool IsSysAdministrator(IEnumerable<string> userRoles)
        {
            return userRoles.Any(p => p == SystemAdministrator);
        }

        public static bool IsAdministrator(IEnumerable<string> userRoles)
        {
            return userRoles.Any(p => p == SystemAdministrator || p == DivisionAdministrator || p == BranchAdministrator || p == SchoolAdministrator);
        }
    }

    public static class UserContextExtensions
    {
        public static bool IsInRole(this IUserContext userContext, string role)
        {
            return userContext.GetRoles().Contains(role);
        }

        public static IEnumerable<string> GetRoles(this IUserContext userContext)
        {
            var roles = userContext.GetValue<List<string>>(CommonUserContextKeys.UserRoles);
            return roles ?? new List<string>();
        }

        public static IEnumerable<ModulePermission> GetPermissions(this IUserContext userContext)
        {
            var permissions = userContext.GetValue<List<ModulePermission>>(CommonUserContextKeys.UserPermissions);
            return permissions ?? new List<ModulePermission>();
        }

        public static bool IsAdministrator(this IUserContext userContext)
        {
            return UserRoles.IsAdministrator(userContext.GetRoles());
        }

        public static bool IsSysAdministrator(this IUserContext userContext)
        {
            return UserRoles.IsSysAdministrator(userContext.GetRoles());
        }

        public static bool IsSystem(this IUserContext userContext)
        {
            var userId = userContext.GetValue<string>(CommonUserContextKeys.UserId);
            return string.IsNullOrEmpty(userId);
        }

        public static bool HasPermission(this IUserContext userContext, params string[] permissionActionKeys)
        {
            if (userContext.IsSystem())
            {
                return true;
            }

            var permissionsDic = userContext.GetValue<Dictionary<string, ModulePermission>>(CommonUserContextKeys.UserPermissionsDic);
            return permissionsDic != null &&
                   permissionActionKeys.Any(permissionAction =>
                       permissionsDic.ContainsKey(permissionAction) &&
                       permissionsDic[permissionAction].GrantedType == PermissionGrantedType.Allow);
        }

        public static bool HasPermissionPrefix(this IUserContext userContext, params string[] permissionActionKeys)
        {
            if (userContext.IsSystem())
            {
                return true;
            }

            var permissions = userContext.GetValue<List<ModulePermission>>(CommonUserContextKeys.UserPermissions);
            return permissions != null &&
                   (permissionActionKeys.Any(permissionAction => HasPermission(userContext, permissionAction)) ||
                   permissionActionKeys.Any(permissionAction => permissions.Any(permission => permission.Action.StartsWith(permissionAction))));
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
