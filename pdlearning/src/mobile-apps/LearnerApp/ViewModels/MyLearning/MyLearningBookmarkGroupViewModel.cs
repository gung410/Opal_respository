using System.Threading.Tasks;
using LearnerApp.Models.Learner;
using LearnerApp.Services;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public class MyLearningBookmarkGroupViewModel : BaseViewModel
    {
        private readonly ICommonServices _commonService;

        private string _digitalContentBookmarkCount;
        private string _learningPathBookmarkCount;
        private string _courseBookmarkCount;
        private string _microLearningBookmarkCount;
        private string _communityBookmarkCount;
        private bool _isEnableCouresBookmarkGroup;
        private bool _isEnableMicrolearningBookmarkGroup;
        private bool _isEnableDigitalContentBookmarkGroup;
        private bool _isEnableLearningPathsBookmarkGroup;
        private bool _isEnableCommunityBookmarkGroup;

        public MyLearningBookmarkGroupViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();
        }

        public string DigitalContentBookmarkCount
        {
            get
            {
                return _digitalContentBookmarkCount;
            }

            set
            {
                _digitalContentBookmarkCount = value;

                RaisePropertyChanged(() => DigitalContentBookmarkCount);
            }
        }

        public string CourseBookmarkCount
        {
            get
            {
                return _courseBookmarkCount;
            }

            set
            {
                _courseBookmarkCount = value;
                RaisePropertyChanged(() => CourseBookmarkCount);
            }
        }

        public string LearningPathBookmarkCount
        {
            get
            {
                return _learningPathBookmarkCount;
            }

            set
            {
                _learningPathBookmarkCount = value;
                RaisePropertyChanged(() => LearningPathBookmarkCount);
            }
        }

        public string MicroLearningBookmarkCount
        {
            get
            {
                return _microLearningBookmarkCount;
            }

            set
            {
                _microLearningBookmarkCount = value;
                RaisePropertyChanged(() => MicroLearningBookmarkCount);
            }
        }

        public string CommunityBookmarkCount
        {
            get
            {
                return _communityBookmarkCount;
            }

            set
            {
                _communityBookmarkCount = value;
                RaisePropertyChanged(() => CommunityBookmarkCount);
            }
        }

        public bool IsEnableCouresBookmarkGroup
        {
            get
            {
                return _isEnableCouresBookmarkGroup;
            }

            set
            {
                _isEnableCouresBookmarkGroup = value;
                RaisePropertyChanged(() => IsEnableCouresBookmarkGroup);
            }
        }

        public bool IsEnableLearningPathsBookmarkGroup
        {
            get
            {
                return _isEnableLearningPathsBookmarkGroup;
            }

            set
            {
                _isEnableLearningPathsBookmarkGroup = value;
                RaisePropertyChanged(() => IsEnableLearningPathsBookmarkGroup);
            }
        }

        public bool IsEnableMicrolearningBookmarkGroup
        {
            get
            {
                return _isEnableMicrolearningBookmarkGroup;
            }

            set
            {
                _isEnableMicrolearningBookmarkGroup = value;
                RaisePropertyChanged(() => IsEnableMicrolearningBookmarkGroup);
            }
        }

        public bool IsEnablDigitalContentBookmarkGroup
        {
            get
            {
                return _isEnableDigitalContentBookmarkGroup;
            }

            set
            {
                _isEnableDigitalContentBookmarkGroup = value;
                RaisePropertyChanged(() => IsEnablDigitalContentBookmarkGroup);
            }
        }

        public bool IsEnableCommunityBookmarkGroup
        {
            get
            {
                return _isEnableCommunityBookmarkGroup;
            }

            set
            {
                _isEnableCommunityBookmarkGroup = value;
                RaisePropertyChanged(() => IsEnableCommunityBookmarkGroup);
            }
        }

        public async Task GetBookmarkGroupCount()
        {
            if (!IsBusy)
            {
                IsBusy = true;

                await Task.WhenAll(GetDigitalContentBookmarkCount(), GetCourseBookmarkCount(), GetMicrolearningCount(), GetLearningPathsBookmarkCount(), GetCommunityBookmarkCount());
            }

            IsBusy = false;
        }

        private async Task GetMicrolearningCount()
        {
            var result = await _commonService.GetBookmarkedCollection(bookmarkType: new string[] { BookmarkType.Microlearning.ToString() }, totalCount: count =>
            {
                IsEnableMicrolearningBookmarkGroup = count > 0;

                string label = count != 1 ? "Microlearnings" : "Microlearning";

                MicroLearningBookmarkCount = $"{label} ({count})";
            });
        }

        private async Task GetDigitalContentBookmarkCount()
        {
            var result = await _commonService.GetMyDigitalContentBookmarked(totalCountAction: count =>
            {
                IsEnablDigitalContentBookmarkGroup = count > 0;

                string label = count != 1 ? "Digital Contents" : "Digital Content";

                DigitalContentBookmarkCount = $"{label} ({count})";
            });
        }

        private async Task GetLearningPathsBookmarkCount()
        {
            var result = await _commonService.GetMyLearningPathsBookmarked(totalCount: count =>
            {
                IsEnableLearningPathsBookmarkGroup = count > 0;

                string label = count != 1 ? "Learning Paths" : "Learning Path";

                LearningPathBookmarkCount = $"{label} ({count})";
            });
        }

        private async Task GetCourseBookmarkCount()
        {
            var result = await _commonService.GetBookmarkedCollection(bookmarkType: new string[] { BookmarkType.Course.ToString() }, totalCount: count =>
            {
                IsEnableCouresBookmarkGroup = count > 0;

                string label = count != 1 ? "Courses" : "Course";

                CourseBookmarkCount = $"{label} ({count})";
            });
        }

        private async Task GetCommunityBookmarkCount()
        {
            var result = await _commonService.GetBookmarkedCommunity(totalCount: count =>
            {
                IsEnableCommunityBookmarkGroup = count > 0;

                string label = count == 1 ? "Community" : "Communities";

                CommunityBookmarkCount = $"{label} ({count})";
            });
        }
    }
}
