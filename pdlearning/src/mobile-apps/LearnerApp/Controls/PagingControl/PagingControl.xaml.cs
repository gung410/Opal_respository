using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.PagingControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PagingControl
    {
        public static readonly BindableProperty TotalPageNumberProperty = BindableProperty.Create(nameof(TotalPageNumber), typeof(int), typeof(PagingControl), propertyChanged: OnTotalPageNumberPropertyChanged);

        public static readonly BindableProperty CurrentPageNumberProperty = BindableProperty.Create(nameof(CurrentPageNumber), typeof(int), typeof(PagingControl), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnCurrentPageNumberPropertyChanged);

        public static readonly BindableProperty OnPageChangedCommandProperty = BindableProperty.Create(nameof(OnPageChangedCommand), typeof(ICommand), typeof(PagingControl));

        public PagingControl()
        {
            InitializeComponent();
        }

        public int TotalPageNumber
        {
            get { return (int)GetValue(TotalPageNumberProperty); }
            set { SetValue(TotalPageNumberProperty, value); }
        }

        public int CurrentPageNumber
        {
            get { return (int)GetValue(CurrentPageNumberProperty); }
            set { SetValue(CurrentPageNumberProperty, value); }
        }

        public ICommand OnPageChangedCommand
        {
            get { return (ICommand)GetValue(OnPageChangedCommandProperty); }
            set { SetValue(OnPageChangedCommandProperty, value); }
        }

        public int SelectedIndex
        {
            get
            {
                return CurrentPageNumber - 1;
            }
        }

        public bool IsPreviousButtonVisible => CurrentPageNumber > 1;

        public bool IsNextButtonVisible => CurrentPageNumber < TotalPageNumber;

        private static void OnCurrentPageNumberPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var pagingControl = (PagingControl)bindable;
            if (newValue != oldValue)
            {
                pagingControl.OnPropertyChanged(nameof(SelectedIndex));
                pagingControl.RecalculatePrevNextButtons();
            }
        }

        private static void OnTotalPageNumberPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var pagingControl = (PagingControl)bindable;
            if (newValue != oldValue)
            {
                pagingControl.SetTotalPageNumber((int)newValue);
                pagingControl.RecalculatePrevNextButtons();
            }
        }

        private void SetTotalPageNumber(int newValue)
        {
            var newSource = new List<int>();
            for (int i = 1; i <= newValue; i++)
            {
                newSource.Add(i);
            }

            Picker.ItemsSource = newSource;
            OnPropertyChanged(nameof(SelectedIndex));
        }

        private void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPageNumber = Picker.SelectedIndex + 1;
            OnPageNumberChanged();
        }

        private void OnBackButtonPressed(object sender, EventArgs e)
        {
            if (CurrentPageNumber <= 1)
            {
                return;
            }

            CurrentPageNumber = CurrentPageNumber - 1;
        }

        private void OnNextButtonTapped(object sender, EventArgs e)
        {
            if (CurrentPageNumber >= TotalPageNumber)
            {
                return;
            }

            CurrentPageNumber = CurrentPageNumber + 1;
        }

        private void OnPageNumberChanged()
        {
            OnPageChangedCommand?.Execute(CurrentPageNumber);
        }

        private void RecalculatePrevNextButtons()
        {
            PickerLabel.Text = TotalPageNumber > 0 ? $"{CurrentPageNumber} / {TotalPageNumber}" : "Loading ...";
            OnPropertyChanged(nameof(IsPreviousButtonVisible));
            OnPropertyChanged(nameof(IsNextButtonVisible));
        }

        private void OnLabelClicked(object sender, EventArgs e)
        {
            Picker.Focus();
        }
    }
}
