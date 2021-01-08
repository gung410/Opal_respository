using System.Collections.Generic;
using Microservice.Uploader.Application.Commands.Enums;

namespace Microservice.Uploader.Application.Constants
{
    public static class FileFormatMappingMineTypeConstant
    {
        public static readonly Dictionary<FileFormat, string> FileFormatMappingMineType
            = new Dictionary<FileFormat, string>
            {
                {
                    FileFormat.Word, "application/msword"
                },
                {
                    FileFormat.Pdf, "application/pdf"
                },
                {
                    FileFormat.Excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                },
                {
                    FileFormat.Png, "image/png"
                },
                {
                    FileFormat.Jpeg, "image/jpeg"
                },
                {
                    FileFormat.Text, "text/plain"
                },
            };
    }
}
