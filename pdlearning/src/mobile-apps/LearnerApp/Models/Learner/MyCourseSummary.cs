using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;

namespace LearnerApp.Models.Learner
{
    /// <summary>
    /// When a learner learns a course, the learner API will store a few course information such as course id, rating, bookmark information, etc.
    /// </summary>
    public class MyCourseSummary
    {
        public string CourseId { get; set; }

        public double Rating { get; set; }

        public BookmarkInfo BookmarkInfo { get; set; }

        public MyCourseInfo MyCourseInfo { get; set; }

        public List<MyLecturesInfo> MyLecturesInfo { get; set; }

        public int ReviewsCount { get; set; }

        public MyClassRun MyClassRun { get; set; }

        public List<MyClassRun> MyClassRuns { get; set; }

        public int CompletedTimes { get; set; }

        public ExpiredMyClassRun ExpiredMyClassRun { get; set; }

        public List<MyClassRun> RejectedMyClassRuns { get; set; }

        public List<MyClassRun> WithdrawnMyClassRuns { get; set; }

        public double GetCourseProgress()
        {
            return MyCourseInfo?.ProgressMeasure ?? 0;
        }

        public string GetCourseStatus()
        {
            if (MyCourseInfo == null)
            {
                return string.Empty;
            }

            if (ExpiredMyClassRun != null)
            {
                string type = ExpiredMyClassRun.RegistrationType == RegistrationType.Manual ? "REGISTRATION" : "NOMINATION";
                return $"{type} UNSUCCESSFUL";
            }

            if (!string.IsNullOrEmpty(MyCourseInfo.Status) && !MyCourseInfo.Status.Equals(StatusLearning.NotStarted.ToString()))
            {
                if (MyCourseInfo.Status == StatusLearning.Failed.ToString())
                {
                    return StatusLearning.Incomplete.ToString();
                }

                if (MyCourseInfo.Status == StatusLearning.Passed.ToString())
                {
                    if (MyCourseInfo.PostCourseEvaluationFormCompleted != null &&
                        MyCourseInfo.PostCourseEvaluationFormCompleted.Value)
                    {
                        return StatusLearning.Completed.ToString();
                    }

                    return StatusLearning.InProgress.ToString();
                }

                return MyCourseInfo.Status;
            }

            if (string.IsNullOrEmpty(MyCourseInfo.MyWithdrawalStatus) && string.IsNullOrEmpty(MyCourseInfo.MyRegistrationStatus))
            {
                return StatusLearning.NotStarted.ToString();
            }

            return MyCourseInfo.DisplayStatus.ToString();
        }
    }
}
