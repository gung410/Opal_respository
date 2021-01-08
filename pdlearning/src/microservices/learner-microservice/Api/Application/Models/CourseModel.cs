using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.Models
{
    public class CourseModel
    {
        public CourseModel(
            Guid courseId,
            double rating,
            int reviewsCount)
        {
            CourseId = courseId;
            Rating = rating;
            ReviewsCount = reviewsCount;
        }

        public Guid CourseId { get; }

        public double Rating { get; }

        public int ReviewsCount { get; }

        public int? CompletedTimes { get; private set; }

        public UserBookmarkModel BookmarkInfo { get; private set; }

        public MyCourseModel MyCourseInfo { get; private set; }

        public List<LectureInMyCourseModel> MyLecturesInfo { get; private set; }

        public List<MyClassRunModel> MyClassRuns { get; private set; }

        public MyClassRunModel MyClassRun { get; private set; }

        public List<MyClassRunBasicInfoModel> RejectedMyClassRuns { get; private set; }

        public List<MyClassRunBasicInfoModel> WithdrawnMyClassRuns { get; private set; }

        public MyClassRunBasicInfoModel ExpiredMyClassRun { get; set; }

        public static CourseModel New(
            Guid courseId,
            double rating,
            int reviewsCount)
        {
            return new CourseModel(
                courseId,
                rating,
                reviewsCount);
        }

        /// <summary>
        /// Notify user that their course has been bookmark.
        /// </summary>
        /// <param name="userBookmark">UserBookmark entity.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithBookmarkInfo(UserBookmark userBookmark)
        {
            if (userBookmark != null)
            {
                BookmarkInfo = new UserBookmarkModel(userBookmark);
            }

            return this;
        }

        /// <summary>
        /// Get the current my course.
        /// </summary>
        /// <param name="myCourse">MyCourse entity.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithMyCourseInfo(MyCourse myCourse)
        {
            if (myCourse != null)
            {
                MyCourseInfo = new MyCourseModel(myCourse);
            }

            return this;
        }

        /// <summary>
        /// Get lectures of the course or class run.
        /// </summary>
        /// <param name="lectureInMyCourses">LectureInMyCourse entity list.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithMyLecturesInfo(List<LectureInMyCourse> lectureInMyCourses)
        {
            if (lectureInMyCourses.Any())
            {
                MyLecturesInfo = lectureInMyCourses
                    .Select(p => new LectureInMyCourseModel(p))
                    .ToList();
            }

            return this;
        }

        /// <summary>
        /// Get completed and in-progress on the workflow.
        /// </summary>
        /// <param name="myClassRuns">MyClassRun entity list.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithMyClassRuns(List<MyClassRun> myClassRuns)
        {
            if (myClassRuns.Any())
            {
                MyClassRuns = myClassRuns
                    .Select(p => new MyClassRunModel(p))
                    .ToList();
            }

            return this;
        }

        /// <summary>
        /// Get the current class run in-progress on the workflow.
        /// </summary>
        /// <param name="myClassRun">MyClassRun entity.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithMyClassRun(MyClassRun myClassRun)
        {
            if (myClassRun != null)
            {
                MyClassRun = new MyClassRunModel(myClassRun);
            }

            return this;
        }

        /// <summary>
        /// Support to display apply again button on client.
        /// </summary>
        /// <param name="myClassRunsRejected"><see cref="MyClassRunBasicInfoModel"/> list.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithRejectedMyClassRuns(List<MyClassRunBasicInfoModel> myClassRunsRejected)
        {
            if (myClassRunsRejected.Any())
            {
                RejectedMyClassRuns = myClassRunsRejected;
            }

            return this;
        }

        /// <summary>
        /// Support to display withdraw reason button on client.
        /// </summary>
        /// <param name="withdrawnMyClassRuns"><see cref="MyClassRunBasicInfoModel"/> list.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithWithdrawnMyClassRuns(List<MyClassRunBasicInfoModel> withdrawnMyClassRuns)
        {
            if (withdrawnMyClassRuns.Any())
            {
                WithdrawnMyClassRuns = withdrawnMyClassRuns;
            }

            return this;
        }

        public CourseModel WithCompletedTimes(int? completedTimes)
        {
            if (completedTimes != null)
            {
                CompletedTimes = completedTimes.Value;
            }

            return this;
        }

        /// <summary>
        /// Get the number of completed to check the user can register for the class in the course.
        /// </summary>
        /// <param name="completedTimes">The number of completed.</param>
        /// <returns>Returns the number of completed in the current course.</returns>
        public CourseModel WithCompletedTimes(int completedTimes)
        {
            CompletedTimes = completedTimes;
            return this;
        }

        /// <summary>
        /// Get my class runs has expired on the workflow.
        /// </summary>
        /// <param name="expiredMyClassRun"><see cref="MyClassRunBasicInfoModel"/> list.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithExpiredMyClassRun(MyClassRunBasicInfoModel expiredMyClassRun)
        {
            if (expiredMyClassRun != null)
            {
                ExpiredMyClassRun = expiredMyClassRun;
            }

            return this;
        }

        /// <summary>
        /// Get my class runs has completed on the workflow.
        /// </summary>
        /// <param name="myClassRuns">MyClassRun entity list.</param>
        /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
        public CourseModel WithMyClassRunsCompleted(List<MyClassRun> myClassRuns)
        {
            if (myClassRuns.Any())
            {
                MyClassRuns = myClassRuns
                    .Where(p => p.IsFinishedLearning())
                    .Select(p => new MyClassRunModel(p))
                    .ToList();
            }

            return this;
        }
    }
}
