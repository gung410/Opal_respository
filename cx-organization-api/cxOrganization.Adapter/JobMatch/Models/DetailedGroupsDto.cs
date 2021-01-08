using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Adapter.JobMatch.Models
{
    public class DetailedGroupsDto
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public List<JobmatchDto> Jobmatches { get; set; }
    }
}
