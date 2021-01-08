using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Domain.Dtos.Users
{
    public class EmployeeDto : UserDtoBase
    {
        /// <summary>
        /// The current department that employee work at
        /// </summary>
        [Required]
        public int EmployerDepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public override int GetParentDepartmentId()
        {
            return EmployerDepartmentId;
        }

    }
}
