using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Common.TaskController;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Backend.ApiHandler;
using LearnerApp.Services.Identity;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.Services.DataManager.InitLogin
{
    public class InitLoginDataManager
    {
        private readonly IMetadataBackendService _metadataBackendService;
        private readonly IPortalBackendService _portalBackendService;
        private readonly IIdentityService _identityService;
        private readonly ICommonServices _commonService;
        private readonly IOrganizationBackendService _organizationBackendService;

        private readonly ApiHandler _apiHandler;

        public InitLoginDataManager()
        {
            _apiHandler = new ApiHandler();
            _identityService = DependencyService.Resolve<IIdentityService>();
            _commonService = DependencyService.Resolve<ICommonServices>();
            _metadataBackendService = BaseViewModel.CreateRestClientFor<IMetadataBackendService>(GlobalSettings.BackendServiceTagging);
            _portalBackendService = BaseViewModel.CreateRestClientFor<IPortalBackendService>(GlobalSettings.BackendServicePortal);
            _organizationBackendService = BaseViewModel.CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
        }

        public async Task InitLoginData()
        {
            ParallelTaskRunner runner = new ParallelTaskRunner();
            await runner.RunAsync(
                GetMetadataTagging,
                GetSiteInformation,
                GetAccountProperties,
                GetMetadataDepartment,
                GetPermission);

            await _commonService.LearningTrackingStartApp();
        }

        private async Task GetMetadataDepartment()
        {
            var department =
                await _apiHandler.ExecuteBackendService(() => _organizationBackendService.GetDepartmentInfomation());

            if (department.HasEmptyResult())
            {
                return;
            }

            Application.Current.Properties.AddMetadataDepartment(department.Payload.Items);
        }

        private async Task GetSiteInformation()
        {
            var siteInformation = await _apiHandler.ExecuteBackendService(() => _portalBackendService.GetSiteInformation());

            if (siteInformation.HasEmptyResult())
            {
                return;
            }

            Application.Current.Properties.AddSiteInformation(siteInformation.Payload.ReleaseDate);
        }

        private async Task GetAccountProperties()
        {
            var accountProperties = await _apiHandler.ExecuteBackendService(() => _identityService.GetAccountPropertiesAsync());

            if (accountProperties.HasEmptyResult())
            {
                return;
            }

            Application.Current.Properties.AddAccountProperties(accountProperties.Payload);
        }

        /// <summary>
        /// We just need to call get list of tagging 1 time and cache to application properties for getting and using anywhere in app.
        /// </summary>
        private async Task GetMetadataTagging()
        {
            var tagging = await _apiHandler.ExecuteBackendService(() => _metadataBackendService.GetMetadata());

            if (tagging.HasEmptyResult())
            {
                return;
            }

            Application.Current.Properties.AddMetadataTagging(tagging.Payload);
        }

        /// <summary>
        /// Get access rights to adjust permission in app.
        /// </summary>
        private async Task GetPermission()
        {
            var permissionResponse = await _apiHandler.ExecuteBackendService(() => _portalBackendService.GetAccessRights());

            if (permissionResponse.HasEmptyResult())
            {
                return;
            }

            var accessDictionary = permissionResponse.Payload.ToDictionary(key => key.Action, value => true);
            IdentityService.AccessRights = accessDictionary;
        }
    }
}
