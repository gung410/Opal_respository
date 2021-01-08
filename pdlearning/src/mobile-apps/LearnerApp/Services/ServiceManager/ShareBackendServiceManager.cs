using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common.TaskController;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.Sharing;
using LearnerApp.Resources.Texts;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Backend.ApiHandler;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Services.ServiceManager
{
    public class ShareBackendServiceManager
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICourseBackendService _courseBackendService;
        private readonly ApiHandler _apiHandler;

        public ShareBackendServiceManager()
        {
            _learnerBackendService = BaseViewModel.CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _courseBackendService = BaseViewModel.CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _apiHandler = new ApiHandler();
        }

        public async Task<ListResultDto<SharingContentItem>> GetShares(int skipCount = 0, int maxResultCount = GlobalSettings.MaxResultPerPage)
        {
            var sharingListResult =
                await _apiHandler.ExecuteBackendService(() => _learnerBackendService.GetShares(skipCount, maxResultCount));

            if (sharingListResult.IsError || sharingListResult.Payload == null)
            {
                return null;
            }

            var sharingList = sharingListResult.Payload.Items;
            var courseList = sharingList
                .Where(s => s.ItemType == BookmarkType.Course || s.ItemType == BookmarkType.Microlearning).ToList();

            var digitalContentList = sharingList
                .Where(s => s.ItemType == BookmarkType.DigitalContent).ToList();

            var parallelTaskRunner = new ParallelTaskRunner(2);

            await parallelTaskRunner.RunAsync(
                async () =>
                {
                    var courseListApiResult = await _apiHandler.ExecuteBackendService(() => _courseBackendService.GetCourseListByIdentifiers(
                        courseList.Select(x => x.ItemId).ToArray()));

                    if (courseListApiResult.Payload == null)
                    {
                        return;
                    }

                    foreach (var courseExtendedInformation in courseListApiResult.Payload)
                    {
                        var correspondingCourse = courseList.FirstOrDefault(x => x.ItemId == courseExtendedInformation.Id);

                        if (correspondingCourse != null)
                        {
                            correspondingCourse.Tags = CourseCardBuilder.GetMetadataTags(courseExtendedInformation);
                        }
                    }
                },
                () =>
                {
                    foreach (var digitalContent in digitalContentList)
                    {
                        digitalContent.Tags = new List<string>()
                        {
                            TextsResource.DIGITAL_CONTENT
                        };
                    }

                    return Task.CompletedTask;
                });

            return sharingListResult.Payload;
        }
    }
}
