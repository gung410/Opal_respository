using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Application.Models
{
    public class ECertificateTemplateModel
    {
        public ECertificateTemplateModel()
        {
        }

        public ECertificateTemplateModel(ECertificateTemplate entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            ECertificateLayoutId = entity.ECertificateLayoutId;
            Status = entity.Status;
            Params = entity.Params;
            CreatedBy = entity.CreatedBy;
        }

        public ECertificateTemplateModel(ECertificateTemplate entity, int totalCoursesUsing, int totalLearnersReceived, bool? hasFullRight = null)
        {
            Id = entity.Id;
            Title = entity.Title;
            ECertificateLayoutId = entity.ECertificateLayoutId;
            Status = entity.Status;
            Params = entity.Params;
            TotalCoursesUsing = totalCoursesUsing;
            TotalLearnersReceived = totalLearnersReceived;
            CreatedBy = entity.CreatedBy;
            HasFullRight = hasFullRight;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid ECertificateLayoutId { get; set; }

        public IEnumerable<ECertificateTemplateParam> Params { get; set; }

        public ECertificateTemplateStatus Status { get; set; }

        public int? TotalCoursesUsing { get; set; }

        public int? TotalLearnersReceived { get; set; }

        public Guid? CreatedBy { get; set; }

        public bool? HasFullRight { get; set; }
    }
}
