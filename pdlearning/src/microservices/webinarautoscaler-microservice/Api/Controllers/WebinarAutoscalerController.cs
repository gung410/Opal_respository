using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.RequestDtos;
using Microservice.WebinarAutoscaler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.WebinarAutoscaler.Controllers
{
    [Route("api/autoscaler")]
    public class WebinarAutoscalerController
    {
        private readonly IBBBServerService _bbbServerService;

        public WebinarAutoscalerController(IBBBServerService bbbServerService)
        {
            _bbbServerService = bbbServerService;
        }

        [HttpGet("getBBBServerPrivateIpByBBBInstanceId/{instanceId}")]
        public Task<string> GetBBBServerPrivateIpByBBBInstanceId(string instanceId)
        {
            return _bbbServerService.GetBBBServerPrivateIpByBBBInstanceIdAsync(instanceId);
        }

        [HttpGet("getTurnServerPublicIps")]
        public Task<List<string>> GetTurnServerPublicIps()
        {
            return _bbbServerService.GetTurnServerPublicIpsAsync();
        }

        [HttpPost("updateBBBServerProtectionState")]
        public Task UpdateBBBProtectionState([FromBody] UpdateBBBProtectionStateRequest updateBBBProtectionStateRequest)
        {
            return _bbbServerService.UpdateBBBProtectionStateAsync(updateBBBProtectionStateRequest);
        }

        [HttpPost("updateBBBServerMeetings")]
        public Task UpdateBBBServerMeetings([FromBody] UpdateBBBServerMeetingsRequest updateBBBServerMeetingsRequest)
        {
            return _bbbServerService.UpdateBBBServerMeetingsAsync(updateBBBServerMeetingsRequest);
        }

        [HttpGet("UpdateBBBReadyStatusState/{instanceId}")]
        public Task UpdateBBBReadyStatusState(string instanceId)
        {
            return _bbbServerService.UpdateBBBReadyStatusStateAsync(instanceId);
        }
    }
}
