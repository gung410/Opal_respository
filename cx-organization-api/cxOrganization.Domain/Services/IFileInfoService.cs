using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos;
using cxOrganization.Domain.RequestDtos.FileRequest;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public interface IFileInfoService
    {
        public Task<PaginatedList<FileInfoDto>> GetFileInfosByUserIdAsync(GetFileByUserIdRequest getFileByOwnerIdRequest);
        public Task<FileInfoDto> CreateFileInfoAsync(FileInfoDto fileInfoEntity);
        public Task<FileInfoDto> UploadFileInfoAsync(IFormFile formFile, IAdvancedWorkContext workContext, FileTarget fileTarget, Guid currentUserExId);
        public Task<byte[]> DownloadFile(string fileName, FileTarget fileTarget, IAdvancedWorkContext workContext);
    }
}
