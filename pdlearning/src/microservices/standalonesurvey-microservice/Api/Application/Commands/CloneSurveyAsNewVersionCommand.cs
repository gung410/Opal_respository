using System;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class CloneSurveyAsNewVersionCommand : BaseStandaloneSurveyCommand
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }

        public Guid NewId { get; set; }

        /// <summary>
        /// The Id of parent record.
        /// </summary>
        public Guid ParentId { get; set; }

        public SurveyStatus Status { get; set; }

        public string NewTitle { get; set; }

        /// <summary>
        /// In case clone to a new form, not to new version, set this property to False. Default is True.
        /// </summary>
        public bool IsCloneToNewVersion { get; set; } = true;
    }
}
