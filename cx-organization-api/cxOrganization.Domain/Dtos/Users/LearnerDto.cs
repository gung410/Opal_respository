using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Domain.Dtos.Users
{
    public class LearnerDto : UserDtoBase
    {
        /// <summary>
        /// Parent department that learner belong to
        /// </summary>
        [Required]
        public int ParentDepartmentId { get; set; }

        public override int GetParentDepartmentId()
        {
            return ParentDepartmentId;
        }
    }
}
