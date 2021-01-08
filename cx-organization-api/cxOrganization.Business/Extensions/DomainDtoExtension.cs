using System.Collections.Generic;
using System.Linq;
using System.Text;
using cxOrganization.Business.Common;
using cxPlatform.Client.ConexusBase;


namespace cxOrganization.Business.Extensions
{
    public static class DomainDtoExtension
    {

        public static bool IsDepartmentArchetype(this ArchetypeEnum archetype)
        {
            return DomainDefinition.IsDepartmentArchetype(archetype);

        }

        public static bool IsUserGroupArchetype(this ArchetypeEnum archetype)
        {
            return DomainDefinition.IsUserGroupArchetype(archetype);
        }

        public static bool IsUserArchetype(this ArchetypeEnum archetype)
        {
            return DomainDefinition.IsUserArchetype(archetype);

        }

        public static string GetResourceName(this ArchetypeEnum archetype)
        {
            return DomainDefinition.GetResourceName(archetype);

        }

        public static List<int> ToInts(this List<ArchetypeEnum> archetypes)
        {
            return archetypes == null ? null : archetypes.Cast<int>().ToList();
        }

        public static EntityStatusReasonEnum GetEntityStatusReasonEnum(this EntityStatusEnum entityStatus)
        {
            switch (entityStatus)
            {
                case EntityStatusEnum.Active:
                    return EntityStatusReasonEnum.Active_None;
                case EntityStatusEnum.Deactive:
                    return EntityStatusReasonEnum.Deactive_ManuallySetDeactive;
                case EntityStatusEnum.Inactive:
                    return EntityStatusReasonEnum.Inactive_ManuallySetInactive;
                case EntityStatusEnum.Pending:
                    return EntityStatusReasonEnum.Pending_AllowLoginWithOTP;
            }
            return EntityStatusReasonEnum.Unknown;
        }

        public static string ToReferrerString(this IdentityDto identityDto)
        {
            if (identityDto != null)
            {
                var referrBuilder = new StringBuilder();
                if (identityDto.Id > 0)
                {
                    referrBuilder.AppendFormat("cxtoken={0}:{1}&resource=/{2}/{3}",
                        identityDto.OwnerId, identityDto.CustomerId,
                        identityDto.Archetype.GetResourceName(),
                        identityDto.Id);
                }
                else
                {
                    referrBuilder.AppendFormat("cxtoken={0}:{1}&resource=/{2}/extId/{3}",
                        identityDto.OwnerId, identityDto.CustomerId,
                        identityDto.Archetype.GetResourceName(), identityDto.ExtId);

                }
                return referrBuilder.ToString();
            }
            return null;
        }

        public static string ToStringInfo(this IdentityDto identityDto, bool includeOwnerCustomer = false)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} with", identityDto.Archetype);
            if (identityDto.Id > 0)
                stringBuilder.AppendFormat(" id {0},", identityDto.Id);
            if (!string.IsNullOrEmpty(identityDto.ExtId))
                stringBuilder.AppendFormat(" extId {0},", identityDto.ExtId);

            return includeOwnerCustomer
                ? string.Format("{0} in customer {1}, owner {2}", stringBuilder.ToString().TrimEnd(','), identityDto.OwnerId, identityDto.CustomerId) :
                stringBuilder.ToString().TrimEnd(',');
        }

        public static string ToStringInfo(this ConexusBaseDto dto, bool includeOwnerCustomer = false)
        {
            return dto.Identity.ToStringInfo(includeOwnerCustomer);
        }
    }
}
