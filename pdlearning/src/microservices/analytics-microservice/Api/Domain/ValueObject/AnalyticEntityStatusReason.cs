namespace Microservice.Analytics.Domain.ValueObject
{
    public enum AnalyticEntityStatusReason
    {
        Unknown = 0,
        Active_None = 100,
        Active_SynchronizedFromSource = 101,
        Active_ManuallySetActive = 102,
        Inactive_SynchronizedFromSource = 200,
        Inactive_NotFoundInSource = 201,
        Inactive_ManuallySetInactive = 202,
        Inactive_Automatically_Inactivity = 204,
        Inactive_Manually_Absence = 205,
        Inactive_Manually_Retirement = 206,
        Inactive_Manually_Resignation = 207,
        Inactive_Manually_Termination = 208,
        Inactive_Manually_LeftWithoutNotice = 209,
        Inactive_SynchronizedFromSource_Absence = 210,
        Inactive_SynchronizedFromSource_Retirement = 211,
        Inactive_SynchronizedFromSource_Resignation = 212,
        Inactive_SynchronizedFromSource_Termination = 213,
        Inactive_SynchronizedFromSource_LeftWithoutAdvanceNotice = 214,
        Deactive_SynchronizedFromSource = 301,
        Deactive_NotFoundInSource = 302,
        Deactive_ManuallySetDeactive = 303,
        Deactive_SetByRelatedObject = 304,
        Deactive_None = 305,
        Deactive_SetByDisconnectedObject = 306,
        Deactive_ResultIsDeletedBecauseTheUserIsDeleted = 307,
        Deactive_ResultIsDeletedBecauseOld = 308,
        Deactive_Duplicated = 309,
        Deactive_ManuallyRejected = 310,
        Deactive_SynchronizedFromSource_Rejected = 311,
        Deactive_ManuallyArchived = 312,
        Deactive_AutomaticallySetDeactive = 313,
        Deactive_SynchronizedFromSource_Archived = 314,
        Deactive_AutomaticallyArchived = 315,
        Deactive_ManuallyRejected_1stLevel = 317,
        Deactive_ManuallyRejected_2ndLevel = 318,
        Deactive_ManuallyRejected_3rdLevel = 319,
        Pending_AllowLoginWithOTP = 401,
        Hidden_SpecicalRegistration = 501,
        Archived_ManuallyArchived = 1201
    }
}
