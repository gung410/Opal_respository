using System.Collections.Generic;

namespace cxOrganization.Adapter.JobMatch.Models
{
    public class JobmatchesDto
    {
        public string Riasec { get; set; }
        public string ClassificationStandard { get; set; }

        public List<MajorGroupsDto> MajorGroups { get; set; }

        public JobmatchesDto()
        {
            MajorGroups = new List<MajorGroupsDto>();
        }
    }
}