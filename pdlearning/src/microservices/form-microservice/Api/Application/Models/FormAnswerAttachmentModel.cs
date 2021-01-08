using System;
using Microservice.Form.Domain.ValueObjects;
using FormAnswerAttachmentEntity = Microservice.Form.Domain.Entities.FormAnswerAttachment;

namespace Microservice.Form.Application.Models
{
    public class FormAnswerAttachmentModel
    {
        public FormAnswerAttachmentModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormAnswerAttachmentModel(FormAnswerAttachmentEntity formAttachmentEntity)
        {
            Id = formAttachmentEntity.Id;
            FormQuestionAnswerId = formAttachmentEntity.FormQuestionAnswerId;
            FileName = formAttachmentEntity.FileName;
            FileType = formAttachmentEntity.FileType;
            FileExtension = formAttachmentEntity.FileExtension;
            FileLocation = formAttachmentEntity.FileLocation;
            FileSize = formAttachmentEntity.FileSize;
            CreatedDate = formAttachmentEntity.CreatedDate;
        }

        public Guid Id { get; set; }

        public Guid? FormQuestionAnswerId { get; set; }

        public string FileName { get; set; }

        public FileType FileType { get; set; }

        public string FileExtension { get; set; }

        public string FileLocation { get; set; }

        public double FileSize { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
