using LearnerApp.Common;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningOpportunityTargetAudienceCard : ContentView
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source),
                typeof(LearningOpportunityCardTransfer),
                typeof(LearningOpportunityTargetAudienceCard),
                null,
                propertyChanged: OnItemsChanged);

        public LearningOpportunityTargetAudienceCard()
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

            var view = (LearningOpportunityTargetAudienceCard)bindable;
            var courseInfo = item.CourseExtendedInformation;
            var prerequisiteCourses = item.PrerequisiteCourses;
            var metadataTags = item.MetadataTags;
            var departments = item.Departments;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (courseInfo == null)
                {
                    view.PreRequisite.Text = GlobalSettings.NotAvailable;

                    // Departments
                    view.DivisionStack.IsVisible = false;
                    view.BranchStack.IsVisible = false;
                    view.ZoneStack.IsVisible = false;
                    view.ClusterStack.IsVisible = false;
                    view.SchoolStack.IsVisible = false;

                    // Metatdata
                    view.Track.Text = GlobalSettings.NotAvailable;
                    view.DevelopmentalRole.Text = GlobalSettings.NotAvailable;
                    view.TeachingLevel.Text = GlobalSettings.NotAvailable;
                    view.TeachingSubject.Text = GlobalSettings.NotAvailable;
                    view.JobFamily.Text = GlobalSettings.NotAvailable;
                    view.TeachingCourseOfStudy.Text = GlobalSettings.NotAvailable;
                    view.CocurricularActivity.Text = GlobalSettings.NotAvailable;
                    view.EASSubstantiveGradeBanding.Text = GlobalSettings.NotAvailable;
                }
                else
                {
                    view.PreRequisite.Text = StringExtension.GetDataInformationFromPrerequisiteCourses(courseInfo.PrerequisiteCourseIds, prerequisiteCourses);

                    // Departments
                    if (GlobalSettings.DepartmentPlaceOfWork.Equals(courseInfo.PlaceOfWork))
                    {
                        view.Division.Text = StringExtension.GetDataInformationFromDepartments(courseInfo.ApplicableDivisionIds, departments);
                        view.Branch.Text = StringExtension.GetDataInformationFromDepartments(courseInfo.ApplicableBranchIds, departments);
                        view.Zone.Text = StringExtension.GetDataInformationFromDepartments(courseInfo.ApplicableZoneIds, departments);
                        view.Cluster.Text = StringExtension.GetDataInformationFromDepartments(courseInfo.ApplicableClusterIds, departments);
                        view.School.Text = StringExtension.GetDataInformationFromDepartments(courseInfo.ApplicableSchoolIds, departments);
                    }
                    else
                    {
                        view.DivisionStack.IsVisible = false;
                        view.BranchStack.IsVisible = false;
                        view.ZoneStack.IsVisible = false;
                        view.ClusterStack.IsVisible = false;
                        view.SchoolStack.IsVisible = false;
                    }

                    // Metatdata
                    view.Track.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.TrackIds, metadataTags);
                    view.DevelopmentalRole.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.DevelopmentalRoleIds, metadataTags);
                    view.TeachingLevel.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.TeachingLevels, metadataTags);
                    view.TeachingSubject.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.TeachingSubjectIds, metadataTags);
                    view.JobFamily.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.JobFamily, metadataTags);
                    view.TeachingCourseOfStudy.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.TeachingCourseStudyIds, metadataTags);
                    view.CocurricularActivity.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.CocurricularActivityIds, metadataTags);
                    view.EASSubstantiveGradeBanding.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.EasSubstantiveGradeBandingIds, metadataTags);
                }
            });
        }
    }
}
