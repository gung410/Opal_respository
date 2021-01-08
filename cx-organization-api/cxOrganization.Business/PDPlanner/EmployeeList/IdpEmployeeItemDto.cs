using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    /// <summary>
    /// Infomation of staff 
    /// </summary>
    public class IdpEmployeeItemDto
    {
        public IdentityDto Identity { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? LastActive { get; set; }
        public DepartmentInfo Department { get; set; }
        public string AvatarUrl { get; set; }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
        public string Email { get; set; }      

        public Dictionary<PDPlanActivity, AssessmentPDPlannerInfo> AssessmentInfos { get; set; }
        public List<UserTypeInfo> SystemRoleInfos { get; set; }
        public List<UserTypeInfo> RoleInfos { get; set; }
        public List<UserTypeInfo> PersonnelGroups { get; set; }
        public List<UserTypeInfo> DevelopmentalRoles { get; set; }
        public List<UserTypeInfo> CareerPaths { get; set; }
        public List<UserTypeInfo> ExperienceCategories { get; set; } 
        public List<UserTypeInfo> LearningFrameworks { get; set; }
        public List<UserGroupInfo> ApprovalGroups { get; set; }
        public List<UserGroupInfo> UserPools { get; set; }
        public List<UserGroupInfo> OtherUserGroups { get; set; }
        [Obsolete("This will be removed, please use ApprovalGroups, UserPools, OtherUserGroups")]
        public List<UserGroupInfo> UserGroupInfos { get; set; }

        public EntityStatusDto EntityStatus { get; set; }
        

    }
}