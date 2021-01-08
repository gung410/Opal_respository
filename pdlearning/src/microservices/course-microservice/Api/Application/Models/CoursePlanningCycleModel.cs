using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class CoursePlanningCycleModel
    {
        public CoursePlanningCycleModel(CoursePlanningCycle entity)
        {
            Id = entity.Id;
            YearCycle = entity.YearCycle;
            Title = entity.Title;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            Description = entity.Description;
            CreatedBy = entity.CreatedBy;
            ChangedBy = entity.ChangedBy;
            IsConfirmedBlockoutDate = entity.IsConfirmedBlockoutDate;
        }

        public CoursePlanningCycleModel(CoursePlanningCycle entity, int numberOfCourses)
        {
            Id = entity.Id;
            YearCycle = entity.YearCycle;
            Title = entity.Title;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            Description = entity.Description;
            CreatedBy = entity.CreatedBy;
            ChangedBy = entity.ChangedBy;
            NumberOfCourses = numberOfCourses;
            IsConfirmedBlockoutDate = entity.IsConfirmedBlockoutDate;
        }

        public Guid Id { get; set; }

        public int YearCycle { get; set; }

        public string Title { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Description { get; set; }

        public int? NumberOfCourses { get; set; }

        public bool IsConfirmedBlockoutDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }
    }
}
