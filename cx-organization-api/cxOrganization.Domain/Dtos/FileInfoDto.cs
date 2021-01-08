using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Entities;
using System;

namespace cxOrganization.Domain.Dtos
{
    public class FileInfoDto
    {
        public static FileInfoDto MapToFileInfoDto(FileInfoEntity fileInfoEntity)
        {
            if (fileInfoEntity is null)
            {
                throw new ArgumentNullException(nameof(fileInfoEntity));
            }

            return new FileInfoDto
            {
                FileInfoId = fileInfoEntity.FileInfoId,
                FileName = fileInfoEntity.FileName,
                FilePath = fileInfoEntity.FilePath,
                OriginalFileName = fileInfoEntity.OriginalFileName,
                Type = fileInfoEntity.Type,
                NumberOfRecord = fileInfoEntity.NumberOfRecord,
                FileTarget = fileInfoEntity.FileTarget,
                CreatedDate = fileInfoEntity.CreatedDate,
                UserGuid = fileInfoEntity.UserGuid
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

        public Guid UserGuid { get; set; }
    }
}
