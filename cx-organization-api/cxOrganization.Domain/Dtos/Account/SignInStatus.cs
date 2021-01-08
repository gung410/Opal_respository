
namespace cxOrganization.Client.Account
{
    public enum SignInStatus 
    {
        Success = 0,
        Failure = 1,
        LockedOut = 2,
        RequiresVerification = 3,
        InvalidUsername = 4,
    }
}
