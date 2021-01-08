using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Domain.Dtos.Users
{
    public class CandidateDto : UserDtoBase
    {
        /// <summary>
        /// Parent department that candidate belong to
        /// </summary>
        [Required]
        public int ParentDepartmentId { get; set; }
        public override int GetParentDepartmentId()
        {
            return ParentDepartmentId;
        }
    }
}
