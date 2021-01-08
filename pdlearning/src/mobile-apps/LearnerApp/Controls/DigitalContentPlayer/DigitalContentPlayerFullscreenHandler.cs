using System.Threading.Tasks;
using LearnerApp.Common.Helper;
using LearnerApp.Models.MyLearning.DigitalContentPlayer;
using LearnerApp.PlatformServices.DroidServices;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
using Xamarin.Forms;

namespace LearnerApp.Controls.DigitalContentPlayer
{
    public class DigitalContentPlayerFullscreenHandler
    {
        private readonly ContentPage _contentPage;
        private readonly VisualElement[] _hideWhenFullscreenElements;
        private readonly StressActionHandler _stressActionHandler = new StressActionHandler();

        private WebViewVideoDigitalContentPlayerData _videoDigitalContentPlayer;
        private bool _useSafeArea = true;

        public DigitalContentPlayerFullscreenHandler(
            ContentPage contentPage,
            VisualElement[] hideWhenFullscreenElements)
        {
            _contentPage = contentPage;
            _hideWhenFullscreenElements = hideWhenFullscreenElements;
        }

        ~DigitalContentPlayerFullscreenHandler()
        {
            CrossDeviceOrientation.Current.OrientationChanged -= OnOrientationChanged;
        }

        public void SetWebViewVideoDigitalPlayerVideoData(WebViewVideoDigitalContentPlayerData videoContentPlayerData)
        {
            if (_videoDigitalContentPlayer != null)
            {
                _videoDigitalContentPlayer.OnDigitalContentPlayerCollapseOrExpand -= VideoDigitalContentPlayerCollapseOrExpand;
            }

            _videoDigitalContentPlayer = videoContentPlayerData;
            _videoDigitalContentPlayer.OnDigitalContentPlayerCollapseOrExpand += VideoDigitalContentPlayerCollapseOrExpand;
        }

        public void Init()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Need to wait the view to be layout
                await Task.Delay(100);
                SetSafeArea(true);
            });

            CrossDeviceOrientation.Current.OrientationChanged += OnOrientationChanged;
        }

        private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Need to wait the view to be layout
                await Task.Delay(100);
                SetSafeArea();
            });
        }

        private async void VideoDigitalContentPlayerCollapseOrExpand(bool isExpand)
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var visualElement in _hideWhenFullscreenElements)
                    {
                        visualElement.IsVisible = !isExpand;
                    }

                    _useSafeArea = !isExpand;
                    CrossDeviceOrientation.Current.LockOrientation(
                        isExpand
                            ? DeviceOrientations.Landscape
                            : DeviceOrientations.Portrait);

                    ShowOrHideAndroidNativeBottomNavigation(isExpand);
                });
            });
        }

        private void SetSafeArea(bool? useSafeArea = null)
        {
            if (useSafeArea != null)
            {
                _useSafeArea = useSafeArea.Value;
            }

            if (_useSafeArea)
            {
                var safeAreaInsets = Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.GetSafeAreaInsets(_contentPage);
                _contentPage.Content.Margin = safeAreaInsets;
            }
            else
            {
                _contentPage.Content.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void ShowOrHideAndroidNativeBottomNavigation(bool isExpand)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                if (isExpand)
                {
                    DependencyService.Resolve<IDroidSpecificServices>()
                        .EnterFullscreenMode();
                }
                else
                {
                    DependencyService.Resolve<IDroidSpecificServices>()
                        .ExitFullscreenMode();
                }
            }
        }
    }
}
