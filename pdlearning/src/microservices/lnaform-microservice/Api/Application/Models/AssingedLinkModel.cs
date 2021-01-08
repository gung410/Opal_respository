using System;

namespace Microservice.LnaForm.Application.Models
{
    public class AssignedLinkModel
    {
        public AssignedLinkModel(string surveyLink, DateTime assignedDate)
        {
            SurveyLink = surveyLink;
            AssignedDate = assignedDate;
        }

        public string SurveyLink { get; }

        public DateTime AssignedDate { get; }
    }
}
