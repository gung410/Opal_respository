namespace LearnerApp.Models.PDPM
{
    public class Identity
    {
        public string ExtId { get; set; }

        public int OwnerId { get; set; }

        public int CustomerId { get; set; }

        public string Archetype { get; set; } = "Employee";

        public int Id { get; set; }
    }
}
