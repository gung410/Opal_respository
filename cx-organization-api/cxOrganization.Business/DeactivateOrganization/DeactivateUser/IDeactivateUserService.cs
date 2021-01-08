
namespace cxOrganization.Business.DeactivateOrganization.DeactivateUser
{
    public interface IDeactivateUserService<TUser>
    {
        DeactivateUsersResultDto Deactivate(DeactivateUsersDto deactivateUsersDto);
    }
}