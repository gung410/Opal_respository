using System;
using System.Collections.Generic;
using Microservice.Form.Application.Commands;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Form.Application.RequestDtos
{
    public class UpdateFormAnswerScoreRequestDto
    {
        public Guid FormAnswerId { get; set; }

        public Guid? MyCourseId { get; set; }

        public IEnumerable<UpdateFormAnswerScoreRequestDtoQuestionAnswer> QuestionAnswers { get; set; }
    }

    public class UpdateFormAnswerScoreRequestDtoQuestionAnswer
    {
        public Guid FormQuestionId { get; set; }

        public double? MarkedScore { get; set; }

        public SaveFormAnswerCommand.UpdateInfoQuestionAnswer ToSaveFormAnswerCommandUpdateInfoQuestionAnswer()
        {
            return new SaveFormAnswerCommand.UpdateInfoQuestionAnswer
            {
                FormQuestionId = FormQuestionId,
                MarkedScore = MarkedScore,
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
