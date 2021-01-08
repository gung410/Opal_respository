using System.Collections.Generic;

namespace cxOrganization.WebServiceAPI.Models
{
    public static class FileExtension
    {
        public const string Json = ".json";
        public const string Excel = ".xls";
        public const string ExcelOpenXML = ".xlsx";
        public const string Csv = ".csv";


        public static readonly Dictionary<string, string> FileTypeContentTypeMappings = new Dictionary<string, string>
        {
            {FileExtension.Csv, "text/csv"},
            {FileExtension.Excel, "application/vnd.ms-excel"},
            {FileExtension.ExcelOpenXML, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},

        };

    }
}
