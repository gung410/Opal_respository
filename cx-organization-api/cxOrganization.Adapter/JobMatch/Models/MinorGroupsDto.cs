using System.Collections.Generic;

namespace cxOrganization.Adapter.JobMatch.Models
{
    public class MinorGroupsDto
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public List<JobmatchDto> Jobmatches { get; set; }
        public List<BroadOccupationsDto> BroadOccupations { get; set; }

        public MinorGroupsDto()
        {
            BroadOccupations = new List<BroadOccupationsDto>();
        }

        public void AddBroadOccupation(BroadOccupationsDto broadOccupation)
        {
            if (BroadOccupations == null)
                BroadOccupations = new List<BroadOccupationsDto>();
            BroadOccupations.Add(broadOccupation);
        }
    }
}