using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microservice.WebinarVideoConverter.Application.Models;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Microservice.WebinarVideoConverter.Application.Services
{
    public class PlaybackScannerService : IPlaybackScannerService
    {
        /// <summary>
        /// Playback directory pattern
        /// Example:
        ///     - E:\presentation\618dcdfb0cd9ae4481164961c4796dd8e3930c8d-1606268814166
        ///     - /data/presentation/618dcdfb0cd9ae4481164961c4796dd8e3930c8d-1606268814166.
        /// </summary>
        private const string PlaybackDirectoryPattern = @"[\\\/][\w\d]{40}-[\w\d]{13}";
        private const string MetadataFileName = "metadata.xml";
        private readonly ILogger<PlaybackScannerService> _logger;

        public PlaybackScannerService(ILogger<PlaybackScannerService> logger)
        {
            _logger = logger;
        }

        public List<RecordMetadata> Collect(string playbacksDir)
        {
            var records = new List<RecordMetadata>();
            List<string> playbackDirs;
            try
            {
                playbackDirs = Directory
                    .EnumerateDirectories(playbacksDir)
                    .Where(dirName => Regex.IsMatch(dirName, PlaybackDirectoryPattern))
                    .ToList();

                _logger.LogInformation("Found {PlaybackCount} playback folder(s) in {Folder}", playbackDirs.Count, playbacksDir);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get playback directories. Directory: {PlaybacksDir}. Message: {Message}.\nDetails: {Details}",
                    playbacksDir,
                    ex.Message,
                    ex.StackTrace);
                throw;
            }

            foreach (var dir in playbackDirs)
            {
                var metadataFilePath = Path.Combine(dir, MetadataFileName);

                if (!File.Exists(metadataFilePath))
                {
                    continue;
                }

                try
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(metadataFilePath);

                    records.Add(new RecordMetadata
                    {
                        InternalMeetingId = xmlDoc.SelectSingleNode("/recording/id").InnerText,
                        MeetingId = new Guid(xmlDoc.SelectSingleNode("/recording/meta/meetingId").InnerText)
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load metadata file. Path: {Path}, message: {Message}, see details: {Details}", metadataFilePath, ex.Message, ex.StackTrace);
                }
            }

            return records;
        }
    }
}
