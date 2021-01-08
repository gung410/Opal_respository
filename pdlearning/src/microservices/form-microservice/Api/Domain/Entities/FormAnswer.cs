using System;
using System.Diagnostics.CodeAnalysis;
using Microservice.Form.Domain.Enums;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class FormAnswer : BaseEntity, ISoftDelete
    {
        public Guid FormId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? MyCourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? AssignmentId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? SubmitDate { get; set; }

        public double? Score { get; set; }

        public double? ScorePercentage { get; set; }

        public short Attempt { get; set; } = 1;

        public FormAnswerFormMetaData FormMetaData { get; set; } = new FormAnswerFormMetaData();

        public bool IsDeleted { get; set; }

        public FormAnswerPassingStatus PassingStatus { get; set; }

        public bool IsCompleted { get; set; }
    }
}
