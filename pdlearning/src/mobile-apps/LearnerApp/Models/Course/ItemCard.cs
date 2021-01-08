using System;
using System.Collections.Generic;
using LearnerApp.Helper;
using LearnerApp.Models.Learner;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Models
{
    /// <summary>
    /// This model is used for display a course card in UI only. Not for client/server communication.
    /// </summary>
    public class ItemCard : ExtendedBindableObject, ICloneable
    {
        private string _originalObjectId;

        private string _thumbnailUrl;

        private BookmarkInfo _bookmarkInfo;

        private bool _isVisibleBookmark;

        public string Id { get; set; }

        public string Status { get; set; }

        public string CourseStatus { get; set; }

        public string Name { get; set; }

        public string CourseCode { get; set; }

        public double Rating { get; set; }

        public int DurationMinutes { get; set; }

        public int ReviewsCount { get; set; }

        public List<string> Tags { get; set; }

        public string PdActivityType { get; set; }

        public string PdAreaThemeId { get; set; }

        public string LearningModeId { get; set; }

        public double ProgressMeasure { get; set; }

        public string ThumbnailUrl
        {
            get
            {
                return _thumbnailUrl;
            }

            set
            {
                _thumbnailUrl = value;
                RaisePropertyChanged(() => ThumbnailUrl);
            }
        }

        public bool IsExpired { get; set; }

        public BookmarkType CardType { get; set; }

        public MyClassRun MyClassRun { get; set; }

        public string OriginalObjectId
        {
            get
            {
                if (_originalObjectId == System.Guid.Empty.ToString())
                {
                    _originalObjectId = Id;
                }

                return _originalObjectId;
            }

            set
            {
                _originalObjectId = value;
            }
        }

        public DateTime BookmarkInfoChangedDate { get; set; }

        public string LearningMode { get; set; }

        public string LectureName { get; set; }

        public bool IsShowClassRunConfirm { get; set; }

        public bool IsShowClassRunReject { get; set; }

        public bool IsVisibleMyClassRunRejectReasonButton { get; set; }

        public string MyClassRunConfirmMessage { get; set; }

        public string MyClassRunRejectReason { get; set; }

        public string MyClassRunRejectMessage { get; set; }

        public BookmarkInfo BookmarkInfo
        {
            get
            {
                return _bookmarkInfo;
            }

            set
            {
                _bookmarkInfo = value;
                RaisePropertyChanged(() => BookmarkInfo);
            }
        }

        public bool IsVisibleBookmark
        {
            get
            {
                return _isVisibleBookmark;
            }

            set
            {
                _isVisibleBookmark = value;
                RaisePropertyChanged(() => IsVisibleBookmark);
                _isVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
            }
        }

        public string Guid { get; set; }

        public string Description { get; set; }

        public int MemberCount { get; set; }

        public string DetailUrl { get; set; }

        /// <summary>
        /// Clone this object into a new instance.
        /// The reason why we need this method because there are some properties can not be cloned easily (reference type).
        /// </summary>
        /// <returns>The new object instance.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
