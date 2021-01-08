using System;
using System.Collections.Generic;
using LearnerApp.Common;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Models.Course
{
    public class ClassRun : BaseViewModel
    {
        private bool _isExpand;

        private bool _isClassRunExpand;

        private bool _isVisibleApplyButton = true;

        private bool _isVisibleSessions = true;

        private bool _isVisibleClassRun = true;

        private bool _isVisibleChangeClassRun;

        private bool _isVisibleWithdrawButton;

        private bool _isVisibleWithdrawalMessage;

        private bool _isLastItem;

        private string _applyText = "Apply";

        private string _withdrawText = "Withdraw";

        private string _changeClassRunText = "Change class run";

        private string _facilitators;

        private bool _canApply = true;

        private bool _canChangeClassRun = true;

        private bool _isVisibleCommunity = false;

        private List<Session> _sessions;

        public string Facilitators
        {
            get
            {
                return _facilitators;
            }

            set
            {
                _facilitators = value;
                RaisePropertyChanged(() => Facilitators);
            }
        }

        public string Id { get; set; }

        public string CourseId { get; set; }

        public string ClassTitle { get; set; }

        public string ClassRunCode { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? PlanningStartTime { get; set; }

        public DateTime? PlanningEndTime { get; set; }

        public string[] FacilitatorIds { get; set; }

        public string[] CoFacilitatorIds { get; set; }

        public int MinClassSize { get; set; }

        public int MaxClassSize { get; set; }

        public DateTime? ApplicationStartDate { get; set; }

        public DateTime? ApplicationEndDate { get; set; }

        public DateTime? RegistrationStartDate { get; set; }

        public DateTime? RegistrationEndDate { get; set; }

        public DateTime? RescheduleStartDateTime { get; set; }

        public DateTime? RescheduleEndDateTime { get; set; }

        public ClassRunRescheduleStatus RescheduleStatus { get; set; }

        public ClassRunStatus Status { get; set; }

        public string CreatedBy { get; set; }

        public string ChangedBy { get; set; }

        public List<Session> Sessions
        {
            get
            {
                return _sessions;
            }

            set
            {
                _sessions = value;
                RaisePropertyChanged(() => Sessions);
            }
        }

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

        public bool IsClassRunExpand
        {
            get
            {
                return _isClassRunExpand;
            }

            set
            {
                _isClassRunExpand = value;
                RaisePropertyChanged(() => IsClassRunExpand);
            }
        }

        public bool IsVisibleApplyButton
        {
            get
            {
                return _isVisibleApplyButton;
            }

            set
            {
                _isVisibleApplyButton = value;
                RaisePropertyChanged(() => IsVisibleApplyButton);
            }
        }

        public string ApplyText
        {
            get
            {
                return _applyText;
            }

            set
            {
                _applyText = value;

                if (_applyText == StatusLearning.Failed.ToString())
                {
                    _applyText = StatusLearning.Incomplete.ToString();
                }

                RaisePropertyChanged(() => ApplyText);
            }
        }

        public string WithdrawText
        {
            get
            {
                return _withdrawText;
            }

            set
            {
                _withdrawText = value;
                RaisePropertyChanged(() => WithdrawText);
            }
        }

        public string ChangeClassRunText
        {
            get
            {
                return _changeClassRunText;
            }

            set
            {
                _changeClassRunText = value;
                RaisePropertyChanged(() => ChangeClassRunText);
            }
        }

        public bool CanApply
        {
            get
            {
                return _canApply;
            }

            set
            {
                _canApply = value;
                RaisePropertyChanged(() => CanApply);
            }
        }

        public bool IsVisibleWithdrawButton
        {
            get
            {
                return _isVisibleWithdrawButton;
            }

            set
            {
                _isVisibleWithdrawButton = value;
                RaisePropertyChanged(() => IsVisibleWithdrawButton);
            }
        }

        public bool IsVisibleWithdrawalMessage
        {
            get
            {
                return _isVisibleWithdrawalMessage;
            }

            set
            {
                _isVisibleWithdrawalMessage = value;
                RaisePropertyChanged(() => IsVisibleWithdrawalMessage);
            }
        }

        public bool IsLastItem
        {
            get
            {
                return _isLastItem;
            }

            set
            {
                _isLastItem = value;
                RaisePropertyChanged(() => IsLastItem);
            }
        }

        public bool IsVisibleSessions
        {
            get
            {
                return _isVisibleSessions;
            }

            set
            {
                _isVisibleSessions = value;
                RaisePropertyChanged(() => IsVisibleSessions);
            }
        }

        public bool CanChangeClassRun
        {
            get
            {
                return _canChangeClassRun;
            }

            set
            {
                _canChangeClassRun = value;
                RaisePropertyChanged(() => CanChangeClassRun);
            }
        }

        public bool IsVisibleChangeClassRun
        {
            get
            {
                return _isVisibleChangeClassRun;
            }

            set
            {
                _isVisibleChangeClassRun = value;
                RaisePropertyChanged(() => IsVisibleChangeClassRun);
            }
        }

        public bool IsVisibleCommunity
        {
            get
            {
                return _isVisibleCommunity;
            }

            set
            {
                _isVisibleCommunity = value;
                RaisePropertyChanged(() => IsVisibleCommunity);
            }
        }

        public bool IsVisibleClassRun
        {
            get
            {
                return _isVisibleClassRun;
            }

            set
            {
                _isVisibleClassRun = value;
                RaisePropertyChanged(() => IsVisibleClassRun);
            }
        }

        public int CurrentCapacity { get; set; }

        public DateTime GetStartDateTime()
        {
            return GetDateTimeLocal.Convert(StartDateTime);
        }

        public DateTime GetEndDateTime()
        {
            return GetDateTimeLocal.Convert(EndDateTime);
        }

        public DateTime GetAplicationStartDateTime()
        {
            return GetDateTimeLocal.Convert(ApplicationStartDate);
        }

        public DateTime GetAplicationEndDateTime()
        {
            return GetDateTimeLocal.Convert(ApplicationEndDate);
        }
    }
}
