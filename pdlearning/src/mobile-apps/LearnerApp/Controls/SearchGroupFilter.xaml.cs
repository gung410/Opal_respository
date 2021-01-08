using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchGroupFilter : ContentView
    {
        public static readonly BindableProperty FilterDataProperty =
            BindableProperty.Create(
                nameof(FilterData),
                typeof(SearchGroupFilterData),
                typeof(SearchGroupFilter),
                null,
                propertyChanged: OnFilterDataChanged);

        public static readonly BindableProperty BackCommandProperty =
            BindableProperty.Create(
                nameof(BackCommand),
                typeof(ICommand),
                typeof(SearchGroupFilter),
                null);

        public static readonly BindableProperty FilterCommandProperty =
            BindableProperty.Create(
                nameof(FilterCommand),
                typeof(ICommand),
                typeof(SearchGroupFilter),
                null);

        private static SearchGroupFilter _view;

        public SearchGroupFilter()
        {
            InitializeComponent();
        }

        public event EventHandler<Dictionary<string, int>> OnFilter;

        public event EventHandler OnBack;

        public SearchGroupFilterData FilterData
        {
            get { return (SearchGroupFilterData)GetValue(FilterDataProperty); }
            set { SetValue(FilterDataProperty, value); }
        }

        public ICommand BackCommand
        {
            get { return (ICommand)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }

        public ICommand FilterCommand
        {
            get { return (ICommand)GetValue(FilterCommandProperty); }
            set { SetValue(FilterCommandProperty, value); }
        }

        private static void OnFilterDataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var dataFilter = newValue as SearchGroupFilterData;

            if (dataFilter == null || dataFilter.Data.Keys.IsNullOrEmpty())
            {
                return;
            }

            _view = (SearchGroupFilter)bindable;

            Device.BeginInvokeOnMainThread(() =>
            {
                _view.GroupFilterWrap.Children.Clear();

                // Search filter
                foreach (var item in dataFilter.Data.Keys)
                {
                    var tapGestureRecognizer = new TapGestureRecognizer
                    {
                        CommandParameter = item
                    };
                    tapGestureRecognizer.Tapped += OnFilterTapped;

                    // Change display text for learning path
                    string label = item;
                    switch (item)
                    {
                        case "Owner":
                            label = "My own";
                            break;
                        case "Shared":
                            label = "Shared from users";
                            break;
                        case "Content":
                            label = "Digital Contents";
                            break;
                        default:
                            break;
                    }

                    // Init frame
                    var frame = new Frame
                    {
                        BackgroundColor = item == dataFilter.CurrentFilter ? Color.FromHex("#8799BA") : Color.FromHex("#d8dce6"),
                        BorderColor = Color.FromHex("#D8DCE6"),
                        CornerRadius = 6,
                        HasShadow = false,
                        Padding = new Thickness(5, 8, 5, 8),
                        Content = new Label
                        {
                            Text = $"{SeparateStringByUppercase.Convert(label)} ({dataFilter.Data[item]})",
                            TextColor = item == dataFilter.CurrentFilter ? Color.White : Color.Gray,
                            FontSize = 11
                        }
                    };
                    frame.GestureRecognizers.Add(tapGestureRecognizer);

                    _view.GroupFilterWrap.Children.Add(frame);
                }

                // Total search count
                int totalSearchCount = dataFilter.Data.Where(p => !p.Key.Contains("All")).Select(p => p.Value).Sum();
                _view.TotalSearchCount.Text = $"{totalSearchCount} {(totalSearchCount == 1 ? "item" : "items")} found";

                // Search back to default
                _view.KeywordLbl.Text = $"Results for <b>{dataFilter.Keyword}</b>";
            });
        }

        private static void OnFilterTapped(object sender, EventArgs e)
        {
            if (_view.FilterData.Data == null || _view.FilterData.Data.Count == 0)
            {
                return;
            }

            var frame = sender as Frame;

            var parent = frame.Parent as WrapLayout;

            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (Frame item in parent.Children)
                {
                    if (item == frame)
                    {
                        item.BackgroundColor = Color.FromHex("#8799BA");
                        (item.Children[0] as Label).TextColor = Color.White;
                    }
                    else
                    {
                        item.BackgroundColor = Color.FromHex("#d8dce6");
                        (item.Children[0] as Label).TextColor = Color.Gray;
                    }
                }
            });

            string currentFilter = (frame.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as string;

            var executeData = new Dictionary<string, int>
            {
                { currentFilter, _view.FilterData.Data[currentFilter] }
            };

            _view.OnFilter?.Invoke(_view, executeData);

            if (_view.FilterCommand != null && _view.FilterCommand.CanExecute(executeData))
            {
                _view.FilterCommand.Execute(executeData);
            }
        }

        private void OnBack_Tapped(object sender, EventArgs e)
        {
            _view.OnBack?.Invoke(_view, null);
            _view.BackCommand?.Execute(null);
        }
    }
}
