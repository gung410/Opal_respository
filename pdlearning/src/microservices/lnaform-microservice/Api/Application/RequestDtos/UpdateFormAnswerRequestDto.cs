using System;
using System.Collections.Generic;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class UpdateFormAnswerRequestDto
    {
        public Guid FormAnswerId { get; set; }

        public IEnumerable<UpdateFormAnswerRequestDtoQuestionAnswer> QuestionAnswers { get; set; }

        public bool IsSubmit { get; set; }
    }

    public class UpdateFormAnswerRequestDtoQuestionAnswer
    {
        public Guid FormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public bool IsSubmit { get; set; }

        public int? SpentTimeInSeconds { get; set; }

        // TODO: ASk Nhon: Why this dto use a class from Command?
        public Commands.SaveFormAnswer.UpdateInfoQuestionAnswer ToSaveFormAnswerCommandUpdateInfoQuestionAnswer()
        {
            return new Commands.SaveFormAnswer.UpdateInfoQuestionAnswer
            {
                AnswerValue = AnswerValue,
                FormQuestionId = FormQuestionId,
                IsSubmit = IsSubmit,
                SpentTimeInSeconds = SpentTimeInSeconds
            };
        }
    }
}
