namespace LearnerApp.Models.PDPM
{
    public class PDORemoveIdentity
    {
        public string ExtId { get; set; }

        public int OwnerId { get; set; } = 3001;

        public int CustomerId { get; set; } = 2052;

        public string Archetype { get; set; } = "Assessment";
    }
}
