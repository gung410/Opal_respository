using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Client.UserGroups
{
    public class TeachingGroupDto : UserGroupDtoBase
    {
        /// <summary>
        /// The subject code of the teaching group, can be a list of subject code, seperated by ","
        /// </summary>
        [Required(ErrorMessage = "The subject code is required.")]
        public string SubjectCode { get; set; }
        public string SubjectUiid { get; set; }
        /// <summary>
        /// The id of school which teaching group belong to
        /// </summary>
        [Required]
        public int? SchoolId { get; set; }

        public override int? GetParentDepartmentId()
        {
            return SchoolId;
        }

        public override int? GetParentUserId()
        {
            return null;
        }
        public override void SetParentDepartmentId(int? parentDepartmentId)
        {
            SchoolId = parentDepartmentId;
        }
    }
}
