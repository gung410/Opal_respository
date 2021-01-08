using System.Collections.Generic;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.Learner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LearningOpportunityAssignmentCard : ContentView
    {
        public static readonly BindableProperty AssignmentDetailsProperty = BindableProperty.Create(
            nameof(AssignmentDetails),
            typeof(List<AssignmentDetail>),
            typeof(LearningOpportunityAssignmentCard),
            null,
            propertyChanged: OnPropertyChanged);

        private static INavigationService _navigationService;
        private static List<AssignmentDetail> _assignmentInfos;
        private static LearningOpportunityAssignmentCard _view;

        public LearningOpportunityAssignmentCard()
        {
            InitializeComponent();
            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        public List<AssignmentDetail> AssignmentDetails
        {
            get { return (List<AssignmentDetail>)GetValue(AssignmentDetailsProperty); }
            set { SetValue(AssignmentDetailsProperty, value); }
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is List<AssignmentDetail> assignmentDetails))
            {
                return;
            }

            _view = (LearningOpportunityAssignmentCard)bindable;
            _assignmentInfos = assignmentDetails;
            _view.EmpltListLbl.IsVisible = _assignmentInfos.IsNullOrEmpty();
            _view.Source.ItemsSource = _assignmentInfos;
        }

        private async void OnItem_Tapped(object sender, System.EventArgs e)
        {
            var assignment = (e as TappedEventArgs)?.Parameter as AssignmentDetail;
            await _navigationService.NavigateToAsync<AssignmentDetailViewModel>(
                AssignmentDetailViewModel.GetNavigationParameters(assignment));
        }

        private async void GotoComments_Tapped(object sender, System.EventArgs e)
        {
            var assignment = (e as TappedEventArgs)?.Parameter as AssignmentDetail;
            await _navigationService.NavigateToAsync<AssignmentDetailViewModel>(
                AssignmentDetailViewModel.GetNavigationParameters(assignment, true));
        }
    }
}
