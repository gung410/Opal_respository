using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microservice.Uploader.Common.Extensions;
using Microservice.Uploader.Options;
using Microsoft.Extensions.Options;

namespace Microservice.Uploader.Infrastructure
{
    public interface IStorageService
    {
        Task<string> GetFileAsBase64String(string storageFileUrl, CancellationToken cancellationToken = default);

        Task<string> GetFilePublicAsBase64String(string fileUrl, CancellationToken cancellationToken = default);
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

        public async Task<string> GetFilePublicAsBase64String(string fileUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                using (HttpClient c = new HttpClient())
                {
                    using (Stream s = await c.GetStreamAsync(fileUrl))
                    {
                        return Convert.ToBase64String(await s.ToByteArrayAsync(cancellationToken));
                    }
                }
            }
            catch (Exception)
            {
                // When HttpClient cannot get the public file in the internet => return null to check in using function
                return null;
            }
        }
    }
}
