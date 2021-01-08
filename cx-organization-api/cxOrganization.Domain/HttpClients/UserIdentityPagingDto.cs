using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.HttpClients
{
    public class UserIdentityPagingDto
    {
        public PagingHeader PagingHeader { get; set; }
        public List<UserIdentityDto> Items { get; set; }
    }



    public class PagingHeader
    {
        public long TotalItems { get; set; }
        public long PageNumber { get; set; }
        public long PageSize { get; set; }
        public long TotalPages { get; set; }
    }

}
