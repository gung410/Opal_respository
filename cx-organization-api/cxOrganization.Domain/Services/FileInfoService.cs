using cxOrganization.Domain.Common;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.RequestDtos.FileRequest;
using cxOrganization.Domain.Services.StorageServices;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public class FileInfoService : IFileInfoService
    {
        private readonly IFileInfoRepository _fileInfoRepository;
        private readonly ILogger<FileInfoService> _logger;
        private readonly IFileStorageService _fileStorageService;
        private readonly AppSettings _appSettings;
        private readonly IMassCreationUserService _massCreationUserService;

        public FileInfoService(
            IFileInfoRepository fileInfoRepository,
            ILogger<FileInfoService> logger,
            IFileStorageService fileStorageService,
            IOptions<AppSettings> appSettingOptions,
            IMassCreationUserService massCreationUserService
        )
        {
            _fileInfoRepository = fileInfoRepository;
            _logger = logger;
            _fileStorageService = fileStorageService;
            _appSettings = appSettingOptions.Value;
            _massCreationUserService = massCreationUserService;
        }

        public async Task<PaginatedList<FileInfoDto>> GetFileInfosByUserIdAsync(GetFileByUserIdRequest getFileByOwnerIdRequest)
        {
            const string header = nameof(GetFileInfosByUserIdAsync);
            try
            {
                // Get queryable
                var fileInfoQueryable = _fileInfoRepository.GetFileInfosByUserIdAsync(getFileByOwnerIdRequest.UserGuid)
                                                           .Where(file => file.FileTarget == getFileByOwnerIdRequest.FileTarget);

                // Search by text
                if (!string.IsNullOrEmpty(getFileByOwnerIdRequest.SearchText))
                {
                    fileInfoQueryable = fileInfoQueryable.Where(file => file.OriginalFileName.Contains(getFileByOwnerIdRequest.SearchText));
                }

                // Order
                if (getFileByOwnerIdRequest.OrderBy == OrderBy.Title)
                {
                    fileInfoQueryable = getFileByOwnerIdRequest.OrderType == OrderByType.Asc
                        ? fileInfoQueryable.OrderBy(file => file.OriginalFileName)
                        : fileInfoQueryable.OrderByDescending(file => file.OriginalFileName);
                }

                if (getFileByOwnerIdRequest.OrderBy == OrderBy.CreatedDate)
                {
                    fileInfoQueryable = getFileByOwnerIdRequest.OrderType == OrderByType.Asc
                        ? fileInfoQueryable.OrderBy(file => file.CreatedDate)
                        : fileInfoQueryable.OrderByDescending(file => file.CreatedDate);
                }


                // Paging and mapping to Dto
                var FilesInfoPagingResult = await fileInfoQueryable.ToPagingAsync(getFileByOwnerIdRequest.PageIndex, getFileByOwnerIdRequest.PageSize);

                return new PaginatedList<FileInfoDto>(
                    FilesInfoPagingResult.Items
                                         .Select(fileEntity => FileInfoDto.MapToFileInfoDto(fileEntity))
                                         .ToList(),
                    getFileByOwnerIdRequest.PageIndex,
                    getFileByOwnerIdRequest.PageSize,
                    FilesInfoPagingResult.HasMoreData)
                {
                    TotalItems = FilesInfoPagingResult.TotalItems
                };
            }

            catch(Exception ex)
            {
                _logger.LogError($"{header} - exception during get file by user id", ex);
                throw ex;
            }
           
        }

        public async Task<FileInfoDto> CreateFileInfoAsync(FileInfoDto fileInfoDto)
        {
            var mappedfileInfoEntity = FileInfoEntity.MapToFileInfoEntity(fileInfoDto);

            var createdBroadcastMessage = await _fileInfoRepository.CreateFileInfoAsync(mappedfileInfoEntity);

            return FileInfoDto.MapToFileInfoDto(createdBroadcastMessage);
        }

        public async Task<FileInfoDto> UploadFileInfoAsync(
            IFormFile formFile,
            IWorkContext workContext,
            FileTarget fileTarget,
            Guid currentUserExId)
        {
            const string header = nameof(UploadFileInfoAsync);
            var originalFileName = formFile.FileName.Replace(" ", "_");

            var fileType = FileExtension.GetValidFileType(originalFileName);

            var fileName = BuildMassUserCreationFileName(workContext.RequestId, originalFileName, fileType.ToString());

            var filePath = UploadHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings.ImportStorageFolder,
                _appSettings.MassUserCreationStorageSubFolder,
                workContext.CurrentOwnerId, workContext.CurrentCustomerId, fileName);

            using (var file = formFile.OpenReadStream())
            {
                var fileOnMemory = new MemoryStream();
                await file.CopyToAsync(fileOnMemory);
                fileOnMemory.ResetPosition();
                int numberOfRecords = 0;

                switch (fileTarget)
                {
                    case FileTarget.MassUserCreation:
                        numberOfRecords = _massCreationUserService.GetNumberOfUserCreationRecord(file);
                        break;
                    default:
                        break;
                }

                try
                {
                    var uploadedFilePath = await _fileStorageService.UploadFileAsync(workContext, fileOnMemory.ToArray(), filePath);

                    var uploadedFileInfo = new FileInfoDto()
                    {
                        CreatedDate = DateTime.UtcNow,
                        FileName = fileName,
                        FilePath = uploadedFilePath,
                        FileTarget = fileTarget,
                        NumberOfRecord = numberOfRecords,
                        OriginalFileName = originalFileName,
                        UserGuid = currentUserExId,
                        Type = fileType.ToString(),
                    };

                    var fileInfo = await CreateFileInfoAsync(uploadedFileInfo);

                    return fileInfo;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{header} - exception during upload file to AWS S3", ex);
                    throw ex;
                }
            }
        }

        public async Task<byte[]> DownloadFile(string fileName, FileTarget fileTarget, IWorkContext workContext)
        {
            string storageSubFolder = string.Empty;

            switch(fileTarget){
                case FileTarget.MassUserCreation:
                    storageSubFolder = _appSettings.MassUserCreationStorageSubFolder;
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(storageSubFolder))
            {
                throw new Exception($"this {fileTarget.ToString()} has not supported yet");
            }

            var fullFilePath = UploadHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, 
                                                                   _appSettings.ImportStorageFolder,
                                                                    storageSubFolder, 
                                                                    workContext.CurrentOwnerId,
                                                                    workContext.CurrentCustomerId,
                                                                    fileName);

            var fileContent = await _fileStorageService.DownloadFileAsync(fullFilePath);

            var memoryStream = new MemoryStream();

            fileContent.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        private string BuildMassUserCreationFileName(string requestId, string fileName, string fileExtension)
        {
            return $"{Path.GetFileNameWithoutExtension(fileName)}_{requestId}.{fileExtension}";
        }
    }
}
