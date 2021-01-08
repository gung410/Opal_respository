using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos;
using cxPlatform.Core.Entities;
using System;

namespace cxOrganization.Domain.Entities
{
    public class FileInfoEntity : ISoftDeleteEntity
    {
        public static FileInfoEntity MapToFileInfoEntity(FileInfoDto fileInfoDto)
        {
            if(fileInfoDto is null)
            {
                throw new ArgumentNullException(nameof(fileInfoDto));
            }

            return new FileInfoEntity
            {
                FileInfoId = fileInfoDto.FileInfoId,
                FileName = fileInfoDto.FileName,
                FilePath = fileInfoDto.FilePath,
                OriginalFileName = fileInfoDto.OriginalFileName,
                Type = fileInfoDto.Type,
                NumberOfRecord = fileInfoDto.NumberOfRecord,
                FileTarget = fileInfoDto.FileTarget,
                CreatedDate = fileInfoDto.CreatedDate,
                UserGuid = fileInfoDto.UserGuid
            };
        }
        public int FileInfoId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string OriginalFileName { get; set; }

        public string Type { get; set; }

        public int NumberOfRecord { get; set; }

        public FileTarget FileTarget { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? Deleted { get; set; }

        public Guid UserGuid { get; set; }
    }
}
