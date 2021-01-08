using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Controls;
using LearnerApp.Controls.MyLearning;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Views;
using Rg.Plugins.Popup.Services;

namespace LearnerApp.Services.Dialog
{
    public class DialogService : IDialogService
    {
        // We expect that DialogService is Singleton instance but somehow it's not. That's why we need static here, need to troubleshoot later.
        private static ConcurrentDictionary<string, DateTime> _messageHistory = new ConcurrentDictionary<string, DateTime>();

        public async Task ShowAlertAsync(string message, string cancelTextBtn, Action<bool> onClosed, bool isVisibleIcon)
        {
            using (var messageStack = new MessageStack(message))
            {
                await messageStack.ShowAlertAsync(message, cancelTextBtn, onClosed, isVisibleIcon);
            }
        }

        public async Task ConfirmAsync(string message, string cancelTextBtn, string confirmedTextBtn, Action<bool> onConfirmed = null)
        {
            using (var messageStack = new MessageStack(message))
            {
                await messageStack.ConfirmAsync(message, cancelTextBtn, confirmedTextBtn, onConfirmed);
            }
        }

        public LoadingPopupController DisplayLoadingIndicator(int popInMilliseconds)
        {
            var popup = new LoadingPopupController();
            popup.Loading(popInMilliseconds);

            return popup;
        }

        public async Task ShowDropDownSelectionPopup(Dictionary<string, string> items, int totalNewNotification, bool isFullScreen, bool isSeparateStringByUppercase = true, Action<string> onSelected = null)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            using (var messageStack = new MessageStack("show-drop-down-selection-popup"))
            {
                await messageStack.ShowDropDownSelectionPopup(items, totalNewNotification, isFullScreen, isSeparateStringByUppercase, onSelected);
            }
        }

        public async Task CreateNewLearningPathsPopup(IEnumerable<ItemCard> courseSelectedCollection, Action<IEnumerable<ItemCard>> onSelectedDone = null)
        {
            MyLearningPathsAddCourse addCourseToLearningPaths = new MyLearningPathsAddCourse(courseSelectedCollection);
            addCourseToLearningPaths.DoneEventHandler += (sender, e) =>
            {
                onSelectedDone?.Invoke(e);
            };
            await PopupNavigation.Instance.PushAsync(addCourseToLearningPaths);
        }

        public async Task ShowMyClassRunRejectReasonPopup(UserInformation userInfo, DateTime changedDate, string reason)
        {
            var popup = new MyClassRunRejectReasonPopup(userInfo, changedDate, reason);

            await PopupNavigation.Instance.PushAsync(popup);
        }

        public async Task ShowAdvancedSearch()
        {
            using (var messageStack = new MessageStack("show-advandce-search"))
            {
                await messageStack.ShowAdvancedSearch();
            }
        }

        public async Task ShowLearningPathsSelectUser(IEnumerable<UserInformation> userCollection, string searchText, Action<IEnumerable<UserInformation>> onConfirmed = null)
        {
            using (var messageStack = new MessageStack("leaerning-paths-select-user"))
            {
                await messageStack.ShowLearningPathsSelectUser(userCollection, searchText, onConfirmed);
            }
        }

        public async Task ConfirmMessageAsync(string message, string cancelTextBtn = "Cancel", string confirmedTextBtn = "Confirm", bool checkValidate = false, Action<bool, string> onConfirmed = null)
        {
            using (var messageStack = new MessageStack(message))
            {
                await messageStack.ConfirmMessageAsync(message, cancelTextBtn, confirmedTextBtn, checkValidate, onConfirmed);
            }
        }

        public async Task AbsenceMessageAsync(string message, string reason, string attachment, string cancelTextBtn = "Cancel")
        {
            using (var messageStack = new MessageStack(message))
            {
                await messageStack.AbsenceMessageAsync(message, reason, attachment, cancelTextBtn);
            }
        }

        public async Task ShowBrokenLinkReportPopup(List<string> urls, Action<BrokenLinkReport> onConfirmed)
        {
            using (var messageStack = new MessageStack("show-broken-link-report"))
            {
                await messageStack.ShowBrokenLinkReportPopup(urls, onConfirmed);
            }
        }

        public async Task ShowEditCommentPopup(UserReview review, bool isMicroLearningType, Action<UserReview> onSaved)
        {
            using (var messageStack = new MessageStack("show-edit-comment"))
            {
                await messageStack.ShowEditCommentPopup(review, isMicroLearningType, onSaved);
            }
        }

        private class MessageStack : IDisposable
        {
            private const int DuplicationCycleTimeInSeconds = 2;
            private readonly string _newMessage;
            private readonly bool _hasDuplicatedMessage;

