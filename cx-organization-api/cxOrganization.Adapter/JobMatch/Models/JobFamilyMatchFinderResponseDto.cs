using System.Collections.Generic;

namespace cxOrganization.Adapter.JobMatch.Models
{
    public class JobFamilyMatchFinderResponseDto
    {
        public List<JobmatchesDto> Matches { get; set; }
        public List<JobmatchesDto> NotFound { get; set; }
    }
}