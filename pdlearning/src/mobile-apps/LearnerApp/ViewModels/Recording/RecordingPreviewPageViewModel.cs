using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Recording
{
    public class RecordingPreviewPageViewModel : BasePageViewModel
    {
        private RecordingPreviewPageTransferData _transferData;
        private IDialogService _dialogService;

        public RecordingPreviewPageViewModel()
        {
            _dialogService = DependencyService.Resolve<IDialogService>();
        }

        public override string PageTitle => string.Empty;

        public ICommand CancelCommand => new Command(async () =>
        {
            bool result = await _transferData.Delegate.OnUserCancel(_transferData.VideoPath);
            if (result)
            {
                await NavigationService.GoBack();
            }
        });

        public ICommand OnTakeAgainCommand => new Command(async () =>
        {
            bool result = await _transferData.Delegate.OnTakeAgainRequest(_transferData.VideoPath);
            if (result)
            {
                await GoBack();
            }
        });

        public ICommand OnSaveCommand => new Command(async () =>
        {
            bool result = await _transferData.Delegate.OnSaveRequest(_transferData.VideoPath);
            if (result)
            {
                await GoBack();
            }
        });

        public string VideoSource => _transferData?.VideoPath;

        public override string RoutingName => NavigationRoutes.RecordingPreview;

        public static NavigationParameters GetNavigationParams(RecordingPreviewPageTransferData data)
        {
            var param = new NavigationParameters();
            param.SetParameter("transfer-data", data);

            return param;
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
             await base.InternalNavigatedTo(navigationParameters);

             _transferData = navigationParameters.GetParameter<RecordingPreviewPageTransferData>("transfer-data");
             RaisePropertyChanged(() => VideoSource);

             if (_transferData.IsTimeout)
            {
                await _dialogService.ShowAlertAsync("Because the video is longer than 10 minutes. So it's automatically stopped");
            }
        }

        private async Task GoBack()
        {
            await NavigationService.GoBack();
        }
    }

    public class RecordingPreviewPageTransferData
    {
        public RecordingPreviewPageTransferData(IRecordingPreviewDelegate @delegate, string videoPath, bool isTimeout)
        {
            Delegate = @delegate;
            VideoPath = videoPath;
            IsTimeout = isTimeout;
        }

        public interface IRecordingPreviewDelegate
        {
            Task<bool> OnUserCancel(string recordedPath);

            Task<bool> OnTakeAgainRequest(string recordedPath);

            Task<bool> OnSaveRequest(string recordedPath);
        }

        public IRecordingPreviewDelegate Delegate { get; }

        public string VideoPath { get; }

        public bool IsTimeout { get; }
    }
}
