using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class LabelStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string status = value.ToString();

            if (string.IsNullOrEmpty(status))
            {
                return string.Empty;
            }

            status = status switch
            {
                nameof(StatusLearning.InProgress) => "IN PROGRESS",
                nameof(StatusLearning.Passed) => "IN PROGRESS",
                nameof(StatusLearning.Completed) => "COMPLETED",
                nameof(StatusLearning.Failed) => "INCOMPLETE",
                nameof(StatusLearning.Incomplete) => "INCOMPLETE",
                nameof(StatusLearning.Expired) => "EXPIRED",
                nameof(StatusCourse.Unpublished) => "UNPUBLISHED",
                nameof(StatusLearning.NotStarted) => "NOT STARTED",

                nameof(MyCourseDisplayStatus.PendingConfirmation) => "REGISTRATION PENDING APPROVAL",
                nameof(MyCourseDisplayStatus.Approved) => "REGISTRATION PENDING CONFIRMATION",
                nameof(MyCourseDisplayStatus.Rejected) => "REGISTRATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.ConfirmedByCA) => "REGISTRATION CONFIRMED",
                nameof(MyCourseDisplayStatus.RejectedByCA) => "REGISTRATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.WaitlistPendingApprovalByLearner) => "ON WAITLIST",
                nameof(MyCourseDisplayStatus.WaitlistConfirmed) => "ON WAITLIST",
                nameof(MyCourseDisplayStatus.WaitlistRejected) => "REGISTRATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.OfferPendingApprovalByLearner) => "ON WAITLIST (OFFER)",
                nameof(MyCourseDisplayStatus.OfferRejected) => "REGISTRATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.OfferConfirmed) => "REGISTRATION CONFIRMED",
                nameof(MyCourseDisplayStatus.Cancelled) => "REGISTRATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.Rescheduled) => "REGISTRATION RESCHEDULED",

                nameof(MyCourseDisplayStatus.NominatedPendingConfirmation) => "NOMINATION PENDING APPROVAL",
                nameof(MyCourseDisplayStatus.NominatedApproved) => "NOMINATION PENDING APPROVAL",
                nameof(MyCourseDisplayStatus.NominatedRejected) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.NominatedConfirmedByCA) => "NOMINATION CONFIRMED",
                nameof(MyCourseDisplayStatus.NominatedRejectedByCA) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.NominatedWaitlistPendingApprovalByLearner) => "ON WAITLIST",
                nameof(MyCourseDisplayStatus.NominatedWaitlistConfirmed) => "ON WAITLIST",
                nameof(MyCourseDisplayStatus.NominatedWaitlistRejected) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.NominatedOfferPendingApprovalByLearner) => "ON WAITLIST (OFFER)",
                nameof(MyCourseDisplayStatus.NominatedOfferRejected) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.NominatedOfferConfirmed) => "NOMINATION CONFIRMED",
                nameof(MyCourseDisplayStatus.NominatedCancelled) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.NominatedRescheduled) => "NOMINATION RESCHEDULED",

                nameof(MyCourseDisplayStatus.AddedByCAPendingConfirmation) => "NOMINATION PENDING APPROVAL",
                nameof(MyCourseDisplayStatus.AddedByCAApproved) => "NOMINATION PENDING APPROVAL",
                nameof(MyCourseDisplayStatus.AddedByCARejected) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.AddedByCAConfirmedByCA) => "NOMINATION CONFIRMED",
                nameof(MyCourseDisplayStatus.AddedByCARejectedByCA) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.AddedByCAWaitlistPendingApprovalByLearner) => "ON WAITLIST",
                nameof(MyCourseDisplayStatus.AddedByCAWaitlistConfirmed) => "ON WAITLIST",
                nameof(MyCourseDisplayStatus.AddedByCAWaitlistRejected) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.AddedByCAOfferPendingApprovalByLearner) => "ON WAITLIST (OFFER)",
                nameof(MyCourseDisplayStatus.AddedByCAOfferRejected) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.AddedByCAOfferConfirmed) => "NOMINATION CONFIRMED",
                nameof(MyCourseDisplayStatus.AddedByCACancelled) => "NOMINATION UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.AddedByCARescheduled) => "NOMINATION RESCHEDULED",

                nameof(MyCourseDisplayStatus.WithdrawalPendingConfirmation) => "WITHDRAWAL PENDING APPROVAL",
                nameof(MyCourseDisplayStatus.WithdrawalApproved) => "WITHDRAWAL PENDING CONFIRMATION",
                nameof(MyCourseDisplayStatus.WithdrawalRejected) => "WITHDRAWAL UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.WithdrawalWithdrawn) => "WITHDRAWAL SUCCESSFUL",
                nameof(MyCourseDisplayStatus.WithdrawalRejectedByCA) => "WITHDRAWAL UNSUCCESSFUL",

                nameof(MyCourseDisplayStatus.ClassRunChangeApproved) => "CHANGE OF CLASS PENDING CONFIRMATION",
                nameof(MyCourseDisplayStatus.ClassRunChangePendingConfirmation) => "CHANGE OF CLASS PENDING APPROVAL",
                nameof(MyCourseDisplayStatus.ClassRunChangeRejected) => "CHANGE OF CLASS UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.ClassRunChangeRejectedByCA) => "CHANGE OF CLASS UNSUCCESSFUL",
                nameof(MyCourseDisplayStatus.ClassRunChangeConfirmedByCA) => "CHANGE OF CLASS CONFIRMED",

                _ => status
            };

            return status;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
