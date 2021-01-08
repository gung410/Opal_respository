using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchAssessmentAnswerQuery : BaseThunderQuery<PagedResultDto<AssessmentAnswerModel>>
    {
        public string SearchText { get; set; }

        public Guid? ParticipantAssignmentTrackId { get; set; }

        public Guid? UserId { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
