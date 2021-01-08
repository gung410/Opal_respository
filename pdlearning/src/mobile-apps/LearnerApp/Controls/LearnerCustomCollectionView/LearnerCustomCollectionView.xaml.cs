using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.LearnerCustomCollectionView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LearnerCustomCollectionView : ILoading
    {
        public static readonly BindableProperty RealItemsSourceProperty =
            BindableProperty.Create(
                nameof(RealItemsSource),
                typeof(IEnumerable),
                typeof(LearnerCustomCollectionView),
                defaultValue: null,
                propertyChanged: OnRealItemsSourceChanged);

        public static readonly BindableProperty IsLoadingProperty =
            BindableProperty.Create(
                nameof(IsLoading),
                typeof(bool),
                typeof(LearnerCustomCollectionView),
                propertyChanged: IsLoadingPropertyChanged);

        public static readonly BindableProperty LoadMoreViewProperty =
            BindableProperty.Create(
                nameof(LoadMoreView),
                typeof(DataTemplate),
                typeof(LearnerCustomCollectionView),
                defaultValue: null);

        public static readonly BindableProperty SkeletonViewNumberProperty =
            BindableProperty.Create(
                nameof(SkeletonViewNumber),
                typeof(int),
                typeof(LearnerCustomCollectionView),
                defaultValue: 3);

        public LearnerCustomCollectionView()
        {
            InitializeComponent();
        }

        public LearnerCustomCollectionViewDataTemplateSelector LoadMoreViewTemplateSelector => new LearnerCustomCollectionViewDataTemplateSelector(this);

        public IEnumerable FinalItemsSource
        {
            get
            {
                if (IsLoading)
                {
                    var list = new List<LoadingItemSkeletonViewModel>();
                    for (int i = 0; i < SkeletonViewNumber; i++)
                    {
                        list.Add(new LoadingItemSkeletonViewModel());
                    }

                    return list;
                }
                else
                {
                    return RealItemsSource;
                }
            }
        }

        public IEnumerable RealItemsSource
        {
            get => (IEnumerable)GetValue(RealItemsSourceProperty);
            set
            {
                SetValue(RealItemsSourceProperty, value);
            }
        }

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set
            {
                SetValue(IsLoadingProperty, value);
            }
        }

        public int SkeletonViewNumber
        {
            get => (int)GetValue(SkeletonViewNumberProperty);
            set
            {
                SetValue(SkeletonViewNumberProperty, value);
            }
        }

        public DataTemplate LoadMoreView
        {
            get => (DataTemplate)GetValue(LoadMoreViewProperty);
            set
            {
                SetValue(LoadMoreViewProperty, value);
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(RemainingItemsThreshold))
            {
                OnPropertyChanged(nameof(FooterTemplate));
            }
        }

        private static void OnRealItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var casted = (LearnerCustomCollectionView)bindable;

            if (newValue != oldValue)
            {
                casted.OnPropertyChanged(nameof(FinalItemsSource));
            }
        }

        private static void IsLoadingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var casted = (LearnerCustomCollectionView)bindable;

            if (!newValue.Equals(oldValue))
            {
                casted.OnPropertyChanged(nameof(FinalItemsSource));
            }
        }
    }

    public class LearnerCustomCollectionViewDataTemplateSelector : DataTemplateSelector
    {
        private readonly LearnerCustomCollectionView _customCollectionView;

        public LearnerCustomCollectionViewDataTemplateSelector(LearnerCustomCollectionView customCollectionView)
        {
            _customCollectionView = customCollectionView;
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (_customCollectionView.RemainingItemsThreshold == -1)
            {
                return new DataTemplate(() => new ContentView());
            }
            else
            {
                return _customCollectionView.LoadMoreView;
            }
        }
    }
}
