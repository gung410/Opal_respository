using System.Threading.Tasks;
using Amazon.S3;
using Microservice.Uploader.Application;
using Microservice.Uploader.Application.Queries;
using Microservice.Uploader.Dtos;
using Microservice.Uploader.Options;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Services
{
    public class AmazonS3PersonalSpaceUploaderService : AmazonS3BaseUploaderService, IAmazonS3PersonalSpaceUploaderService
    {
        private readonly AmazonS3Options _options;
        private readonly IAmazonS3 _client;
        private readonly ScormProcessingOptions _scormOptions;
        private readonly IAmazonS3KeyBuilderService _keyBuilderService;
        private readonly IThunderCqrs _thunderCqrs;

        public AmazonS3PersonalSpaceUploaderService(
            IOptions<AmazonS3Options> options,
            IOptions<AcceptanceOptions> acceptanceOptions,
            IOptions<AllowedFilesOptions> allowedOptions,
            IOptions<ScormProcessingOptions> scormOptions,
            IAmazonS3 amazonS3,
            IAmazonS3KeyBuilderService keyBuilderService,
            IThunderCqrs thunderCqrs) : base(options, acceptanceOptions, allowedOptions, scormOptions, amazonS3, keyBuilderService)
        {
            _options = options.Value;
            _scormOptions = scormOptions.Value;
            _client = amazonS3;
            _keyBuilderService = keyBuilderService;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task AdditionalFileValidation(CreateMultipartUploadSessionRequest request)
        {
            var personalSpace = await _thunderCqrs.SendQuery(new GetPersonalSpaceByUserIdQuery
            {
                UserId = request.UserId
            });
            if (personalSpace == null)
            {
                throw new PersonalSpaceAccessDeniedException();
            }

            if (!personalSpace.IsStorageUnlimited && personalSpace.TotalUsed + request.FileSize > personalSpace.TotalSpace)
            {
                throw new BusinessLogicException("File size exceeds total space");
            }
        }
    }
}
