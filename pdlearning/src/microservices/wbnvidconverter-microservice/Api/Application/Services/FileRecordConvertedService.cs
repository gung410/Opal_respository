using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace Microservice.WebinarVideoConverter.Application.Services
{
    public class FileRecordConvertedService : IRecordFileService
    {
        private readonly RecordingConvertOptions _recordingConvertOptions;

        public FileRecordConvertedService(
            IOptions<RecordingConvertOptions> recordingConvertOptions)
        {
            _recordingConvertOptions = recordingConvertOptions.Value;
        }

        /// <inheritdoc/>
        public List<string> GetConvertedFileNames()
        {
            var directory = new DirectoryInfo(_recordingConvertOptions.ConvertedVideoDir);
            var files = directory.GetFiles("*.mp4");

            // Get rid of extension. from filename-abc.mp4 to filename-abc
            Func<FileInfo, string> getRidOfExtension = file => file.Name.Substring(0, file.Name.IndexOf('.'));

            return files.Select(getRidOfExtension).ToList();
        }
    }
}
