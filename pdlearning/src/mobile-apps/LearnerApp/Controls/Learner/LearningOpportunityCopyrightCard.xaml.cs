using System.Linq;
using LearnerApp.Common;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningOpportunityCopyrightCard : ContentView
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source),
                typeof(LearningOpportunityCardTransfer),
                typeof(LearningOpportunityCopyrightCard),
                null,
                propertyChanged: OnItemsChanged);

        public LearningOpportunityCopyrightCard()
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

            var view = (LearningOpportunityCopyrightCard)bindable;
            var courseInfo = item.CourseExtendedInformation;
            var userInformations = item.UserInformations;

            if (courseInfo == null)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                if (userInformations == null)
                {
                    view.AllowPersonalDownload.IsVisible = false;
                    view.AllowNonCommerInMOEReuseWithModification.IsVisible = false;
                    view.AllowNonCommerReuseWithModification.IsVisible = false;
                    view.AllowNonCommerInMoeReuseWithoutModification.IsVisible = false;
                    view.AllowNonCommerReuseWithoutModification.IsVisible = false;

                    view.CopyrightOwner.Text = GlobalSettings.NotAvailable;
                    view.AcknowledgementAndCredit.Text = GlobalSettings.NotAvailable;
                    view.Remarks.Text = GlobalSettings.NotAvailable;
                    view.CourseContentCreator.Text = GlobalSettings.NotAvailable;
                }
                else
                {
                    view.AllowPersonalDownload.IsVisible = courseInfo.AllowPersonalDownload;
                    view.AllowNonCommerInMOEReuseWithModification.IsVisible = courseInfo.AllowNonCommerInMOEReuseWithModification;
                    view.AllowNonCommerReuseWithModification.IsVisible = courseInfo.AllowNonCommerReuseWithModification;
                    view.AllowNonCommerInMoeReuseWithoutModification.IsVisible = courseInfo.AllowNonCommerInMoeReuseWithoutModification;
                    view.AllowNonCommerReuseWithoutModification.IsVisible = courseInfo.AllowNonCommerReuseWithoutModification;

                    view.CopyrightOwner.Text = string.IsNullOrEmpty(courseInfo.CopyrightOwner) ? GlobalSettings.NotAvailable : courseInfo.CopyrightOwner;
                    view.AcknowledgementAndCredit.Text = string.IsNullOrEmpty(courseInfo.AcknowledgementAndCredit) ? GlobalSettings.NotAvailable : courseInfo.AcknowledgementAndCredit;
                    view.Remarks.Text = string.IsNullOrEmpty(courseInfo.Remarks) ? GlobalSettings.NotAvailable : courseInfo.Remarks;

                    var collaborativeContentCreatorIds = userInformations.Select(user => user.FullName).ToList();
                    view.CourseContentCreator.Text = StringExtension.GetInformationFromList(collaborativeContentCreatorIds);
                }
            });
        }
    }
}
