using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services.StorageServices
{
    public class FileStorageS3Service : IFileStorageService
    {
        private readonly IAmazonS3 _client;
        private readonly ILogger _logger;
        private readonly AwsSettings _awsSettings;
        public FileStorageS3Service(ILogger<FileStorageS3Service> logger, IAmazonS3 client, IOptions<AwsSettings> awsSettingsOption)
        {
            _logger = logger;
            _client = client;
            _awsSettings = awsSettingsOption.Value;
        }

        public async Task<Stream> DownloadFileAsync(string filePath)
        {
            var fileName = "";
            var bucketName = _awsSettings.BucketName;
            try
            {
                fileName = EscapeFileName(filePath);

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };

                var response = await _client.GetObjectAsync(request);
                return response.ResponseStream;
            }
            catch (AmazonS3Exception e)
            {
                if (e.ErrorCode == "NoSuchKey")
                {
                    _logger.LogError(e, $"The '{fileName}' does not exist in bucket '{bucketName}'', s3 server when downloading. {e.Message}");

                    throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND,$"File {fileName} does not exist", e);
                }

                WriteErrorLogWhenDownloading(e, fileName, bucketName);

            }
            catch (Exception ex)
            {
                WriteErrorLogWhenDownloading(ex, fileName, bucketName);
            }
            return new MemoryStream();
        }

        private void WriteErrorLogWhenDownloading(Exception e, string fileName, string bucketName)
        {
            _logger.LogError(e, $"Unexpected error occurred when downloading '{fileName}' to bucket '{bucketName}', s3 server. {e.Message}");
        }

        public async Task<string> UploadFileAsync(IAdvancedWorkContext workContext, byte[] data, string filePath)
        {
            var fileName = "";
            var bucketName = _awsSettings.BucketName;
            try
            {
                fileName = EscapeFileName(filePath);

                using (var utility = new TransferUtility(_client))
                {
                    var request = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = fileName
                    };
                    AddMetadataCollection(workContext, request);

                    using (var inputStream = new MemoryStream(data))
                    {
                        request.InputStream = inputStream;
                        await utility.UploadAsync(request);
                    }
                }

                return fileName;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Unexpected error occurred when uploading '{fileName}' to bucket '{bucketName}', s3 server. {ex.Message}");
            }

            return null;
        }

        private void AddMetadataCollection(IAdvancedWorkContext workContext, TransferUtilityUploadRequest transferUtilityUploadRequest)
        {
            if (workContext == null) return;

            transferUtilityUploadRequest.Metadata.Add("ClientId", workContext.ClientId);
            transferUtilityUploadRequest.Metadata.Add("UserCxId", workContext.UserIdCXID);
            transferUtilityUploadRequest.Metadata.Add("RequestId", workContext.RequestId);
            transferUtilityUploadRequest.Metadata.Add("CorrelationId", workContext.CorrelationId);
        }

        public char FilePathDelimiter => '/';

        private string EscapeFileName(string filePath)
        {
            return string.Join(FilePathDelimiter, filePath.Split('/', '\\'));
        }
    }
}