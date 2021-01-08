using System;
using System.Threading.Tasks;

namespace Microservice.Analytics.Application.Services.Abstractions
{
    public interface IAnalyticsCSLService
    {
        Task SetToDateAsync(int id, DateTime? toDate = null);
    }
}
