using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveSessionCommand : BaseThunderCommand
    {
        public SaveSessionCommandEntityData SessionData { get; set; }

        public bool UpdatePreRecordClipOnly { get; set; }

        public bool IsCreate { get; set; }
    }

    public class SaveSessionCommandEntityData
    {
        public Guid Id { get; set; }

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
    }
}
