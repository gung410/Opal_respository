using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.Models
{
    public class LearnerLearningPathModel
    {
        public LearnerLearningPathModel(UserBookmark userBookmark)
        {
            BookmarkInfo = new UserBookmarkModel(userBookmark);
        }

        public LearnerLearningPathModel(
            Guid id,
            string title,
            Guid createdBy,
            string thumbnailUrl,
            bool isPublic)
        {
            Id = id;
            Title = title;
            CreatedBy = createdBy;
            ThumbnailUrl = thumbnailUrl;
            IsPublic = isPublic;
        }

        public Guid Id { get; }

        public string Title { get; }

        public Guid CreatedBy { get; }

        public string ThumbnailUrl { get; }

        public UserBookmarkModel BookmarkInfo { get; private set; }

        public List<LearnerLearningPathCourseModel> Courses { get; private set; }

        public bool IsPublic { get; }

        public static LearnerLearningPathModel New(
            Guid id,
            string title,
            Guid createdBy,
            string thumbnailUrl,
            bool isPublic)
        {
            return new LearnerLearningPathModel(id, title, createdBy, thumbnailUrl, isPublic);
        }

        public LearnerLearningPathModel WithBookmarkInfo(UserBookmark userBookmark)
        {
            if (userBookmark != null)
            {
                BookmarkInfo = new UserBookmarkModel(userBookmark);
            }

            return this;
        }

        public LearnerLearningPathModel WithLearningPathCourses(
            List<LearnerLearningPathCourse> learningPathCourses)
        {
            if (learningPathCourses != null)
            {
                Courses = learningPathCourses
                    .Select(s => new LearnerLearningPathCourseModel(s))
                    .ToList();
            }

            return this;
        }
    }
}
