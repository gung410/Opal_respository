
namespace cxOrganization.Adapter.Shared.Entity
{
    public enum EntityStatusEnum
    {
        Unknown = 0,
        Active = 1,
        Inactive = 2,
        Deactive = 3,
        Pending = 4,
        PendingApproval1st = 5,
        PendingApproval2nd = 6,
        PendingApproval3rd = 7,
        New = 8,
        IdentityServerLocked = 9,
        Hidden = 10,
        Rejected = 11,
        All = 99
    }
}
