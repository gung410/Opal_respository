using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class LearningStatusEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MyCourseStatus))
            {
                return true;
            }

            var myCourseStatus = (MyCourseStatus)value;

            if (CourseLearningStatus.START.ToString().Equals(myCourseStatus.LearningStatus) && myCourseStatus.IsTableOfContentEmpty && myCourseStatus.IsMicroLearningType)
            {
                return false;
            }

            if (myCourseStatus.IsAddToPlan)
            {
                return true;
            }

            if (myCourseStatus.MyCourseDisplayStatus == "Nomination Unsuccessful" || myCourseStatus.MyCourseDisplayStatus == "Registration Unsuccessful")
            {
                return true;
            }

            if (myCourseStatus.ClassRunStatus == ClassRunStatus.Unpublished)
            {
                return false;
            }

            if (!myCourseStatus.IsAddToPlan && !string.IsNullOrEmpty(myCourseStatus.Status) && myCourseStatus.Status.Equals(StatusLearning.Failed.ToString()))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(myCourseStatus.Status) && (myCourseStatus.Status.Equals(StatusLearning.InProgress.ToString()) || myCourseStatus.Status.Equals(StatusLearning.Completed.ToString())))
            {
                return true;
            }

            if (!myCourseStatus.IsMicroLearningType && !string.IsNullOrEmpty(myCourseStatus.Status) && myCourseStatus.Status.Equals(StatusLearning.Completed.ToString()))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(myCourseStatus.MyWithdrawalStatus)
                && !myCourseStatus.MyWithdrawalStatus.Equals(WithdrawalStatus.Rejected.ToString())
                && !myCourseStatus.MyWithdrawalStatus.Equals(WithdrawalStatus.Withdrawn.ToString()))
            {
                return false;
            }

            if (myCourseStatus.MyCourseDisplayStatus == MyCourseDisplayStatus.Cancelled.ToString()
                && myCourseStatus.MyRegistrationStatus == RegistrationStatus.Approved.ToString())
            {
                return true;
            }

            if (myCourseStatus.MyCourseDisplayStatus == MyCourseDisplayStatus.WithdrawalWithdrawn.ToString())
            {
                return true;
            }

            if (myCourseStatus.MyRegistrationStatus == nameof(RegistrationStatus.ConfirmedBeforeStartDate) &&
                myCourseStatus.MyCourseDisplayStatus == nameof(MyCourseDisplayStatus.NominatedConfirmedByCA))
            {
                return true;
            }

            return myCourseStatus.MyRegistrationStatus switch
            {
                nameof(RegistrationStatus.PendingConfirmation) => false,
                nameof(RegistrationStatus.Approved) => false,
                nameof(RegistrationStatus.WaitlistPendingApprovalByLearner) => false,
                nameof(RegistrationStatus.WaitlistConfirmed) => false,
                nameof(RegistrationStatus.OfferPendingApprovalByLearner) => false,
                nameof(RegistrationStatus.WaitlistUnsuccessful) => false,
                nameof(RegistrationStatus.ConfirmedBeforeStartDate) => false,
                _ => true,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
