using System.Collections.Generic;
using cxOrganization.Domain.DomainEnums;

namespace cxOrganization.Domain.Dtos.BroadcastMessage
{
    public class BroadcastMessageSearchRequest
    {
        public string SearchText { get; set; }

        public List<BroadcastMessageStatus> SearchStatus { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public OrderBy OrderBy { get; set; }

        public OrderByType OrderType { get; set; }
    }
}
