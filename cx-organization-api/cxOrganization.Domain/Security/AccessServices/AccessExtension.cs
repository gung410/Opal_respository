
namespace cxOrganization.Domain.Security.AccessServices
{
    public static class AccessExtension
    {
        public static bool IsAllowedAccess(this AccessStatus accessStatus)
        {
            return accessStatus == AccessStatus.AccessGranted;
        }
    }
}
