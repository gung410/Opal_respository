using System.Collections.Generic;
using LearnerApp.Models.Learner;

namespace LearnerApp.Models.Sharing
{
    public class SharingContentItem
    {
        public string ItemId { get; set; }

        /// <summary>
        /// Only available for DigitalContent and Course.
        /// </summary>
        public BookmarkType ItemType { get; set; }

        public string Title { get; set; }

        public List<string> SharedByUsers { get; set; }

        /// <summary>
        /// This will be mapped in service manager layer
        /// </summary>
        public List<string> Tags { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
