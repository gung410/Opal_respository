using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class AnnouncementTemplateModel
    {
        public AnnouncementTemplateModel()
        {
        }

        public AnnouncementTemplateModel(AnnouncementTemplate entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Message = entity.Message;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            ChangedDate = entity.ChangedDate;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
