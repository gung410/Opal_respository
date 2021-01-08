using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.LearnerCustomRepeatableStackView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LearnerCustomRepeatableStackView : ILoading
    {
        public static readonly BindableProperty RealItemsSourceProperty =
            BindableProperty.Create(
                nameof(RealItemsSource),
                typeof(IEnumerable),
                typeof(LearnerCustomRepeatableStackView),
                defaultValue: null,
                propertyChanged: OnRealItemsSourceChanged);

        public static readonly BindableProperty IsLoadingProperty =
            BindableProperty.Create(
                nameof(IsLoading),
                typeof(bool),
                typeof(LearnerCustomRepeatableStackView),
                defaultValue: false,
                propertyChanged: IsLoadingPropertyChanged);

        public LearnerCustomRepeatableStackView()
        {
            InitializeComponent();
        }

        public IEnumerable FinalItemsSource
        {
            get
            {
                if (IsLoading)
                {
                    var list = new List<LoadingItemSkeletonViewModel>();
                    for (int i = 0; i < 3; i++)
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

        private static void OnRealItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var casted = (LearnerCustomRepeatableStackView)bindable;

            if (newValue != oldValue)
            {
                casted.OnPropertyChanged(nameof(FinalItemsSource));
            }
        }

        private static void IsLoadingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var casted = (LearnerCustomRepeatableStackView)bindable;

            if (!newValue.Equals(oldValue))
            {
                casted.OnPropertyChanged(nameof(FinalItemsSource));
            }
        }
    }
}
