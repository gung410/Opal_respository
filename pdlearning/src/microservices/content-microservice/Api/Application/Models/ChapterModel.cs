using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Content.Domain.ValueObject;
using ChapterEntity = Microservice.Content.Domain.Entities.Chapter;

namespace Microservice.Content.Application.Models
{
    public class ChapterModel
    {
        public ChapterModel()
        {
            Attachments = new List<ChapterAttachmentModel>();
        }

        public ChapterModel(ChapterEntity chapter)
        {
            Id = chapter.Id;
            CreatedBy = chapter.CreatedBy;
            CreatedDate = chapter.CreatedDate;
            ChangedBy = chapter.ChangedBy;
            ChangedDate = chapter.ChangedDate;
            ObjectId = chapter.ObjectId;
            OriginalObjectId = chapter.OriginalObjectId;
            Title = chapter.Title;
            Description = chapter.Description;
            TimeStart = chapter.TimeStart;
            TimeEnd = chapter.TimeEnd;
            SourceType = chapter.SourceType;
            Note = chapter.Note;
            Attachments = chapter.Attachments
                .Select(chapterAttachments => new ChapterAttachmentModel()
                {
                    Id = chapterAttachments.Id,
                    ObjectId = chapterAttachments.Id,
                    FileLocation = chapterAttachments.FileLocation,
                    FileName = chapterAttachments.FileName,
                    CreatedDate = chapterAttachments.CreatedDate,
                    ChangedDate = chapterAttachments.ChangedDate
                })
                .ToList();
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public Guid OriginalObjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? TimeStart { get; set; }

        public int? TimeEnd { get; set; }

        public VideoSourceType SourceType { get; set; }

        public string Note { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? ChangedBy { get; set; }

        public DateTime? ChangedDate { get; set; }

        public ICollection<ChapterAttachmentModel> Attachments { get; set; }
    }
}
