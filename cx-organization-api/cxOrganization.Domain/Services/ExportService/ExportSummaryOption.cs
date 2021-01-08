namespace cxOrganization.Domain.Services.ExportService
{
    public class ExportSummaryOption
    {
        public bool CountTotal { get; set; }
        public string CountTotalDisplayText { get; set; }
        public string CountByField { get; set; }
        public string CountByFieldValueCaption { get; set; }
        public bool ShowTotalBeforeCountByField { get; set; }

        public bool ShouldCountByField()
        {
            return !string.IsNullOrEmpty(CountByField);
        }
    }
}