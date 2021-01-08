using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models.Learner;
using LearnerApp.Models.Sharing;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Sharing
{
    public class SharingContentItemViewModel : BaseViewModel
    {
        private readonly SharingContentItem _sharingContentItem;

        public SharingContentItemViewModel(SharingContentItem dto)
        {
            _sharingContentItem = dto;
        }

        public string Title => _sharingContentItem.Title;

        public string SharedBy
        {
            get
            {
                if (_sharingContentItem.SharedByUsers.IsNullOrEmpty())
                {
                    return null;
                }

                if (_sharingContentItem.SharedByUsers.Count > 1)
                {
                    string suffix;
                    if (_sharingContentItem.SharedByUsers.Count > 2)
                    {
                        suffix = (_sharingContentItem.SharedByUsers.Count - 1) + "others";
                    }
                    else
                    {
                        suffix = "another";
                    }

                    return $"{_sharingContentItem.SharedByUsers.First()} and {suffix}";
                }
                else
                {
                    return _sharingContentItem.SharedByUsers.First();
                }
            }
        }

        public string FileExtension => _sharingContentItem.ThumbnailUrl;

        public List<string> Tags => _sharingContentItem?.Tags ?? new List<string>();

        public bool ShouldShowThumbnail => _sharingContentItem.ItemType == BookmarkType.DigitalContent;

        public ICommand ItemClickedCommand => new Command(ItemClicked);

        private async void ItemClicked()
        {
            switch (_sharingContentItem.ItemType)
            {
                case BookmarkType.Course:
                case BookmarkType.Microlearning:
                    await NavigationService.NavigateToAsync<CourseDetailsViewModel>(
                        CourseDetailsViewModel.GetNavigationParameters(
                            _sharingContentItem.ItemId,
                            _sharingContentItem.ItemType));
                    break;
                case BookmarkType.DigitalContent:
                    await NavigationService.NavigateToAsync<MyDigitalContentDetailsViewModel>(
                        MyDigitalContentDetailsViewModel.GetNavigationParameters(
                            _sharingContentItem.ItemId));
                    break;
            }
        }
    }
}
