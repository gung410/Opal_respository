using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microservice.WebinarVideoConverter.Application.Models;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microservice.WebinarVideoConverter.Application.Services
{
    public class UploadService : IUploaderService
    {
        private readonly ILogger _logger;
        private readonly AmazonS3Options _amazonS3Options;
        private readonly RecordingConvertOptions _recordingConvertOptions;
        private readonly IAmazonS3 _s3Client;

        public UploadService(
            ILogger<UploadService> logger,
            IOptions<AmazonS3Options> amazonS3Options,
            IOptions<RecordingConvertOptions> recordingConvertOptions,
            IAmazonS3 s3Client)
        {
            _logger = logger;
            _amazonS3Options = amazonS3Options.Value;
            _recordingConvertOptions = recordingConvertOptions.Value;
            _s3Client = s3Client;
        }

        public async Task<UploadResultModel> UploadMeetingRecordAsync(string internalMeetingId)
        {
            var recordPath = $"{_recordingConvertOptions.ConvertedVideoDir}/{internalMeetingId}.mp4";

            if (!File.Exists(recordPath))
            {
                return new UploadResultModel
                {
                    IsSuccess = false
                };
            }

            try
            {
                FileInfo fileInfo = new FileInfo(recordPath);
                using (var fileToUpload = new FileStream(recordPath, FileMode.Open, FileAccess.Read))
                {
                    var fileTransferUtility = new TransferUtility(_s3Client);
                    var fileName = Path.GetFileName(recordPath);
                    var filePath = $"{_amazonS3Options.WebinarPlaybacksDir}/{fileName}";
                    await fileTransferUtility.UploadAsync(recordPath, _amazonS3Options.BucketName, filePath);
                    return new UploadResultModel
                    {
                        IsSuccess = true,
                        FilePath = filePath,
                        FileSize = fileInfo.Length,
                    };
                }
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered on server. Message: {Message} when writing an object", e.Message);
                return new UploadResultModel
                {
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error encountered on client when uploading file. Detail: {Messsage}", ex.Message);
                return new UploadResultModel
                {
                    IsSuccess = false
                };
            }
        }
    }
}
