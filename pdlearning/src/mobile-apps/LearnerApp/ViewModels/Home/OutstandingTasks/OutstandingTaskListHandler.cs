using System;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Backend.ApiHandler;

namespace LearnerApp.ViewModels.Home.OutstandingTasks
{
    public class OutstandingTaskListHandler
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ApiHandler _apiHandler;
        private string _navigatedOutstandingTaskId;

        public OutstandingTaskListHandler(ILearnerBackendService learnerBackendService)
        {
            _learnerBackendService = learnerBackendService;
            OutstandingTaskNavigationMessagingCenter.SubscribeOnNavigatedTo(this, OnOutstandingTaskNavigated);
            _apiHandler = new ApiHandler();
        }

        ~OutstandingTaskListHandler()
        {
            OutstandingTaskNavigationMessagingCenter.UnsubscribeOnNavigatedTo(this);
        }

        public async Task ReloadRowIfNeeded(
            LearnerObservableCollection<object> outstandingTasks,
            int totalCount,
            Action<int> setTotalCount)
        {
            if (_navigatedOutstandingTaskId.IsNullOrEmpty())
            {
                return;
            }

            var existedRow = outstandingTasks
                .OfType<OutstandingTaskItemViewModel>()
                .FirstOrDefault(x => x.Id == _navigatedOutstandingTaskId);

            if (existedRow == null)
            {
                return;
            }

            var existedRowIndex = outstandingTasks.IndexOf(existedRow);

            outstandingTasks.Replace(existedRowIndex, new LoadingItemSkeletonViewModel());

            var updatedOutstandingTask =
                await _apiHandler.ExecuteBackendService(() => _learnerBackendService.GetOutstandingTaskById(_navigatedOutstandingTaskId));

            if (updatedOutstandingTask.IsError)
            {
                outstandingTasks.Replace(existedRowIndex, existedRow);
                return;
            }

            if (updatedOutstandingTask.Payload == null)
            {
                outstandingTasks.RemoveAt(existedRowIndex);
                setTotalCount?.Invoke(totalCount - 1);
            }
            else
            {
                outstandingTasks.Replace(
                    existedRowIndex,
                    new OutstandingTaskItemViewModel(updatedOutstandingTask.Payload));
            }
        }

        private void OnOutstandingTaskNavigated(object sender, OutstandingTaskNavigatedToArguments args)
        {
            _navigatedOutstandingTaskId = args.OutstandingTaskId;
        }
    }
}
