using System;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Domain.Entities
{
    public class LectureContent : FullAuditedEntity, ISoftDelete
    {
        public Guid LectureId { get; set; }

        public Guid? ResourceId { get; set; }

        public string MimeType { get; set; }

        public string Value { get; set; }

        public LectureContentType Type { get; set; }

        public string Title { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string ExternalID { get; set; }

        public virtual LectureQuizConfig QuizConfig { get; set; }

        public virtual LectureDigitalContentConfig DigitalContentConfig { get; set; }

        public LectureContent Clone(Guid createdBy, Guid newLectureId)
        {
            return new LectureContent
            {
                Id = Guid.NewGuid(),
                LectureId = newLectureId,
                CreatedDate = Clock.Now,
                Type = Type,
                CreatedBy = createdBy,
                Title = Title,
                Value = Value,
                MimeType = MimeType,
                ResourceId = ResourceId,
                QuizConfig = QuizConfig?.Clone(),
                DigitalContentConfig = DigitalContentConfig?.Clone()
            };
        }

        public void UpdateQuizConfig(LectureQuizConfig data)
        {
            if (QuizConfig == null)
            {
                QuizConfig = new LectureQuizConfig
                {
                    ByPassPassingRate = data.ByPassPassingRate,
                    DisplayPollResultToLearners = data.DisplayPollResultToLearners
                };
            }
            else
            {
                QuizConfig.ByPassPassingRate = data.ByPassPassingRate;
                QuizConfig.DisplayPollResultToLearners = data.DisplayPollResultToLearners;
            }
        }

        public void UpdateDigitalContentConfig(LectureDigitalContentConfig data)
        {
            if (DigitalContentConfig == null)
            {
                DigitalContentConfig = new LectureDigitalContentConfig
                {
                    CanDownload = data.CanDownload
                };
            }
            else
            {
                DigitalContentConfig.CanDownload = data.CanDownload;
            }
        }
    }

    public class LectureQuizConfig
    {
        public bool ByPassPassingRate { get; set; } = true;

        public bool DisplayPollResultToLearners { get; set; } = true;

        public LectureQuizConfig Clone()
        {
            return new LectureQuizConfig()
            {
                ByPassPassingRate = ByPassPassingRate,
                DisplayPollResultToLearners = DisplayPollResultToLearners
            };
        }
    }

    public class LectureDigitalContentConfig
    {
        public bool CanDownload { get; set; } = false;

        public LectureDigitalContentConfig Clone()
        {
            return new LectureDigitalContentConfig()
            {
                CanDownload = CanDownload
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
