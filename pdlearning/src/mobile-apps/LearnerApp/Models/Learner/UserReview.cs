using System;
using LearnerApp.Common;
using Newtonsoft.Json;

namespace LearnerApp.Models.Learner
{
    public class UserReview
    {
        private DateTime? _changedDate;

        public string Id { get; set; }

        public string UserId { get; set; }

        public string ItemId { get; set; }

        public PdActivityType ItemType { get; set; }

        public string ParentCommentId { get; set; }

        public string Version { get; set; }

        public string ItemName { get; set; }

        public string UserFullName { get; set; }

        public string CommentTitle { get; set; }

        public string CommentContent { get; set; }

        public double Rate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ChangedDate
        {
            get
            {
                return _changedDate;
            }

            set
            {
                _changedDate = value;

                if (value.HasValue)
                {
                    CreatedDate = value.Value;
                }
            }
        }

        public string ChangedBy { get; set; }

        [JsonIgnore]
        public bool IsOwnerReview { get; set; }

        [JsonIgnore]
        public bool IsVisibleRating { get; set; }
    }
}
