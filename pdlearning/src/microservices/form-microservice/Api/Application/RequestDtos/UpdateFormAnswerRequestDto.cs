using System;
using System.Collections.Generic;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Form.Application.RequestDtos
{
    public class UpdateFormAnswerRequestDto
    {
        public Guid FormAnswerId { get; set; }

        public Guid? MyCourseId { get; set; }

        public IEnumerable<UpdateFormAnswerRequestDtoQuestionAnswer> QuestionAnswers { get; set; }

        public bool IsSubmit { get; set; }
    }

    public class UpdateFormAnswerRequestDtoQuestionAnswer
    {
        public Guid FormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public bool IsSubmit { get; set; }

        public int? SpentTimeInSeconds { get; set; }

        public IEnumerable<FormAnswerAttachmentModel> FormAnswerAttachments { get; set; }

        public SaveFormAnswerCommand.UpdateInfoQuestionAnswer ToSaveFormAnswerCommandUpdateInfoQuestionAnswer()
        {
            return new SaveFormAnswerCommand.UpdateInfoQuestionAnswer
            {
                AnswerValue = AnswerValue,
                FormQuestionId = FormQuestionId,
                FormAnswerAttachments = FormAnswerAttachments,
                IsSubmit = IsSubmit,
                SpentTimeInSeconds = SpentTimeInSeconds
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
