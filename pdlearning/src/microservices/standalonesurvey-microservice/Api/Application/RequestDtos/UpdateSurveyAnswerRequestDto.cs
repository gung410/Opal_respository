using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.Commands.SaveFormAnswer;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class UpdateSurveyAnswerRequestDto : HasSubModuleInfoBase
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

        public UpdateInfoQuestionAnswer ToSaveFormAnswerCommandUpdateInfoQuestionAnswer()
        {
            return new UpdateInfoQuestionAnswer
            {
                AnswerValue = AnswerValue,
                FormQuestionId = FormQuestionId,
                IsSubmit = IsSubmit,
                SpentTimeInSeconds = SpentTimeInSeconds
            };
        }
    }
}
