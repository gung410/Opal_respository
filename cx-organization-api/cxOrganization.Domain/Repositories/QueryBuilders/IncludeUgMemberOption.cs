namespace cxOrganization.Domain.Repositories.QueryBuilders
{
    public enum IncludeUgMemberOption
    {
        None,
        UgMember,
        UserGroup,
        UserGroupUser,
        UgMemberUser,
        UserGroup_UgMemberUser

    }
}