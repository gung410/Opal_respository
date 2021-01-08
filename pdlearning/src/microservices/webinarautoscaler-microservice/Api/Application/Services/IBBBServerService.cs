using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Models;
using Microservice.WebinarAutoscaler.Application.RequestDtos;

namespace Microservice.WebinarAutoscaler.Application.Services
{
    public interface IBBBServerService
    {
        Task UpdateBBBProtectionStateAsync(UpdateBBBProtectionStateRequest request);

        Task<string> GetBBBServerPrivateIpByBBBInstanceIdAsync(string instanceId);

        Task UpdateBBBReadyStatusStateAsync(string instanceId);

        Task UpdateBBBServerMeetingsAsync(UpdateBBBServerMeetingsRequest updateBBBServerMeetingsRequest);

        Task<List<string>> GetTurnServerPublicIpsAsync();
    }
}
