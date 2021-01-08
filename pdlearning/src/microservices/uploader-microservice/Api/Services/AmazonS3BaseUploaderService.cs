using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microservice.Uploader.Application.RequestDtos;
using Microservice.Uploader.Dtos;
using Microservice.Uploader.Options;
using Microservice.Uploader.Signers;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Uploader.Services
{
    public class AmazonS3BaseUploaderService
    {
        private readonly AmazonS3Options _options;
        private readonly AcceptanceOptions _acceptanceOptions;
        private readonly AllowedFilesOptions _allowedOptions;
        private readonly IAmazonS3 _client;
        private readonly ScormProcessingOptions _scormOptions;
        private readonly IAmazonS3KeyBuilderService _keyBuilderService;

        public AmazonS3BaseUploaderService(
            IOptions<AmazonS3Options> options,
            IOptions<AcceptanceOptions> acceptanceOptions,
            IOptions<AllowedFilesOptions> allowedOptions,
            IOptions<ScormProcessingOptions> scormOptions,
            IAmazonS3 amazonS3,
            IAmazonS3KeyBuilderService keyBuilderService)
        {
            _options = options.Value;
            _acceptanceOptions = acceptanceOptions.Value;
            _allowedOptions = allowedOptions.Value;
            _scormOptions = scormOptions.Value;
            _client = amazonS3;
            _keyBuilderService = keyBuilderService;
        }

        public string GetFile(string key)
        {
            return _client.GetPreSignedURL(
                new GetPreSignedUrlRequest()
                {
                    BucketName = _options.BucketName,
                    Key = key,
                    Expires = DateTime.Now.AddMinutes(_options.FileExpirationMinutes)
                });
        }

        public List<GetFileResult> GetFiles(string[] keys)
        {
            List<GetFileResult> result = new List<GetFileResult>();

            foreach (var key in keys)
            {
                result.Add(new GetFileResult()
                {
                    Key = key,
                    Url = _client.GetPreSignedURL(
                        new GetPreSignedUrlRequest()
                        {
                            BucketName = _options.BucketName,
                            Key = key,
                            Expires = DateTime.Now.AddMinutes(_options.FileExpirationMinutes)
                        })
                });
            }

            return result;
        }

        public async Task<CreateMultipartUploadSessionResult> CreateMultipartUpload(CreateMultipartUploadSessionRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var isValid = _acceptanceOptions.DirectoryName.Any(item => item == request.Folder);
            if (!isValid)
            {
                throw new BusinessLogicException("NotAccepted");
            }

            if (!_allowedOptions.AllowedExtensions.Contains(request.FileExtension))
            {
                throw new BusinessLogicException("This file type is not supported");
            }

            var contentType = request.FileExtension == "svg" ? "image/svg+xml" : "application/octet-stream";

            await AdditionalFileValidation(request);

            var response = await _client.InitiateMultipartUploadAsync(
                new InitiateMultipartUploadRequest()
                {
                    BucketName = _options.BucketName,
                    Key = _keyBuilderService.BuildTemporaryStorageKey(request.FileId, request.FileExtension, request.Folder),
                    ContentType = contentType,
                },
                cancellationToken);

            return new CreateMultipartUploadSessionResult()
            {
                UploadId = response.UploadId
            };
        }

        public CreateMultipartPreSignedUrlResult CreateMultipartPreSignedUrl(CreateMultipartPreSignedUrlRequest request)
        {
            var endpointUrl = _keyBuilderService.BuildEndpointUrl(request.FileId, request.FileExtension, request.Folder);
            var expirationTime = DateTime.UtcNow.AddSeconds(_options.PartExpirationInSecond);
            var period = Convert.ToInt64((expirationTime.ToUniversalTime() - DateTime.UtcNow).TotalSeconds);
            var query = $"partNumber={request.PartNumber}" +
                        $"&uploadId={request.UploadId}" +
                        $"&{AmazonSignerBase.XAmzExpires}={AmazonHttpHelpers.UrlEncode(period.ToString())}";
            var headers = new Dictionary<string, string>();
            var signer = new AmazonQueryParameterSigner
            {
                EndpointUri = new Uri(endpointUrl),
                HttpMethod = "PUT",
                Service = "s3",
                Region = _options.Region
            };
            var authorization = signer.ComputeSignature(
                headers,
                query,
                "UNSIGNED-PAYLOAD",
                _options.AccessKey,
                _options.SecretKey);

            return new CreateMultipartPreSignedUrlResult() { Url = $"{endpointUrl}?{query}&{authorization}" };
        }

        public async Task CompleteMultipartUpload(CompleteMultipartUploadSessionRequest request, CancellationToken cancellationToken = default)
        {
            await this._client.CompleteMultipartUploadAsync(
                new CompleteMultipartUploadRequest()
                {
                    BucketName = _options.BucketName,
                    Key = _keyBuilderService.BuildTemporaryStorageKey(request.FileId, request.FileExtension, request.Folder),
                    UploadId = request.UploadId,
                    PartETags = request.PartETags
                },
                cancellationToken);
        }

        public async Task AbortMultipartUpload(AbortMultipartUploadSessionRequest request, CancellationToken cancellationToken = default)
        {
            await this._client.AbortMultipartUploadAsync(
                new AbortMultipartUploadRequest()
                {
                    BucketName = _options.BucketName,
                    Key = _keyBuilderService.BuildTemporaryStorageKey(request.FileId, request.FileExtension, request.Folder),
                    UploadId = request.UploadId,
                },
                cancellationToken);
        }

        public async Task<CompleteMultipartFileResult> CompleteMultipartFile(CompleteMultipartFileResquest request, CancellationToken cancellationToken = default)
        {
            var sourceKey = _keyBuilderService.BuildTemporaryStorageKey(request.FileId, request.FileExtension, request.Folder);
            var destKey = _keyBuilderService.BuildPermanentStorageKey(request.FileId, request.FileExtension, request.Folder);
            var existed = await ExistsWithBucketCheck(_options.BucketName, sourceKey, cancellationToken);

            if (!existed)
            {
                throw new GeneralException($"The file with id {request.FileId} does not exist");
            }

            if (!request.IsTemporary)
            {
                await _client.CopyObjectAsync(
                new CopyObjectRequest
                {
                    DestinationBucket = _options.BucketName,
                    DestinationKey = destKey,
                    SourceBucket = _options.BucketName,
                    SourceKey = sourceKey
                },
                cancellationToken);
                await _client.DeleteObjectAsync(
                    new DeleteObjectRequest()
                    {
                        BucketName = _options.BucketName,
                        Key = sourceKey
                    },
                    cancellationToken);
            }

            return new CompleteMultipartFileResult()
            {
                Location = request.IsTemporary ? sourceKey : destKey
            };
        }

#pragma warning disable CA1801 // Review unused parameters
        protected virtual Task AdditionalFileValidation(CreateMultipartUploadSessionRequest request)
#pragma warning restore CA1801 // Review unused parameters
        {
            return Task.CompletedTask;
        }

        protected async Task<bool> ExistsWithBucketCheck(string bucketName, string key, CancellationToken cancellationToken)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                await _client.GetObjectMetadataAsync(request, cancellationToken);

                return true;
            }
            catch (AmazonS3Exception e)
            {
                if (string.Equals(e.ErrorCode, "NoSuchBucket"))
                {
                    return false;
                }
                else if (string.Equals(e.ErrorCode, "NotFound"))
                {
                    return false;
                }

                throw;
            }
        }

        protected void WriteStatusToFile(string fileFolder, string fileName, string statsus)
        {
            string flagFilePath = Path.Combine(fileFolder, fileName);
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }

            System.IO.File.WriteAllText(flagFilePath, statsus);
        }

        protected string GenerateFileName(ExtractScormPackageRequest request)
        {
            return request.FileId.ToString() + request.FileExtension + "_status.txt";
        }
    }
}
