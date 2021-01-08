using System;

namespace cxOrganization.Client.Departments
{
    [Serializable]
    public class DataOwnerDto : DepartmentDtoBase
    {
        /// <summary>
        /// 9 digits. Also known as Business Registration Number. Present on all business documents.
        /// </summary>
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
}
