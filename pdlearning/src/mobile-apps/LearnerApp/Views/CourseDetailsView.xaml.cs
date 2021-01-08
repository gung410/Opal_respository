using System;
using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerTabControl;
using LearnerApp.Models.Course;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class CourseDetailsView
    {
        private readonly CourseDetailsViewModel _viewModel;
        private List<Tuple<Layout, LearnerTabControlItemViewModel>> _priorityStack;
        private double _previousScrollY;
        private bool _scrollingBusy;

        public CourseDetailsView()
        {
            InitializeComponent();

            _viewModel = (CourseDetailsViewModel)BindingContext;
            _priorityStack = new List<Tuple<Layout, LearnerTabControlItemViewModel>>()
            {
                new Tuple<Layout, LearnerTabControlItemViewModel>(this.AboutStack, _viewModel.AboutTab),
                new Tuple<Layout, LearnerTabControlItemViewModel>(this.ClassRunStack, _viewModel.ClassRunTab),
                new Tuple<Layout, LearnerTabControlItemViewModel>(this.ContentStack, _viewModel.ContentTab),
                new Tuple<Layout, LearnerTabControlItemViewModel>(this.AssignmentStack, _viewModel.AssignmentTab),
                new Tuple<Layout, LearnerTabControlItemViewModel>(this.ReviewsStack, _viewModel.ReviewTab),
            };
        }

        private async void LearningTabControl_OnOnTabClicked(object sender, LearnerTabControlItemViewModel e)
        {
            ContentView view = null;
            _viewModel.ClearTabSelected();
            e.IsSelected = true;
            switch (e.Id)
            {
                case CourseDetailsViewModel.AboutTabId:
                    view = AboutStack;
                    break;
                case CourseDetailsViewModel.ClassRunTabId:
                    view = ClassRunStack;
                    break;
                case CourseDetailsViewModel.ContentTabId:
                    view = ContentStack;
                    break;
                case CourseDetailsViewModel.AssignTabId:
                    view = AssignmentStack;
                    break;
                case CourseDetailsViewModel.ReviewTabId:
                    view = ReviewsStack;
                    break;
            }

            _scrollingBusy = true;
            await ScrollStack.ScrollToAsync(view, ScrollToPosition.Start, true);
            _scrollingBusy = false;
        }

        private void ScrollStack_OnScrolled(object sender, ScrolledEventArgs e)
        {
            _viewModel.IsScrolling = true;

            if (_scrollingBusy)
            {
                _previousScrollY = e.ScrollY;
                _viewModel.IsScrolling = false;
                return;
            }

            var size = ((View)ScrollStack.Parent).Height;
            var scrollY = ScrollStack.ScrollY;
            var currentDisplaySizeStart = scrollY;
            var currentDisplaySizeEnd = scrollY + size;
            var deltaY = e.ScrollY - _previousScrollY;

            List<Tuple<Layout, LearnerTabControlItemViewModel>> priorityStack = _priorityStack
                .Where(x => x.Item2.IsVisible)
                .ToList();

            // Scroll down
            if (deltaY > 0)
            {
                priorityStack.Reverse();
            }

            _previousScrollY = e.ScrollY;

            _viewModel.IsScrolling = false;

            foreach (var priorityView in priorityStack)
            {
                double startPointY = priorityView.Item1.Y;
                double endPointY = priorityView.Item1.Y + priorityView.Item1.Height;

                if (deltaY > 0)
                {
                    startPointY += 100;
                }

                if (!(currentDisplaySizeStart <= endPointY) || !(currentDisplaySizeEnd >= startPointY))
                {
                    continue;
                }

                // If current tab is already selected, ignore
                if (priorityView.Item2.IsSelected)
                {
                    return;
                }

                _viewModel.ClearTabSelected();
                priorityView.Item2.IsSelected = true;
                TabControl.ScrollToSelectedTab();

                return;
            }
        }

        private async void About_ShowMore_Tapped(object sender, EventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                bool isVisible = _viewModel.IsVisibleAboutInformationStack;
                _viewModel.IsVisibleAboutInformationStack = !isVisible;

                if (!isVisible)
                {
                    await _viewModel.GetCourseDetailDataForContentView();
                }

                AboutShowMoreBtn.Text = isVisible ? "SHOW MORE" : "SHOW LESS";
            });
        }

        private void LearningStatus_Clicked(object sender, EventArgs e)
        {
            _viewModel.EnrollCommand.Execute(null);
        }

        private void LearningOpportunityClassRunCardView_OnSetLearningStatus(object sender, EventArgs e)
        {
            switch (sender)
            {
                case RegistrationStatus registrationStatus:
                    {
                        _viewModel.MyCourseStatus = new MyCourseStatus
                        {
                            Status = _viewModel.MyCourseStatus.Status,
                            MyRegistrationStatus = registrationStatus.ToString(),
                            MyWithdrawalStatus = _viewModel.MyCourseStatus.MyWithdrawalStatus,
                            MyCourseDisplayStatus = registrationStatus.ToString(),
                            IsNominated = _viewModel.MyCourseStatus.IsNominated,
                            IsAddToPlan = false,
                            IsMicroLearningType = _viewModel.IsMicroLearningType,
                            ClassRunStatus = _viewModel.ClassRunInProgress?.Status ?? ClassRunStatus.None,
                            IsVisibleLearningStatus = _viewModel.IsVisibleLearningStatus,
                            LearningStatus = _viewModel.LearningStatus,
                            IsCourseCompleted = _viewModel.IsCourseCompleted,
                            IsTableOfContentEmpty = _viewModel.IsTableOfContentEmpty
                        };

                        if (registrationStatus == RegistrationStatus.PendingConfirmation || registrationStatus == RegistrationStatus.WaitlistConfirmed)
                        {
                            _viewModel.LearningStatus = CourseLearningStatus.REMOVEFROMPDPLAN.ToString();
                        }
                        else if (registrationStatus == RegistrationStatus.Approved)
                        {
                            _viewModel.LearningStatus = CourseLearningStatus.REMOVEFROMPDPLAN.ToString();
                            MessagingCenter.Unsubscribe<CourseDetailsViewModel>(this, "on-reload-course-details");
                            MessagingCenter.Send(this, "on-reload-course-details");
                        }
                        else if (registrationStatus == RegistrationStatus.ConfirmedByCA)
                        {
                            MessagingCenter.Unsubscribe<CourseDetailsViewModel>(this, "on-reload-course-details");
                            MessagingCenter.Send(this, "on-reload-course-details");
                        }

                        break;
                    }

                case WithdrawalStatus withdrawal:

                    var withdrawalStatus = sender.ToString() switch
                    {
                        nameof(WithdrawalStatus.PendingConfirmation) => MyCourseDisplayStatus
                            .WithdrawalPendingConfirmation.ToString(),
                        nameof(WithdrawalStatus.Rejected) => MyCourseDisplayStatus.WithdrawalRejected.ToString(),
                        nameof(WithdrawalStatus.Approved) => MyCourseDisplayStatus.WithdrawalApproved.ToString(),
                        _ => string.Empty
                    };

                    _viewModel.MyCourseStatus = new MyCourseStatus
                    {
                        Status = _viewModel.MyCourseStatus.Status,
                        MyRegistrationStatus = _viewModel.MyCourseStatus.MyRegistrationStatus,
                        MyWithdrawalStatus = sender.ToString(),
                        MyCourseDisplayStatus = withdrawalStatus,
                        IsNominated = _viewModel.MyCourseStatus.IsNominated,
                        IsAddToPlan = false,
                        IsMicroLearningType = _viewModel.IsMicroLearningType,
                        ClassRunStatus = _viewModel.ClassRunInProgress?.Status ?? ClassRunStatus.None,
                        IsVisibleLearningStatus = _viewModel.IsVisibleLearningStatus,
                        LearningStatus = _viewModel.LearningStatus,
                        IsCourseCompleted = _viewModel.IsCourseCompleted,
                        IsTableOfContentEmpty = _viewModel.IsTableOfContentEmpty
                    };

                    if (withdrawal == WithdrawalStatus.Approved)
                    {
                        MessagingCenter.Unsubscribe<CourseDetailsViewModel>(this, "on-reload-course-details");
                        MessagingCenter.Send(this, "on-reload-course-details");
                    }

                    break;

                case ClassRunChangeStatus classRunChange:

                    var classRunChangeStatus = sender.ToString() switch
                    {
                        nameof(ClassRunChangeStatus.PendingConfirmation) => MyCourseDisplayStatus
                            .ClassRunChangePendingConfirmation.ToString(),
                        nameof(ClassRunChangeStatus.Rejected) => MyCourseDisplayStatus.ClassRunChangeRejected.ToString(),
                        nameof(ClassRunChangeStatus.Approved) => MyCourseDisplayStatus.ClassRunChangeApproved.ToString(),
                        nameof(ClassRunChangeStatus.ConfirmedByCA) => MyCourseDisplayStatus.ClassRunChangeConfirmedByCA.ToString(),
                        nameof(ClassRunChangeStatus.RejectedByCA) => MyCourseDisplayStatus.RejectedByCA.ToString(),
                        _ => string.Empty
                    };

                    _viewModel.MyCourseStatus = new MyCourseStatus
                    {
                        Status = _viewModel.MyCourseStatus.Status,
                        MyRegistrationStatus = _viewModel.MyCourseStatus.MyRegistrationStatus,
                        MyWithdrawalStatus = _viewModel.MyCourseStatus.MyWithdrawalStatus,
                        MyCourseDisplayStatus = classRunChangeStatus,
                        IsNominated = _viewModel.MyCourseStatus.IsNominated,
                        IsAddToPlan = false,
                        IsMicroLearningType = _viewModel.IsMicroLearningType,
                        ClassRunStatus = _viewModel.ClassRunInProgress?.Status ?? ClassRunStatus.None,
                        IsVisibleLearningStatus = _viewModel.IsVisibleLearningStatus,
                        LearningStatus = _viewModel.LearningStatus,
                        IsCourseCompleted = _viewModel.IsCourseCompleted,
                        IsTableOfContentEmpty = _viewModel.IsTableOfContentEmpty
                    };

                    if (classRunChange == ClassRunChangeStatus.Approved)
                    {
                        MessagingCenter.Unsubscribe<CourseDetailsViewModel>(this, "on-reload-course-details");
                        MessagingCenter.Send(this, "on-reload-course-details");
                    }

                    break;
            }
        }
    }
}
