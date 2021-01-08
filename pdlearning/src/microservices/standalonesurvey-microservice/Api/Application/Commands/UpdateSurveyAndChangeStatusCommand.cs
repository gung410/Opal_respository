using System;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class UpdateSurveyAndChangeStatusCommand : SaveSurveyCommand
    {
        /// <summary>
        /// The ID will be ID of new record if the form status changed to Unpublised.
        /// </summary>
        public Guid NewFormID { get; set; }

        /// <summary>
        /// Set this field to true if need update to new version.
        /// </summary>
        public bool? IsUpdateToNewVersion { get; set; }
    }
}
