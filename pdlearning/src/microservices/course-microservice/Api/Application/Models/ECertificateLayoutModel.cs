using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Application.Models
{
    public class ECertificateLayoutModel
    {
        public ECertificateLayoutModel()
        {
        }

        public ECertificateLayoutModel(ECertificateLayout entity, string base64PreviewImage)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;
            Params = entity.Params;
            Base64PreviewImage = base64PreviewImage;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<ECertificateLayoutParam> Params { get; set; }

        public string Base64PreviewImage { get; set; }
    }
}
