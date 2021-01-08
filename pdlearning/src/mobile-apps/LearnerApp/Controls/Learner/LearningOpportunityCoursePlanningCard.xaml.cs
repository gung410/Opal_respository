using LearnerApp.Common;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningOpportunityCoursePlanningCard : ContentView
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source),
                typeof(LearningOpportunityCardTransfer),
                typeof(LearningOpportunityCoursePlanningCard),
                null,
                propertyChanged: OnItemsChanged);

        public LearningOpportunityCoursePlanningCard()
        {
            InitializeComponent();
        }

        public LearningOpportunityCardTransfer Source
        {
            get { return (LearningOpportunityCardTransfer)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is LearningOpportunityCardTransfer item))
            {
                return;
            }

            var view = (LearningOpportunityCoursePlanningCard)bindable;
            var courseInfo = item.CourseExtendedInformation;
            var metadata = item.MetadataTags;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (courseInfo == null)
                {
                    view.NatureOfCourse.Text = GlobalSettings.NotAvailable;
                    view.TotalHoursAttendWithinYear.Text = GlobalSettings.NotAvailable;
                    view.MinParticipantPerClass.Text = GlobalSettings.NotAvailable;
                    view.MaxParticipantPerClass.Text = GlobalSettings.NotAvailable;
                }
                else
                {
                    view.NatureOfCourse.Text = StringExtension.GetInformation(courseInfo.NatureOfCourse, metadata);
                    view.TotalHoursAttendWithinYear.Text = string.IsNullOrEmpty(courseInfo.TotalHoursAttendWithinYear.ToString()) ? GlobalSettings.NotAvailable : courseInfo.TotalHoursAttendWithinYear.ToString();
                    view.MinParticipantPerClass.Text = string.IsNullOrEmpty(courseInfo.MinParticipantPerClass.ToString()) ? GlobalSettings.NotAvailable : courseInfo.MinParticipantPerClass.ToString();
                    view.MaxParticipantPerClass.Text = string.IsNullOrEmpty(courseInfo.MaxParticipantPerClass.ToString()) ? GlobalSettings.NotAvailable : courseInfo.MaxParticipantPerClass.ToString();
                }
            });
        }
    }
}
