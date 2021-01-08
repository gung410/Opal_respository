using System;
using System.Collections.Generic;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class SaveFormAnswerCommand : BaseThunderCommand
    {
        public bool IsCreation { get; set; }

        public bool IsMarking { get; set; }

        public Guid UserId { get; set; }

        public Guid FormId { get; set; }

        public Guid? MyCourseId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? AssignmentId { get; set; }

        public Guid FormAnswerId { get; set; }

        public UpdateInfo UpdateFormAnswerInfo { get; set; }

#pragma warning disable CA1034 // Nested types should not be visible
        public class UpdateInfo
        {
            public IEnumerable<UpdateInfoQuestionAnswer> QuestionAnswers { get; set; }

            public bool IsSubmit { get; set; }
        }

        public class UpdateInfoQuestionAnswer
        {
            public Guid FormQuestionId { get; set; }

            public object AnswerValue { get; set; }

            public IEnumerable<FormAnswerAttachmentModel> FormAnswerAttachments { get; set; }

            public bool IsSubmit { get; set; }

            public double? MarkedScore { get; set; }

            public int? SpentTimeInSeconds { get; set; }
        }
#pragma warning restore CA1034 // Nested types should not be visible
    }
}
