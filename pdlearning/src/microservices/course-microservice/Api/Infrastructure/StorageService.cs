using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Settings;
using Microsoft.Extensions.Options;

namespace Microservice.Course.Infrastructure
{
    public interface IStorageService
    {
        Task<string> GetFileAsBase64String(string storageFileUrl, CancellationToken cancellationToken = default);
    }

    public class StorageService : IStorageService
    {
        private readonly AmazonS3Options _amazonS3Options;
        private readonly IAmazonS3 _amazonS3Client;

        public StorageService(IAmazonS3 amazonS3Client, IOptions<AmazonS3Options> amazonS3Options)
        {
            _amazonS3Client = amazonS3Client;
            _amazonS3Options = amazonS3Options.Value;
        }

        public async Task<string> GetFileAsBase64String(string storageFileUrl, CancellationToken cancellationToken = default)
        {
            var request = new GetObjectRequest
            {
                BucketName = _amazonS3Options.BucketName,
                Key = new Uri(storageFileUrl).LocalPath.Substring(1)
            };

            using var response = await _amazonS3Client.GetObjectAsync(request, cancellationToken);

            await using var responseStream = response.ResponseStream;

            return Convert.ToBase64String(await responseStream.ToByteArrayAsync(cancellationToken));
        }
    }
}
