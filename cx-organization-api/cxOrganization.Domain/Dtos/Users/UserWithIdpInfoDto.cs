
using cxOrganization.Domain.Dtos.UserGroups;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using cxOrganization.Client.DepartmentTypes;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Dtos.Users
{
    public class UserWithIdpInfoDto 
    {
        public UserWithIdpInfoDto()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            CustomData = new Dictionary<string, object>(comparer);
        }

        public string DepartmentName { get; set; }
        public List<UserGroupDto> Groups { get; set; }
        public IDictionary<string, dynamic> JsonDynamicAttributes { get; set; }
        public int DepartmentId { get; set; }
        [JsonExtensionData]
        public Dictionary<string, object> CustomData { get; set; }

        // <summary>
        /// Password
        /// </summary>
        [MaxLength(64, ErrorMessage = "Password max length is 64")]
        public string Password { get; set; }
        /// <summary>
        /// First name 
        /// </summary>
        [MaxLength(64, ErrorMessage = "FirstName max length is 64")]
        public string FirstName { get; set; }
        /// <summary>
        /// Last name 
        /// </summary>
        [MaxLength(64, ErrorMessage = "LastName max length is 64")]
        public string LastName { get; set; }
        /// <summary>
        /// Numeric value of country code (example: 47)
        /// </summary>
        public int? MobileCountryCode { get; set; }

        /// <summary>
        /// Mobile number
        /// </summary>
        [MinLength(5, ErrorMessage = "MobileNumber min length is 5")]
        [MaxLength(15, ErrorMessage = "MobileNumber max length is 15")]
        public string MobileNumber { get; set; }

        /// <summary>
        /// Email address, do not need to check vaild format
        /// </summary>
        [MaxLength(256, ErrorMessage = "EmailAddress max length is 256")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Social Security number
        /// </summary>
        [MaxLength(64, ErrorMessage = "SSN max length is 64")]
        public string SSN { get; set; }

        /// <summary>
        /// List of examples of valid gender types:
        ///   0 = Male
        ///   1 = Female
        ///   2 = N/A
        /// </summary>
        public short? Gender { get; set; }
        /// <summary>
        /// Date of birth
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        [MaxLength(128, ErrorMessage = "Tag max length is 128")]
        public string Tag { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime? Created { get; set; }
        /// <summary>
        /// Get parent department id
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Gets or sets the Force User Login Again.
        /// </summary>
        /// <value>
        /// The Force User Login Again.
        /// </value>
        public bool? ForceLoginAgain { get; set; }

        /// <summary>
        /// Get or set list of roles users
        /// </summary>
        public EntityStatusDto EntityStatus { get; set; }
        public IdpIdentityDto Identity { get; set; }
        public List<DepartmentTypeDto> DepartmentTypes { get; set; }
        /// <summary>
        /// Gets or sets the ext Identifier.
        /// </summary>
        /// <value>The ext Identifier.</value>
        public string ExtId { get; set; }
    }
}
