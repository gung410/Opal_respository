using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.UserOnBoarding;

namespace LearnerApp.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string cancelTextBtn = "Go back", Action<bool> onClosed = null, bool isVisibleIcon = true);

        Task ConfirmAsync(string message, string cancelTextBtn = "Cancel", string confirmedTextBtn = "Confirm", Action<bool> onConfirmed = null);

        Task ConfirmMessageAsync(string message, string cancelTextBtn = "Cancel", string confirmedTextBtn = "Confirm", bool checkValidate = false, Action<bool, string> onConfirmed = null);

        Task AbsenceMessageAsync(string message, string reason, string attachment, string cancelTextBtn = "Cancel");

        Task ShowDropDownSelectionPopup(Dictionary<string, string> items, int totalNewNotification = 0, bool isFullScreen = false, bool isSeparateStringByUppercase = true, Action<string> onSelected = null);

        LoadingPopupController DisplayLoadingIndicator(int popInMilliseconds = 0);

        Task ShowMyClassRunRejectReasonPopup(UserInformation userInfo, DateTime changedDate, string reason);

        Task ShowAdvancedSearch();

        Task CreateNewLearningPathsPopup(IEnumerable<ItemCard> courseSelectedCollection, Action<IEnumerable<ItemCard>> onSelectedDone = null);

        Task ShowLearningPathsSelectUser(IEnumerable<UserInformation> userCollection, string seachText, Action<IEnumerable<UserInformation>> onConfirmed = null);

        Task ShowBrokenLinkReportPopup(List<string> urls, Action<BrokenLinkReport> onConfirmed = null);

        Task ShowEditCommentPopup(UserReview review, bool isMicroLearningType = true, Action<UserReview> onSaved = null);
    }
}
