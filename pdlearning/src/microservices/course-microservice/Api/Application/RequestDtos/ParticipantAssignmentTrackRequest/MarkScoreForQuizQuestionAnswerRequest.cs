using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class MarkScoreForQuizQuestionAnswerRequest
    {
        public Guid ParticipantAssignmentTrackId { get; set; }

        public List<MarkScoreForQuizQuestionAnswerDto> MarkScoreForQuizQuestionAnswers { get; set; }
    }
}
