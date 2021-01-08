using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities.Abstractions;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.Interfaces;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    public class Assignment : BaseContent, IContent
    {
        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public AssignmentType Type { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public virtual AssignmentAssessmentConfig AssessmentConfig { get; set; }

        public static ExpressionValidator<Assignment> CanUseToAssignAssessmentValidator()
        {
            return new ExpressionValidator<Assignment>(
                p => p.AssessmentConfig != null,
                "Can only use assignment has assessment to assign assessment.");
        }

        public Assignment CloneForClassRun(Guid classRunId, Guid createdBy)
        {
            return new Assignment
            {
                Id = Guid.NewGuid(),
                CreatedDate = Clock.Now,
                CourseId = CourseId,
                ClassRunId = classRunId,
                Title = Title,
                Type = Type,
                CreatedBy = createdBy
            };
        }

        public Assignment CloneForCourse(Guid courseId, Guid createdBy)
        {
            return new Assignment
            {
                Id = Guid.NewGuid(),
                CreatedDate = Clock.Now,
                CourseId = courseId,
                ClassRunId = ClassRunId,
                Title = Title,
                Type = Type,
                CreatedBy = createdBy
            };
        }

        public override Guid ForTargetId()
        {
            return ClassRunId ?? CourseId;
        }

        public Validation ValidateCanUseToAssignAssessment()
        {
            return CanUseToAssignAssessmentValidator().Validate(this);
        }

        public Assignment UpdateAssessmentConfig(
            Guid? assessmentId,
            ScoreMode? scoreMode = null,
            int? numberAutoAssessor = null)
        {
            if (assessmentId != null && AssessmentConfig == null)
            {
                AssessmentConfig = new AssignmentAssessmentConfig();
            }

            if (assessmentId == null)
            {
                if (AssessmentConfig != null)
                {
                    AssessmentConfig = null;
                }
            }
            else
            {
                AssessmentConfig.AssessmentId = assessmentId.Value;
                if (numberAutoAssessor != null)
                {
                    AssessmentConfig.NumberAutoAssessor = numberAutoAssessor.Value;
                }

                if (scoreMode != null)
                {
                    AssessmentConfig.ScoreMode = scoreMode.Value;
                }
            }

            return this;
        }
    }

    public class AssignmentAssessmentConfig
    {
        public Guid AssessmentId { get; set; }

        public int NumberAutoAssessor { get; set; }

        public ScoreMode ScoreMode { get; set; } = ScoreMode.Instructor;
    }
}
