using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.LoadingSwapView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingSwapView
    {
        public static readonly BindableProperty IsLoadingProperty =
            BindableProperty.Create(
                nameof(IsLoading),
                typeof(bool),
                typeof(LoadingSwapView),
                propertyChanged: IsLoadingPropertyChanged);

        public static readonly BindableProperty ActiveViewProperty =
            BindableProperty.Create(
                nameof(ActiveView),
                typeof(View),
                typeof(LoadingSwapView),
                defaultValue: null,
                propertyChanged: ActiveViewPropertyChanged);

        public static readonly BindableProperty LoadingViewProperty =
            BindableProperty.Create(
                nameof(LoadingView),
                typeof(View),
                typeof(LoadingSwapView),
                defaultValue: null,
                propertyChanged: LoadingViewPropertyChanged);

        public LoadingSwapView()
        {
            InitializeComponent();
        }

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set
            {
                SetValue(IsLoadingProperty, value);
            }
        }

        public bool ShowingRealContent => !IsLoading;

        public View ActiveView
        {
            get => (View)GetValue(ActiveViewProperty);
            set
            {
                SetValue(ActiveViewProperty, value);
            }
        }

        public View LoadingView
        {
            get => (View)GetValue(LoadingViewProperty);
            set
            {
                SetValue(LoadingViewProperty, value);
            }
        }

        public View FinalView
        {
            get
            {
                return IsLoading ? LoadingView : ActiveView;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (ActiveView != null)
            {
                ActiveView.BindingContext = BindingContext;
            }

            if (LoadingView != null)
            {
                LoadingView.BindingContext = BindingContext;
            }
        }

        private static void IsLoadingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var casted = (LoadingSwapView)bindable;
        }

        private static void ActiveViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var casted = (LoadingSwapView)bindable;
            if (casted.ActiveView != null)
            {
                casted.ActiveView.BindingContext = casted.BindingContext;
                casted.ActiveLayout.Content = casted.ActiveView;
            }
        }

        private static void LoadingViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var casted = (LoadingSwapView)bindable;
            if (casted.LoadingView != null)
            {
                casted.LoadingView.BindingContext = casted.BindingContext;
                casted.LoadingLayout.Content = casted.LoadingView;
            }
        }
    }
}
