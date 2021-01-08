using System;
using System.ComponentModel.DataAnnotations;
using cxOrganization.Domain.Services.ExportService;

namespace cxOrganization.WebServiceAPI.Models
{
    public class ExportUserStatisticsDto
    {
        public ExportUserStatisticsDto()
        {
            ExportOption = new ExportOption
            {
                CsvDelimiter = ",",
                ExportType = ExportType.Excel,
                SummaryPosition = SummaryPosition.None
            };
        }
        [Required]
        public ExportOption ExportOption { get; set; }

        public EmailOption EmailOption { get; set; }

        public bool SendEmail { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    
    }
}