using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.Services.ServiceManager;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Sharing.Home
{
      public class SharingSectionViewModel : BaseViewModel
    {
        private readonly ShareBackendServiceManager _shareBackendServiceManager;
        private readonly INavigationService _navigationService;

        private int _totalCount;
        private bool _isLoading = true;
        private bool _isShowShowSharingSection;
        private bool _isShowList;

        public SharingSectionViewModel()
        {
            _shareBackendServiceManager = new ShareBackendServiceManager();
            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        public LearnerObservableCollection<SharingContentItemViewModel> SharingSource { get; private set; }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
            }
        }

        public bool IsShowSharingSection
        {
            get
            {
                return _isShowShowSharingSection;
            }

            set
            {
                _isShowShowSharingSection = value;
                RaisePropertyChanged(() => IsShowSharingSection);
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
            set
            {
                _isShowList = value;
                RaisePropertyChanged(() => IsShowList);
            }
        }

        public ICommand ShowAllCommand => new Command(ShowAll);

        public async Task LoadData()
        {
            if (!IsShowSharingSection)
            {
                IsShowSharingSection = false;
                return;
            }

            IsLoading = true;
            var sharingList =
                await _shareBackendServiceManager.GetShares(0, 3);

            if (sharingList == null)
            {
                IsLoading = false;
                TotalCount = 0;
                return;
            }

            var sharingContentItems =
                sharingList.Items.Take(3).Select(x => new SharingContentItemViewModel(x)).ToList();

            TotalCount = sharingList.TotalCount;
            SharingSource = new LearnerObservableCollection<SharingContentItemViewModel>(sharingContentItems);
            RaisePropertyChanged(() => SharingSource);
            IsLoading = false;
        }

        private async void ShowAll(object obj)
        {
            await _navigationService.NavigateToAsync<SharingListPageViewModel>();
        }
    }
}
