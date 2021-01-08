using System;
using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Client.Departments
{
    [Serializable]
    public class SchoolOwnerDto : DepartmentDtoBase
    {
        /// <summary>
        /// Insert a SchoolOwner we need to specify DepartmentType as County or Municipality:
        /// 3 types are avaiable:
        /// 1. Unknown
        /// 2. County
        /// 3. Municipality
        /// </summary>
        public DepartmentTypeEnum DepartmentType { get; set; }

        /// <summary>
        /// 9 digits. Also known as Business Registration Number. Present on all business documents.
        /// </summary>
        [Required]
        [MaxLength(16, ErrorMessage = "OrganizationNumber max length is 16")]
        public string OrganizationNumber { get; set; }
        /// <summary>
        /// Schoolowner Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Schoolowner postal code
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// The name of City
        /// </summary>
        public string City { get; set; }
    }

    public enum DepartmentTypeEnum
    {
        Unknown = 0,
        County = 1,
        Municipality = 2
    }
}
