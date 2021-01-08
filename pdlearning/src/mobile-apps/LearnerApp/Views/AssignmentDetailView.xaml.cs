using System;
using System.Threading.Tasks;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class AssignmentDetailView
    {
        public AssignmentDetailView()
        {
            InitializeComponent();

            ViewModel.RequestGoToCommentList += async () =>
            {
                await Task.Delay(200); // Wait for 200ms before starting to scroll to wait for the layout finish update
                Comment_Clicked(null, null);
            };
        }

        private AssignmentDetailViewModel ViewModel => (AssignmentDetailViewModel)BindingContext;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            double widthTab = width / 2;

            Device.BeginInvokeOnMainThread(() =>
            {
                InformationTab.WidthRequest = widthTab;
                CommentTab.WidthRequest = widthTab;
            });
        }

        private async void InformationTab_Clicked(object sender, EventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                InformationTabLine.IsVisible = true;
                CommentTabLine.IsVisible = false;

                await ScrollStack.ScrollToAsync(InformationStack, ScrollToPosition.Start, true);
            });
        }

        private async void Comment_Clicked(object sender, EventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                InformationTabLine.IsVisible = false;
                CommentTabLine.IsVisible = true;

                var commentStackPosStart = ScrollStack.GetScrollPositionForElement(CommentStack, ScrollToPosition.Start).Y;
                var commentStackPosEnd = commentStackPosStart + CommentStack.Height;

                var scrollY = commentStackPosEnd;

                scrollY = scrollY - ScrollStack.Height;

                if (scrollY > 0)
                {
                    await ScrollStack.ScrollToAsync(0, scrollY, true);
                }
            });
        }
    }
}
