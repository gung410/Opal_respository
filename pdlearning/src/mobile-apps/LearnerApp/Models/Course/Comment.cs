using System;
using Newtonsoft.Json;

namespace LearnerApp.Models.Course
{
    public class Comment
    {
        public string Id { get; set; }

        public string ObjectId { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public string Action { get; set; }

        [JsonIgnore]
        public string AvatarUrl { get; set; }

        [JsonIgnore]
        public string FullName { get; set; }

        [JsonIgnore]
        public string DepartmentName { get; set; }
    }
}
