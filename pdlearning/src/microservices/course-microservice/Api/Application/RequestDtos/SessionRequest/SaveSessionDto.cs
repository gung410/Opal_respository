using System;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveSessionDto
    {
        public Guid? Id { get; set; }

        public Guid ClassRunId { get; set; }

        public string SessionTitle { get; set; }

        public string Venue { get; set; }

        public bool LearningMethod { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool UsePreRecordClip { get; set; }

        /// <summary>
        /// This is video path of PreRecordId from ccpm. This is a relative S3 path.
        /// </summary>
        public string PreRecordPath { get; set; }

        /// <summary>
        /// This is video content id from ccpm.
        /// </summary>
        public Guid? PreRecordId { get; set; }

        public SaveSessionCommand ToCommand(bool updatePreRecordClipOnly)
        {
            var sessionData = new SaveSessionCommandEntityData()
            {
                Id = Id ?? Guid.NewGuid(),
                ClassRunId = ClassRunId,
                SessionTitle = SessionTitle,
                StartDateTime = StartDateTime,
                EndDateTime = EndDateTime,
                Venue = Venue,
                LearningMethod = LearningMethod,
                UsePreRecordClip = UsePreRecordClip,
                PreRecordId = PreRecordId,
                PreRecordPath = PreRecordPath
            };

            return new SaveSessionCommand()
            {
                UpdatePreRecordClipOnly = updatePreRecordClipOnly,
                IsCreate = !Id.HasValue,

                // Progress data for StartDateTime/EndDateTime
                SessionData = sessionData
            };
        }
    }
}
