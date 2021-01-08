using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace LearnerApp.Models
{
    public enum TableOfContentType
    {
        Course,
        Section,
        Lecture,
        Assignment
    }

    public class TableOfContent
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public TableOfContentType Type { get; set; }

        public string Icon { get; set; }

        public List<TableOfContent> Items { get; set; }

        public int Order { get; set; }

        public AdditionalInfo AdditionalInfo { get; set; }

        [JsonIgnore]
        public Color LectureCompletedColor { get; set; }

        [JsonIgnore]
        public Color LectureCompletedBorderColor { get; set; }

        [JsonIgnore]
        public Color LectureCompletedTextColor { get; set; }

        [JsonIgnore]
        public bool IsClickable { get; set; }
    }
}
