using System;
using System.Collections.Generic;
using System.Linq;
using FFImageLoading.Svg.Forms;
using LearnerApp.Common;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DropDownSelectionPopup
    {
        public EventHandler<string> OnItemSelectedEventHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownSelectionPopup"/> class.
        /// </summary>
        /// <param name="items">Dictionary key is item's name and value is item's icon.</param>
        /// <param name="totalNewNotification">For display remaining value on item.</param>
        /// <param name="isFullScreen">Make popup fullscreen.</param>
        /// <param name="isSeparateStringByUppercase">Enable separate string by uppercase.</param>
        public DropDownSelectionPopup(Dictionary<string, string> items, int totalNewNotification, bool isFullScreen, bool isSeparateStringByUppercase)
        {
            InitializeComponent();

            foreach (string item in items.Keys)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var tapGestureRecognizer = new TapGestureRecognizer
                    {
                        CommandParameter = item
                    };
                    tapGestureRecognizer.Tapped += CourseStatusList_Clicked;

                    var itemStack = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = !string.IsNullOrEmpty(items[item]) ? LayoutOptions.FillAndExpand : LayoutOptions.CenterAndExpand,
                        Spacing = 0,
                        HeightRequest = 60
                    };
                    itemStack.GestureRecognizers.Add(tapGestureRecognizer);

                    if (!string.IsNullOrEmpty(items[item]))
                    {
                        itemStack.Children.Add(new SvgCachedImage
                        {
                            Aspect = Aspect.Fill,
                            Margin = new Thickness(20, 0, 20, 0),
                            HeightRequest = 20,
                            WidthRequest = 20,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            Source = items[item],
                        });
                    }

                    var title = new Label
                    {
                        FontSize = 16,
                        Text = isSeparateStringByUppercase ? SeparateStringByUppercase.Convert(item) : item,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = !string.IsNullOrEmpty(items[item]) ? LayoutOptions.StartAndExpand : LayoutOptions.CenterAndExpand,
                        TextColor = Color.FromHex("#303450")
                    };

                    itemStack.Children.Add(title);

                    if (item.Equals("Notifications") && totalNewNotification > 0)
                    {
                        title.Margin = new Thickness(25, 0, 0, 0);

                        var circle = new Frame
                        {
                            Margin = new Thickness(5, 10, 0, 0),
                            VerticalOptions = LayoutOptions.Start,
                            HorizontalOptions = LayoutOptions.Center,
                            HasShadow = false,
                            Padding = 0,
                            BackgroundColor = Color.Red,
                            BorderColor = Color.Red,
                            WidthRequest = 20,
                            HeightRequest = 20,
                            IsClippedToBounds = true,
                            CornerRadius = 10
                        };

                        var notiValue = new Label
                        {
                            Text = totalNewNotification.ToString(),
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            FontSize = 8,
                            TextColor = Color.White
                        };

                        circle.Content = notiValue;

                        itemStack.Children.Add(circle);

                        ItemsSourseStack.Children.Add(itemStack);
                    }
                    else
                    {
                        ItemsSourseStack.Children.Add(itemStack);
                    }
                });
            }

            if (isFullScreen)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MainStack.CornerRadius = 0;
                    MainStack.Margin = 0;
                    MainStack.HorizontalOptions = LayoutOptions.FillAndExpand;
                    MainStack.VerticalOptions = LayoutOptions.FillAndExpand;

                    ItemsSourseStack.BackgroundColor = Color.White;
                    ItemsSourseStack.Margin = new Thickness(0, 50, 0, 0);
                    CloseBtnStack.IsVisible = true;
                });
            }
        }

        private void CourseStatusList_Clicked(object sender, EventArgs e)
        {
            string param = (e as TappedEventArgs).Parameter as string;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (PopupNavigation.Instance.PopupStack.Any())
                {
                    PopupNavigation.Instance.RemovePageAsync(this);
                }
            });

            OnItemSelectedEventHandle?.Invoke(this, param);
        }

        private void Close_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (PopupNavigation.Instance.PopupStack.Any())
                {
                    PopupNavigation.Instance.RemovePageAsync(this);
                }
            });
        }
    }
}
