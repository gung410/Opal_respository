using System.Collections.Generic;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningOpportunityContentsCard : ContentView
    {
        public static readonly BindableProperty ToCItemsProperty = BindableProperty.Create(
            nameof(ToCItems),
            typeof(List<TableOfContent>),
            typeof(LearningOpportunityContentsCard),
            null,
            propertyChanged: OnToCItemsChanged);

        public static readonly BindableProperty MyLectureInfoItemsProperty = BindableProperty.Create(
            nameof(MyLectureInfoItems),
            typeof(List<MyLecturesInfo>),
            typeof(LearningOpportunityContentsCard),
            null,
            propertyChanged: OnMyLectureInfoItemsChanged);

        public static readonly BindableProperty LearningContentTransferDataProperty = BindableProperty.Create(
            nameof(LearningContentTransfer),
            typeof(LearningContentTransfer),
            typeof(LearningOpportunityContentsCard),
            null,
            propertyChanged: OnLearningContentTransferObjChanged);

        private static List<TableOfContent> _tocTtems;

        private static LearningContentTransfer _learningContentTransfer;

        public LearningOpportunityContentsCard()
        {
            InitializeComponent();

            NavigationService = DependencyService.Resolve<INavigationService>();
        }

        public List<TableOfContent> ToCItems
        {
            get { return (List<TableOfContent>)GetValue(ToCItemsProperty); }
            set { SetValue(ToCItemsProperty, value); }
        }

        public LearningContentTransfer LearningContentTransferData
        {
            get { return (LearningContentTransfer)GetValue(LearningContentTransferDataProperty); }
            set { SetValue(LearningContentTransferDataProperty, value); }
        }

        public List<MyLecturesInfo> MyLectureInfoItems
        {
            get { return (List<MyLecturesInfo>)GetValue(MyLectureInfoItemsProperty); }
            set { SetValue(MyLectureInfoItemsProperty, value); }
        }

        protected INavigationService NavigationService { get; }

        private static void OnLearningContentTransferObjChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is LearningContentTransfer learningContentTransferData))
            {
                return;
            }

            _learningContentTransfer = learningContentTransferData;
        }

        private static void OnToCItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is List<TableOfContent> tocTtems))
            {
                return;
            }

            for (int i = 0; i < tocTtems.Count; i++)
            {
                tocTtems[i].Order = i + 1;
            }

            _tocTtems = tocTtems;
        }

        private static void OnMyLectureInfoItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is List<MyLecturesInfo> myLectureInfoItems))
            {
                return;
            }

            if (_tocTtems.IsNullOrEmpty())
            {
                return;
            }

            if (!myLectureInfoItems.IsNullOrEmpty())
            {
                for (int index = 0; index < _tocTtems.Count; index++)
                {
                    _tocTtems[index].Order = index + 1;

                    if (_tocTtems[index].Type == TableOfContentType.Section)
                    {
                        // In case Section, if existed any item not existed in myLectureInfoItems => that lecture not completed => Color.White
                        if (_tocTtems[index].Items != null && _tocTtems[index].Items.Exists(p => myLectureInfoItems.Exists(g => g.LectureId == p.Id && g.Status == StatusLearning.Completed)))
                        {
                            _tocTtems[index].IsClickable = true;
                            SetSectionCompletedColor(_tocTtems[index]);
                        }
                        else
                        {
                            SetSectionNotCompletedColor(_tocTtems[index]);
                        }
                    }
                    else
                    {
                        // In case Lecture, if that lecture not completed => Color.White
                        if (myLectureInfoItems.Exists(p => p.LectureId == _tocTtems[index].Id && p.Status == StatusLearning.Completed))
                        {
                            _tocTtems[index].IsClickable = true;
                            SetSectionCompletedColor(_tocTtems[index]);
                        }
                        else
                        {
                            SetSectionNotCompletedColor(_tocTtems[index]);
                        }
                    }
                }
            }
            else
            {
                for (int index = 0; index < _tocTtems.Count; index++)
                {
                    _tocTtems[index].Order = index + 1;
                    SetSectionNotCompletedColor(_tocTtems[index]);
                }
            }

            ((LearningOpportunityContentsCard)bindable).Source.ItemsSource = _tocTtems;
        }

        private static void SetSectionCompletedColor(TableOfContent item)
        {
            item.LectureCompletedColor = (Color)Application.Current.Resources["Completed"];
            item.LectureCompletedBorderColor = (Color)Application.Current.Resources["Completed"];
            item.LectureCompletedTextColor = Color.White;
        }

        private static void SetSectionNotCompletedColor(TableOfContent item)
        {
            item.LectureCompletedColor = Color.White;
            item.LectureCompletedBorderColor = Color.FromHex("#D8D8D8");
            item.LectureCompletedTextColor = (Color)Application.Current.Resources["MainBodyTextColor"];
        }

        // Hide for click item in selection
        /*private async void OnItem_Tapped(object sender, System.EventArgs e)
        {
            string id = (e as TappedEventArgs)?.Parameter as string;

            TableOfContent currentSelectedItem = _tocTtems.FirstOrDefault(p => p.Id == id);

            if (currentSelectedItem == null || currentSelectedItem.Type == TableOfContentType.Section || !currentSelectedItem.IsClickable)
            {
                return;
            }

            LearningContentTransfer learnerContentTransfer = new LearningContentTransfer
            {
                Lectures = _learningContentTransfer.Lectures,
                CourseId = _learningContentTransfer.CourseId,
                MyCourseId = _learningContentTransfer.MyCourseId,
                IsCourseCompleted = _learningContentTransfer.IsCourseCompleted,
                ThumbnailUrl = _learningContentTransfer.ThumbnailUrl,
                LectureIndex = _learningContentTransfer.Lectures.FindIndex(p => p.Id == currentSelectedItem.Id),
                CourseName = _learningContentTransfer.CourseName,
                OwnReview = _learningContentTransfer.OwnReview
            };

            var parameters = new NavigationParameters();
            parameters.SetParameter("lectures-data", learnerContentTransfer);

            await NavigationService.NavigateToAsync<LearningContentViewModel>(parameters);
        }*/
    }
}
