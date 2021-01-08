using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Domain.Entities
{
    public class FormQuestionAttachment : Entity, ISoftDelete
    {
        public Guid FormQuestionId { get; set; }

        [MaxLength(255)]
        public string FileName { get; set; }

        [MaxLength(100)]
        public string FileType { get; set; }

        [MaxLength(10)]
        public string FileExtension { get; set; }

        [MaxLength(1000)]
        public string FileLocation { get; set; }

        [Column(TypeName = "VARCHAR(255)")]
        [MaxLength(255)]
        public string ExternalId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
