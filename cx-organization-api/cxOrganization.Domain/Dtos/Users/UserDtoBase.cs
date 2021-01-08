using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;
using cxOrganization.Client.UserTypes;
using Newtonsoft.Json;

namespace cxOrganization.Domain.Dtos.Users
{
    /// <summary>
    /// Base clase for properties that are common for Learner, Emploee
    /// </summary>
    public abstract class UserDtoBase : ConexusBaseDto
    {
        protected UserDtoBase()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            CustomData = new Dictionary<string, object>(comparer);
        }

        /// <summary>
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
        public abstract int GetParentDepartmentId();

        /// <summary>
        /// Get or set list of login service claims of users
        /// </summary>
        public List<LoginServiceClaimDto> LoginServiceClaims { get; set; }

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
        public List<UserTypeDto> Roles { get; set; }

        [JsonExtensionData] public Dictionary<string, object> CustomData { get; set; }

        public string OtpValue { get; set; }

        public DateTime? OtpExpiration { get; set; }

        public bool? ResetOtp { get; set; }
        public IDictionary<string, dynamic> JsonDynamicAttributes { get; set; }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}".Trim();
        }

        public void AddOrUpdateJsonProperty(string propertyName, object value)
        {
            if (value == null || value.ToString() == string.Empty)
                return;
            if (JsonDynamicAttributes == null)
            {
                JsonDynamicAttributes = new Dictionary<string, object>();
            }

            if (JsonDynamicAttributes.ContainsKey(propertyName))
            {
                JsonDynamicAttributes[propertyName] = value;
            }
            else
            {
                JsonDynamicAttributes.Add(propertyName, value);
            }
        }

        public void AddJsonPropertyIfNotExisting(string propertyName, object value)
        {
            if (value == null || value.ToString() == string.Empty)
                return;
            if (JsonDynamicAttributes == null)
            {
                JsonDynamicAttributes = new Dictionary<string, object>();
            }

            if (!JsonDynamicAttributes.ContainsKey(propertyName))
            {
                JsonDynamicAttributes.Add(propertyName, value);
            }
        }

        public dynamic GetJsonPropertyValue(string propertyName)
        {
            if (JsonDynamicAttributes != null && JsonDynamicAttributes.TryGetValue(propertyName, out var value))
            {
                return value;
            }

            return null;
        }
    }
}
