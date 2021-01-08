using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning
{
    public class MyLearningPathsBookmarkedViewModel : BaseViewModel
    {
        private readonly ICommonServices _commonService;
        private readonly ICourseBackendService _courseBackendService;

        private int _totalBookmarkCount;
        private int _skipCount = 0;

        public MyLearningPathsBookmarkedViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);

            TotalBookmarkCount = -1;

            MessagingCenter.Subscribe<MyLearningPathsDetailsViewModel, LearningPath>(this, "learning-path-bookmarked", (sender, arg) =>
            {
                if (Collection.IsNullOrEmpty())
                {
                    return;
                }

                if (arg.BookmarkInfo == null)
                {
                    Collection.Remove(arg);
                }
                else
                {
                    Collection.Insert(0, arg);
                }
            });
        }

        public ICommand LoadmoreBookmarkCommand => new Command(async () => await OnLoadMore());

        public ICommand RefreshCommand => new Command(async () => await LoadLearningPathsBookmarkCollection(false));

        public LearnerObservableCollection<LearningPath> Collection { get; set; }

        public int TotalBookmarkCount
        {
            get
            {
                return _totalBookmarkCount;
            }

            set
            {
                _totalBookmarkCount = value;

                if (value != -1)
                {
                    RaisePropertyChanged(() => TotalBookmarkCount);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<MyLearningPathsDetailsViewModel, LearningPath>(this, "learning-path-bookmarked");
        }

        public async Task LoadLearningPathsBookmarkCollection(bool isloadMore)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            if (!isloadMore)
            {
                _skipCount = 0;
                Collection = new LearnerObservableCollection<LearningPath>();
            }
            else
            {
                _skipCount++;
            }

            var learningPathsBookmarkCollection = await _commonService.GetMyLearningPathsBookmarked(skipCount: _skipCount * GlobalSettings.MaxResultPerPage, totalCount: count =>
            {
                TotalBookmarkCount = count;
            });

            if (learningPathsBookmarkCollection.Count > 0)
            {
                var learningPathFromLMM = learningPathsBookmarkCollection.Where(p => p.Id == Guid.Empty.ToString()).ToList();

                if (learningPathFromLMM.Count > 0)
                {
                    var ids = learningPathFromLMM.Select(p => p.BookmarkInfo.ItemId).ToArray();

                    var learningPathInfo = await ExecuteBackendService(() => _courseBackendService.GetLearningPathsByIds(ids));

                    for (int i = 0; i < learningPathsBookmarkCollection.Count; ++i)
                    {
                        var learningPathFromLmm = learningPathInfo.Payload.FirstOrDefault(p => p.Id == learningPathsBookmarkCollection[i].BookmarkInfo.ItemId);

                        if (learningPathFromLmm != null)
                        {
                            learningPathFromLmm.BookmarkInfo = learningPathsBookmarkCollection[i].BookmarkInfo;
                            learningPathsBookmarkCollection[i] = learningPathFromLmm;
                        }
                    }
                }
            }

            Collection.AddRange(learningPathsBookmarkCollection);
            if (!isloadMore)
            {
                RaisePropertyChanged(() => Collection);
            }

            IsBusy = false;
        }

        private async Task OnLoadMore()
        {
            if (Collection.Count < TotalBookmarkCount)
            {
                await LoadLearningPathsBookmarkCollection(true);
            }
        }
    }
}
