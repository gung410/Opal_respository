using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.Achievement;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Backend.ApiHandler;
using LearnerApp.Services.Identity;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.Services.ServiceManager
{
    public class AchievementBackendServiceManager : BaseBackendServiceManager
    {
        private readonly IBadgeBackendService _badgeBackendService;
        private readonly ICourseBackendService _courseBackendService;
        private readonly IUploaderBackendService _uploadBackendService;
        private readonly IIdentityService _identityService;

        private readonly ApiHandler _apiHandler;
        private ICommunityBackendService _communityBackendService;

        public AchievementBackendServiceManager()
        {
            _identityService = DependencyService.Resolve<IIdentityService>();
            _badgeBackendService = BaseViewModel.CreateRestClientFor<IBadgeBackendService>(GlobalSettings.BackendServiceBadge);
            _courseBackendService = BaseViewModel.CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _uploadBackendService = BaseViewModel.CreateRestClientFor<IUploaderBackendService>(GlobalSettings.BackendServiceUploader);
            _communityBackendService = BaseViewModel.CreateRestClientFor<ICommunityBackendService>(GlobalSettings.BackendServiceSocial);
            _apiHandler = new ApiHandler();
        }

        public async Task<ListResultDto<AchievementECertificateServiceManagerDto>> GetAchievementCertificate(int skipCount = 0, int maxResultCount = GlobalSettings.MaxResultPerPage)
        {
            var ecertificatesResult =
                await _apiHandler.ExecuteBackendService(() => _courseBackendService.GetMyAwardedECertificates(
                    skipCount,
                    maxResultCount));

            if (ecertificatesResult.IsError || ecertificatesResult.Payload == null)
            {
                return null;
            }

            var result = new ListResultDto<AchievementECertificateServiceManagerDto>()
            {
                TotalCount = ecertificatesResult.Payload.TotalCount,
                Items = new List<AchievementECertificateServiceManagerDto>()
            };

            var eCertificateList = ecertificatesResult.Payload.Items;

            var coursesInfoResult =
                await _apiHandler.ExecuteBackendService(() => _courseBackendService.GetCourseListByIdentifiers(
                    eCertificateList.Select(x => x.CourseId).ToArray()));

            var coursesInfo = coursesInfoResult.Payload;
            foreach (var eCertificate in eCertificateList)
            {
                var courseInfo = coursesInfo?.FirstOrDefault(x => x.Id == eCertificate.CourseId);

                result.Items.Add(new AchievementECertificateServiceManagerDto()
                {
                    ECertificate = eCertificate,
                    CourseExtendedInformation = courseInfo
                });
            }

            return result;
        }

        public async Task<AchievementBadgeInfoDto[]> GetAchievementBadgesInfo()
        {
            var badgesInfo =
                await _apiHandler.ExecuteBackendService(() => _badgeBackendService.GetBadgeInfo());

            return badgesInfo.Payload;
        }

        public async Task<ListResultDto<AchievementBadgeModel>> GetAchievementBadges(
            AchievementBadgeInfoDto[] badgeInfoDtos,
            int skip = 0,
            int maximumResult = GlobalSettings.MaxResultPerPage)
        {
            var result = new ListResultDto<AchievementBadgeModel>();
            var userBadgeResult =
                await _apiHandler.ExecuteBackendService(() => _badgeBackendService.GetUserBadges(skip, maximumResult));

            if (userBadgeResult.IsError || userBadgeResult.Payload == null)
            {
                result.TotalCount = 0;
            }
            else
            {
                var communityBadges = userBadgeResult.Payload.Items;
                var communityDataList = await _apiHandler.ExecuteBackendService(
                    () => _communityBackendService.GetCommunityByIds(new GetCommunityByIdRequestModel(
                    communityBadges.Select(x => x.CommunityId).ToArray())));

                foreach (var userBadge in communityBadges)
                {
                    var communityData =
                        communityDataList.Payload?.Results?.FirstOrDefault(x => x.Id == userBadge.CommunityId);

                    var badgeInfo = badgeInfoDtos.FirstOrDefault(x => x.Id == userBadge.BadgeId);

                    result.Items.Add(new AchievementBadgeModel(userBadge, badgeInfo, communityData?.Name));
                }

                result.TotalCount = communityBadges.Count;
            }

            var imageUrls = result.Items.Select(x => x.BadgeImageUrl).ToArray();

            if (imageUrls.Any())
            {
                var badgesList = result.Items;

                var signedUrlsResult =
                    await _apiHandler.ExecuteBackendService(() => _uploadBackendService.GetThumbnails(imageUrls));

                if (signedUrlsResult.IsError == false && signedUrlsResult.Payload != null)
                {
                    foreach (var signedUrl in signedUrlsResult.Payload)
                    {
                        var badgeDtos = badgesList.Where(x => x.BadgeImageUrl == signedUrl.Key);
                        foreach (var badgeDto in badgeDtos)
                        {
                            badgeDto.BadgeImageUrl = signedUrl.Url;
                        }
                    }
                }
            }

            return result;
        }

        public async Task<string> GetCertificateImageFile(string registrationId)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/temp_certificate.png";

            await _apiHandler.ExecuteBackendService(() =>
                DownloadFile(
                    folderPath,
                    async () =>
                    {
                        return await _courseBackendService.DownloadECertificate(registrationId, "Image");
                    }));

            return folderPath;
        }

        public string GetDownloadUrlForECertificate(string eCertificateId)
        {
            return string.Format(
                GlobalSettings.BackendServiceCourse +
                "/api/ecertificate/{0}/download-ecertificate?fileFormat=Pdf",
                eCertificateId);
        }
    }
}
