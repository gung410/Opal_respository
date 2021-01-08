using System;
using Microservice.Uploader.Domain.Entities;
using Microservice.Uploader.Domain.ValueObjects;

namespace Microservice.Uploader.Application.Models
{
    public class PersonalFileModel
    {
        public PersonalFileModel()
        {
        }

        public PersonalFileModel(PersonalFile entity)
        {
            this.Id = entity.Id;
            this.UserId = entity.UserId;
            this.FileName = entity.FileName;
            this.FileType = entity.FileType;
            this.FileExtension = entity.FileExtension;
            this.FileSize = entity.FileSize;
            this.FileLocation = entity.FileLocation;
            this.CreatedDate = entity.CreatedDate;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string FileName { get; set; }

        public FileType FileType { get; set; }

        public string FileExtension { get; set; }

        public double FileSize { get; set; }

        public string FileLocation { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
