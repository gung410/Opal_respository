using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class CreateOrUpdateSectionDto
    {
        public Guid? Id { get; set; }

        public string Description { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public int? CreditsAward { get; set; }
    }
}
