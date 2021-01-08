using System.Collections.Generic;

namespace LearnerApp.Models.MyLearning
{
    public class LearningPathSharing
    {
        public string Id { get; set; }

        public string ItemId { get; set; }

        public SharingType ItemType { get; set; }

        public string CreatedBy { get; set; }

        public List<LearningPathShareUser> Users { get; set; }
    }
}
