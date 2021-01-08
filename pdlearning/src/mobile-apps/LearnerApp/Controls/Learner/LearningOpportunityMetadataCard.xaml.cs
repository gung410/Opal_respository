using LearnerApp.Common;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningOpportunityMetadataCard : ContentView
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source),
                typeof(LearningOpportunityCardTransfer),
                typeof(LearningOpportunityMetadataCard),
                null,
                propertyChanged: OnItemsChanged);

        public LearningOpportunityMetadataCard()
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

            if (item.MetadataTags.IsNullOrEmpty())
            {
                return;
            }

            if (item.CourseExtendedInformation == null)
            {
                return;
            }

            var courseInfo = item.CourseExtendedInformation;
            var metadataTags = item.MetadataTags;
            var view = (LearningOpportunityMetadataCard)bindable;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (courseInfo == null)
                {
                    view.ServiceSchemes.Text = GlobalSettings.NotAvailable;
                    view.Subject.Text = GlobalSettings.NotAvailable;
                    view.PDAreaTheme.Text = GlobalSettings.NotAvailable;
                    view.CourseLevel.Text = GlobalSettings.NotAvailable;
                    view.LearningFramework.Text = GlobalSettings.NotAvailable;
                    view.LearningDimension.Text = GlobalSettings.NotAvailable;
                    view.LearningArea.Text = GlobalSettings.NotAvailable;
                    view.LearningSubArea.Text = GlobalSettings.NotAvailable;
                    view.TeacherOutcome.Text = GlobalSettings.NotAvailable;
                }
                else
                {
                    view.ServiceSchemes.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.ServiceSchemeIds, metadataTags);
                    view.Subject.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.SubjectAreaIds, metadataTags);
                    view.PDAreaTheme.Text = StringExtension.GetInformation(courseInfo.PdAreaThemeId, metadataTags);
                    view.CourseLevel.Text = StringExtension.GetInformation(courseInfo.CourseLevel, metadataTags);
                    view.LearningFramework.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.LearningFrameworkIds, metadataTags);
                    view.LearningDimension.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.LearningDimensionIds, metadataTags);
                    view.LearningArea.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.LearningAreaIds, metadataTags);
                    view.LearningSubArea.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.LearningSubAreaIds, metadataTags);
                    view.TeacherOutcome.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.TeacherOutcomeIds, metadataTags);
                }
            });
        }
    }
}
