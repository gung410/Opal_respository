using System.Collections.Generic;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    public class IdpEmployeeListDto
    {
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<IdpEmployeeItemDto> Items { get; set; }
      //  public FilterOptions FilterOptions { get; set; }
        public bool HasMoreData { get; set; }
    } 
}