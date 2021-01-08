using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxOrganization.Domain.Services.ExportService;
using cxOrganization.Domain.Services.Reports;

namespace cxOrganization.WebServiceAPI.Models
{
    public class ExportUserEventLogInfoDto 
    {
        public ExportUserEventLogInfoDto()
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

        public DateTime? EventCreatedBefore { get; set; }
        public DateTime? EventCreatedAfter { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        [Required]
        public List<UserEventType> EventTypes { get; set; }
    }
}