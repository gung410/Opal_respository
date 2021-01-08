using System.Collections.Generic;

namespace cxOrganization.Adapter.JobMatch.Models
{
    public class BroadOccupationsDto
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public List<DetailedGroupsDto> DetailedOccupations { get; set; }
        public List<JobmatchDto> Jobmatches { get; set; }

        public BroadOccupationsDto()
        {
        }

        public void AddDetailedOccupation(DetailedGroupsDto detailedOccupation)
        {
            if (DetailedOccupations == null)
                DetailedOccupations = new List<DetailedGroupsDto>();
            DetailedOccupations.Add(detailedOccupation);
        }
    }
}