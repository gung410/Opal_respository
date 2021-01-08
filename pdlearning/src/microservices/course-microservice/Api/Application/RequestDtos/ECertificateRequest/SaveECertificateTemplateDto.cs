using System;
using System.Collections.Generic;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveECertificateTemplateDto
    {
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public Guid ECertificateLayoutId { get; set; }

        public List<ECertificateTemplateParam> Params { get; set; }

        public ECertificateTemplateStatus Status { get; set; }

        public SaveECertificateTemplateCommand ToCommand()
        {
            return new SaveECertificateTemplateCommand()
            {
                // Progress data for StartDateTime/EndDateTime
                Id = Id ?? Guid.NewGuid(),
                IsCreate = !Id.HasValue,
                ECertificateLayoutId = ECertificateLayoutId,
                Title = Title,
                Params = Params,
                Status = Status
            };
        }
    }
}
