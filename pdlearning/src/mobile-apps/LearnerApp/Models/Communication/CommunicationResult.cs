using System.Collections.Generic;

namespace LearnerApp.Models.Communication
{
    public class CommunicationResult<T>
    {
        public int TotalCount { get; set; }

        public int TotalUnreadCount { get; set; }

        public int TotalNewCount { get; set; }

        public List<T> PageItems { get; set; }
    }
}
