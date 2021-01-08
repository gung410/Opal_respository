using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure
{
    public class LearnerSeeder : IDbContextSeeder
    {
        private readonly Guid _userId = new Guid("7a4b59c9-9fb5-4d58-9e1f-579cf6bcb204");
        private readonly Guid _myCourseId0 = new Guid("e37b4dbf-9f0a-4272-a788-2f3cc5a15a7a");
        private readonly Guid _myCourseId1 = new Guid("6c901d54-cb20-4367-b4bc-5f687f4cb062");
        private readonly Guid _myCourseId2 = new Guid("b7414da4-9a89-4ebb-b0af-12d6a3c09984");
        private readonly Guid _myCourseId3 = new Guid("f6465145-a361-40bb-9bf5-e41a0d335213");
        private readonly Guid _myCourseId4 = new Guid("63689f4a-9c10-48e2-be23-fe5509a9661e");
        private readonly Guid _myCourseId5 = new Guid("32871bc6-c7cb-4c89-8c4a-a296c5d80550");
        private readonly Guid _myCourseId6 = new Guid("b05179a8-1324-4739-a53f-ddc16c413210");

        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
            /*
            SeedUserProfile(context);
            SeedMyCourses(context);
            SeedBookmarks(context);
            SeedReviews(context);
            */
        }

        private void SeedUserProfile(LearnerDbContext context)
        {
            var existingUser = context.UserProfiles.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId);
            if (existingUser != null)
            {
                return;
            }

            context.UserProfiles.Add(new UserProfile()
            {
                Id = _userId,
                UserId = _userId,
                CreatedDate = Clock.Now,
                DateOfBirth = new DateTime(1996, 5, 16),
                Email = "learnerTest@mailinator.com",
                Gender = true,
                PlaceOfWork = "Orient",
                TeachingLevel = TeachingLevel.Secondary,
                CreatedBy = _userId
            });
        }

        private void SeedMyCourses(LearnerDbContext context)
        {
            var existingMyCourse0 = context.MyCourses.IgnoreQueryFilters().FirstOrDefault(p => (p.UserId == _userId && p.CourseId == _myCourseId0) || p.Id == _myCourseId0);
            if (existingMyCourse0 == null)
            {
                context.MyCourses.Add(new MyCourse()
                {
                    Id = _myCourseId0,
                    CourseId = _myCourseId0,
                    UserId = _userId,
                    Version = "1",
                    CreatedDate = Clock.Now.AddDays(-3),
                    StartDate = Clock.Now.AddDays(-2),
                    LastLogin = Clock.Now.AddDays(-1),
                    ProgressMeasure = 0,
                    Status = MyCourseStatus.InProgress,
                    CreatedBy = _userId
                });
            }

            var hasAnyLectureInMyCourse0 = context.LecturesInMyCourse.IgnoreQueryFilters().Any(p => p.UserId == _userId && p.LectureId == Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8A"));
            if (!hasAnyLectureInMyCourse0)
            {
                context.LecturesInMyCourse.AddRange(new List<LectureInMyCourse>
                {
                    new LectureInMyCourse { Id = Guid.NewGuid(), LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8A"), MyCourseId = _myCourseId0, UserId = _userId, Status = LectureStatus.NotStarted, CreatedBy = _userId, CreatedDate = Clock.Now },
                    new LectureInMyCourse { Id = Guid.NewGuid(), LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8B"), MyCourseId = _myCourseId0, UserId = _userId, Status = LectureStatus.NotStarted, CreatedBy = _userId, CreatedDate = Clock.Now },
                    new LectureInMyCourse { Id = Guid.NewGuid(), LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8C"), MyCourseId = _myCourseId0, UserId = _userId, Status = LectureStatus.NotStarted, CreatedBy = _userId, CreatedDate = Clock.Now },
                    new LectureInMyCourse { Id = Guid.NewGuid(), LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8D"), MyCourseId = _myCourseId0, UserId = _userId, Status = LectureStatus.NotStarted, CreatedBy = _userId, CreatedDate = Clock.Now },
                    new LectureInMyCourse { Id = Guid.NewGuid(), LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8E"), MyCourseId = _myCourseId0, UserId = _userId, Status = LectureStatus.NotStarted, CreatedBy = _userId, CreatedDate = Clock.Now }
                });
            }

            var existingMyCourse1 = context.MyCourses.IgnoreQueryFilters().FirstOrDefault(p => (p.UserId == _userId && p.CourseId == _myCourseId1) || p.Id == _myCourseId1);
            if (existingMyCourse1 == null)
            {
                context.MyCourses.Add(new MyCourse()
                {
                    Id = _myCourseId1,
                    CourseId = _myCourseId1,
                    UserId = _userId,
                    Version = "1",
                    CreatedDate = Clock.Now.AddDays(-3),
                    StartDate = Clock.Now.AddDays(-2),
                    LastLogin = Clock.Now.AddDays(-1),
                    ProgressMeasure = 30,
                    Status = MyCourseStatus.InProgress,
                    CreatedBy = _userId
                });
            }

            var existingMyCourse2 = context.MyCourses.IgnoreQueryFilters().FirstOrDefault(p => (p.UserId == _userId && p.CourseId == _myCourseId2) || p.Id == _myCourseId2);
            if (existingMyCourse2 == null)
            {
                context.MyCourses.Add(new MyCourse()
                {
                    Id = _myCourseId2,
                    CourseId = _myCourseId2,
                    UserId = _userId,
                    Version = "1",
                    CreatedDate = Clock.Now.AddDays(-5),
                    StartDate = Clock.Now.AddDays(-3),
                    LastLogin = Clock.Now.AddDays(-2),
                    ProgressMeasure = 0,
                    Status = MyCourseStatus.InProgress,
                    CreatedBy = _userId
                });
            }

            var existingMyCourse3 = context.MyCourses.IgnoreQueryFilters().FirstOrDefault(p => (p.UserId == _userId && p.CourseId == _myCourseId3) || p.Id == _myCourseId3);
            if (existingMyCourse3 == null)
            {
                context.MyCourses.Add(new MyCourse()
                {
                    Id = _myCourseId3,
                    CourseId = _myCourseId3,
                    UserId = _userId,
                    Version = "1",
                    CreatedDate = Clock.Now.AddDays(-5),
                    StartDate = Clock.Now.AddDays(-3),
                    LastLogin = Clock.Now.AddDays(-2),
                    ProgressMeasure = 90,
                    Status = MyCourseStatus.InProgress,
                    CreatedBy = _userId
                });
            }

            var existingMyCourse4 = context.MyCourses.IgnoreQueryFilters().FirstOrDefault(p => (p.UserId == _userId && p.CourseId == _myCourseId4) || p.Id == _myCourseId4);
            if (existingMyCourse4 == null)
            {
                context.MyCourses.Add(new MyCourse()
                {
                    Id = _myCourseId4,
                    CourseId = _myCourseId4,
                    UserId = _userId,
                    Version = "1",
                    CreatedDate = Clock.Now.AddDays(-5),
                    StartDate = Clock.Now.AddDays(-3),
                    LastLogin = Clock.Now.AddDays(-2),
                    ProgressMeasure = 50,
                    Status = MyCourseStatus.InProgress,
                    CreatedBy = _userId
                });
            }

            var existingMyCourse5 = context.MyCourses.IgnoreQueryFilters().FirstOrDefault(p => (p.UserId == _userId && p.CourseId == _myCourseId5) || p.Id == _myCourseId5);
            if (existingMyCourse5 == null)
            {
                context.MyCourses.Add(new MyCourse()
                {
                    Id = _myCourseId5,
                    CourseId = _myCourseId5,
                    UserId = _userId,
                    Version = "1",
                    CreatedDate = Clock.Now.AddDays(-5),
                    StartDate = Clock.Now.AddDays(-3),
                    LastLogin = Clock.Now.AddDays(-2),
                    CompletedDate = Clock.Now.AddDays(-2),
                    EndDate = Clock.Now.AddDays(-2),
                    ProgressMeasure = 100,
                    Status = MyCourseStatus.Completed,
                    CreatedBy = _userId
                });
            }

            var existingMyCourse6 = context.MyCourses.IgnoreQueryFilters().FirstOrDefault(p => (p.UserId == _userId && p.CourseId == _myCourseId6) || p.Id == _myCourseId6);
            if (existingMyCourse6 == null)
            {
                context.MyCourses.Add(new MyCourse()
                {
                    Id = _myCourseId6,
                    CourseId = _myCourseId6,
                    UserId = _userId,
                    Version = "1",
                    CreatedDate = Clock.Now.AddDays(-4),
                    StartDate = Clock.Now.AddDays(-3),
                    LastLogin = Clock.Now.AddDays(-2),
                    CompletedDate = Clock.Now.AddDays(-1),
                    EndDate = Clock.Now.AddDays(-1),
                    ProgressMeasure = 100,
                    Status = MyCourseStatus.Completed,
                    CreatedBy = _userId
                });
            }
        }

        private void SeedBookmarks(LearnerDbContext context)
        {
            var existingBookmark1 = context.UserBookmarks.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId && p.ItemId == _myCourseId1);
            if (existingBookmark1 != null)
            {
                return;
            }

            context.UserBookmarks.Add(new UserBookmark()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                CreatedDate = Clock.Now,
                ItemId = _myCourseId1,
                ItemType = BookmarkType.Course,
                CreatedBy = _userId
            });

            var existingBookmark2 = context.UserBookmarks.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId && p.ItemId == _myCourseId2);
            if (existingBookmark2 != null)
            {
                return;
            }

            context.UserBookmarks.Add(new UserBookmark()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                CreatedDate = Clock.Now,
                ItemId = _myCourseId2,
                ItemType = BookmarkType.Course,
                CreatedBy = _userId
            });

            var existingBookmark3 = context.UserBookmarks.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId && p.ItemId == _myCourseId5);
            if (existingBookmark3 != null)
            {
                return;
            }

            context.UserBookmarks.Add(new UserBookmark()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                CreatedDate = Clock.Now,
                ItemId = _myCourseId5,
                ItemType = BookmarkType.Course,
                CreatedBy = _userId
            });

            var existingBookmark4 = context.UserBookmarks.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId && p.ItemId == _myCourseId6);
            if (existingBookmark4 != null)
            {
                return;
            }

            context.UserBookmarks.Add(new UserBookmark()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                CreatedDate = Clock.Now,
                ItemId = _myCourseId6,
                ItemType = BookmarkType.Course,
                CreatedBy = _userId
            });
        }

        private void SeedReviews(LearnerDbContext context)
        {
            var existingReview1 = context.CourseReviews.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId && p.CourseId == _myCourseId1);
            if (existingReview1 != null)
            {
                return;
            }

            context.CourseReviews.Add(new CourseReview()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                CreatedDate = Clock.Now,
                CourseId = _myCourseId1,
                UserFullName = "Ling How Doong",
                Rate = 1,
                CommentContent = "Not very good.",
                CreatedBy = _userId
            });

            var existingReview2 = context.CourseReviews.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId && p.CourseId == _myCourseId2);
            if (existingReview2 != null)
            {
                return;
            }

            context.CourseReviews.Add(new CourseReview()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                CreatedDate = Clock.Now,
                CourseId = _myCourseId2,
                UserFullName = "Ling How Doong",
                Rate = 2,
                CommentContent = "Quite good.",
                CreatedBy = _userId
            });

            var existingReview3 = context.CourseReviews.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId && p.CourseId == _myCourseId3);
            if (existingReview3 != null)
            {
                return;
            }

            context.CourseReviews.Add(new CourseReview()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                CreatedDate = Clock.Now,
                CourseId = _myCourseId3,
                UserFullName = "Ling How Doong",
                Rate = 3,
                CommentContent = "Execellent",
                CreatedBy = _userId
            });

            var existingReview4 = context.CourseReviews.IgnoreQueryFilters().FirstOrDefault(p => p.UserId == _userId && p.CourseId == _myCourseId6);
            if (existingReview4 != null)
            {
                return;
            }

            context.CourseReviews.Add(new CourseReview()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                CreatedDate = Clock.Now,
                CourseId = _myCourseId6,
                UserFullName = "Ling How Doong",
                Rate = 3,
                CommentContent = "Very good.",
                CreatedBy = _userId
            });
        }
    }
}
