using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxOrganization.Domain.Services.ExportService;

namespace cxOrganization.WebServiceAPI.Models
{
    public class ExportUserAccountDetailsDto
    {
        public ExportUserAccountDetailsDto()
        {
            ExportOption = new ExportOption
            {
                CsvDelimiter = ",",
                ExportType = ExportType.Csv,
                SummaryPosition = SummaryPosition.Bottom
            };
            SeparatedByAccountType = true;
        }
        [Required]
        public ExportOption ExportOption { get; set; }

        public EmailOption EmailOption { get; set; }

        public bool SendEmail { get; set; }
        public List<cxPlatform.Client.ConexusBase.EntityStatusEnum> UserEntityStatuses { get; set; }
        public List<int> ParentDepartmentIds { get; set; }
        public bool FilterOnSubDepartment { get; set; }
        public DateTime? UserCreatedAfter { get; set; }
        public DateTime? UserCreatedBefore { get; set; }
        public DateTime? LastLoginAfter { get; set; }
        public DateTime? LastLoginBefore { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool SeparatedByAccountType { get; set; }

    }
    public class ExportPrivilegedUserAccountDto
    {
        public ExportPrivilegedUserAccountDto()
        {
            ExportOption = new ExportOption
            {
                CsvDelimiter = ",",
                ExportType = ExportType.Csv,
                SummaryPosition = SummaryPosition.Bottom
            };
            SeparatedByAccountType = true;
        }
        [Required]
        public ExportOption ExportOption { get; set; }

        public EmailOption EmailOption { get; set; }

        public bool SendEmail { get; set; }
        public List<cxPlatform.Client.ConexusBase.EntityStatusEnum> UserEntityStatuses { get; set; }
        public List<int> ParentDepartmentIds { get; set; }
        public bool FilterOnSubDepartment { get; set; }
        public DateTime? UserCreatedAfter { get; set; }
        public DateTime? UserCreatedBefore { get; set; }
        public DateTime? LastLoginAfter { get; set; }
        public DateTime? LastLoginBefore { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool SeparatedByAccountType { get; set; }

    }
}