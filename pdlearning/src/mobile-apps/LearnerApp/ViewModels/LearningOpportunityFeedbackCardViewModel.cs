using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels
{
    public class LearningOpportunityFeedbackCardViewModel : BaseViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;

        public LearningOpportunityFeedbackCardViewModel()
        {
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
        }

        public async Task<bool> SubmitFeedback(object reviewItem)
        {
            var result = await ExecuteBackendService(() => _learnerBackendService.CreateUserReview(reviewItem));

            return !result.HasEmptyResult();
        }

        public async Task<bool> ReSubmitFeedback(string itemId, object reviewItem)
        {
            var result = await ExecuteBackendService(() => _learnerBackendService.UpdateUserReview(itemId, reviewItem));

            return !result.HasEmptyResult();
        }
    }
}
