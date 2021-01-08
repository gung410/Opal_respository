using System;
using LearnerApp.Common;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Models.Course
{
    public class Session : BaseViewModel
    {
        private bool _isExpand;
        private bool _canCheckin;
        private bool _canAbsent;
        private bool _showAbsenceMessage;
        private bool _canJoinWebinar;
        private string _status;
        private string _venue;

        public bool IsExpand
        {
            get
            {
                return _isExpand;
            }

            set
            {
                _isExpand = value;
                RaisePropertyChanged(() => IsExpand);
            }
        }

        public bool CanJoinWebinar
        {
            get
            {
                return _canJoinWebinar;
            }

            set
            {
                _canJoinWebinar = value;
                RaisePropertyChanged(() => CanJoinWebinar);
            }
        }

        public bool LearningMethod { get; set; }

        public string Id { get; set; }

        public string ClassRunId { get; set; }

        public string SessionTitle { get; set; }

        public string Venue
        {
            get
            {
                return _venue;
            }

            set
            {
                _venue = value;
                RaisePropertyChanged(() => Venue);
            }
        }

        public DateTime SessionDate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? RescheduleStartDateTime { get; set; }

        public DateTime? RescheduleEndDateTime { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ChangedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public bool CanCheckin
        {
            get
            {
                return _canCheckin;
            }

            set
            {
                _canCheckin = value;
                RaisePropertyChanged(() => CanCheckin);
            }
        }

        public bool CanAbsent
        {
            get
            {
                return _canAbsent;
            }

            set
            {
                _canAbsent = value;
                RaisePropertyChanged(() => CanAbsent);
            }
        }

        public bool ShowAbsenceMessage
        {
            get
            {
                return _showAbsenceMessage;
            }

            set
            {
                _showAbsenceMessage = value;
                RaisePropertyChanged(() => ShowAbsenceMessage);
            }
        }

        public DateTime GetStartDateTime()
        {
            return GetDateTimeLocal.Convert(StartDateTime);
        }

        public DateTime GetEndDateTime()
        {
            return GetDateTimeLocal.Convert(EndDateTime);
        }
    }
}
