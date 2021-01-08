using cxOrganization.Client.UserTypes;
using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Dtos.Users
{
    /// <summary>
    /// This class holds the user info which is public to everyone. DON'T add any sensitive data into this class.
    /// </summary>
    public class PublicUserInfo
    {
        /// <summary>
        /// The full name of the user.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The CxId of the user.
        /// </summary>
        public string UserCxId { get; set; }

        /// <summary>
        /// The avatar url of the user.
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// The identifier of the department which the user belongs to.
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// The name of the department which the user belongs to.
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// The external identifier of the type of the organization/department of the user.
        /// </summary>
        public string TypeOfOrganization { get; set; }

        /// <summary>
        /// The address of the organization/department of the user.
        /// </summary>
        public string OrganizationAddress { get; set; }

        /// <summary>
        /// The designation of the user. It could be a plain text or a guid, if it is a guid then you should call the learning catalog api in order to get the display text.
        /// </summary>
        public string Designation { get; set; }

        /// <summary>
        /// The list of external identifiers of the portfolios of the user.
        /// </summary>
        public List<string> Portfolios { get; set; }

        /// <summary>
        /// The list of external identifiers of the role-specific proficiencies of the user.
        /// </summary>
        public List<string> RoleSpecificProficiencies { get; set; }

        /// <summary>
        /// The external identifier of the service scheme of the user.
        /// </summary>
        public string ServiceScheme { get; set; }

        /// <summary>
        /// The list of external identifiers of the teaching subjects of the user.
        /// </summary>
        public List<string> TeachingSubjects { get; set; }

        /// <summary>
        /// The list of external identifiers of the teaching levels of the user.
        /// </summary>
        public List<string> TeachingLevels { get; set; }

        /// <summary>
        /// The list of external identifiers of the job families of the user.
        /// </summary>
        public List<string> JobFamilies { get; set; }

        /// <summary>
        /// The list of external identifiers of teaching course of study of the user.
        /// </summary>
        public List<string> TeachingCourseOfStudy { get; set; }

        /// <summary>
        /// The list of external identifiers of co-curricular activities of the user.
        /// </summary>
        public List<string> CoCurricularActivities { get; set; }

        /// <summary>
        /// The list of external identifiers of the areas of professional interest of the user.
        /// </summary>
        public List<string> AreasOfProfessionalInterest { get; set; }

        /// <summary>
        /// The list of notification preferences of the user.
        /// </summary>
        public List<string> NotificationPreferences { get; set; }

        /// <summary>
        /// The last login date of the user.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// The DevelopmentRoles of the user.
        /// </summary>
        public List<string> DevelopmentRoles { get; set; }
    }
}
