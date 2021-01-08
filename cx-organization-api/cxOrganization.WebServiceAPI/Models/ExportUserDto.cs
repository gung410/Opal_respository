using System.ComponentModel.DataAnnotations;
using cxOrganization.Domain.Services.ExportService;

namespace cxOrganization.WebServiceAPI.Models
{
    public class ExportUserDto : SearchInput
    {
        public ExportUserDto()
        {
            ExportOption = new ExportOption
            {
                CsvDelimiter = ",",
                ExportType = ExportType.Csv,
                SummaryPosition = SummaryPosition.Bottom
            };
        }
        [Required]
        public ExportOption ExportOption { get; set; }

        public EmailOption EmailOption { get; set; }

        public bool SendEmail { get; set; }
    }
}