using System;

namespace Microservice.LnaForm.Application.Models
{
    public class FormSectionModel
    {
        public FormSectionModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormSectionModel(Domain.Entities.FormSection entity)
        {
            Id = entity.Id;
            FormId = entity.FormId;
            MainDescription = entity.MainDescription;
            AdditionalDescription = entity.AdditionalDescription;
            Priority = entity.Priority;
            NextQuestionId = entity.NextQuestionId;
            IsDeleted = entity.IsDeleted;
            CreatedDate = entity.CreatedDate;
            ChangedDate = entity.ChangedDate;
        }

        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public string MainDescription { get; set; }

        public string AdditionalDescription { get; set; }

        public int Priority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
