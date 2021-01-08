using System;
using LearnerApp.Common;

namespace LearnerApp.Models.Course
{
    public class UpComingTransferData
    {
        public Session Session { get; set; }

        public AttendanceTracking AttendanceTracking { get; set; }

        public string GetSessionId()
        {
            return Session?.Id;
        }

        public string GetSessionTitle()
        {
            return Session?.SessionTitle;
        }

        public string GetStartDate()
        {
            string startDate = string.Empty;

            if (Session?.StartDateTime != null)
            {
                startDate = ConvertToLocaTimeString((DateTime)Session.StartDateTime);
            }

            return startDate;
        }

        public string GetEndDate()
        {
            string endDate = string.Empty;

            if (Session?.EndDateTime != null)
            {
                endDate = ConvertToLocaTimeString((DateTime)Session.EndDateTime);
            }

            return endDate;
        }

        public string GetSessionVenue()
        {
            return Session?.Venue;
        }

        public bool CanShowCheckin()
        {
            bool canShowCheckin = false;
            DateTime localNow = DateTime.Now.ToUniversalTime();

            if (Session.StartDateTime != null)
            {
                canShowCheckin = Session.GetStartDateTime().Date.CompareTo(localNow.Date) == 0;
            }

            return canShowCheckin;
        }

        public bool CanShowAbsent()
        {
            bool canShowAbsent = false;

            if (AttendanceTracking != null)
            {
                canShowAbsent = AttendanceTracking.Status == AttendanceTrackingStatus.Absent && string.IsNullOrEmpty(AttendanceTracking.ReasonForAbsence);
            }

            return canShowAbsent;
        }

        private string ConvertToLocaTimeString(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local).ToString("dd/MM/yyyy hh:mm tt");
        }
    }
}
