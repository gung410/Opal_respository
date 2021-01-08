using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cxEvent.Client
{
    public class EventFilterObject
    {
        /// <summary>
        /// Eventtype name 
        /// </summary>
        [Required]
        public List<string> EventTypeNames { get; set; }
        /// <summary>
        /// Department identifiers
        /// </summary>
        public List<int?> DepartmentIds { get; set; }
        /// <summary>
        /// User identifiers
        /// </summary>
        public List<int?> UserIds { get; set; }
        /// <summary>
        /// Created by User identifiers
        /// </summary>
        public List<int?> CreatedByIds { get; set; }
        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Page index
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Order by (The default setting is order by EventId descending)
        /// Supported fields :EventTypeName, UserIdentity.Id, ApplicationName, CorrelationId, DepartmentIdentity.Id, EntityStatus.StatusId, GroupIdentity.Id, IPNumber
        /// Example : UserIdentity.Id desc
        /// </summary>
        public string OrderBy { get; set; }
    }
}
