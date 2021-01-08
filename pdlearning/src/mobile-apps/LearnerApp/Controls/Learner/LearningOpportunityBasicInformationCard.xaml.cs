using System;
using System.Linq;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Effects;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningOpportunityBasicInformationCard : ContentView
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source),
                typeof(LearningOpportunityInformationCardTransfer),
                typeof(LearningOpportunityBasicInformationCard),
                null,
                propertyChanged: OnItemsChanged);

        public static readonly BindableProperty IsScrollingProperty =
            BindableProperty.Create(
                nameof(IsScrolling),
                typeof(bool),
                typeof(LearningOpportunityBasicInformationCard),
                propertyChanged: OnIsScrollingChanged);

        public static readonly BindableProperty OpenCommunityCommandProperty =
            BindableProperty.Create(
                nameof(OpenCommunityCommand),
                typeof(ICommand),
                typeof(LearningOpportunityBasicInformationCard));

        public static readonly BindableProperty CourseNameProperty =
            BindableProperty.Create(
                nameof(CourseName),
                typeof(string),
                typeof(LearningOpportunityBasicInformationCard));

        public static readonly BindableProperty IsVisibleCommunityProperty =
            BindableProperty.Create(
                nameof(IsVisibleCommunity),
                typeof(bool),
                typeof(LearningOpportunityBasicInformationCard),
                null,
                propertyChanged: OnCommunityVisiblechanged);

        public LearningOpportunityBasicInformationCard()
        {
            InitializeComponent();
        }

        public LearningOpportunityInformationCardTransfer Source
        {
            get { return (LearningOpportunityInformationCardTransfer)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool IsScrolling
        {
            get => (bool)GetValue(IsScrollingProperty);
            set
            {
                SetValue(IsScrollingProperty, value);
            }
        }

        public ICommand OpenCommunityCommand
        {
            get { return (ICommand)GetValue(OpenCommunityCommandProperty); }
            set { SetValue(OpenCommunityCommandProperty, value); }
        }

        public string CourseName
        {
            get { return (string)GetValue(CourseNameProperty); }
            set { SetValue(CourseNameProperty, value); }
        }

        public bool IsVisibleCommunity
        {
            get { return (bool)GetValue(IsVisibleCommunityProperty); }
            set { SetValue(IsVisibleCommunityProperty, value); }
        }

        private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is LearningOpportunityInformationCardTransfer item))
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
            var view = (LearningOpportunityBasicInformationCard)bindable;
            view.ToolTip.IsVisible = false;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (courseInfo == null)
                {
                    view.TypeOfPDActivity.Text = GlobalSettings.NotAvailable;
                    view.Cost.Text = GlobalSettings.NotAvailable;
                    view.ModeOfLearning.Text = GlobalSettings.NotAvailable;
                    view.Category.Text = GlobalSettings.NotAvailable;
                    view.CourseCode.Text = GlobalSettings.NotAvailable;
                    view.TraisiCourseCode.Text = GlobalSettings.NotAvailable;
                    view.CourseOutlineStructure.Text = GlobalSettings.NotAvailable;
                    view.ObjectivesOutcomes.Text = GlobalSettings.NotAvailable;
                    view.Community.Text = GlobalSettings.NotAvailable;
                    view.NumberOfViews.Text = GlobalSettings.NotAvailable;
                    view.NumberOfShares.Text = GlobalSettings.NotAvailable;
                }
                else
                {
                    view.TypeOfPDActivity.Text = StringExtension.GetInformation(courseInfo.PdActivityType, metadataTags);
                    view.ToolTip.IsVisible = courseInfo.NotionalCost > 0;
                    view.Cost.Text = courseInfo.GetCost();
                    view.ModeOfLearning.Text = StringExtension.GetInformation(courseInfo.LearningMode, metadataTags);
                    view.Category.Text = StringExtension.GetDataInformationFromMetadatas(courseInfo.CategoryIds, metadataTags);
                    view.CourseCode.Text = string.IsNullOrEmpty(courseInfo.CourseCode) ? GlobalSettings.NotAvailable : courseInfo.CourseCode;
                    view.TraisiCourseCode.Text = string.IsNullOrEmpty(courseInfo.ExternalCode) ? GlobalSettings.NotAvailable : courseInfo.ExternalCode;
                    view.CourseOutlineStructure.Text = string.IsNullOrEmpty(courseInfo.CourseOutlineStructure) ? GlobalSettings.NotAvailable : courseInfo.CourseOutlineStructure;
                    view.ObjectivesOutcomes.Text = string.IsNullOrEmpty(courseInfo.CourseObjective) ? GlobalSettings.NotAvailable : courseInfo.CourseObjective;
                    view.Community.Text = $"Community for {view.CourseName}";
                    view.NumberOfViews.Text = courseInfo.TotalView.ToString();
                    view.NumberOfShares.Text = courseInfo.TotalShare.ToString();
                    view.Community.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = view.OpenCommunityCommand,
                    });
                    view.CommunityStack.IsVisible = view.IsVisibleCommunity;
                }
            });
        }

        private static void OnIsScrollingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (LearningOpportunityBasicInformationCard)bindable;
            var control = view.CostStack.Children.FirstOrDefault(TooltipEffect.GetHasTooltip);
            SetHasTooltip(control);
        }

        private static void SetHasTooltip(BindableObject view)
        {
            if (view == null)
            {
                return;
            }

            TooltipEffect.SetHasTooltip(view, false);
            TooltipEffect.SetHasTooltip(view, true);
        }

        private static void OnCommunityVisiblechanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is bool))
            {
                return;
            }

            var view = (LearningOpportunityBasicInformationCard)bindable;
            Device.BeginInvokeOnMainThread(() =>
            {
                view.CommunityStack.IsVisible = (bool)newValue;
            });
        }

        private void Handle_Tapped(object sender, EventArgs e)
        {
            var view = CostStack.Children.FirstOrDefault(TooltipEffect.GetHasTooltip);
            SetHasTooltip(view);
        }
    }
}
