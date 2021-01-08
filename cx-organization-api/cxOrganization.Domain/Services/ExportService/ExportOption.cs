using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Domain.Services.ExportService
{
    public class ExportOption
    {
        public ExportOption()
        {
            ExportFields = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);
            VerticalExportFields = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);
        }
        public string CsvDelimiter { get; set; }

        [Required]
        public Dictionary<string, dynamic> ExportFields { get; set; }
        public Dictionary<string, dynamic> VerticalExportFields { get; set; }
        public ExportSummaryOption SummaryOption { get; set; }
        public SummaryPosition SummaryPosition { get; set; }
        public bool? ShowRecordType { get; set; }
        public ExportType ExportType { get; set; }
        public string DateTimeFormat { get; set; }
        public int? TimeZoneOffset { get; set; }
        public string ExportTitle { get; set; }
        public List<InfoRecord> InfoRecords { get; set; }
        public bool AddDateTimeAsInfoRecords { get; set; }
        public bool? ShowRowNumber { get; set; }
        public string RowNumberColumnCaption { get; set; }
    }

    public enum ExportType
    {
        Csv,
        Excel
    }

    public enum SummaryPosition
    {
        Top,
        Bottom,
        Separated,
        None
    }

    public class ExportFieldInfo
    {
        public string Caption { get; set; }
        public string DisplayFormat { get; set; }
        public bool IsGroupField { get; set; }
    }

    public class EmailOption
    {
        public string Subject { get; set; }
    }

    public class InfoRecord
    {
        public string Caption { get; set; }
        public object Value { get; set; }
        public string Format { get; set; }
    }
}