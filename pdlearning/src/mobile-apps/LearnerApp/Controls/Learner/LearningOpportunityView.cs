using System;
using System.Collections.Generic;
using System.Windows.Input;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public class LearningOpportunityView : ContentView
    {
        public static readonly BindableProperty BookmarkTappedCommandProperty =
            BindableProperty.Create(
                nameof(BookmarkTappedCommand),
                typeof(ICommand),
                typeof(LearningOpportunityView),
                null);

        public static readonly BindableProperty BookmarkTappedCommandParameterProperty =
            BindableProperty.Create(
                nameof(BookmarkTappedCommandParameter),
                typeof(object),
                typeof(LearningOpportunityView),
                null);

        public static readonly BindableProperty ItemTappedCommandProperty =
            BindableProperty.Create(
                nameof(ItemTappedCommand),
                typeof(ICommand),
                typeof(LearningOpportunityView),
                null);

        public static readonly BindableProperty ItemTappedCommandParameterProperty =
            BindableProperty.Create(
                nameof(ItemTappedCommandParameter),
                typeof(object),
                typeof(LearningOpportunityView),
                null);

        public static BindableProperty StatusProperty =
             BindableProperty.Create(
                 nameof(Status),
                 typeof(string),
                 typeof(LearningOpportunityView),
                 string.Empty);

        public static BindableProperty CourseIdProperty =
             BindableProperty.Create(
                 nameof(CourseId),
                 typeof(string),
                 typeof(LearningOpportunityView),
                 string.Empty);

        public static BindableProperty ViewIdProperty =
             BindableProperty.Create(
                 nameof(ViewId),
                 typeof(string),
                 typeof(LearningOpportunityView),
                 string.Empty);

        public static BindableProperty HasBookmarkProperty =
             BindableProperty.Create(
                 nameof(HasBookmark),
                 typeof(bool),
                 typeof(LearningOpportunityView),
                 false,
                 BindingMode.TwoWay,
                 propertyChanged: HasBookmarkPropertyChanged);

        public static BindableProperty DurationMinutesProperty =
             BindableProperty.Create(
                 nameof(DurationMinutes),
                 typeof(int),
                 typeof(LearningOpportunityView),
                 0);

        public static BindableProperty CourseCodeProperty =
             BindableProperty.Create(
                 nameof(CourseCode),
                 typeof(string),
                 typeof(LearningOpportunityView),
                 string.Empty);

        public static BindableProperty CourseNameProperty =
            BindableProperty.Create(
                nameof(CourseName),
                typeof(string),
                typeof(LearningOpportunityView),
                string.Empty);

        public static BindableProperty BackgroundImageSourceProperty =
            BindableProperty.Create(
                nameof(BackgroundImageSource),
                typeof(string),
                typeof(LearningOpportunityView),
                string.Empty);

        public static BindableProperty ProgressMeasureProperty =
          BindableProperty.Create(
              nameof(ProgressMeasure),
              typeof(double),
              typeof(LearningOpportunityView));

        public static BindableProperty RatingNumberProperty =
          BindableProperty.Create(
              nameof(RatingNumber),
              typeof(double),
              typeof(LearningOpportunityView),
              0.0);

        public static BindableProperty ReviewsCountProperty =
          BindableProperty.Create(
              nameof(ReviewsCount),
              typeof(int),
              typeof(LearningOpportunityView),
              0);

        public static BindableProperty ThumbnailUrlProperty =
          BindableProperty.Create(
              nameof(ThumbnailUrl),
              typeof(string),
              typeof(LearningOpportunityView),
              string.Empty);

        public static BindableProperty TagsProperty =
          BindableProperty.Create(
              nameof(Tags),
              typeof(List<string>),
              typeof(LearningOpportunityView));

        private readonly Debouncer _debouncer = new Debouncer();

        public event EventHandler<BookmarkChangedEventArgs> BookmarkChanged;

        public ICommand BookmarkTappedCommand
        {
            get { return (ICommand)GetValue(BookmarkTappedCommandProperty); }
            set { SetValue(BookmarkTappedCommandProperty, value); }
        }

        public object BookmarkTappedCommandParameter
        {
            get { return GetValue(BookmarkTappedCommandParameterProperty); }
            set { SetValue(BookmarkTappedCommandParameterProperty, value); }
        }

        public ICommand ItemTappedCommand
        {
            get { return (ICommand)GetValue(ItemTappedCommandProperty); }
            set { SetValue(ItemTappedCommandProperty, value); }
        }

        public object ItemTappedCommandParameter
        {
            get { return GetValue(ItemTappedCommandParameterProperty); }
            set { SetValue(ItemTappedCommandParameterProperty, value); }
        }

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public string CourseId
        {
            get { return (string)GetValue(CourseIdProperty); }
            set { SetValue(CourseIdProperty, value); }
        }

        public string ViewId
        {
            get { return (string)GetValue(ViewIdProperty); }
            set { SetValue(ViewIdProperty, value); }
        }

        public bool HasBookmark
        {
            get { return (bool)GetValue(HasBookmarkProperty); }
            set { SetValue(HasBookmarkProperty, value); }
        }

        public int DurationMinutes
        {
            get { return (int)GetValue(DurationMinutesProperty); }
            set { SetValue(DurationMinutesProperty, value); }
        }

        public int ReviewsCount
        {
            get { return (int)GetValue(ReviewsCountProperty); }
            set { SetValue(ReviewsCountProperty, value); }
        }

        public string CourseCode
        {
            get { return (string)GetValue(CourseCodeProperty); }
            set { SetValue(CourseCodeProperty, value); }
        }

        public string CourseName
        {
            get { return (string)GetValue(CourseNameProperty); }
            set { SetValue(CourseNameProperty, value); }
        }

        public string BackgroundImageSource
        {
            get { return (string)GetValue(BackgroundImageSourceProperty); }
            set { SetValue(BackgroundImageSourceProperty, value); }
        }

        public double ProgressMeasure
        {
            get { return (double)GetValue(ProgressMeasureProperty); }
            set { SetValue(ProgressMeasureProperty, value); }
        }

        public double RatingNumber
        {
            get { return (double)GetValue(RatingNumberProperty); }
            set { SetValue(RatingNumberProperty, value); }
        }

        public string ThumbnailUrl
        {
            get { return (string)GetValue(ThumbnailUrlProperty); }
            set { SetValue(ThumbnailUrlProperty, value); }
        }

        public List<string> Tags
        {
            get { return (List<string>)GetValue(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }

        public void OnBookmarkTapped(object sender, EventArgs e)
        {
            if (!(e is TappedEventArgs))
            {
                return;
            }

            _debouncer.Debouce(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    HasBookmark = !HasBookmark;
                });
            });
        }

        public void OnItemTapped(object sender, EventArgs e)
        {
            if (!Status.Equals(nameof(StatusLearning.Expired)))
            {
                var command = ItemTappedCommand;
                if (command != null)
                {
                    var commandParameter = ItemTappedCommandParameter;
                    if (command.CanExecute(commandParameter))
                    {
                        command.Execute(commandParameter);
                    }
                }
            }
        }

        protected virtual void OnBookmarkChanged(EventArgs args)
        {
        }

        private static void HasBookmarkPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (LearningOpportunityView)bindable;
            var command = view.BookmarkTappedCommand;
            if (command != null)
            {
                var commandParameter = view.BookmarkTappedCommandParameter;
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }

            var args = new BookmarkChangedEventArgs(oldValue, newValue);
            view.BookmarkChanged?.Invoke(view, args);
            view.OnBookmarkChanged(args);
        }
    }
}
