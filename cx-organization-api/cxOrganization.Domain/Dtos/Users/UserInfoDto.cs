using System.Collections.Generic;
using Newtonsoft.Json;

namespace cxOrganization.Domain.Dtos.Users
{
    public class UserInfoDto
    {
        public UserInfo BasicInfo { get; set; }
        public List<string> TagIds { get; set; }
        public Tags TagGroups { get; set; }
        public TagPriorityGroup TagIdsGroupByPriority { get; set; }
    }
    public class Tags
    {
        [JsonProperty("designation")]
        public string Designation { get; set; }
        [JsonProperty("portfolio")]
        public List<string> Portfolio { get; set; }
        [JsonProperty("teachingCourseOfStudy")]
        public List<string> TeachingCourseOfStudy { get; set; }
        [JsonProperty("teachingLevels")]
        public List<string> TeachingLevels { get; set; }
        [JsonProperty("teachingSubjects")]
        public List<string> TeachingSubjects { get; set; }
        [JsonProperty("cocurricularActivities")]
        public List<string> CocurricularActivities { get; set; }
        [JsonProperty("learningFrameworks")]
        public List<string> LearningFrameworks { get; set; }
        [JsonProperty("serviceSchemes")]
        public List<string> ServiceSchemes { get; set; }
        [JsonProperty("professionalInterestArea")]
        public List<string> ProfessionalInterests { get; set; }
        [JsonProperty("developmentalRoles")]
        public List<string> DevelopmentalRoles { get; set; }
        [JsonProperty("tracks")]
        public List<string> Tracks { get; set; }
        [JsonProperty("jobFamily")]
        public List<string> JobFamilies { get; set; }
    }
    public class UserInfo : UserDtoBase
    {
        public int DepartmentId { get; set; }
        public override int GetParentDepartmentId()
        {
            return DepartmentId;
        }
    }

    public class TagPriorityGroup
    {
        public List<string> HighTagIds { get; set; }
        public List<string> ModerateTagIds { get; set; }
        public List<string> LowTagIds { get; set; }
    }
}
