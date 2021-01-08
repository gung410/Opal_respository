using System;
using System.Threading.Tasks;

namespace Microservice.Analytics.Application.Services.Abstractions
{
    public interface IAnalyticsPdpmPdPlanStatusChangeService
    {
        public Task CreateHistoryItem(Guid resultExtId, long resultId, int statusId, Guid byUserId);
    }
}
