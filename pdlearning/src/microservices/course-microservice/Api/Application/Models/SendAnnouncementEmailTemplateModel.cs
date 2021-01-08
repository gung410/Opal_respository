namespace Microservice.Course.Application.Models
{
    public class SendAnnouncementEmailTemplateModel : EmailTemplateModel
    {
        public string UserNameTagValue
        {
            get => Tags.ContainsKey(nameof(UserNameTagValue)) ? Tags[nameof(UserNameTagValue)] : string.Empty;

            set => Tags[nameof(UserNameTagValue)] = value;
        }

        public string CourseTitleTagValue
        {
            get => Tags.ContainsKey(nameof(CourseTitleTagValue)) ? Tags[nameof(CourseTitleTagValue)] : string.Empty;

            set => Tags[nameof(CourseTitleTagValue)] = value;
        }

        public string CourseCodeTagValue
        {
            get => Tags.ContainsKey(nameof(CourseCodeTagValue)) ? Tags[nameof(CourseCodeTagValue)] : string.Empty;

            set => Tags[nameof(CourseCodeTagValue)] = value;
        }

        public string CourseAdminNameTagValue
        {
            get => Tags.ContainsKey(nameof(CourseAdminNameTagValue)) ? Tags[nameof(CourseAdminNameTagValue)] : string.Empty;

            set => Tags[nameof(CourseAdminNameTagValue)] = value;
        }

        public string CourseAdminEmailTagValue
        {
            get => Tags.ContainsKey(nameof(CourseAdminEmailTagValue)) ? Tags[nameof(CourseAdminEmailTagValue)] : string.Empty;

            set => Tags[nameof(CourseAdminEmailTagValue)] = value;
        }

        public string ListSessionTagValue
        {
            get => Tags.ContainsKey(nameof(ListSessionTagValue)) ? Tags[nameof(ListSessionTagValue)] : string.Empty;

            set => Tags[nameof(ListSessionTagValue)] = value;
        }

        public string DetailUrlTagValue
        {
            get => Tags.ContainsKey(nameof(DetailUrlTagValue)) ? Tags[nameof(DetailUrlTagValue)] : string.Empty;

            set => Tags[nameof(DetailUrlTagValue)] = value;
        }
    }
}
