using System;

namespace cxOrganization.Domain.Enums
{
    public enum EventType
    {
       [Obsolete("Please use type <archetype>_CREATED instead of")]
        USER_CREATED,
       [Obsolete("Please use type <archetype>_UPDATED instead of")]
        USER_UPDATED,
        USERNAME_CHANGED,
        RESET_PASSWORD,
        PASSWORD_CHANGED = 31,
        USER_DELETED,
        USER_MOVED,
        SET_NEW_LEADER,
        DEPARTMENT,
       [Obsolete("Please use type <archetype>_CREATED instead of")]
        DEPARTMENT_CREATED,
       [Obsolete("Please use type <archetype>_UPDATED instead of")]
        DEPARTMENT_UPDATED,
        DEPARTMENT_DELETED,
        DEPARTMENT_MOVED,
       [Obsolete("Please use type <archetype>_CREATED instead of")]
        USERGROUP_CREATED,
       [Obsolete("Please use type <archetype>_UPDATED instead of")]
        USERGROUP_UPDATED,
        CREATED,
        UPDATED,
        ENTITYSTATUS_CHANGED,
        MOVED,
        USER_MEMBERSHIP_CREATED,
        USER_MEMBERSHIP_DELETED
    }
}
