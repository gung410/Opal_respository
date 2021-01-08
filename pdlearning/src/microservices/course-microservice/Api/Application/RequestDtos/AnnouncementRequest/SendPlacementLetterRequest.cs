using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class SendPlacementLetterRequest
    {
        public List<Guid> Ids { get; set; }

        public string Base64Message { get; set; }

        public string UserNameTag { get; set; }

        public string CourseTitleTag { get; set; }

        public string CourseCodeTag { get; set; }

        public string CourseAdminNameTag { get; set; }

        public string CourseAdminEmailTag { get; set; }

        public string ListSessionTag { get; set; }

        public string DetailUrlTag { get; set; }
    }
}
