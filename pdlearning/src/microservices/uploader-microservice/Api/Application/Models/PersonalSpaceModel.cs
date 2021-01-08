using System;
using Microservice.Uploader.Domain.Entities;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Uploader.Application.Models
{
    public class PersonalSpaceModel
    {
        public PersonalSpaceModel()
        {
        }

        public PersonalSpaceModel(PersonalSpace entity)
        {
            this.Id = entity.Id;
            this.UserId = entity.UserId;
            this.TotalSpace = entity.TotalSpace;
            this.TotalUsed = entity.TotalUsed;
            this.IsStorageUnlimited = entity.IsStorageUnlimited;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public double TotalSpace { get; set; }

        public double TotalUsed { get; set; }

        public bool IsStorageUnlimited { get; set; }
    }
}
