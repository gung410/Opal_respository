using System;
using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Resources.Texts;
using Xamarin.Forms;

namespace LearnerApp.Services
{
    public static class CourseCardBuilder
    {
        private static List<ClassRun> _classRuns;

        public static List<ItemCard> BuildCourseCardListAsync(
            List<MyCourseSummary> myCourseList,
            List<CourseExtendedInformation> courseInformationList,
            List<ClassRun> classRuns,
            List<MyLectureName> currentLectureNames)
        {
            _classRuns = classRuns;

            var courseCard = myCourseList
                .Join(courseInformationList, myCourse => myCourse.CourseId, courseInformation => courseInformation.Id, Build)
                .ToList();

            if (!currentLectureNames.IsNullOrEmpty())
            {
                foreach (var card in courseCard)
                {
                    if (!string.IsNullOrEmpty(card.LectureName))
                    {
                        card.LectureName = currentLectureNames.FirstOrDefault(p => p.LectureId == card.LectureName)?.Name;
                    }
                }
            }

            return courseCard;
        }

        public static List<string> GetMetadataTags(CourseExtendedInformation courseInformation)
        {
            var tags = new List<string>();
            var metadataTagging = Application.Current.Properties.GetMetadataTagging();
            if (!metadataTagging.IsNullOrEmpty())
            {
                // PDO Opportunity
                var pdType = metadataTagging.FirstOrDefault(p => p.TagId == courseInformation.PdActivityType);

                if (pdType != null)
                {
                    tags.Add(pdType.DisplayText);
                }

                // Mode of Learning
                var learningMode = metadataTagging.FirstOrDefault(p => p.TagId == courseInformation.LearningMode);

                if (learningMode != null)
                {
                    tags.Add(learningMode.DisplayText);
                }

                // PD Subject / Theme
                var pdSubjectTheme = metadataTagging.FirstOrDefault(p => p.TagId == courseInformation.PdAreaThemeId);

                if (pdSubjectTheme != null)
                {
                    tags.Add(pdSubjectTheme.DisplayText);
                }
            }

            return tags;
        }

        private static ItemCard Build(MyCourseSummary myCourseSummary, CourseExtendedInformation courseInformation)
        {
            if (myCourseSummary == null)
            {
                throw new ArgumentNullException(nameof(myCourseSummary));
            }

            if (courseInformation == null)
            {
                throw new ArgumentNullException(nameof(courseInformation));
            }

            // Determine the status of a course.
            // If the course information is unpublished, the course will be unpublished immediately.
            // If the course information has expired status, the course will be expired immediately.
            // Otherwise we will use the current course status from learner.
            string status;
            if (courseInformation.Status.Equals(nameof(StatusCourse.Unpublished)))
            {
                status = nameof(StatusCourse.Unpublished);
            }
            else
            {
                if (courseInformation.Status.Equals(nameof(StatusLearning.Archived)))
                {
                    status = nameof(StatusLearning.Archived);
                }
                else
                {
                    status = courseInformation.IsCourseExpired() ? nameof(StatusLearning.Expired) : myCourseSummary.GetCourseStatus();
                }
            }

            var tagging = GetMetadataTags(courseInformation);
            string modeOfLearning = string.Empty;

            var metadataTagging = Application.Current.Properties.GetMetadataTagging();

            if (!metadataTagging.IsNullOrEmpty())
            {
                // Mode of Learning
                var learningMode = metadataTagging.FirstOrDefault(p => p.TagId == courseInformation.LearningMode);
                if (learningMode != null)
                {
                    modeOfLearning = learningMode.DisplayText;
                }
            }

            var data = new ItemCard
            {
                Status = status,
                Rating = myCourseSummary.Rating,
                CourseCode = courseInformation.CourseCode,
                Name = courseInformation.CourseName,
                LectureName = myCourseSummary.MyCourseInfo?.CurrentLecture,
                DurationMinutes = courseInformation.DurationMinutes,
                Id = courseInformation.Id,
                Tags = tagging,
                ThumbnailUrl = string.IsNullOrEmpty(courseInformation.ThumbnailUrl) ? "image_place_holder_h150.png" : courseInformation.ThumbnailUrl,
                IsExpired = courseInformation.IsCourseExpired(),
                ProgressMeasure = myCourseSummary.GetCourseProgress(),
                CourseStatus = courseInformation.Status,
                ReviewsCount = myCourseSummary.ReviewsCount,
                MyClassRun = myCourseSummary.MyClassRun,
                BookmarkInfo = myCourseSummary.BookmarkInfo,
                CardType = tagging.Contains(BookmarkType.Microlearning.ToString()) ? BookmarkType.Microlearning : BookmarkType.Course,
                LearningMode = modeOfLearning,
                LearningModeId = courseInformation.LearningMode,
                PdActivityType = courseInformation.PdActivityType,
                PdAreaThemeId = courseInformation.PdAreaThemeId,
                IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark(),
            };

            if (myCourseSummary.BookmarkInfo != null)
            {
                data.BookmarkInfoChangedDate = myCourseSummary.BookmarkInfo.CreatedDate;
            }

            if (myCourseSummary.MyCourseInfo != null && myCourseSummary.MyClassRun != null)
            {
                string classRunName = string.Empty;
                if (_classRuns != null)
                {
                    classRunName = _classRuns.FirstOrDefault(p => p.Id == myCourseSummary.MyClassRun.ClassRunId)?.ClassTitle;
                }

                if (myCourseSummary.MyCourseInfo.MyRegistrationStatus == RegistrationStatus.WaitlistPendingApprovalByLearner.ToString() ||
                            myCourseSummary.MyCourseInfo.MyRegistrationStatus == RegistrationStatus.OfferPendingApprovalByLearner.ToString())
                {
                    data.IsShowClassRunConfirm = true;

                    if (myCourseSummary.MyClassRun.Status == RegistrationStatus.WaitlistPendingApprovalByLearner.ToString())
                    {
                        data.MyClassRunConfirmMessage = $"{TextsResource.WAITLIST_PLACED}: {classRunName}";
                    }
                    else
                    {
                        data.MyClassRunConfirmMessage = $"{TextsResource.WAITLIST_OFFERED} {classRunName}";
                    }
                }
                else if (myCourseSummary.MyCourseInfo.MyRegistrationStatus == RegistrationStatus.Rejected.ToString() ||
                            myCourseSummary.MyCourseInfo.MyRegistrationStatus == RegistrationStatus.OfferRejected.ToString() ||
                            myCourseSummary.MyCourseInfo.MyRegistrationStatus == RegistrationStatus.RejectedByCA.ToString() ||
                            myCourseSummary.MyCourseInfo.MyRegistrationStatus == RegistrationStatus.WaitlistRejected.ToString())
                {
                    data.IsShowClassRunReject = true;

                    data.MyClassRunRejectReason = myCourseSummary.MyClassRun.Comment;

                    data.IsVisibleMyClassRunRejectReasonButton = !string.IsNullOrEmpty(myCourseSummary.MyClassRun.Comment);

                    data.MyClassRunRejectMessage = $"Your request to join was declined for Class Run: {classRunName}";
                }
            }

            return data;
        }
    }
}
