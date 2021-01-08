using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels
{
    public class MyLearningDigitalViewModel : BaseViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IContentBackendService _contentBackendService;

        public MyLearningDigitalViewModel()
        {
            _contentBackendService = CreateRestClientFor<IContentBackendService>(GlobalSettings.BackendServiceContent);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
        }

        public async Task<List<ItemCard>> GetMyDigitalContent(
            int skipCount,
            string keyword,
            bool includeStatistic,
            MyLearningStatus statusFilter,
            Action<int> totalFilterCount = null,
            Action<List<SearchStatistics>> searchStatistics = null)
        {
            var summaryResult = await ExecuteBackendService(() => _learnerBackendService.GetMyLearningDigitalContent(new
            {
                SkipCount = skipCount,
                MaxResultCount = GlobalSettings.MaxResultPerPage,
                StatisticsFilter = new string[] { MyLearningStatus.InProgress.ToString(), MyLearningStatus.Completed.ToString() },
                IncludeStatistic = includeStatistic,
                SearchText = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(keyword),
                OrderBy = "CreatedDate desc",
                StatusFilter = statusFilter.ToString()
            }));

            totalFilterCount?.Invoke(summaryResult.Payload?.TotalCount ?? 0);
            if (!string.IsNullOrEmpty(keyword) && includeStatistic)
            {
                searchStatistics?.Invoke(summaryResult.Payload.Statistics);
            }

            if (summaryResult.HasEmptyResult() || summaryResult.Payload.TotalCount == 0)
            {
                return null;
            }

            var detailResult = await ExecuteBackendService(() => _contentBackendService.GetDigitalContentDetails(summaryResult.Payload.Items.Select(p => p.DigitalContentId).ToArray()));

            if (detailResult.HasEmptyResult() || detailResult.Payload.Count == 0)
            {
                return null;
            }

            var digitalCard = DigitalContentCardBuilder.BuildDigitalContentCardList(summaryResult.Payload.Items, detailResult.Payload);

            return digitalCard;
        }
    }
}
