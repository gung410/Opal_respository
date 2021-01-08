using System;

namespace Microservice.Form.Application.Services
{
    public class UpdateFormSectionRequestDto
    {
        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public string MainDescription { get; set; }

        public string AdditionalDescription { get; set; }

        public int Priority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
