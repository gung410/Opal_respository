using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Achievement.Badge
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AchievementBadgePageView
    {
        public AchievementBadgePageView()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            GridItemsLayout.Span = (int)(width / 175);
        }
    }
}
