using System;
using System.Collections.Generic;
using LearnerApp.ViewModels.Base;
using Newtonsoft.Json;

namespace LearnerApp.Models.MyLearning
{
    public class LearningPath : ExtendedBindableObject
    {
        [JsonIgnore]
        private BookmarkInfo _bookmarkInfo;

        private bool _isVisibleBookmark = true;

        public string Id { get; set; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public bool IsPublic { get; set; }

        [JsonIgnore]
        public bool IsFromLMM { get; set; }

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
            }
        }

        public List<LearningPathsCourse> ListCourses
        {
            set
            {
                Courses = value;
            }
        }

        public List<LearningPathsCourse> Courses { get; set; }
    }
}
