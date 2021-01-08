using System;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class LectureInMyCourseModel
    {
        public LectureInMyCourseModel(LectureInMyCourse lectureInMyCourse)
        {
            Id = lectureInMyCourse.Id;
            MyCourseId = lectureInMyCourse.MyCourseId;
            LectureId = lectureInMyCourse.LectureId;
            UserId = lectureInMyCourse.UserId;
            Status = lectureInMyCourse.Status;
            ReviewStatus = lectureInMyCourse.ReviewStatus;
            StartDate = lectureInMyCourse.StartDate;
            EndDate = lectureInMyCourse.EndDate;
            LastLogin = lectureInMyCourse.LastLogin;
            CreatedDate = lectureInMyCourse.CreatedDate;
            CreatedBy = lectureInMyCourse.CreatedBy;
            ChangedDate = lectureInMyCourse.ChangedDate;
            ChangedBy = lectureInMyCourse.ChangedBy;
            Version = lectureInMyCourse.Version;
        }

        public Guid Id { get; set; }

        public Guid MyCourseId { get; set; }

        public Guid LectureId { get; set; }

        public Guid UserId { get; set; }

        public LectureStatus Status { get; set; }

        public string ReviewStatus { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid? ChangedBy { get; set; }

        public string Version { get; set; }
    }
}
