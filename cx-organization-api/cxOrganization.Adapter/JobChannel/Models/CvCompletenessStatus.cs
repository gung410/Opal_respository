
namespace cxOrganization.Adapter.JobChannel.Models
{
    internal class CvCompletenessStatus
    {
        public string ObjectId { get; set; }
        public int Status { get; set; }
        public CompletenessStatus CV { get; set; }
    }
}
