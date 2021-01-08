using System;
using System.Collections.Generic;
using System.Linq;
using cxEvent.Client;
using cxOrganization.Business.Extensions;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Business.Connection
{
    public static class ConnectionHelper
    {
        public static GrouptypeEnum ConvertToGroupType(this ConnectionType connectionType)
        {
            //For now, values of ConnectionType and GrouptypeEnum are same, then we can cast directly

            var connectionTypeValue = (int) connectionType;
            return Enum.IsDefined(typeof(GrouptypeEnum), connectionTypeValue) ? (GrouptypeEnum)connectionTypeValue : GrouptypeEnum.Default;
        }
        public static List<GrouptypeEnum> ConvertToGroupTypes(this List<ConnectionType> connectionTypes)
        {
            return connectionTypes == null ? null : connectionTypes.Select(ConvertToGroupType).ToList();
        }
        public static ConnectionType ConvertToConnectionType(this GrouptypeEnum grouptype)
        {
            //For now, values of ConnectionType and GrouptypeEnum are same, then we can cast directly

            var grouptypeEnumValue = (int)grouptype;
            return Enum.IsDefined(typeof(ConnectionType),(int)grouptypeEnumValue) ? (ConnectionType)grouptypeEnumValue : ConnectionType.Corporate;
        }
        public static List<ConnectionType> ConvertToConnectionTypes(this List<GrouptypeEnum> grouptypes)
        {
            return grouptypes == null ? null : grouptypes.Select(ConvertToConnectionType).ToList();
        }

        public static ConnectionTypeConfig GetByConnectionType(this IReadOnlyCollection<ConnectionTypeConfig> connectionConfigs, ConnectionType connectionType)
        {
            return connectionConfigs.FirstOrDefault(c => c.ConnectionType == connectionType) 
                ?? connectionConfigs.FirstOrDefault(c => c.ConnectionType == null) ;///connection config with ConnectionType==null is default
        }

        public static List<string> GetEventTypeNames(this IReadOnlyCollection<ConnectionEventTypeConfig> connectionEventTypeConfigs, EntityStatusEnum? fromStatus, EntityStatusEnum toStatus)
        {
            return connectionEventTypeConfigs.Where(
               c =>
                   c.FromStatus == fromStatus && c.ToStatus == toStatus &&
                   !String.IsNullOrEmpty(c.EventTypeName)).Select(c => c.EventTypeName).Distinct().ToList();
        }
        public static List<string> GetEventTypeNames(this ConnectionTypeConfig connectionTypeConfig, EntityStatusEnum? fromStatus, EntityStatusEnum toStatus)
        {
            if (connectionTypeConfig != null && connectionTypeConfig.EventTypeConfigs != null)
            {
                return connectionTypeConfig.EventTypeConfigs.GetEventTypeNames(fromStatus, toStatus); 
                
            }
            return new List<string>();
        }

        public static void EnsureConnectionMemberValuesForUpdating(ConnectionMemberDto connectionMemberDto, UGMemberEntity existingUgMemberEntity)
        {
            var isRemovingConnection = IsRemovingConnectioMember(connectionMemberDto, (EntityStatusEnum?)existingUgMemberEntity.EntityStatusId);

            if(isRemovingConnection)
            {
                connectionMemberDto.validFrom = existingUgMemberEntity.validFrom;
                connectionMemberDto.ValidTo = DateTime.Now;
            }

            connectionMemberDto.EntityStatus = connectionMemberDto.EntityStatus ?? new EntityStatusDto();
            if (connectionMemberDto.UserIdentity != null)
            {
                connectionMemberDto.UserIdentity.Id = existingUgMemberEntity.UserId;
                if (existingUgMemberEntity.User != null)
                {
                    connectionMemberDto.UserIdentity.ExtId = existingUgMemberEntity.User.ExtId;
                }
            }
            if (connectionMemberDto.Identity == null) connectionMemberDto.Identity = new IdentityDto();
            connectionMemberDto.Identity.Id = existingUgMemberEntity.UGMemberId;

            connectionMemberDto.CreatedBy = existingUgMemberEntity.CreatedBy ?? 0;
            connectionMemberDto.Created = existingUgMemberEntity.Created ?? DateTime.MinValue;
            connectionMemberDto.EntityStatus.EntityVersion = connectionMemberDto.EntityStatus.EntityVersion ?? existingUgMemberEntity.EntityVersion;
            connectionMemberDto.EntityStatus.Deleted = existingUgMemberEntity.Deleted.HasValue;
            connectionMemberDto.EntityStatus.LastUpdated = connectionMemberDto.EntityStatus.LastUpdated <= existingUgMemberEntity.LastUpdated
                ? DateTime.Now :
                connectionMemberDto.EntityStatus.LastUpdated;
        }

        public static MembershipDto BuildUserMembership(int ownerId, int customerId, int connectionSourceId,
            ConnectionMemberDto connectionMemberDto, IdentityDto updatedByIdentity, int currentUserId,
            EntityStatusEnum defaultStatus = EntityStatusEnum.Active)
        {
            var userMembership = new MembershipDto
            {
                GroupId = connectionSourceId,
                Identity = new IdentityDto()
                {
                    OwnerId = ownerId,
                    CustomerId = customerId,
                    Id = connectionMemberDto.Identity == null || connectionMemberDto.Identity.Id<=0 ? null : connectionMemberDto.Identity.Id,
                },
                MemberRoleId = (int?) connectionMemberDto.MemberRoleId,
                ReferrerArchetypeId = (int?) connectionMemberDto.ReferrerArchetype,
                ReferrerToken = connectionMemberDto.ReferrerToken,
                ReferrerResource = connectionMemberDto.ReferrerResource,
                validFrom = connectionMemberDto.validFrom,
                ValidTo = connectionMemberDto.ValidTo,
                EntityStatus = connectionMemberDto.EntityStatus,
                Created = connectionMemberDto.Created,
                CreatedBy = connectionMemberDto.CreatedBy,
                PeriodId = connectionMemberDto.PeriodId
            };
            if (connectionMemberDto.IsUserMember())
            {
                userMembership.MemberId = (int?) connectionMemberDto.UserIdentity.Id;
                userMembership.Identity.Archetype = connectionMemberDto.UserIdentity.Archetype;

            }
            if (updatedByIdentity != null && updatedByIdentity.OwnerId == ownerId)
            {
                userMembership.EntityStatus.LastUpdatedBy = (int) updatedByIdentity.Id.Value;
            }
            EnsureEntityStatusValue(userMembership, currentUserId, defaultStatus);
            return userMembership;
        }

        public static UserGroupDtoBase GenerateConnectionSourceAsUserGroup(GrouptypeEnum groupType, ConnectionSourceDto connectionSource,
            IdentityDto updatedByIdentity, int currentUserId, EntityStatusEnum defaultStatus = EntityStatusEnum.Active)
        {
            int? parentDepartmentId = null;
            int? parentUserId = null;
            if (connectionSource.Parent.Archetype.IsDepartmentArchetype())
            {
                parentDepartmentId = (int?)connectionSource.Parent.Id; }
            else if (connectionSource.Parent.Archetype.IsUserArchetype())
            {
                parentUserId = (int?)connectionSource.Parent.Id;
            }

            UserGroupDtoBase userGroup;
            switch (connectionSource.Identity.Archetype)
            {
                case ArchetypeEnum.TeachingGroup:
                    userGroup = new TeachingGroupDto
                    {
                        SchoolId = parentDepartmentId
                    };
                    break;

                default:
                    userGroup = new CandidatePoolDto()
                    {
                        ParentDepartmentId = parentDepartmentId,
                        ParentUserId = parentUserId
                    };
                    break;
            }
            userGroup.Identity = connectionSource.Identity;
            userGroup.Description = connectionSource.Description;
            userGroup.Name = connectionSource.Name;
            userGroup.ReferrerArchetypeId = (int?) connectionSource.ReferrerArchetype;
            userGroup.ReferrerToken = connectionSource.ReferrerToken;
            userGroup.ReferrerResource = connectionSource.ReferrerResource;
            userGroup.Type = groupType;
            userGroup.EntityStatus = connectionSource.EntityStatus ?? new EntityStatusDto();

            if (updatedByIdentity != null && updatedByIdentity.OwnerId == connectionSource.Identity.OwnerId)
            {
                userGroup.EntityStatus.LastUpdatedBy = (int) updatedByIdentity.Id.Value;
            }

            EnsureEntityStatusValue(userGroup, currentUserId, defaultStatus);

            return userGroup;
        }

        public static void EnsureEntityStatusValue(ConexusBaseDto dto, int currentUserId,  EntityStatusEnum defaultStatus = EntityStatusEnum.Active)
        {
            if ( dto.EntityStatus.StatusId <= 0)  dto.EntityStatus.StatusId = defaultStatus;
            if ( dto.EntityStatus.StatusReasonId <= 0)  dto.EntityStatus.StatusReasonId =  dto.EntityStatus.StatusId.GetEntityStatusReasonEnum();
            if ( dto.EntityStatus.LastUpdatedBy <= 0)  dto.EntityStatus.LastUpdatedBy = currentUserId;
            if (dto.EntityStatus.LastUpdated == DateTime.MinValue) dto.EntityStatus.LastUpdated = DateTime.Now;
        }

        public static void EnsureConnectionMemberValuesForInserting(int ownerId, ConnectionMemberDto connectionMemberDto, UserEntity userEntity, 
            IdentityDto updatedByIdentity, int currentUserId)
        {
            connectionMemberDto.EntityStatus = connectionMemberDto.EntityStatus ?? new EntityStatusDto();
            if (userEntity != null)
            {
                connectionMemberDto.UserIdentity.Id = userEntity.UserId;
            }

            if (connectionMemberDto.Identity != null) connectionMemberDto.Identity.Id = null;

            if (updatedByIdentity != null && updatedByIdentity.OwnerId == ownerId)
            {
                connectionMemberDto.CreatedBy = (int) updatedByIdentity.Id;
            }
            if (connectionMemberDto.CreatedBy <= 0)
            {
                connectionMemberDto.CreatedBy = connectionMemberDto.EntityStatus.LastUpdatedBy <= 0 ?
                    currentUserId : connectionMemberDto.EntityStatus.LastUpdatedBy;
            }
            connectionMemberDto.Created = connectionMemberDto.Created ?? DateTime.Now;
            connectionMemberDto.validFrom = connectionMemberDto.validFrom ?? (connectionMemberDto.EntityStatus == null || connectionMemberDto.EntityStatus.StatusId != EntityStatusEnum.Pending ? DateTime.Now : connectionMemberDto.validFrom);
        }

        public static BusinessEventDto GenerateBusinessEventDto(IRequestContext requestContext, string eventTypeName, IdentityBaseDto userIdentity, 
            IdentityBaseDto groupIdentity, IdentityBaseDto departmentIdentity, object additionInfo, IdentityDto updatedByDto)
        {
            var bussinessEvent = new BusinessEventDto()
            {
                EventTypeName = eventTypeName,
                Identity = new IdentityDto()
                {
                    OwnerId = requestContext.CurrentOwnerId,
                    CustomerId = requestContext.CurrentCustomerId,
                    Archetype = ArchetypeEnum.BusinessEvent
                },
                GroupIdentity = groupIdentity,
                DepartmentIdentity = departmentIdentity,
                UserIdentity = userIdentity,
                CreatedDate = DateTime.Now,
                ApplicationName = requestContext.ApplicationName,
                AdditionalInformation = additionInfo,
                EntityStatus = new EntityStatusDto()
                {
                    StatusId = EntityStatusEnum.Active,
                    StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource,
                    LastExternallySynchronized = DateTime.Now,
                    LastUpdated = DateTime.Now,
                }
            };
            if(updatedByDto != null && updatedByDto.OwnerId== bussinessEvent.Identity.OwnerId)
            {
                bussinessEvent.CreatedBy = (int) updatedByDto.Id.Value;

            }
            else
            {
                bussinessEvent.CreatedBy = requestContext.CurrentUserId;
            }
            return bussinessEvent;
        }

        public static object BuildEventAdditionalInformation(ConnectionType connectionType, List<string> referrerInfos)
        {
            var referrer = referrerInfos ?? new List<string>();
            var additionalInformation = new
            {
                referrer,
                connectionType = connectionType.ToString()
              
            };
            return additionalInformation;
        }

        public static List<BusinessEventDto> GenerateBusinessEventDto(IRequestContext requestContext, IdentityDto connectionSourceIdentity,
            IdentityBaseDto connectionMemberIdentity, IdentityBaseDto userDepartmentIdentity, IdentityDto lastUpdatedByIdentity,
            ConnectionType connectionType, List<string> eventTypeNames, List<string> referrerInfos)
        {
          

            var additionInfo = BuildEventAdditionalInformation(connectionType, referrerInfos);

            var businessEvents = new List<BusinessEventDto>();
            foreach (var eventTypeName in eventTypeNames)
            {
                var bussinessEvent = GenerateBusinessEventDto(requestContext, eventTypeName,
                    connectionMemberIdentity, connectionSourceIdentity, userDepartmentIdentity, additionInfo, lastUpdatedByIdentity);
                businessEvents.Add(bussinessEvent);

            }

            return businessEvents;
        }

        public static bool IsRemovingConnectioMember(ConnectionMemberDto connectionMemberDto, EntityStatusEnum? oldStatus)
        {
            //If old status is Pending, and new status is removed status, we are rejecting connection, it's not a removal.
            return connectionMemberDto.EntityStatus != null
                && IsRemovedStatus(connectionMemberDto.EntityStatus.StatusId)
                && !IsRemovedStatus(oldStatus) 
                && !IsPendingStatus(oldStatus);
            
        }
        public static bool IsRemovedStatus(EntityStatusEnum? statusEnum)
        {
            return statusEnum == EntityStatusEnum.Deactive;
        }

        public static bool IsPendingStatus(EntityStatusEnum? statusEnum)
        {
            return statusEnum == EntityStatusEnum.Pending;
        }
    }
}
