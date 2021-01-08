using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Thunder.Platform.Core.Timing;

namespace Microservice.Content.Application.Commands
{
    public class CommandUtilities
    {
        public static LearningContent CloneLearningContent(
            DigitalContent existingDigitalContent,
            Guid newId,
            Guid userId,
            DigitalContentStatus newStatus,
            Func<string, string> funcCloneTitle)
        {
            var existingLearningContent = existingDigitalContent as LearningContent;
            return new LearningContent
            {
                Id = newId,
                Title = funcCloneTitle(existingLearningContent.Title),
                Status = newStatus,
                Type = existingLearningContent.Type,
                Description = existingLearningContent.Description,
                HtmlContent = existingLearningContent.HtmlContent,
                CreatedBy = userId,
                CreatedDate = Clock.Now,
                OwnerId = userId,
                Source = existingLearningContent.Source,
                Publisher = existingLearningContent.Publisher,
                Copyright = existingLearningContent.Copyright,
                TermsOfUse = existingLearningContent.TermsOfUse,
                ExpiredDate = existingLearningContent.ExpiredDate,
                LicenseType = existingLearningContent.LicenseType,
                StartDate = existingLearningContent.StartDate,
                AcknowledgementAndCredit = existingLearningContent.AcknowledgementAndCredit,
                IsAllowDownload = existingLearningContent.IsAllowDownload,
                IsAllowModification = existingLearningContent.IsAllowModification,
                IsAllowReusable = existingLearningContent.IsAllowReusable,
                LicenseTerritory = existingLearningContent.LicenseTerritory,
                Remarks = existingLearningContent.Remarks,
                Ownership = existingLearningContent.Ownership,
                PrimaryApprovingOfficerId = existingLearningContent.PrimaryApprovingOfficerId,
                AlternativeApprovingOfficerId = existingLearningContent.AlternativeApprovingOfficerId,
                DepartmentId = existingLearningContent.DepartmentId,
                IsAutoPublish = existingDigitalContent.IsAutoPublish,
                AutoPublishDate = existingDigitalContent.AutoPublishDate
            };
        }

        public static UploadedContent CloneUploadedContent(
           DigitalContent existingDigitalContent,
           Guid newId,
           Guid userId,
           DigitalContentStatus newStatus,
           Func<string, string> funcCloneTitle)
        {
            var existingUploadedContent = existingDigitalContent as UploadedContent;
            return new UploadedContent
            {
                Id = newId,
                Title = funcCloneTitle(existingUploadedContent.Title),
                Status = newStatus,
                Type = existingUploadedContent.Type,
                Description = existingUploadedContent.Description,
                FileName = existingUploadedContent.FileName,
                FileType = existingUploadedContent.FileType,
                FileExtension = existingUploadedContent.FileExtension,
                FileSize = existingUploadedContent.FileSize,
                FileLocation = existingUploadedContent.FileLocation,
                FileDuration = existingUploadedContent.FileDuration,
                CreatedBy = userId,
                CreatedDate = Clock.Now,
                OwnerId = userId,
                Source = existingUploadedContent.Source,
                Publisher = existingUploadedContent.Publisher,
                Copyright = existingUploadedContent.Copyright,
                TermsOfUse = existingUploadedContent.TermsOfUse,
                ExpiredDate = existingUploadedContent.ExpiredDate,
                LicenseType = existingUploadedContent.LicenseType,
                StartDate = existingUploadedContent.StartDate,
                AcknowledgementAndCredit = existingUploadedContent.AcknowledgementAndCredit,
                IsAllowDownload = existingUploadedContent.IsAllowDownload,
                IsAllowModification = existingUploadedContent.IsAllowModification,
                IsAllowReusable = existingUploadedContent.IsAllowReusable,
                LicenseTerritory = existingUploadedContent.LicenseTerritory,
                Remarks = existingUploadedContent.Remarks,
                Ownership = existingUploadedContent.Ownership,
                PrimaryApprovingOfficerId = existingUploadedContent.PrimaryApprovingOfficerId,
                AlternativeApprovingOfficerId = existingUploadedContent.AlternativeApprovingOfficerId,
                DepartmentId = existingUploadedContent.DepartmentId,
                IsAutoPublish = existingDigitalContent.IsAutoPublish,
                AutoPublishDate = existingDigitalContent.AutoPublishDate
            };
        }

        public static List<AttributionElement> CloneAttributionElement(
            List<AttributionElement> sourcettributionElements,
            Guid newDigitalContentId)
        {
            return sourcettributionElements.Select(_ => new AttributionElement
            {
                Id = Guid.NewGuid(),
                DigitalContentId = newDigitalContentId,
                LicenseType = _.LicenseType,
                Author = _.Author,
                Title = _.Title,
                Source = _.Source,
                CreatedDate = Clock.Now
            }).ToList();
        }

        public static List<Chapter> CloneChapter(
            List<Chapter> sourceChapters,
            Guid newDigitalContentId,
            Guid userId)
        {
            return sourceChapters.Select(_ => new Chapter
            {
                Id = Guid.NewGuid(),
                ObjectId = newDigitalContentId,
                OriginalObjectId = _.OriginalObjectId,
                Title = _.Title,
                Description = _.Description,
                TimeStart = _.TimeStart,
                TimeEnd = _.TimeEnd,
                CreatedBy = userId,
                CreatedDate = Clock.Now,
                SourceType = _.SourceType,
                Attachments = _.Attachments.Select(m => new ChapterAttachment
                {
                    Id = Guid.NewGuid(),
                    ObjectId = _.Id,
                    FileLocation = m.FileLocation,
                    FileName = m.FileName,
                    CreatedDate = Clock.Now
                }).ToList()
            }).ToList();
        }
    }
}
