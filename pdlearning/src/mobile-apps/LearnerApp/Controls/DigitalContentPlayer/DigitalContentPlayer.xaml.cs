using System;
using System.Collections.Generic;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.MyLearning.DigitalContentPlayer;
using LibVLCSharp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.DigitalContentPlayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DigitalContentPlayer
    {
        public static readonly BindableProperty DataProperty =
            BindableProperty.Create(
                nameof(Data),
                typeof(BaseDigitalContentPlayerData),
                typeof(DigitalContentPlayer),
                null,
                propertyChanged: OnDataChanged);

        public static readonly BindableProperty BrokenLinkCommandProperty =
            BindableProperty.Create(
                nameof(BrokenLinkCommand),
                typeof(ICommand),
                typeof(DigitalContentPlayer));

        private List<string> _brokenLinkReport;

        public DigitalContentPlayer()
        {
            InitializeComponent();

            Core.Initialize();
        }

        public BaseDigitalContentPlayerData Data
        {
            get { return (BaseDigitalContentPlayerData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public ICommand BrokenLinkCommand
        {
            get { return (ICommand)GetValue(BrokenLinkCommandProperty); }
            set { SetValue(BrokenLinkCommandProperty, value); }
        }

        public void SetDataChanged(BaseDigitalContentPlayerData data)
        {
            Player.Children.Clear();
            MetadataView.Children.Clear();

            var view = data.GetContentView();
            Player.Children.Add(view);
            data.LoadStyleForDigitalContentPlayer(this);

            data.AddMetadataView(MetadataView);

            var brokenLinkReport = data.GetBrokenLink();
            _brokenLinkReport = brokenLinkReport;

            if (!brokenLinkReport.IsNullOrEmpty())
            {
                var brokenLinkTapped = new TapGestureRecognizer();
                brokenLinkTapped.Tapped += BrokenLink_Tapped;

                var brokenLinkStack = new StackLayout();
                brokenLinkStack.GestureRecognizers.Add(brokenLinkTapped);

                var brokenLbl = new Label
                {
                    Text = "Report broken link",
                    FontSize = 16,
                    TextColor = Color.FromHex("79B3EE"),
                    TextDecorations = TextDecorations.Underline
                };

                brokenLinkStack.Children.Add(brokenLbl);

                MetadataView.Children.Add(brokenLinkStack);
            }
        }

        private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != null && newValue is BaseDigitalContentPlayerData data)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var digitalContentPlayer = (DigitalContentPlayer)bindable;
                    if (oldValue is BaseDigitalContentPlayerData oldDigitalContentPlayerData)
                    {
                        oldDigitalContentPlayerData.OnClearPlayer();
                    }

                    digitalContentPlayer.SetDataChanged(data);
                });
            }
        }

        private void BrokenLink_Tapped(object sender, EventArgs e)
        {
            if (BrokenLinkCommand != null && BrokenLinkCommand.CanExecute(_brokenLinkReport))
            {
                BrokenLinkCommand.Execute(_brokenLinkReport);
            }
        }
    }
}
