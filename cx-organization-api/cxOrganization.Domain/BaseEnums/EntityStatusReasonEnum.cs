namespace cxPlatform.Client.ConexusBase
{
    public enum EntityStatusReasonEnum
    {
        Unknown = 0,
        //Active = 1,
        Active_None = 100,
        Active_SynchronizedFromSource = 101,
        Active_ManuallySetActive = 102,
        //Inactive = 2,
        Inactive_SynchronizedFromSource = 200,
        Inactive_NotFoundInSource = 201,
        Inactive_ManuallySetInactive = 202,
        //Deactive = 3
        Deactive_SynchronizedFromSource = 301,
        Deactive_NotFoundInSource = 302,
        Deactive_ManuallySetDeactive = 303,
        Deactive_ResultIsDeletedBecauseTheUserIsDeleted = 307,
        Deactive_ResultIsDeletedBecauseOld = 308,
        //Pending =4
        Pending_AllowLoginwithOTP = 401
    }
}
