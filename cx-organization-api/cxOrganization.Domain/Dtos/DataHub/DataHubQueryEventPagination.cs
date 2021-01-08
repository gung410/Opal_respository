using System.Collections.Generic;

namespace cxOrganization.Domain.Dtos.DataHub
{
    public class DataHubQueryEventPagination
    {
        public int Count { get; set; }
        public DataHubQueryPaginationInfo PageInfo { get; set; }
        public List<GenericLogEventMessage> Items { get; set; }
    }
}