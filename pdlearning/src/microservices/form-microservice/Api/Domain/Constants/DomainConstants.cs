using System.Collections.Generic;

namespace Microservice.Form.Domain.Constants
{
    public static class DomainConstants
    {
        public static readonly int DefaultStringMaxLength = 8000;
        public static readonly List<string> PostCourseSurveyTemplateTitles = new List<string>
        {
            "Type 1: FEEDBACK FORM FOR COURSE/WORKSHOP/MASTER CLASS/SEMINAR/CONFERENCE",
            "Type 2: FEEDBACK FORM FOR E-LEARNING COURSE",
            "Type 3: FEEDBACK FORM FOR BLENDED COURSE/WORKSHOP/MASTER CLASS",
            "Type 4: FEEDBACK FORM FOR LEARNING EVENT"
        };
    }
}
