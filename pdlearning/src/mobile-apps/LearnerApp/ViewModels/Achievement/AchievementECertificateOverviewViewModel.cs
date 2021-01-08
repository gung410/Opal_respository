using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Services.ServiceManager;
using LearnerApp.ViewModels.Achievement.ECertificate;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Achievement
{
    public class AchievementECertificateOverviewViewModel : BaseViewModel
    {
        private readonly AchievementBackendServiceManager _achievementBackendSm;

        private int _totalCount;
        private bool _isLoading = true;

        public AchievementECertificateOverviewViewModel()
        {
            _achievementBackendSm = new AchievementBackendServiceManager();
        }

        public LearnerObservableCollection<AchievementECertificateItemViewModel> ECertificates { get; private set; }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
                RaisePropertyChanged(() => IsShowList);
            }
        }

        public bool IsShowList
        {
            get => _isLoading || TotalCount > 0;
        }

        public ICommand ShowAllCommand => new Command(ShowAll);

        public async Task LoadData()
        {
            IsLoading = true;
            var list =
                await _achievementBackendSm.GetAchievementCertificate(0, 3);

            if (list == null)
            {
                IsLoading = false;
                TotalCount = 0;
                return;
            }

            var eCertificates =
                list.Items.Take(3).Select(x => new AchievementECertificateItemViewModel(x)).ToList();

            TotalCount = list.TotalCount;
            ECertificates = new LearnerObservableCollection<AchievementECertificateItemViewModel>(eCertificates);
            RaisePropertyChanged(() => ECertificates);
            IsLoading = false;
        }

        private async void ShowAll()
        {
            await NavigationService.NavigateToAsync<AchievementECertificatePageViewModel>();
        }
    }
}
