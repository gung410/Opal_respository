using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

// ReSharper disable once CheckNamespace
// For this, use namespace to group multiple classes together.
namespace Microservice.StandaloneSurvey.Application.Commands.SaveFormAnswer
{
    public class SaveSurveyAnswerCommand : BaseStandaloneSurveyCommand
    {
        public bool IsCreation { get; set; }

        public Guid UserId { get; set; }

        public Guid SurveyId { get; set; }

        public Guid? ResourceId { get; set; }

        public Guid SurveyAnswerId { get; set; }

        public UpdateInfo UpdateFormAnswerInfo { get; set; }
    }

    public class UpdateInfo
    {
        public IEnumerable<UpdateInfoQuestionAnswer> QuestionAnswers { get; set; }

        public bool IsSubmit { get; set; }
    }

    public class UpdateInfoQuestionAnswer
    {
        public Guid FormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public bool IsSubmit { get; set; }

        public int? SpentTimeInSeconds { get; set; }
    }
}
