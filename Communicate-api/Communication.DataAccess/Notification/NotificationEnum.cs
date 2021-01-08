using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.DataAccess.Notification
{
    public enum NotificationType
    {
        Default,
        PushNotification,
        Email,
        Banner
    }
    public enum NotificationPurpose
    {
        AccountDeleted,
        AccountDeletedEmail,
        AccountEmailChange,
        AccountStatusChange,
        AccountSuspended,
        ActivateUnlockEmail,
        ActivateUserEmail,
        ApprovalProcessEmail,
        ApprovedUserForActiveEmail,
        ChangeStatusUserEmail,
        CreateAccountManually,
        SAMDownloadExportFile,
        DownloadExportUser,
        DownloadExportUserEventLog,
        DownloadFile,
        EmailChangedConfirmation,
        FileDownload,
        MannuallyCreatedUserEmail,
        RejectUserEmail,
        ResetPassword,
        ResetPasswordEmail,
        SendToNewEmailAddressEmail,
        SendToNewEmailAddressEmailV2,
        SendToOldEmailAddressEmail,
        SendToOldEmailAddressEmailV2,
        SignUpEmail,
        SuspendUserEmail,
        SystemErrorReport,
        SystemNotification,
        UnlockUserEmail,
        WelcomeEmail,
        WelcomeEmail_Learner,
        WelcomeEmail_NonLearner,


        CAOApprovedClassrunCancelation,
        CAOApprovedClassrunReschedulation,
        ClassrunForNonMicroPublishedNotifyCollaborator,
        CourseAdminRejectLearnerRegistration,
        CourseAdminSendOfferWaitListToRegistration,
        CourseMLUApprovedNotifyCollaborator,
        CoursePublishedNotifyLearner,
        CourseRequestApprovalApproved,
        CourseRequestApprovalRejected,
        CourseUpdatedRequestApproval,
        LearnerAcceptPlaceOffer,
        LearnerDeclinePlaceOffer,
        LearnerInWaitListClassRun,
        LearnerRegistrationClassRun,
        LearnerRegistrationClassRunConfirm,
        LearnerWithdrawClassrunPending,
        NewCourseRequestApproval,

        ContentExpiredSystemAlert,
        ContentGoingExpiredSystemAlert,

        ApprovedLearningDirection,
        ApprovedLearningNeed,
        ApprovedLearningPlan,
        ApprovedPDPlan,
        PDPMDownloadExportFile,
        RejectedLearningDirection,
        RejectedLearningNeed,
        RejectedLearningPlan,
        RejectedPDPlan,
        RemindReviewLearningDirection,
        RemindReviewLearningPlan,
        RemindStartLearningPlanCycle,
        RemindSubmitLearningPlan,
        SubmittedLearningDirection,
        SubmittedLearningNeed,
        SubmittedLearningPlan,
        SubmittedPDPlan
    }
}
