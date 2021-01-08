using System.Collections.Generic;

namespace cxOrganization.Client.Account
{
    public class UserDto : AccountBaseDto
    {
        public UserDto()
        {
            //Set default Gender
            Gender = -1;
            UserGroups = new List<UserGroup>();
            UserTypes = new List<UserType>();
            PropValues = new List<PropValue>();
        }
        /// <summary>
        /// The user identifier in database
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// The last name of user
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// The first name of user
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// The mobile number of user
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// The mobile country code of the user (ex: 47)
        /// </summary>
        public int MobileCountryCode { get; set; }
        /// <summary>
        /// The username of user
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The password of user
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// List of examples of languages:
        /// 1	nb-NO
        /// 2	en-US
        /// 3	DE
        /// 4	SA
        /// 5	AR
        /// 6	nn-NO
        /// 10	da-DK
        /// </summary>
        public int LanguageId { get; set; }
        /// <summary>
        /// List of examples of valid gender types:
        ///   0 = Male
        ///   1 = Female
        ///   2 = N/A
        /// </summary>
        public short Gender { get; set; }
        /// <summary>
        /// Social Security number
        /// </summary>
        public string Ssn { get; set; }
        /// <summary>
        /// The email of user
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The department id that user belong to
        /// </summary>
        public int DepartmentId { get; set; }
        public DepartmentExternalIdentityDto DepartmentExternal { get; set; }
        public string ExternalId { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public List<UserType> UserTypes { get; set; }
        public List<PropValue> PropValues { get; set; }
    }

    public class DepartmentExternalIdentityDto
    {
        public string DepartmentExtId { get; set; }
        public string CustomerExtId { get; set; }
    }

    public class UserType
    {
        public int UserTypeId { get; set; }
        public string ExtId { get; set; }
        public string UserTypeName { get; set; }
    }

    public class PropValue
    {
        public int PropertyId { get; set; }
        public string Value { get; set; }
    }

    public class UserGroup
    {
        public int UserGroupId { get; set; }
        public string ExtId { get; set; }
        public string UserGroupName { get; set; }
    }
}
