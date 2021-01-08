using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Controllers
{
    [Route("api/participantAssignmentTrack")]
    public class ParticipantAssignmentTrackController : BaseController<ParticipantAssignmentTrackService>
    {
        public ParticipantAssignmentTrackController(IUserContext userContext, ParticipantAssignmentTrackService appService) : base(userContext, appService)
        {
        }

        [HttpGet("{participantAssignmentTrackId:guid}")]
        public async Task<ParticipantAssignmentTrackModel> GetParticipantAssignmentTrackById(Guid participantAssignmentTrackId)
        {
            return await AppService.GetParticipantAssignmentTrackById(participantAssignmentTrackId);
        }

        [HttpPost("getByIds")]
        public async Task<List<ParticipantAssignmentTrackModel>> GetParticipantAssignmentTrackByIds([FromBody] List<Guid> participantAssignmentTrackIds)
        {
            return await AppService.GetParticipantAssignmentTrackByIds(participantAssignmentTrackIds);
        }

        [HttpPost("getParticipantAssignmentTracks")]
        public async Task<PagedResultDto<ParticipantAssignmentTrackModel>> GetParticipantAssignmentTracks([FromBody] GetParticipantAssignmentTracksRequest request)
        {
            return await AppService.GetParticipantAssignmentTracks(request);
        }

        [HttpPost("assignAssignment")]
        public async Task<IEnumerable<ParticipantAssignmentTrackModel>> AssignAssignment([FromBody] AssignAssignmentRequest request)
        {
            return await AppService.AssignAssignment(request);
        }

        [HttpPost("markScore")]
        public async Task<ParticipantAssignmentTrackModel> MarkScoreForQuizQuestionAnswer([FromBody] MarkScoreForQuizQuestionAnswerRequest request)
        {
            return await AppService.MarkScoreForQuizQuestionAnswer(request);
        }

        [HttpPost("saveAssignmentQuizAnswer")]
        public async Task<ParticipantAssignmentTrackModel> SaveAssignmentQuizAnswer([FromBody] SaveAssignmentQuizAnswerRequest request)
        {
            return await AppService.SaveAssignmentQuizAnswer(request);
        }
    }
}
