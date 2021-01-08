using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class CourseCommunityVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MyCourseStatus myCourseStatus)
            {
                var registrationStatus = myCourseStatus.MyRegistrationStatus;
                var courseStatus = myCourseStatus.Status;
                return (registrationStatus == nameof(RegistrationStatus.OfferConfirmed)
                        || registrationStatus == nameof(RegistrationStatus.ConfirmedByCA)
                        || registrationStatus == nameof(RegistrationStatus.ConfirmedBeforeStartDate)
                        || courseStatus == nameof(StatusLearning.Completed)
                        || courseStatus == nameof(StatusLearning.Failed)) && myCourseStatus.MyCourseDisplayStatus != nameof(MyCourseDisplayStatus.WithdrawalWithdrawn);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}