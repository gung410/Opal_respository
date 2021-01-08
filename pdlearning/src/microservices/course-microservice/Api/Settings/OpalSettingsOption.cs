namespace Microservice.Course.Settings
{
    public class OpalSettingsOption
    {
        public const string OpalSettings = "OpalSettings";

        public int OfferExpireLifeTime { get; set; }

        public int AssignmentExtendedDays { get; set; }

        public int BeforeAssignmentDeadlineDays { get; set; }

        public int BeforeAssignmentExtendedDays { get; set; }

        // The setting to notify pending approval courses which have been submitted [number of days] ago
        public int NotifyPendingApprovalCourseSubmittedDaysAgo { get; set; }

        // The setting to notify pending approval content which have been submitted [number of days] ago
        public int NotifyPendingApprovalContentSubmittedDaysAgo { get; set; }

        public int MaxMinutesCanJoinWebinarEarly { get; set; }

        public int CreateWebinarDelayTimeInMinutes { get; set; }
    }
}
