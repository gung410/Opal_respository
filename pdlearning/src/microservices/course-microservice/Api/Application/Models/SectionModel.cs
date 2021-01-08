using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class SectionModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public int? CreditsAward { get; set; }

        public static SectionModel Create(Section section)
        {
            return new SectionModel
            {
                Id = section.Id,
                Description = section.Description,
                Title = section.Title,
                CourseId = section.CourseId,
                ClassRunId = section.ClassRunId,
                Order = section.Order.GetValueOrDefault(),
                CreditsAward = section.CreditsAward
            };
        }
    }
}
