namespace LearnerApp.Models
{
    public class Community
    {
        public string Id { get; set; }

        public string Guid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public int Visibility { get; set; }

        public int JoinPolicy { get; set; }

        public int Status { get; set; }

        public object Tags { get; set; }

        public int Members { get; set; }

        public CommunityOwner Owner { get; set; }
    }
}
