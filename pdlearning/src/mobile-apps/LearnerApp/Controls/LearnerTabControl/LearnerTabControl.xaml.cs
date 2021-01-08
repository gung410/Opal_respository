using System;
using System.Linq;
using System.Windows.Input;
using LearnerApp.Controls.LearnerObservableCollection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.LearnerTabControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LearnerTabControl : ContentView
    {
        public static BindableProperty ItemSourceProperty =
            BindableProperty.Create(
                nameof(ItemSource),
                typeof(LearnerObservableCollection<LearnerTabControlItemViewModel>),
                typeof(LearnerTabControl),
                null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: ItemsChanged);

        public LearnerTabControl()
        {
            InitializeComponent();
        }

        public event EventHandler<LearnerTabControlItemViewModel> OnTabClicked;

        public LearnerObservableCollection<LearnerTabControlItemViewModel> ItemSource
        {
            get { return (LearnerObservableCollection<LearnerTabControlItemViewModel>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public ICommand InternalTabClickCommand => new Command(InternalTabClicked);

        public async void ScrollToSelectedTab()
        {
            var selectedItem = ItemSource.FirstOrDefault(x => x.IsSelected);
            if (selectedItem == null)
            {
                return;
            }

            var selectedView = this.RepeatableStack.Children[ItemSource.IndexOf(selectedItem)];

            await this.ScrollView.ScrollToAsync(selectedView, ScrollToPosition.End, true);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            double widthTab = width / 3;

            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var child in this.RepeatableStack.Children)
                {
                    child.WidthRequest = widthTab;
                }
            });
        }

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LearnerTabControl learnerTabControl)
            {
                learnerTabControl.ItemSource = (LearnerObservableCollection<LearnerTabControlItemViewModel>)newValue;
            }
        }

        private void InternalTabClicked(object obj)
        {
            var item = (LearnerTabControlItemViewModel)obj;

            OnTabClicked?.Invoke(this, item);
        }
    }
}
