using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace cxOrganization.Domain.Common
{
    public static class FileExtension
    {
        public const string Excel = ".xls";
        public const string ExcelOpenXML = ".xlsx";
        public const string Csv = ".csv";
        public const string Text = ".txt";

        private static Dictionary<string, FileType> fileTypeMappings = new Dictionary<string, FileType>(StringComparer.CurrentCultureIgnoreCase)
        {
            {Text, FileType.Text},
            {Csv, FileType.Csv},
            {Excel, FileType.Excel},
            {ExcelOpenXML, FileType.ExcelOpenXML}
        };



        public static FileType ToFileType(string fileExtension, FileType defaultType = FileType.Unknown)
        {
            if (fileTypeMappings.TryGetValue(fileExtension, out var fileType))
                return fileType;
            return defaultType;
        }

        public static FileType GetValidFileType(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileType = FileExtension.ToFileType(fileExtension);
            if (fileType == FileType.Unknown)
            {
                throw new InvalidException($"File extension '{fileExtension}' is not supported to import");
            }

            return fileType;

        }
    }
}
