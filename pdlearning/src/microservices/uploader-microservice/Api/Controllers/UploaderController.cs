using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Uploader.Application.Constants;
using Microservice.Uploader.Application.RequestDtos;
using Microservice.Uploader.Dtos;
using Microservice.Uploader.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Microservice.Uploader.Controllers
{
    [Route("api/uploader")]
    public class UploaderController : ApplicationApiController
    {
        private readonly IAmazonS3PersonalSpaceUploaderService _personalFileUploaderService;
        private readonly IAmazonS3UploaderService _uploaderService;
        private readonly IDownloadLearningContentService _downloadService;

        public UploaderController(
            IUserContext userContext,
            IAmazonS3PersonalSpaceUploaderService personalFileUploaderService,
            IAmazonS3UploaderService uploaderService,
            IDownloadLearningContentService downloadService) : base(userContext)
        {
            _uploaderService = uploaderService;
            _downloadService = downloadService;
            _personalFileUploaderService = personalFileUploaderService;
        }

        [HttpGet("getFile")]
        public string GetFile([FromQuery] string key)
        {
            return _uploaderService.GetFile(key);
        }

        [HttpPost("getFiles")]
        public List<GetFileResult> GetFiles([FromBody] string[] keys)
        {
            return _uploaderService.GetFiles(keys);
        }

        [HttpPost("createPersonalFileMultipartUploadSession")]
        public async Task<CreateMultipartUploadSessionResult> CreatePersonalFileMultipartUpload([FromBody] CreateMultipartUploadSessionRequest request, CancellationToken cancellationToken)
        {
            request.UserId = CurrentUserId;
            return await _personalFileUploaderService.CreateMultipartUpload(request, cancellationToken);
        }

        [HttpPost("createMultipartUploadSession")]
        public async Task<CreateMultipartUploadSessionResult> CreateMultipartUpload([FromBody] CreateMultipartUploadSessionRequest request, CancellationToken cancellationToken)
        {
            return await _uploaderService.CreateMultipartUpload(request, cancellationToken);
        }

        [HttpPost("createMultipartPreSignedUrl")]
        public CreateMultipartPreSignedUrlResult CreateMultipartPreSignedUrl([FromBody] CreateMultipartPreSignedUrlRequest request)
        {
            return _uploaderService.CreateMultipartPreSignedUrl(request);
        }

        [HttpPost("completeMultipartUploadSession")]
        public async Task CompleteMultipartUpload([FromBody] CompleteMultipartUploadSessionRequest request, CancellationToken cancellationToken)
        {
            await _uploaderService.CompleteMultipartUpload(request, cancellationToken);
        }

        [HttpPost("abortMultipartUploadSession")]
        public async Task AbortMultipartUpload([FromBody] AbortMultipartUploadSessionRequest request, CancellationToken cancellationToken)
        {
            await _uploaderService.AbortMultipartUpload(request, cancellationToken);
        }

        [HttpPost("completeMultipartFile")]
        public async Task<CompleteMultipartFileResult> CompleteMultipartFile([FromBody] CompleteMultipartFileResquest request, CancellationToken cancellationToken)
        {
            return await _uploaderService.CompleteMultipartFile(request, cancellationToken);
        }

        [HttpPost("extractScormPackage")]
        public async Task ExtractScormPackage([FromBody] ExtractScormPackageRequest request, CancellationToken cancellationToken)
        {
            await _uploaderService.ExtractScormPackage(request, cancellationToken);
        }

        [HttpPost("getScormProcessingStatus")]
        public async Task<ScormProcessingStatusResult> GetScormProcessingStatus([FromBody] ExtractScormPackageRequest request, CancellationToken cancellationToken)
        {
           return await _uploaderService.GetScormProcessingStatus(request, cancellationToken);
        }

        [HttpPost("downloadLearningContent")]
        public async Task<IActionResult> DownloadLearningContent([FromBody] DownloadLearningContentRequest request, CancellationToken cancellationToken)
        {
            var result = await _downloadService.DownloadLearningContent(request, cancellationToken);

            var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            return DownloadFile(result.HtmlContentConverted, FileFormatMappingMineTypeConstant.FileFormatMappingMineType[result.FileFormat], $"DownloadContent_{currentDate}.doc");
        }
    }
}
