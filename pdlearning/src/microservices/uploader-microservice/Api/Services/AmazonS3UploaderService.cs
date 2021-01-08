using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microservice.Uploader.Application.Commands.Enums;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Application.RequestDtos;
using Microservice.Uploader.Infrastructure;
using Microservice.Uploader.Options;
using Microsoft.Extensions.Options;
using Svg;

namespace Microservice.Uploader.Services
{
    public class AmazonS3UploaderService : AmazonS3BaseUploaderService, IAmazonS3UploaderService
    {
        private readonly AmazonS3Options _options;
        private readonly IAmazonS3 _client;
        private readonly ScormProcessingOptions _scormOptions;
        private readonly IAmazonS3KeyBuilderService _keyBuilderService;
        private readonly IStorageService _storageService;

        public AmazonS3UploaderService(
            IOptions<AmazonS3Options> options,
            IOptions<AcceptanceOptions> acceptanceOptions,
            IOptions<AllowedFilesOptions> allowedOptions,
            IOptions<ScormProcessingOptions> scormOptions,
            IAmazonS3 amazonS3,
            IStorageService storageService,
            IAmazonS3KeyBuilderService keyBuilderService) : base(options, acceptanceOptions, allowedOptions, scormOptions, amazonS3, keyBuilderService)
        {
            _options = options.Value;
            _scormOptions = scormOptions.Value;
            _client = amazonS3;
            _keyBuilderService = keyBuilderService;
            _storageService = storageService;
        }

        public async Task ExtractScormPackage(ExtractScormPackageRequest request, CancellationToken cancellationToken = default)
        {
            var permanentStorageKey = _keyBuilderService.BuildPermanentStorageKey(request.FileId, request.FileExtension, request.Folder);
            var tempFile = Path.Combine(Path.GetTempPath(), $"{permanentStorageKey}_");
            var tempDirectory = Path.Combine(Path.GetTempPath(), permanentStorageKey);

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }

            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory);
            }

            Directory.CreateDirectory(tempDirectory);
            string fileStatusName = GenerateFileName(request);

            try
            {
                WriteStatusToFile(tempDirectory, fileStatusName, ScormProcessingStatus.Extracting.ToString());
                var cts = new CancellationTokenSource();
                cts.CancelAfter(_scormOptions.MaximumProcessTimeInMinute * 60 * 1000);

                cts.Token.Register(() =>
                {
                    WriteStatusToFile(tempDirectory, fileStatusName, ScormProcessingStatus.Timeout.ToString());
                });

                await _client.DownloadToFilePathAsync(_options.BucketName, permanentStorageKey, tempFile, null, cts.Token);
                try
                {
                    ZipFile.ExtractToDirectory(tempFile, tempDirectory);
                }
                catch (Exception e)
                {
                    WriteStatusToFile(tempDirectory, fileStatusName, ScormProcessingStatus.ExtractingFailure.ToString());
                    Debug.WriteLine(e);

                    return;
                }

                if (!File.Exists(Path.Combine(tempDirectory, "imsmanifest.xml")))
                {
                    WriteStatusToFile(tempDirectory, fileStatusName, ScormProcessingStatus.Invalid.ToString());
                    return;
                }

                var directoryTransferUtility = new TransferUtility(_client);

                WriteStatusToFile(tempDirectory, fileStatusName, ScormProcessingStatus.Processing.ToString());

                await directoryTransferUtility.UploadDirectoryAsync(
                    new TransferUtilityUploadDirectoryRequest()
                    {
                        BucketName = _options.BucketName,
                        KeyPrefix = permanentStorageKey,
                        Directory = tempDirectory,
                        SearchOption = SearchOption.AllDirectories,
                        SearchPattern = "*.*"
                    },
                    cts.Token);
                try
                {
                    File.Delete(tempFile);
                    Directory.Delete(tempDirectory, true);
                }
                catch (Exception e)
                {
                    // Ignore errors
                    Debug.WriteLine(e);
                }

                WriteStatusToFile(tempDirectory, fileStatusName, ScormProcessingStatus.Completed.ToString());
            }
            catch (Exception e)
            {
                WriteStatusToFile(tempDirectory, fileStatusName, ScormProcessingStatus.Failure.ToString());
                Debug.WriteLine(e);
            }
        }

        public async Task<ScormProcessingStatusResult> GetScormProcessingStatus(ExtractScormPackageRequest request, CancellationToken cancellationToken)
        {
            var status = ScormProcessingStatus.Processing;
            var permanentStorageKey = _keyBuilderService.BuildPermanentStorageKey(request.FileId, request.FileExtension, request.Folder);
            var tempDirectory = Path.Combine(Path.GetTempPath(), permanentStorageKey);
            string fileStatusName = GenerateFileName(request);

            string flagFilePath = Path.Combine(tempDirectory, fileStatusName);
            if (File.Exists(flagFilePath))
            {
                string statusWritten = await File.ReadAllTextAsync(flagFilePath, cancellationToken);
                _ = Enum.TryParse(statusWritten, out status);
            }

            switch (status)
            {
                case ScormProcessingStatus.Completed:
                case ScormProcessingStatus.Failure:
                case ScormProcessingStatus.Timeout:
                    try
                    {
                        File.Delete(flagFilePath);
                    }
                    catch (Exception)
                    {
                        // Ignore exception;
                    }

                    break;
            }

            return new ScormProcessingStatusResult() { Status = status };
        }
    }
}
