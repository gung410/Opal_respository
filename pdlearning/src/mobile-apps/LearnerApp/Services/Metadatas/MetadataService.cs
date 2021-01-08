using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.Services.Metadatas
{
    public class MetadataService : BaseViewModel, IMetadataService
    {
        private readonly IMetadataBackendService _metadataBackendService;
        private readonly IPortalBackendService _portalBackendService;

        public MetadataService()
        {
            _metadataBackendService = CreateRestClientFor<IMetadataBackendService>(GlobalSettings.BackendServiceTagging);
            _portalBackendService = CreateRestClientFor<IPortalBackendService>(GlobalSettings.BackendServicePortal);
        }

        public Task InitMetadata()
        {
            // Note: We use list because we want to write code as a flow instead of passing params as params[].
            var initTasks = new List<Func<Task>>
                {
                    InitSiteInformation,
                    InitMetadataTagging
                }.GetEnumerator();

            return TaskHelper.RunSequential(initTasks);
        }

        private async Task InitSiteInformation()
        {
            if (Application.Current.Properties.ContainsKey("site-information"))
            {
                return;
            }

            var siteInformation = await ExecuteBackendService(() => _portalBackendService.GetSiteInformation());

            if (siteInformation.HasEmptyResult())
            {
                return;
            }

            Application.Current.Properties.AddSiteInformation(siteInformation.Payload.ReleaseDate);
        }

        private async Task InitMetadataTagging()
        {
            if (Application.Current.Properties.ContainsKey("metadata-tagging"))
            {
                return;
            }

            var tagging = await ExecuteBackendService(() => _metadataBackendService.GetMetadata());

            if (tagging.HasEmptyResult())
            {
                return;
            }

            Application.Current.Properties.AddMetadataTagging(tagging.Payload);
        }
    }
}
