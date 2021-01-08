namespace cxOrganization.Domain.Dtos.DataHub
{
    public class DataHubQueryPaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int PageCount { get; set; }
        public int ItemCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}