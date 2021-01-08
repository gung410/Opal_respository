using System;
using System.Collections.Generic;
using LearnerApp.Common;
using LearnerApp.Models;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningContentSelectionLectureCard : ContentView
    {
        public static readonly BindableProperty LectureItemsProperty = BindableProperty.Create("LectureItems", typeof(List<TableOfContent>), typeof(LearningContentSelectionLectureCard), null, propertyChanged: OnLectureItemsChanged);

        public static readonly BindableProperty MyLecturesInfoItemsProperty = BindableProperty.Create("MyLecturesInfoItems", typeof(List<MyLecturesInfo>), typeof(LearningContentSelectionLectureCard), null, propertyChanged: OnMyLecturesInfoItemsChanged);

        private static List<TableOfContent> _allLectureItems;

        public LearningContentSelectionLectureCard()
        {
            InitializeComponent();
        }

        public List<TableOfContent> LectureItems
        {
            get { return (List<TableOfContent>)GetValue(LectureItemsProperty); }
            set { SetValue(LectureItemsProperty, value); }
        }

        public List<MyLecturesInfo> MyLecturesInfoItems
        {
            get { return (List<MyLecturesInfo>)GetValue(MyLecturesInfoItemsProperty); }
            set { SetValue(MyLecturesInfoItemsProperty, value); }
        }

        private static void OnMyLecturesInfoItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is List<MyLecturesInfo> myLecturesInfoItems))
            {
                return;
            }

            if (_allLectureItems == null)
            {
                return;
            }

            // 2. Compare and get lecture completed
            List<TableOfContent> lecturesCompleted = new List<TableOfContent>();

            for (int i = 0; i < _allLectureItems.Count; i++)
            {
                _allLectureItems[i].Order = i + 1;

                // Get lecture completed
                if (myLecturesInfoItems.Exists(p => p.LectureId == _allLectureItems[i].Id && p.Status != StatusLearning.Completed))
                {
                    _allLectureItems[i].LectureCompletedColor = Color.White;
                    _allLectureItems[i].LectureCompletedBorderColor = Color.FromHex("#D8D8D8");
                    _allLectureItems[i].LectureCompletedTextColor = (Color)Application.Current.Resources["MainBodyTextColor"];
                }
                else
                {
                    _allLectureItems[i].IsClickable = true;
                    _allLectureItems[i].LectureCompletedColor = (Color)Application.Current.Resources["Completed"];
                    _allLectureItems[i].LectureCompletedBorderColor = (Color)Application.Current.Resources["Completed"];
                    _allLectureItems[i].LectureCompletedTextColor = Color.White;
                }

                lecturesCompleted.Add(_allLectureItems[i]);
            }

            ((LearningContentSelectionLectureCard)bindable).Source.ItemsSource = lecturesCompleted;
        }

        private static void OnLectureItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // 1. Get all lectures of current course
            if (!(newValue is List<TableOfContent> items))
            {
                return;
            }

            _allLectureItems = items;
        }

        private void Source_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            TableOfContent selectedItem = e.SelectedItem as TableOfContent;

            if (!selectedItem.IsClickable)
            {
                return;
            }

            MessagingCenter.Unsubscribe<LearningContentSelectionLectureCard, int>(this, "lecture-selected-item");
            MessagingCenter.Send(this, "lecture-selected-item", _allLectureItems.IndexOf(selectedItem));

            ((ListView)sender).SelectedItem = null;
        }
    }
}
