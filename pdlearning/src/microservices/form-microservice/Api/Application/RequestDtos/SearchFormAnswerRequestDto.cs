using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.RequestDtos
{
    public class SearchFormAnswerRequestDto
    {
        public string SearchText { get; set; }

        public Guid? UserId { get; set; }

        public Guid? FormId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? MyCourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? AssignmentId { get; set; }

        public bool? IsSubmitted { get; set; }

        public bool? IsCompleted { get; set; }

        public bool? BeforeDueDate { get; set; }

        public bool? BeforeTimeLimit { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
