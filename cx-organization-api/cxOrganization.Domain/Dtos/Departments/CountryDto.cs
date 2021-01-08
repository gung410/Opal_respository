using System;
using System.ComponentModel.DataAnnotations;


namespace cxOrganization.Client.Departments
{
    [Serializable]
    public class CountryDto : DepartmentDtoBase
    {
        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Postal Code
        /// </summary>
        public string PostalCode { get; set; }
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
