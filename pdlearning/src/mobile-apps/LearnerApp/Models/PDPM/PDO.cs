namespace LearnerApp.Models.PDPM
{
    public class PDO
    {
        public ObjectiveInfo ObjectiveInfo { get; set; }

        public Answer Answer { get; set; }

        public bool ForceCreateResult { get; set; } = true;

        public AdditionalProperties AdditionalProperties { get; set; }

        public string Timestamp { get; set; }
    }
}
