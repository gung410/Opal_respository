using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserUserTypes
    {
        public Guid UserHistoryId { get; set; }

        public Guid UserId { get; set; }

        public string UserTypeId { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }

        public virtual SAM_UserTypes UserType { get; set; }
    }
}
