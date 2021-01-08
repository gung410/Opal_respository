using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services.Reports
{
    public class UserStatisticsDto
    {
        /// <summary>
        /// Count account by entity status
        /// </summary>
        public UserAccountStatisticsDto AccountStatistics { get; set; }

        /// <summary>
        /// Count user based on on-boarding data
        /// </summary>
        public UserOnBoardingStatisticsDto OnBoardingStatistics { get; set; }

        /// <summary>
        /// Count event based
        /// </summary>
        public UserEventStatisticsDto EventStatistics { get; set; }
    }

    public class UserAccountStatisticsDto : Dictionary<(EntityStatusEnum EntityStatus, string OrganizationTypeName), Dictionary<AccountType, int>>
    {

    }

    public class UserOnBoardingStatisticsDto
    {
        public UserOnBoardingStatisticsDto()
        {
            NotStarted = new Dictionary<AccountType, int>();
            Started = new Dictionary<AccountType, int>();
            Completed = new Dictionary<AccountType, int>();
        }
        /// <summary>
        /// Number of user that is created but not started on-boarding in given timespan
        /// </summary>
        public Dictionary<AccountType, int> NotStarted { get; set; }
       
        /// <summary>
        /// Number of user that is started on-boarding in given timespan but not completed in that time yet
        /// </summary>
        public Dictionary<AccountType, int> Started { get; set; }

        /// <summary>
        /// Number of user that is completed on-boarding in given timespan
        /// </summary>
        public Dictionary<AccountType, int> Completed { get; set; }
    }

    public class UserEventStatisticsDto : Dictionary<UserEventType, Dictionary<AccountType, Dictionary<EventValueType, int>>>
    {

    }

    public class UserAccountStatisticsInfo {
        public int? EntityStatusId { get; set; }
        public short Locked { get; set; }
        public string TypeOfOrganizationUnitId { get; set; }
        public int NumberOfUser { get; set; }
    }
}
