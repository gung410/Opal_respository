using LibVLCSharp.Forms.Shared;
using LibVLCSharp.Shared;
using Telerik.XamarinForms.PdfViewer;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class AudioDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        private readonly string _source;

        private MediaPlayerElement _element;

        public AudioDigitalContentPlayerData(string source)
        {
            _source = source;
        }

        public override View GetContentView()
        {
            var libVLC = new LibVLC();
            var media = new Media(libVLC, _source, FromType.FromLocation);

            _element = new MediaPlayerElement
            {
                MediaPlayer = new MediaPlayer(media)
                {
                    Title = 0,
                    EnableHardwareDecoding = true,
                    Fullscreen = true,
                },
                PlaybackControls = new PlaybackControls
                {
                    Foreground = Color.Gray,
                    KeepScreenOn = true
                },
            };

            _element.BackgroundColor = Color.Transparent;
            _element.HorizontalOptions = LayoutOptions.CenterAndExpand;
            _element.VerticalOptions = LayoutOptions.EndAndExpand;

            _element.MediaPlayer.SetRole(MediaPlayerRole.Music);
            _element.MediaPlayer.SetPause(true);

            return _element;
        }

        public override void OnClearPlayer()
        {
            base.OnClearPlayer();

            _element.MediaPlayer.Stop();
        }

        public override void LoadStyleForDigitalContentPlayer(Controls.DigitalContentPlayer.DigitalContentPlayer digitalContentPlayer)
        {
            base.LoadStyleForDigitalContentPlayer(digitalContentPlayer);
            _element.MediaPlayer.Play();
        }

        private async void PdfSource_Exception(object sender, SourceExceptionEventArgs e)
        {
            await DialogService.ShowAlertAsync("PDF source error");
        }
    }
}
