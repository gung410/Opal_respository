using System;
using LearnerApp.Models.Learner;

namespace LearnerApp.Models
{
    public class BookmarkInfo
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public BookmarkType ItemType { get; set; }

        public string ItemId { get; set; }

        public string ItemName { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public string ChangedBy { get; set; }
    }
}
