using System.Collections.Generic;

namespace LearnerApp.Models.MyLearning
{
    public class MyLearningCardSource
    {
        public string Icon { get; set; }

        public string Title { get; set; }

        public int LearningPathItems { get; set; }

        public List<string> DigitalContentTagging { get; set; }

        public int DigitalContentRating { get; set; }
    }
}
