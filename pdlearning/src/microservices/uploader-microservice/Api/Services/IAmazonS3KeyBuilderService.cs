using System;

namespace Microservice.Uploader.Services
{
    public interface IAmazonS3KeyBuilderService
    {
        string BuildTemporaryStorageKey(Guid fileId, string extension, string folder = null);

        string BuildPermanentStorageKey(Guid fileId, string extension, string folder = null);

#pragma warning disable CA1055 // URI-like return values should not be strings
        string BuildEndpointUrl(Guid fileId, string extension, string folder = null);
#pragma warning restore CA1055 // URI-like return values should not be strings
    }
}
