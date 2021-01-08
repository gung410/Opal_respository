using System;
using System.Collections.Generic;
using System.Text;
using cxOrganization.Domain.DomainEnums;

namespace cxOrganization.Domain.Dtos.Users
{
    public class CountUserResultDto
    {
        public UserGroupByField GroupByField { get; set; }
        public int TotalUser { get; set; }
        public List<CountUserValueDto> CountValues { get; set; }
    }
    public class CountUserValueDto
    {
        public object GroupValue { get; set; }
        public int UserCount { get; set; }
    }
}
