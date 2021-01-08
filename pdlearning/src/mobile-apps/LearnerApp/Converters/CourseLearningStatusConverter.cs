using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class CourseLearningStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string apply = string.Empty;

            if (!(value is MyCourseStatus))
            {
                return apply;
            }

            var status = (MyCourseStatus)value;

            if (!string.IsNullOrEmpty(status.Status) && !status.Status.Equals(StatusLearning.NotStarted.ToString()))
            {
                var courseStatus = status.Status switch
                {
                    nameof(StatusLearning.InProgress) => "In Progress",
                    nameof(StatusLearning.Passed) => "In Progress",
                    nameof(StatusLearning.Failed) => "Failed",
                    nameof(StatusLearning.Completed) => StatusLearning.Completed.ToString(),
                    nameof(StatusLearning.Incomplete) => StatusLearning.Incomplete.ToString(),
                    _ => string.Empty
                };

                return courseStatus;
            }

            return status.MyCourseDisplayStatus switch
            {
                nameof(MyCourseDisplayStatus.PendingConfirmation) => "Registration Pending Approval",
                nameof(MyCourseDisplayStatus.Approved) => "Registration Pending Confirmation",
                nameof(MyCourseDisplayStatus.Rejected) => "Registration Unsuccessful",
                nameof(MyCourseDisplayStatus.ConfirmedByCA) => "Registration Confirmed",
                nameof(MyCourseDisplayStatus.RejectedByCA) => "Registration Unsuccessful",
                nameof(MyCourseDisplayStatus.WaitlistPendingApprovalByLearner) => "On Waitlist",
                nameof(MyCourseDisplayStatus.WaitlistConfirmed) => "On Waitlist",
                nameof(MyCourseDisplayStatus.WaitlistRejected) => "Registration Unsuccessful",
                nameof(MyCourseDisplayStatus.OfferPendingApprovalByLearner) => "On Waitlist (Offer)",
                nameof(MyCourseDisplayStatus.OfferRejected) => "Registration Unsuccessful",
                nameof(MyCourseDisplayStatus.OfferConfirmed) => "Registration Confirmed",
                nameof(MyCourseDisplayStatus.Cancelled) => "Registration Unsuccessful",
                nameof(MyCourseDisplayStatus.Rescheduled) => "Registration Rescheduled",

                nameof(MyCourseDisplayStatus.NominatedPendingConfirmation) => "Nomination Pending Approval",
                nameof(MyCourseDisplayStatus.NominatedApproved) => "Nomination Pending Approval",
                nameof(MyCourseDisplayStatus.NominatedRejected) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.NominatedConfirmedByCA) => "Nomination Confirmed",
                nameof(MyCourseDisplayStatus.NominatedRejectedByCA) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.NominatedWaitlistPendingApprovalByLearner) => "On Waitlist",
                nameof(MyCourseDisplayStatus.NominatedWaitlistConfirmed) => "On Waitlist",
                nameof(MyCourseDisplayStatus.NominatedWaitlistRejected) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.NominatedOfferPendingApprovalByLearner) => "On Waitlist (Offer)",
                nameof(MyCourseDisplayStatus.NominatedOfferRejected) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.NominatedOfferConfirmed) => "Nomination Confirmed",
                nameof(MyCourseDisplayStatus.NominatedCancelled) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.NominatedRescheduled) => "Nomination Rescheduled",

                nameof(MyCourseDisplayStatus.AddedByCAPendingConfirmation) => "Nomination Pending Approval",
                nameof(MyCourseDisplayStatus.AddedByCAApproved) => "Nomination Pending Approval",
                nameof(MyCourseDisplayStatus.AddedByCARejected) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.AddedByCAConfirmedByCA) => "Nomination Confirmed",
                nameof(MyCourseDisplayStatus.AddedByCARejectedByCA) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.AddedByCAWaitlistPendingApprovalByLearner) => "On Waitlist",
                nameof(MyCourseDisplayStatus.AddedByCAWaitlistConfirmed) => "On Waitlist",
                nameof(MyCourseDisplayStatus.AddedByCAWaitlistRejected) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.AddedByCAOfferPendingApprovalByLearner) => "On Waitlist (Offer)",
                nameof(MyCourseDisplayStatus.AddedByCAOfferRejected) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.AddedByCAOfferConfirmed) => "Nomination Confirmed",
                nameof(MyCourseDisplayStatus.AddedByCACancelled) => "Nomination Unsuccessful",
                nameof(MyCourseDisplayStatus.AddedByCARescheduled) => "Nomination Rescheduled",

                nameof(MyCourseDisplayStatus.WithdrawalPendingConfirmation) => "Withdrawal Pending Approval",
                nameof(MyCourseDisplayStatus.WithdrawalApproved) => "Withdrawal Pending Confirmation",
                nameof(MyCourseDisplayStatus.WithdrawalRejected) => "Withdrawal Unsuccessful",
                nameof(MyCourseDisplayStatus.WithdrawalWithdrawn) => "Withdrawal Successful",
                nameof(MyCourseDisplayStatus.WithdrawalRejectedByCA) => "Withdrawal Unsuccessful",

                nameof(MyCourseDisplayStatus.ClassRunChangeApproved) => "Change of Class Pending Confirmation",
                nameof(MyCourseDisplayStatus.ClassRunChangePendingConfirmation) => "Change of Class Pending Approval",
                nameof(MyCourseDisplayStatus.ClassRunChangeRejected) => "Change of Class Unsuccessful",
                nameof(MyCourseDisplayStatus.ClassRunChangeRejectedByCA) => "Change of Class Unsuccessful",
                nameof(MyCourseDisplayStatus.ClassRunChangeConfirmedByCA) => "Change of Class Confirmed",
                _ => status.MyCourseDisplayStatus,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
