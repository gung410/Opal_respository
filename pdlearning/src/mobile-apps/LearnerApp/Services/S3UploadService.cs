using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models.Upload;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;
using Plugin.FilePicker.Abstractions;
using Plugin.Media.Abstractions;

namespace LearnerApp.Services
{
    public class S3UploadService : BaseViewModel, IS3UploadService
    {
        private readonly IUploaderBackendService _uploaderBackendService;

        /// <summary>
        /// Upload to folder.
        /// </summary>
        private string _uploadFolder;

        /// <summary>
        /// 1048576 bytes = 1MB.
        /// </summary>
        private int _byteInMegabyte = 1048576;

        private string _fileId;
        private string _uploadId;
        private string _fileExtension;
        private List<MultipartEtag> _multiPartsTag;
        private Stream _fileStream;
        private HttpClient _httpClient;

        public S3UploadService()
        {
            _uploaderBackendService = CreateRestClientFor<IUploaderBackendService>(GlobalSettings.BackendServiceUploader);
            _httpClient = new HttpClient();
        }

        public async Task<string> UploadFile(MediaFile file, string folder)
        {
            _uploadFolder = folder;
            _fileStream = file.GetStream();

            _fileExtension = Path.GetExtension(file.Path).Replace(".", string.Empty);
            _fileId = Guid.NewGuid().ToString();

            _uploadId = await CreateUploadSession();

            if (string.IsNullOrEmpty(_uploadId))
            {
                return string.Empty;
            }

            _multiPartsTag = await StartUploadParts();

            if (_multiPartsTag.IsNullOrEmpty())
            {
                return string.Empty;
            }

            await CompleteUploadSession();

            string uploadedLocation = await CompleteFileUpload();

            return $"{GlobalSettings.CloudFrontUrl}/{uploadedLocation}";
        }

        public async Task<string> UploadFile(FileData file, string folder)
        {
            _uploadFolder = folder;
            _fileStream = file.GetStream();

            _fileExtension = Path.GetExtension(file.FileName).Replace(".", string.Empty).ReplaceExtensionUppercaseToLowercase();
            _fileId = Guid.NewGuid().ToString();

            _uploadId = await CreateUploadSession();

            if (string.IsNullOrEmpty(_uploadId))
            {
                return string.Empty;
            }

            _multiPartsTag = await StartUploadParts();

            if (_multiPartsTag.IsNullOrEmpty())
            {
                return string.Empty;
            }

            await CompleteUploadSession();

            string uploadedLocation = await CompleteFileUpload();

            return $"{GlobalSettings.CloudFrontUrl}/{uploadedLocation}";
        }

        private async Task<string> CreateUploadSession()
        {
            var response = await ExecuteBackendService(() => _uploaderBackendService.CreateMultipartUploadSession(new { FileId = _fileId, FileExtension = _fileExtension, Folder = _uploadFolder }));

            return response.IsError ? string.Empty : response.Payload.UploadId;
        }

        private async Task<List<MultipartEtag>> StartUploadParts()
        {
            var preSignedUrlResult = await ExecuteBackendService(() => _uploaderBackendService.CreateMultipartPreSignedUrl(new { UploadId = _uploadId, FileId = _fileId, FileExtension = _fileExtension, Folder = _uploadFolder, PartNumber = 1 }));
            var multiParts = new List<MultipartEtag>();

            if (preSignedUrlResult.IsError)
            {
                return null;
            }

            string preSignedUrl = preSignedUrlResult.Payload.Url;

            var httpContent = new StreamContent(_fileStream, _byteInMegabyte);

            var requestor = await _httpClient.PutAsync(preSignedUrl, httpContent);

            if (requestor.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            multiParts.Add(new MultipartEtag { ETag = requestor.Headers.ETag.Tag, PartNumber = 1 });

            return multiParts;
        }

        private async Task CompleteUploadSession()
        {
            await ExecuteBackendService(() => _uploaderBackendService.CompleteMultipartUploadSession(new { FileId = _fileId, PartETags = _multiPartsTag, UploadId = _uploadId, Folder = _uploadFolder, FileExtension = _fileExtension }));
        }

        private async Task<string> CompleteFileUpload()
        {
            var result = await ExecuteBackendService(() => _uploaderBackendService.CompleteMultipartFile(new { FileId = _fileId, PartETags = _multiPartsTag, UploadId = _uploadId, Folder = _uploadFolder, FileExtension = _fileExtension }));

            return result.HasEmptyResult() ? null : result.Payload.Location;
        }
    }
}
