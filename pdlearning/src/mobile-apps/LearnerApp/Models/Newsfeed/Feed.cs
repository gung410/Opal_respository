namespace LearnerApp.Models.Newsfeed
{
    public class Feed : FeedBase
    {
        private string _updateInfo;

        public string Id { get; set; }

        public int PostId { get; set; }

        public string CourseId { get; set; }

        public string CourseName { get; set; }

        public string ThumbnailUrl { get; set; } = "image_place_holder_h150.png";

        public string UpdateInfo
        {
            get
            {
                return _updateInfo;
            }

            set
            {
                switch (value)
                {
                    case "CourseInfoUpdated":
                        _updateInfo = "The information of the course has been updated";
                        break;
                    case "CourseContentUpdated":
                        _updateInfo = "The content of the course has been updated";
                        break;
                    case "CourseSuggestedToUser":
                        _updateInfo = "The information of the course has been updated";
                        break;
                    default:
                        _updateInfo = "Not defined";
                        break;
                }
            }
        }

        public string Url { get; set; }

        public string UserId { get; set; }
    }
}
