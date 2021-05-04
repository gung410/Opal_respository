using System;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.RequestDtos.FileRequest;
using cxOrganization.Domain.Services;
using cxOrganization.WebServiceAPI.Models;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    [Route("file")]
    public class FileController : ApiControllerBase
    {
        private readonly IFileInfoService _fileInfoService;
        private readonly IAdvancedWorkContext _workContext;
        public FileController(
                IFileInfoService fileInfoService,
                IAdvancedWorkContext workContext
                )
        {
            _fileInfoService = fileInfoService;
            _workContext = workContext;
        }

        [Route("massusercreation")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilesInfoByUserId([FromQuery] GetFileByUserIdRequest getFileByUserIdRequest)
        {
            var filesResult = await _fileInfoService.GetFileInfosByUserIdAsync(getFileByUserIdRequest);

            return Ok(filesResult);
        }

        [HttpPost("upload/massusercreation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadMassUserCreationFile([FromForm] UploadFileRequest uploadFileRequest)
        {
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);
            try
            {
                var fileResult = await this._fileInfoService.UploadFileInfoAsync(uploadFileRequest.File,
                                                                                 copiedWorkContext,
                                                                                 FileTarget.MassUserCreation,
                                                                                 uploadFileRequest.CurrentUserExId);
                return Ok(fileResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("download/massusercreation/{filename}")]
        [HttpGet]
        public async Task<IActionResult> DownloadMassUserCreationFile(string filename)
        {
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);
            var fileData = await this._fileInfoService.DownloadFile(filename, FileTarget.MassUserCreation, copiedWorkContext);
            var contentType = GetContentTypeForFileExtension(System.IO.Path.GetExtension(filename));

            return File(fileData, contentType, filename);
        }

        private string GetContentTypeForFileExtension(string fileExtension)
        {
            if (FileExtension.FileTypeContentTypeMappings.TryGetValue(fileExtension, out var contentType))
            {
                return contentType;
            }
            return "application/json";
        }
    }
}
