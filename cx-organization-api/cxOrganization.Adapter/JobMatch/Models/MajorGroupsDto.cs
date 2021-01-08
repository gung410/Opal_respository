using System.Collections.Generic;

namespace cxOrganization.Adapter.JobMatch.Models
{
    public class MajorGroupsDto
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public List<JobmatchDto> Jobmatches { get; set; }
        public List<MinorGroupsDto> MinorGroups { get; set; }

        public MajorGroupsDto()
        {
            //MinorGroups = new List<MinorGroupsDto>();
        }

        public void AddMinorGroup(MinorGroupsDto minorGroup)
        {
            if (MinorGroups == null)
                MinorGroups = new List<MinorGroupsDto>();
            MinorGroups.Add(minorGroup);
        }
        
    }
}