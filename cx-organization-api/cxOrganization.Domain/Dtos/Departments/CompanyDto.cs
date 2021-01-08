using System;
using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Client.Departments
{
    [Serializable]
    public class CompanyDto : DepartmentDtoBase
    {
        /// <summary>
        /// 9 digits. Also known as Business Registration Number. Present on all business documents.
        /// </summary>
        [Required]
        [MaxLength(16, ErrorMessage = "OrganizationNumber max length is 16")]
        public string OrganizationNumber { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Postal Code
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// The name of City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Phone Number
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email Address
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}
