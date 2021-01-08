using cxOrganization.Domain.DomainEnums;
using System;

namespace cxOrganization.Domain.RequestDtos.FileRequest
{
    public class GetFileByUserIdRequest
    {
        public string SearchText { get; set; }

        public Guid UserGuid { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public FileTarget FileTarget { get; set; }

        public OrderBy OrderBy { get; set; }

        public OrderByType OrderType { get; set; }
    }
}
