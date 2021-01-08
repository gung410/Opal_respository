using System;
using Microservice.Form.Domain.ValueObjects;

namespace Microservice.Form.Domain.Entities
{
    public class FormAnswerAttachment : BaseEntity
    {
        public Guid FormQuestionAnswerId { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public FileType FileType { get; set; }

        public string FileLocation { get; set; }

        public double FileSize { get; set; }
    }
}
