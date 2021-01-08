using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microservice.Uploader.Application.Commands.Enums;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Application.RequestDtos;
using Microservice.Uploader.Infrastructure;
using Svg;

namespace Microservice.Uploader.Services
{
    public class DownloadLearningContentService : IDownloadLearningContentService
    {
        private readonly IStorageService _storageService;

        public DownloadLearningContentService(
            IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<DownloadLearningContentResultModel> DownloadLearningContent(DownloadLearningContentRequest request, CancellationToken cancellationToken)
        {
            // Process HTML content
            var htmlContentConverted = await UpdateHtmlContent(HttpUtility.HtmlDecode(request.HtmlContent), cancellationToken);

            return new DownloadLearningContentResultModel()
            {
                HtmlContentConverted = Encoding.UTF8.GetBytes(htmlContentConverted),
                FileFormat = FileFormat.Word
            };
        }

        /// <summary>
        /// Process HTML content
        /// 1. Replace video => hyperlink
        /// 2. Img tag => Get file from S3 => convert to base64 => update tag in HTML Content.
        /// </summary>
        /// <param name="htmlContent">The HTML content store in LMM.</param>
        /// <returns>HTML Converted.</returns>
        private async Task<string> UpdateHtmlContent(string htmlContent, CancellationToken cancellationToken)
        {
            // Replace video tag HTML content => href
            var htmlContentReplacedVideo = ReplaceVideoContent(htmlContent);

            // Replace img tag HTML content => to base64 => update it into HTML
            var replacedImageContent = await RelaceImageContent(htmlContentReplacedVideo, cancellationToken);

            // Add head and footer for HTML content.
            var header = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>Export HTML Learning Content</title></head><body>";
            var footer = "</body></html>";
            return $"{header}{replacedImageContent}{footer}";
        }

        private string ReplaceVideoContent(string htmlContent)
        {
            // Regex to get video tag
            Regex regexVideo = new Regex(@"<video.*?>((.|\n)*)<\/video>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // Regex to get src in <source src="url" /> .
            Regex regexSrc = new Regex(@"src=(?:(['""])(?<src>(?:(?!\1).)*)\1|(?<src>[^\s>]+))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            MatchCollection mcVideos = regexVideo.Matches(htmlContent);
            foreach (Match mVideo in mcVideos)
            {
                // Regex to get the source tag like: <source type="video/mp4" ... >
                Regex regexVideoSource = new Regex(@"<source type=""video/.*"" src=(?:(['""])(?<src>(?:(?!\1).)*)\1|(?<src>[^\s>]+))>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match mcVideo = regexVideoSource.Match(mVideo.Value);
                if (regexSrc.IsMatch(mcVideo.Groups[0].Value))
                {
                    Match mSrc = regexSrc.Match(mVideo.Groups[0].Value);
                    if (mSrc != null)
                    {
                        // Get the value inside src attribute
                        string b = mSrc.Groups["src"].Value;

                        // prepare video template to replace <video />
                        var newVideoHtml = $"<a href=\"{b}\">Click here to watch the video</a><br/>";
                        htmlContent = htmlContent.Replace(mVideo.Value, newVideoHtml);
                    }
                }
            }

            return htmlContent;
        }

        private async Task<string> RelaceImageContent(string htmlContent, CancellationToken cancellationToken)
        {
            var listSourceUrls = GetSourceUrls(htmlContent);

            if (listSourceUrls.Count == 0)
            {
                return htmlContent;
            }

            // Get list of files from S3 via source url
            var listUrlWithBase64 = await GetBase64FileFromS3ViaSourceUrl(listSourceUrls, cancellationToken);

            // Replace image src using base64
            return ReplaceImageSrcInHtmlContent(htmlContent, listUrlWithBase64);
        }

        private async Task<List<FileFormatWIthUrlModel>> GetBase64FileFromS3ViaSourceUrl(List<string> listSourceUrls, CancellationToken cancellationToken)
        {
            var result = new List<FileFormatWIthUrlModel>();
            foreach (var item in listSourceUrls)
            {
                var fileBase64 = string.Empty;
                try
                {
                    fileBase64 = await _storageService.GetFileAsBase64String(item, cancellationToken);
                }
                catch (Exception)
                {
                    fileBase64 = await _storageService.GetFilePublicAsBase64String(item, cancellationToken);
                }

                // Get extension of the file
                var extensionFile = item.Substring(item.LastIndexOf(".") + 1, item.Length - item.LastIndexOf(".") - 1);

                if (extensionFile == "svg")
                {
                    var byteArray = Convert.FromBase64String(fileBase64);
                    using (var stream = new MemoryStream(byteArray))
                    {
                        // Create image file converted from SVG file (base64)
                        var svgDocument = SvgDocument.Open<SvgDocument>(stream);
                        var bitmap = svgDocument.Draw();
                        extensionFile = "png";

                        var tempGuid = Guid.NewGuid().ToString();
                        var tempDirectory = Path.Combine(Path.GetTempPath(), tempGuid);
                        var tempFile = Path.Combine(tempDirectory, $"{tempGuid}.{extensionFile}");

                        DeleteTempFile(tempFile);
                        DeleteDirectory(tempDirectory);

                        Directory.CreateDirectory(tempDirectory);

                        bitmap.Save(tempFile);
                        byte[] imageBytes = File.ReadAllBytes(tempFile);
                        string base64String = Convert.ToBase64String(imageBytes);
                        fileBase64 = base64String;

                        // delete temporary file & folder.
                        DeleteTempFile(tempFile);
                        DeleteDirectory(tempDirectory);
                    }
                }

                var newUrlItem = new FileFormatWIthUrlModel()
                {
                    Url = item,
                    Base64Value = fileBase64,
                    Extension = extensionFile,
                };
                result.Add(newUrlItem);
            }

            return result;
        }

        private void DeleteTempFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
        }

        private string ReplaceImageSrcInHtmlContent(string htmlContent, List<FileFormatWIthUrlModel> urlsWithBase64)
        {
            try
            {
                var htmlRegexPattern = string.Join('|', urlsWithBase64.Select(p => p.Url).ToList());

                // Convert all src inside img tag to format <img src="data:image/[file extension];base64, base64String />
                return Regex.Replace(
                    htmlContent,
                    htmlRegexPattern,
                    p => $"data:image/{urlsWithBase64.FirstOrDefault(x => x.Url == p.Value).Extension};base64,{urlsWithBase64.FirstOrDefault(x => x.Url == p.Value).Base64Value}");
            }
            catch (Exception ex)
            {
                var a = ex;
                throw;
            }
        }

        private List<string> GetSourceUrls(string htmlContent)
        {
            // regex to get all <img /> tag inside the html content
            var regexPattern = new Regex(@"<img.+?src=[""''](.+?)[""''].*?>");
            var matches = regexPattern.Matches(htmlContent);
            var urls = new List<string>();
            foreach (Match match in matches)
            {
                if (match != null && match.Groups[1] != null)
                {
                    urls.Add(match.Groups[1].Value);
                }
            }

            return urls;
        }
    }
}
