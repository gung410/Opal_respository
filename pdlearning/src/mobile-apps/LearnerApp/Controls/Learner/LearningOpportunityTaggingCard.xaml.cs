using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningOpportunityTaggingCard : ContentView
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source),
                typeof(LearningOpportunityCardTransfer),
                typeof(LearningOpportunityTaggingCard),
                null,
                propertyChanged: OnItemsChanged);

        public LearningOpportunityTaggingCard()
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

            var view = (LearningOpportunityTaggingCard)bindable;

            view.LearningOpportunityCopyrightCard.Source = item;
            view.LearningOpportunityCoursePlanningCard.Source = item;
            view.LearningOpportunityMetadataCard.Source = item;
            view.LearningOpportunityTargetAudienceCard.Source = item;
        }
    }
}
