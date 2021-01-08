using System;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.LnaForm.Domain.Entities
{
    public class FormAnswer : BaseEntity, ISoftDelete
    {
        public Guid FormId { get; set; }

        public Guid? ResourceId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? SubmitDate { get; set; }

        public short Attempt { get; set; } = 1;

        public FormAnswerFormMetaData FormMetaData { get; set; } = new FormAnswerFormMetaData();

        public Guid OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsCompleted { get; set; }
    }
}
