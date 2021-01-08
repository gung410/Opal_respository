using System;
using Microservice.Uploader.Options;
using Microsoft.Extensions.Options;

namespace Microservice.Uploader.Services
{
    public class AmazonS3KeyBuilderService : IAmazonS3KeyBuilderService
    {
        private readonly AmazonS3Options _options;
        private readonly string _defaultFolder = "others";

        public AmazonS3KeyBuilderService(IOptions<AmazonS3Options> options)
        {
            _options = options.Value;
        }

        public string BuildTemporaryStorageKey(Guid fileId, string extension, string folder = null)
        {
            return $"{_options.TemporaryFolder}/{folder ?? this._defaultFolder}/{extension}/{fileId}.{extension}";
        }

        public string BuildPermanentStorageKey(Guid fileId, string extension, string folder = null)
        {
            return $"{_options.PermanentFolder}/{folder ?? this._defaultFolder}/{extension}/{fileId}.{extension}";
        }

#pragma warning disable CA1055 // URI-like return values should not be strings
        public string BuildEndpointUrl(Guid fileId, string extension, string folder = null)
#pragma warning restore CA1055 // URI-like return values should not be strings
        {
            return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{_options.TemporaryFolder}/{folder ?? this._defaultFolder}/{extension}/{fileId}.{extension}";
        }
    }
}
