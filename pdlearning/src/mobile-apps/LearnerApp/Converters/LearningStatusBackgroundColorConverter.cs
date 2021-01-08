using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class LearningStatusBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color availableColor = (Color)Application.Current.Resources["MainBodyTextColor"];

            if (!(value is MyCourseStatus))
            {
                return availableColor;
            }

            var myCourseStatus = (MyCourseStatus)value;

            if (CourseLearningStatus.START.ToString().Equals(myCourseStatus.LearningStatus) && myCourseStatus.IsTableOfContentEmpty && myCourseStatus.IsMicroLearningType)
            {
                return (Color)Application.Current.Resources["RegisteredColor"];
            }

            if (myCourseStatus.IsAddToPlan)
            {
                return availableColor;
            }

            if (myCourseStatus.MyCourseDisplayStatus == "Nomination Unsuccessful" || myCourseStatus.MyCourseDisplayStatus == "Registration Unsuccessful")
            {
                return availableColor;
            }

            if (myCourseStatus.ClassRunStatus == ClassRunStatus.Unpublished)
            {
                return (Color)Application.Current.Resources["RegisteredColor"];
            }

            if (!myCourseStatus.IsAddToPlan && !string.IsNullOrEmpty(myCourseStatus.Status) && myCourseStatus.Status.Equals(StatusLearning.Failed.ToString()))
            {
                return availableColor;
            }

            if (!string.IsNullOrEmpty(myCourseStatus.Status) && (myCourseStatus.Status.Equals(StatusLearning.InProgress.ToString()) || myCourseStatus.Status.Equals(StatusLearning.Completed.ToString())))
            {
                return availableColor;
            }

            if (!myCourseStatus.IsMicroLearningType && !string.IsNullOrEmpty(myCourseStatus.Status) && myCourseStatus.Status.Equals(StatusLearning.Completed.ToString()))
            {
                return (Color)Application.Current.Resources["RegisteredColor"];
            }

            if (!string.IsNullOrEmpty(myCourseStatus.MyWithdrawalStatus)
                && !myCourseStatus.MyWithdrawalStatus.Equals(WithdrawalStatus.Rejected.ToString())
                && !myCourseStatus.MyWithdrawalStatus.Equals(WithdrawalStatus.Withdrawn.ToString()))
            {
                return (Color)Application.Current.Resources["RegisteredColor"];
            }

            if (myCourseStatus.MyCourseDisplayStatus == MyCourseDisplayStatus.Cancelled.ToString()
                && myCourseStatus.MyRegistrationStatus == RegistrationStatus.Approved.ToString())
            {
                return availableColor;
            }

            if (myCourseStatus.MyCourseDisplayStatus == MyCourseDisplayStatus.WithdrawalWithdrawn.ToString())
            {
                return availableColor;
            }

            if (myCourseStatus.MyRegistrationStatus == nameof(RegistrationStatus.ConfirmedBeforeStartDate) &&
              myCourseStatus.MyCourseDisplayStatus == nameof(MyCourseDisplayStatus.NominatedConfirmedByCA))
            {
                return availableColor;
            }

            return myCourseStatus.MyRegistrationStatus switch
            {
                nameof(RegistrationStatus.PendingConfirmation) => (Color)Application.Current.Resources["RegisteredColor"],
                nameof(RegistrationStatus.Approved) => (Color)Application.Current.Resources["RegisteredColor"],
                nameof(RegistrationStatus.WaitlistPendingApprovalByLearner) => (Color)Application.Current.Resources["RegisteredColor"],
                nameof(RegistrationStatus.WaitlistConfirmed) => (Color)Application.Current.Resources["RegisteredColor"],
                nameof(RegistrationStatus.OfferPendingApprovalByLearner) => (Color)Application.Current.Resources["RegisteredColor"],
                nameof(RegistrationStatus.WaitlistUnsuccessful) => (Color)Application.Current.Resources["RegisteredColor"],
                nameof(RegistrationStatus.ConfirmedBeforeStartDate) => (Color)Application.Current.Resources["RegisteredColor"],
                _ => availableColor,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
