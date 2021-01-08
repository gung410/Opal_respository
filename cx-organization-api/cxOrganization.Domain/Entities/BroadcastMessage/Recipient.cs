using System;
using System.Collections.Generic;
using System.Text.Json;
using cxOrganization.Domain.Entities.BaseValueObject;

namespace cxOrganization.Domain.Entities.BroadcastMessage
{
    public class Recipient : ValueObject<Recipient>
    {
        public IEnumerable<int> DepartmentIds { get; private set; }

        public IEnumerable<int> RoleIds { get; private set; }

        public IEnumerable<Guid> UserIds { get; private set; }

        public IEnumerable<int> GroupIds { get; private set; }

        public Recipient(IEnumerable<int> departmentIds, IEnumerable<int> roleIds, IEnumerable<Guid> userIds, IEnumerable<int> groupIds)
        {
            DepartmentIds = departmentIds;
            RoleIds = roleIds;
            UserIds = userIds;
            GroupIds = groupIds;
        }

        /// <summary>
        /// Compare two Recipients.
        /// Logic to compare equal value object MUST belong to the derived class but not in the base.
        /// </summary>
        /// <param name="anotherRecipient"></param>
        /// <returns></returns>
        protected override bool EqualsCore(Recipient anotherRecipient)
        {
            if (anotherRecipient is null)
            {
                return false;
            }
            return JsonSerializer.Serialize(this) == JsonSerializer.Serialize(anotherRecipient);
        }

        protected override int GetHashCodeCore()
        {
            return (UserIds.GetHashCode() * 2) ^ RoleIds.GetHashCode();
        }
    }
}
