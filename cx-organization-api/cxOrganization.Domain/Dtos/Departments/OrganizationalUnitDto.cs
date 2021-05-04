using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Client.Departments
{
    [Serializable]
    public class OrganizationalUnitDto : DepartmentDtoBase
    {
        /// <summary>
        /// 9 digits. Also known as Business Registration Number. Present on all business documents.
        /// </summary>
        [StringLength(4, ErrorMessage = "School code cannot exceed 4 numbers. ")]
        public string OrganizationNumber { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        [StringLength(500, ErrorMessage = "Address value cannot exceed 500 characters. ")]
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
