using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class PendingPopupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MyCourseStatus))
            {
                return false;
            }

            var myCourseStatus = (MyCourseStatus)value;

            if (myCourseStatus.MyCourseDisplayStatus == "Nomination Unsuccessful" || myCourseStatus.MyCourseDisplayStatus == "Registration Unsuccessful")
            {
                return false;
            }

            if (!string.IsNullOrEmpty(myCourseStatus.MyWithdrawalStatus)
                && !myCourseStatus.MyWithdrawalStatus.Equals(WithdrawalStatus.Rejected.ToString()))
            {
                return false;
            }

            return myCourseStatus.MyRegistrationStatus switch
            {
                nameof(RegistrationStatus.WaitlistPendingApprovalByLearner) => true,
                nameof(RegistrationStatus.OfferPendingApprovalByLearner) => true,
                _ => false,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