            public MessageStack(string newMessage)
            {
                _newMessage = newMessage;

                if (_messageHistory != null && _messageHistory.ContainsKey(_newMessage))
                {
                    _messageHistory.TryGetValue(_newMessage, out DateTime value);

                    if ((value.AddSeconds(DuplicationCycleTimeInSeconds) - DateTime.Now).TotalSeconds < DuplicationCycleTimeInSeconds)
                    {
                        _hasDuplicatedMessage = true;

                        return;
                    }
                }

                _hasDuplicatedMessage = false;
                _messageHistory = new ConcurrentDictionary<string, DateTime>();
                _messageHistory.TryAdd(newMessage, DateTime.Now);
            }

            public Task ShowAlertAsync(string message, string cancelTextBtn, Action<bool> onClosed, bool isVisibleIcon)
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                ErrorPopup errorPopup = new ErrorPopup(message, cancelTextBtn, isVisibleIcon);

                errorPopup.CloseEventHandler += (sender, e) =>
                {
                    onClosed?.Invoke(e);
                };

                return PopupNavigation.Instance.PushAsync(errorPopup);
            }

            public Task ConfirmAsync(string message, string cancelTextBtn, string confirmedTextBtn, Action<bool> onConfirmed)
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                ConfirmPopup confirmPopup = new ConfirmPopup(message, cancelTextBtn, confirmedTextBtn);
                confirmPopup.ConfirmedEventHandle += (sender, e) =>
                {
                    onConfirmed?.Invoke(e);
                };

                return PopupNavigation.Instance.PushAsync(confirmPopup);
            }

            public Task ConfirmMessageAsync(string message, string cancelTextBtn, string confirmedTextBtn, bool checkValidate, Action<bool, string> onConfirmed)
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                ConfirmMessagePopup confirmMessagePopup = new ConfirmMessagePopup(message, cancelTextBtn, confirmedTextBtn, checkValidate);
                confirmMessagePopup.CustomEventHandler += (sender, e) =>
                {
                    onConfirmed?.Invoke(e.IsConfirm, e.Reason);
                };

                return PopupNavigation.Instance.PushAsync(confirmMessagePopup);
            }

            public Task AbsenceMessageAsync(string message, string reason, string attachment, string cancelTextBtn)
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                AbsenceMessagePopup absenceMessagePopup = new AbsenceMessagePopup(message, reason, attachment, cancelTextBtn);

                return PopupNavigation.Instance.PushAsync(absenceMessagePopup);
            }

            public Task ShowAdvancedSearch()
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                var popup = new AdvancedSearchView();

                return PopupNavigation.Instance.PushAsync(popup);
            }

            public Task ShowDropDownSelectionPopup(Dictionary<string, string> items, int totalNewNotification, bool isFullScreen, bool isSeparateStringByUppercase, Action<string> onSelected)
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                DropDownSelectionPopup dropDownSelectionPopup = new DropDownSelectionPopup(items, totalNewNotification, isFullScreen, isSeparateStringByUppercase);
                dropDownSelectionPopup.OnItemSelectedEventHandle += (sender, e) =>
                {
                    onSelected?.Invoke(e);
                };

                return PopupNavigation.Instance.PushAsync(dropDownSelectionPopup);
            }

            public Task ShowLearningPathsSelectUser(IEnumerable<UserInformation> userCollection, string searchText, Action<List<UserInformation>> onConfirmed)
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                MyLearningPathsSelectUser myLearningPathsSelectUserViewModel = new MyLearningPathsSelectUser(userCollection, searchText);
                myLearningPathsSelectUserViewModel.OnUsersSelectedEventHandler += (sender, e) =>
                {
                    onConfirmed?.Invoke(e);
                };

                return PopupNavigation.Instance.PushAsync(myLearningPathsSelectUserViewModel);
            }

            public Task ShowBrokenLinkReportPopup(List<string> urls, Action<BrokenLinkReport> onConfirmed)
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                BrokenLinkReportPopup brokenLinkReportPopup = new BrokenLinkReportPopup(urls);
                brokenLinkReportPopup.OnConfirmedEventHandler += (sender, e) =>
                {
                    onConfirmed?.Invoke(e);
                };

                return PopupNavigation.Instance.PushAsync(brokenLinkReportPopup);
            }

            public Task ShowEditCommentPopup(UserReview review, bool isMicroLearningType, Action<UserReview> onSaved)
            {
                if (_hasDuplicatedMessage)
                {
                    return Task.CompletedTask;
                }

                EditCommentPopup editComment = new EditCommentPopup(review, isMicroLearningType);
                editComment.OnSaveEditCommentEventHandler += (sender, e) =>
                {
                    onSaved?.Invoke(e);
                };

                return PopupNavigation.Instance.PushAsync(editComment);
            }

            public void Dispose()
            {
                _messageHistory?.Clear();
            }
        }
    }
}
