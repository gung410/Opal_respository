using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Models.PdCatelogue;
using LearnerApp.Resources.Texts;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Dialog;
using LearnerApp.ViewModels.Base;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public class MyLearningPathsAddCourseViewModel : BaseViewModel
    {
        private readonly IPdCatelogueService _pdCatelogueService;
        private readonly ICommonServices _commonService;
        private readonly IDialogService _dialogService;

        private string _textSearch;
        private string _textSearched;
        private string _emptyListStatus;
        private int _paging = 1;
        private bool _isVisibleSearchResult;

        private ItemCard _itemSelected;

        public MyLearningPathsAddCourseViewModel(IEnumerable<ItemCard> courseSelectedCollection)
        {
            _pdCatelogueService = CreateRestClientFor<IPdCatelogueService>(GlobalSettings.BackendPdCatelogueService);
            _commonService = DependencyService.Resolve<ICommonServices>();
            _dialogService = DependencyService.Resolve<IDialogService>();

            CourseSelected = new List<ItemCard>();

            if (!courseSelectedCollection.IsNullOrEmpty())
            {
                CourseSelected.AddRange(courseSelectedCollection);
            }

            EmptyListStatus = "Loading...";
            IsVisibleSearchResult = false;
        }

        public LearnerObservableCollection<ItemCard> CourseCollection { get; set; }

        public ICommand SearchCommand => new Command(async () => await OnSearch());

        public ICommand DismissSearchCommand => new Command(async () => await OnDismissSearch());

        public List<ItemCard> CourseSelected { get; set; }

        public int TotalCount { get; set; }

        public bool IsVisibleSearchResult
        {
            get
            {
                return _isVisibleSearchResult;
            }

            set
            {
                _isVisibleSearchResult = value;
                RaisePropertyChanged(() => IsVisibleSearchResult);
            }
        }

        public string EmptyListStatus
        {
            get
            {
                return _emptyListStatus;
            }

            set
            {
                _emptyListStatus = value;
                RaisePropertyChanged(() => EmptyListStatus);
            }
        }

        public string TextSearch
        {
            get
            {
                return _textSearch;
            }

            set
            {
                _textSearch = value;
                RaisePropertyChanged(() => TextSearch);
            }
        }

        public string TextSearched
        {
            get
            {
                return _textSearched;
            }

            set
            {
                _textSearched = value;
                RaisePropertyChanged(() => TextSearched);
            }
        }

        public ItemCard ItemSelected
        {
            get
            {
                return _itemSelected;
            }

            set
            {
                _itemSelected = value;

                OnItemSelected();
            }
        }

        public async Task OnLoadmore()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            if (CourseCollection != null)
            {
                _paging++;

                if (IsVisibleSearchResult)
                {
                    await SearchCollection();
                }
                else
                {
                    await GetNewlyAddedCollection();
                }
            }

            IsBusy = false;
        }

        public async Task GetNewlyAddedCollection()
        {
            var newlyAddCourseCards = await _commonService.GetNewlyAddedCollection(page: _paging, totalCount =>
            {
                TotalCount = totalCount;
            });

            if (!newlyAddCourseCards.IsNullOrEmpty())
            {
                if (_paging != 1)
                {
                    CourseCollection.AddRange(newlyAddCourseCards);
                }
                else
                {
                    CourseCollection = new LearnerObservableCollection<ItemCard>(newlyAddCourseCards);
                }
            }

            if (CourseCollection.IsNullOrEmpty())
            {
                EmptyListStatus = TextsResource.NOTHING_HERE_YET;
                CourseCollection = new LearnerObservableCollection<ItemCard>();
            }

            RaisePropertyChanged(() => CourseCollection);
        }

        private async Task OnSearch()
        {
            TextSearched = string.IsNullOrEmpty(TextSearch) ? "Results for" : $"Results for <b>{TextSearch}</b>";

            if (string.IsNullOrEmpty(TextSearch))
            {
                return;
            }

            _paging = 1;
            IsVisibleSearchResult = true;

            await SearchCollection();
        }

        private async Task SearchCollection()
        {
            using (_dialogService.DisplayLoadingIndicator())
            {
                var searchParam = new PdCatelogueSearchFilter
                {
                    ResourceTypesFilter = new string[] { "course", "microlearning" },
                    StatisticResourceTypes = new string[] { "course", "microlearning", "content" },
                    SearchCriteria = new PdSearchCriteria
                    {
                        Tags = new List<string> { "equals" },
                        ResourceType = new List<string> { "contains", "course", "microlearning", "content" },
                        RegistrationMethod = new string[] { "contains", "public", "restricted", "resourceType:course" },
                        StartDate = new string[] { "lte", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), "resourceType:content" },
                        ExpiredDate = new string[] { "gt", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), "resourceType:content" },
                        IsArchived = new string[] { "equals", "false", "resourceType:content" }
                    },
                    SearchText = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(TextSearch),
                    Page = _paging,
                };

                var searchCourses = await ExecuteBackendService(() => _pdCatelogueService.Search(searchParam));

                if (!searchCourses.HasEmptyResult() && searchCourses.Payload.Total > 0)
                {
                    TotalCount = searchCourses.Payload.Total;

                    var resourceTypeGroup = searchCourses.Payload.Resources.GroupBy(p => p.ResourceType);

                    var courseCard = await _commonService.GetItemCardByGroup(resourceTypeGroup);

                    if (_paging != 1)
                    {
                        CourseCollection.AddRange(courseCard);
                    }
                    else
                    {
                        CourseCollection = new LearnerObservableCollection<ItemCard>(courseCard);
                    }
                }
                else
                {
                    EmptyListStatus = TextsResource.NOTHING_HERE_YET;

                    CourseCollection = new LearnerObservableCollection<ItemCard>();
                }

                RaisePropertyChanged(() => CourseCollection);
            }
        }

        private async Task OnDismissSearch()
        {
            _paging = 1;

            await GetNewlyAddedCollection();

            TextSearch = string.Empty;
            IsVisibleSearchResult = false;
        }

        private void OnItemSelected()
        {
            if (!CourseSelected.Exists(p => p.Id == ItemSelected.Id))
            {
                CourseSelected.Add(ItemSelected);

                CrossToastPopUp.Current.ShowToastSuccess("The course has been added successfully");
            }
            else
            {
                CrossToastPopUp.Current.ShowToastWarning("Already added");
            }

            CourseCollection.Remove(ItemSelected);
            RaisePropertyChanged(() => CourseCollection);
        }
    }
}
