using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class NewsfeedViewModel : BasePageViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICommonServices _commonService;

        public NewsfeedViewModel()
        {
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _commonService = DependencyService.Resolve<ICommonServices>();
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.NewsFeedDetails;

        public async Task<ItemCard> GetCourseCard(string courseId)
        {
            var courseSummaryResult = await ExecuteBackendService(() => _learnerBackendService.GetMyCourseSummary(courseId));

            if (courseSummaryResult.IsError || courseSummaryResult.Payload == null)
            {
                return null;
            }

            var card = await _commonService.BuildCourseCardList(new List<MyCourseSummary>() { courseSummaryResult.Payload });

            if (card.IsNullOrEmpty())
            {
                return null;
            }

            return card[0];
        }
    }
}
