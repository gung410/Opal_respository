using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microservice.Course.Application.Enums;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

namespace Microservice.Course.Infrastructure
{
    public static class ExportHelper
    {
        public static RenderingResult TelerikReportExportSingleFormat(
            string layoutPath,
            ReportGeneralOutputFormatType generalOutputFormat,
            params Telerik.Reporting.Parameter[] parameters)
        {
            var deviceInfo = new System.Collections.Hashtable();

            var reportProcessor = new ReportProcessor();
            var uriReportSource = new UriReportSource
            {
                Uri = Path.Combine(Directory.GetCurrentDirectory(), layoutPath)
            };
            uriReportSource.Parameters.AddRange(parameters);

            var result = reportProcessor.RenderReport(generalOutputFormat.ToString(), uriReportSource, deviceInfo);

            return result;
        }

        public static string TelerikReportExportSingleFormatBase64String(
            string layoutPath,
            ReportGeneralOutputFormatType generalOutputFormat,
            params Telerik.Reporting.Parameter[] parameters)
        {
            return Convert.ToBase64String(TelerikReportExportSingleFormat(layoutPath, generalOutputFormat, parameters).DocumentBytes);
        }

        public static string TelerikReportExportMultiFormatBase64String(
            string layoutPath,
            ReportGeneralOutputFormatType generalOutputFormat,
            string outputFormat,
            params Telerik.Reporting.Parameter[] parameters)
        {
            // ***Declarative (TRDP/TRDX) report definitions***
            // Uri is the path to the TRDP/TRDX file
            var reportSource = new UriReportSource
            {
                Uri = Path.Combine(Directory.GetCurrentDirectory(), layoutPath)
            };
            reportSource.Parameters.AddRange(parameters);

            // specify the output format of the produced image.
            var deviceInfo = new System.Collections.Hashtable();
            if (!string.IsNullOrWhiteSpace(outputFormat))
            {
                deviceInfo["OutputFormat"] = outputFormat;
            }

            var streams = new List<Stream>();
            var reportProcessor = new ReportProcessor();
            var result = reportProcessor.RenderReport(
             generalOutputFormat.ToString(),
             reportSource,
             deviceInfo,
             (name, extension, encoding, mimeType) =>
             {
                 var fs = new MemoryStream();
                 streams.Add(fs);
                 return fs;
             },
             out _);

            // Close stream and get base64 string
            var contentBase64 = new StringBuilder();
            foreach (var stream in streams)
            {
                var bytes = (stream as MemoryStream)?.ToArray() ?? default(ReadOnlySpan<byte>);
                contentBase64.Append(Convert.ToBase64String(bytes));
                stream.Close();
            }

            streams.Clear();

            return result ? contentBase64.ToString() : string.Empty;
        }
    }
}
