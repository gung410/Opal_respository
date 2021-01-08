using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Services;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public class MyLearningCourseBookmarkViewModel : BaseViewModel
    {
        private readonly ICommonServices _commonService;

        private int _totalBookmarkCount;
        private int _skipCount = 0;
        private BookmarkType _currentBookmarkType;

        public MyLearningCourseBookmarkViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();

            TotalBookmarkCount = -1;

            CourseBookmarkMessagingCenter.Subscribe(this, (sender, arg) =>
            {
                if (CourseCollection.IsNullOrEmpty())
                {
                    return;
                }

                var item = CourseCollection.FirstOrDefault(x => x.Id == arg.CourseId);
                if (item == null)
                {
                    return;
                }

                if (item.BookmarkInfo == null)
                {
                    CourseCollection.Remove(item);
                }
                else
                {
                    CourseCollection.Insert(0, item);
                }
            });
        }

        public ICommand LoadmoreBookmarkCommand => new Command(async () => await OnLoadMore());

        public ICommand RefreshCommand => new Command(async () => await OnRefresh());

        public LearnerObservableCollection<ItemCard> CourseCollection { get; set; }

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
            CourseBookmarkMessagingCenter.Unsubscribe(this);
        }

        public async Task LoadCourseBookmarkCollection(BookmarkType bookmarkType)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            _currentBookmarkType = bookmarkType;

            _skipCount = 0;

            var bookmarkCollection = await _commonService.GetBookmarkedCollection(bookmarkType: new string[] { _currentBookmarkType.ToString() }, skipCount: _skipCount, totalCount: count =>
            {
                TotalBookmarkCount = count;
            });

            _skipCount++;

            CourseCollection = new LearnerObservableCollection<ItemCard>(bookmarkCollection);
            RaisePropertyChanged(() => CourseCollection);

            IsBusy = false;
        }

        private async Task OnLoadMore()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var moreBookmarkItems = await _commonService.GetBookmarkedCollection(bookmarkType: new string[] { _currentBookmarkType.ToString() }, skipCount: _skipCount * GlobalSettings.MaxResultPerPage);

            if (!moreBookmarkItems.IsNullOrEmpty())
            {
                _skipCount++;

                CourseCollection.AddRange(moreBookmarkItems);
            }

            IsBusy = false;
        }

        private async Task OnRefresh()
        {
            if (!CourseCollection.IsNullOrEmpty())
            {
                CourseCollection.Clear();
            }

            await LoadCourseBookmarkCollection(_currentBookmarkType);
        }
    }
}
