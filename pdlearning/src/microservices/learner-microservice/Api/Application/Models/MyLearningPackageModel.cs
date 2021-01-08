using System;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class MyLearningPackageModel
    {
        public MyLearningPackageModel(MyLearningPackage myLearningPackage)
        {
            Id = myLearningPackage.Id;
            UserId = myLearningPackage.UserId;
            MyLectureId = myLearningPackage.MyLectureId;
            MyDigitalContentId = myLearningPackage.MyDigitalContentId;
            Type = myLearningPackage.Type;
            State = myLearningPackage.State;
            LessonStatus = myLearningPackage.LessonStatus;
            CompletionStatus = myLearningPackage.CompletionStatus;
            SuccessStatus = myLearningPackage.SuccessStatus;
            CreatedDate = myLearningPackage.CreatedDate;
            CreatedBy = myLearningPackage.CreatedBy;
            ChangedDate = myLearningPackage.ChangedDate;
            ChangedBy = myLearningPackage.ChangedBy;
            TimeSpan = myLearningPackage.TimeSpan;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? MyLectureId { get; set; }

        public Guid? MyDigitalContentId { get; set; }

        public LearningPackageType Type { get; set; }

        public string State { get; set; }

        public string LessonStatus { get; set; }

        public string CompletionStatus { get; set; }

        public string SuccessStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid? ChangedBy { get; set; }

        public int? TimeSpan { get; set; }
    }
}
