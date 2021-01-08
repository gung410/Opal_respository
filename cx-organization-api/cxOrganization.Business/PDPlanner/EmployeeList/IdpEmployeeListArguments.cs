using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using cxOrganization.Business.Extensions;
using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    public class IdpEmployeeListArguments
    {
        public const int MaxPageSize = 1000;

        public IdpEmployeeListArguments()
        {
            SortField = IdpEmployeeListSortField.FirstName;
            SortOrder = SortOrder.Ascending;
            PageSize = MaxPageSize;
            PageIndex = 1;
        }

        /// <summary>
        /// The maximum count of item to return. Default is 1000.
        /// </summary>
        [DefaultValue(MaxPageSize)]
      
        public int? PageSize { get; set; }

        /// <summary>
        /// The index of page for getting
        /// </summary>
        [DefaultValue(1)]
      
        public int? PageIndex { get; set; }

        /// <summary>
        /// List of userIds
        /// </summary>
      
        public List<int> UserIds { get; set; }

        /// <summary>
        /// Entity status of user. If there is no value set, system will handle with configured statuses as default
        /// </summary>
      
        public List<EntityStatusEnum> EntityStatuses { get; set; }

        /// <summary>
        /// List of department where employee belong to
        /// </summary>
      
        public List<int> DepartmentIds { get; set; }
    
        /// <summary>
        /// The keyword to search staff by first name, last name
        /// </summary>
      
        public string IdpEmployeeSearchKey { get; set; }


        /// <summary>
        /// Set list of activity info
        /// </summary>

        public Dictionary<PDPlanActivity, EmployeeListActivity> PDPlanActivities { get; set; }

        /// <summary>
        /// The field which is used for sorting  <see cref="IdpEmployeeListSortField"/>
        /// </summary>

        [Obsolete("This will be removed, please use OrderBy")]
        public IdpEmployeeListSortField SortField { get; set; }

        // <summary>
        /// The sort order <see cref="SortOrder"/>
        /// </summary>
      
        [Obsolete("This will be removed, please use OrderBy")]
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// The order by clause. Sample '{FirstName: Ascending}'
        /// </summary>
      
        public Dictionary<IdpEmployeeListSortField, SortOrder> OrderBy { get; set; }

        /// <summary>
        /// include filter options
        /// </summary>
      
        /// <summary>
        /// Set true to also get user of descendant department of given departments
        /// </summary>
      
        public bool? FilterOnSubDepartment { get; set; }

        /// <summary>
        /// List of filter on user DynamicAttributes. Each element should follow format 'attributeName1=value1,value2'
        /// </summary>
      
        public List<string> UserDynamicAttributes { get; set; }

        /// <summary>
        /// List of list of user type id.
        /// </summary>
      
        public List<List<int>> MultiUserTypeIds { get; set; }
     
        /// <summary>
        /// List of list of user type extid.
        /// </summary>
      
        public List<List<string>> MultiUserTypeExtIds { get; set; }

        /// <summary>
        /// List of list of groupIds.
        /// </summary>
      
        public List<List<int>> MultiUserGroupIds { get; set; }

        /// <summary>
        /// A date-time value that user created on or after
        /// </summary>
      
        public DateTime? CreatedAfter { get; set; }

        /// <summary>
        ///  A date-time value that user created on or before
        /// </summary>
      
        public DateTime? CreatedBefore { get; set; }

        /// <summary>
        ///  A date-time value that user should expired on or after that
        /// </summary>
      
        public DateTime? ExpirationDateAfter { get; set; }

        /// <summary>
        ///  A date-time value that user should expired on or before that
        /// </summary>
      
        public DateTime? ExpirationDateBefore { get; set; }

        /// <summary>
        /// List of department type ids of parent department of user
        /// </summary>
      
        public List<int> OrganizationalUnitTypeIds { get; set; }

        /// <summary>
        /// Age range of user
        /// </summary>
      
        public List<AgeRange> AgeRanges { get; set; }
       
        /// <summary>
        /// Gender of user
        /// </summary>
      
        public List<Gender> Genders { get; set; }

        /// <summary>
        /// Set true to get user that is external data
        /// </summary>
      
        public bool? ExternallyMastered { get; set; }

      

        /// <summary>
        /// Set true to get data of the current user.
        /// </summary>
      
        public bool ForCurrentUser { get; set; }
   

    
        public int GetPageIndex()
        {
            if (PageIndex == null || PageIndex <= 0)
                return 1;
            return PageIndex.Value;
        }

        public int GetPageSize()
        {
            if (PageSize == null || PageSize <= 0 || PageSize > MaxPageSize)
                return MaxPageSize;
            return PageSize.Value;
        }

        /// <summary>
        /// Check to see if the current logged-in user is trying to view his data.
        /// </summary>
        /// <param name="currentLoggedInUserId"></param>
        /// <returns></returns>
        public bool IsViewingHimself(long currentLoggedInUserId)
        {
            return !UserIds.IsNullOrEmpty()
                && UserIds.Count == 1 && UserIds.First() == currentLoggedInUserId;
        }

        /// <summary>
        /// Determines whether the filter parameters has any filter on user groups.
        /// </summary>
        /// <returns></returns>
        public bool HasFilteringOnUserGroups()
        {
            return MultiUserGroupIds != null
                && MultiUserGroupIds.Count > 0
                && MultiUserGroupIds.Any(p => !p.IsNullOrEmpty());
        }
    }



    public class EmployeeListActivity
    {
        /// <summary>
        /// Filter Assessment statusIds pdplanner
        /// </summary>
        public List<int> StatusTypeIds { get; set; }

        /// <summary>
        /// Assessment statusIds pdplanner is expected to return if existing.
        /// </summary>

        public List<int> AllowedStatusTypeIds { get; set; }

        /// <summary>
        /// Default status if result id is not found
        /// </summary>

        public int? DefaultStatusTypeId { get; set; }

        public int ActivityId { get; set; }

        public List<StatusTypeLogFilter> StatusTypeLogs { get; set; }

    }

    public class StatusTypeLogFilter
    {
        public List<int> StatusTypeIds { get; set; }
        public DateTime? ChangedAfter { get; set; }
        public DateTime? ChangedBefore { get; set; }
    }


}