using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Connection
{
    public static class ConnectionBuilder 
    {
        public static ConnectionDto GenerateConnectionDto(UserGroupEntity groupEntity,
            List<UGMemberEntity> members = null, int? memberCount=0)
        {
            var connectionDto = new ConnectionDto
            {
                Source = GenerateConnectionSourceDto(groupEntity, memberCount),
                Members = new List<ConnectionMemberDto>(),


            };

            if (members != null && members.Any())
            {

                connectionDto.Members = members.Select(m => GenerateConnectionMemberDto(m, groupEntity.OwnerId)).ToList();
            }
            return connectionDto;
        }

        public static ConnectionSourceDto GenerateConnectionSourceDto(UserGroupEntity groupEntity, int? memberCount = 0, int defaultCustomerId=0)
        {
            var connectionSource = new ConnectionSourceDto
            {
                Identity = new IdentityDto
                {
                    Archetype = groupEntity.ArchetypeId.HasValue ? (ArchetypeEnum)groupEntity.ArchetypeId : ArchetypeEnum.Unknown,
                    ExtId = groupEntity.ExtId,
                    Id = groupEntity.UserGroupId,
                    OwnerId = groupEntity.OwnerId
                },
                Name = groupEntity.Name,
                Description = groupEntity.Description,
                Type = ((GrouptypeEnum)groupEntity.UserGroupTypeId).ConvertToConnectionType(),
                ReferrerArchetype = (ArchetypeEnum?)groupEntity.ReferrerArchetypeId,
                ReferrerToken = groupEntity.ReferrerToken,
                ReferrerResource = groupEntity.ReferrerResource,
                MemberCount = memberCount,
                EntityStatus = new EntityStatusDto
                {
                    EntityVersion = groupEntity.EntityVersion,
                    LastUpdated = groupEntity.LastUpdated,
                    LastUpdatedBy = groupEntity.LastUpdatedBy ?? 0,
                    StatusId = groupEntity.EntityStatusId.HasValue ? (EntityStatusEnum)groupEntity.EntityStatusId : EntityStatusEnum.Unknown,
                    StatusReasonId = groupEntity.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)groupEntity.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                    LastExternallySynchronized = groupEntity.LastSynchronized,
                    Deleted = groupEntity.Deleted.HasValue
                }
            };
            connectionSource.Parent= BuildSourceParent(groupEntity);
            connectionSource.Identity.CustomerId = connectionSource.Parent != null && connectionSource.Parent.CustomerId > 0
                ? connectionSource.Parent.CustomerId
                : defaultCustomerId;


            if (connectionSource.Identity.CustomerId == 0) connectionSource.Identity.CustomerId = defaultCustomerId;
            return connectionSource;
        }

        private static IdentityDto BuildSourceParent(UserGroupEntity groupEntity)
        {
            if (groupEntity.Department != null)
            {
               return new IdentityDto
                {
                    Archetype = groupEntity.Department.ArchetypeId.HasValue ? (ArchetypeEnum)groupEntity.Department.ArchetypeId : ArchetypeEnum.Unknown,
                    ExtId = groupEntity.Department.ExtId,
                    Id = groupEntity.Department.DepartmentId,
                    CustomerId = groupEntity.Department.CustomerId ?? 0,
                    OwnerId = groupEntity.Department.OwnerId
                };

            }
            else if (groupEntity.User != null)
            {
              return new IdentityDto
                {
                    Archetype = groupEntity.User.ArchetypeId.HasValue ? (ArchetypeEnum)groupEntity.User.ArchetypeId : ArchetypeEnum.Unknown,
                    ExtId = groupEntity.User.ExtId,
                    Id = groupEntity.User.UserId,
                    CustomerId = groupEntity.User.CustomerId ?? 0,
                    OwnerId = groupEntity.User.OwnerId
                };

            }
            return null;
        }

        public static ConnectionSourceDto GenerateConnectionSourceDto(UserGroupDtoBase userGroup, IdentityDto parentIdentity)
        {
            return new ConnectionSourceDto
            {
                Identity =userGroup.Identity,
                Parent = parentIdentity,
                Name = userGroup.Name,
                Description = userGroup.Description,
                Type = userGroup.Type.ConvertToConnectionType(),
                ReferrerArchetype = (ArchetypeEnum?)userGroup.ReferrerArchetypeId,
                ReferrerToken = userGroup.ReferrerToken,
                ReferrerResource = userGroup.ReferrerResource,
                EntityStatus = userGroup.EntityStatus
            };
        }
        public static ConnectionMemberDto GenerateConnectionMemberDto(
            UGMemberEntity ugu,
            int ownerId,
            bool includeConnectionSource = false,
            bool shouldHideDateOfBirth = false)
        {
            var user = ugu.User;
            var connectionMember = new ConnectionMemberDto
            {
                Identity = new IdentityDto()
                {
                    Id = ugu.UGMemberId,
                    ExtId = ugu.ExtId,
                    Archetype = ArchetypeEnum.Unknown,
                    CustomerId = ugu.CustomerId ?? 0,
                    OwnerId = ownerId
                },
                MemberRoleId = (MemberRoleEnum?) ugu.MemberRoleId,
                ReferrerToken = ugu.ReferrerToken,
                ReferrerArchetype = (ArchetypeEnum?) ugu.ReferrerArchetypeId,
                ReferrerResource = ugu.ReferrerResource,
                Created = ugu.Created,
                CreatedBy = ugu.CreatedBy ?? 0,
                validFrom = ugu.validFrom,
                ValidTo = ugu.ValidTo,
                PeriodId = ugu.PeriodId,
                DisplayName = ugu.DisplayName,
                EntityStatus = new EntityStatusDto
                {
                    EntityVersion = ugu.EntityVersion,
                    LastUpdated = ugu.LastUpdated ?? DateTime.MinValue,
                    LastUpdatedBy = ugu.LastUpdatedBy ?? 0,
                    StatusId = ugu.EntityStatusId.HasValue ? (EntityStatusEnum) ugu.EntityStatusId.Value : EntityStatusEnum.Unknown,
                    StatusReasonId = ugu.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum) ugu.EntityStatusReasonId.Value : EntityStatusReasonEnum.Unknown,
                    LastExternallySynchronized = ugu.LastSynchronized,
                    Deleted = ugu.Deleted.HasValue
                },

            };
            if (user != null)
            {
                connectionMember.UserIdentity = new IdentityDto
                {
                    Archetype = user.ArchetypeId.HasValue ? (ArchetypeEnum)user.ArchetypeId : ArchetypeEnum.Unknown,
                    ExtId = user.ExtId,
                    Id = user.UserId,
                    CustomerId = user.CustomerId ?? 0,
                    OwnerId = user.OwnerId
                };
                connectionMember.FirstName = string.IsNullOrEmpty(user.FirstName) ? null : user.FirstName;
                connectionMember.LastName = string.IsNullOrEmpty(user.LastName) ? null : user.LastName;
                connectionMember.EmailAddress = string.IsNullOrEmpty(user.Email) ? null : user.Email;
                connectionMember.Gender = user.Gender;
                connectionMember.DateOfBirth = shouldHideDateOfBirth ? null : user.DateOfBirth;
                connectionMember.MobileCountryCode = user.CountryCode;
                connectionMember.MobileNumber = string.IsNullOrEmpty(user.Mobile) ? null : user.Mobile;
            }
            if (includeConnectionSource && ugu.UserGroup != null)
            {
                connectionMember.Source = GenerateConnectionSourceDto(ugu.UserGroup, defaultCustomerId: connectionMember.Identity.CustomerId);
            }
            return connectionMember;
        }
        public static ConnectionMemberDto GenerateConnectionMemberDto(
            MembershipDto ugu,
            UserEntity user,
            bool shouldHideDateOfBirth = false)
        {
            var connectionMember = new ConnectionMemberDto
            {
                Identity = new IdentityDto
                {
                    Id = ugu.Identity.Id,
                    ExtId = ugu.Identity.ExtId,
                    Archetype = ugu.Identity.Archetype,
                    OwnerId = ugu.Identity.OwnerId,
                    CustomerId = ugu.Identity.CustomerId
                },
                MemberRoleId = (MemberRoleEnum?) ugu.MemberRoleId,
                ReferrerToken = ugu.ReferrerToken,
                ReferrerArchetype = (ArchetypeEnum?) ugu.ReferrerArchetypeId,
                ReferrerResource = ugu.ReferrerResource,
                Created = ugu.Created,
                CreatedBy = ugu.CreatedBy ?? 0,
                validFrom = ugu.validFrom,
                ValidTo = ugu.ValidTo,
                PeriodId = ugu.PeriodId,
                DisplayName = ugu.DisplayName,
                EntityStatus = ugu.EntityStatus
            };
            if (user != null)
            {
                connectionMember.UserIdentity = new IdentityDto
                {
                    Archetype = user.ArchetypeId.HasValue ? (ArchetypeEnum) user.ArchetypeId : ArchetypeEnum.Unknown,
                    ExtId = user.ExtId,
                    Id = user.UserId,
                    CustomerId = user.CustomerId ?? 0,
                    OwnerId = user.OwnerId
                };
                connectionMember.FirstName = string.IsNullOrEmpty(user.FirstName) ? null : user.FirstName;
                connectionMember.LastName = string.IsNullOrEmpty(user.LastName) ? null : user.LastName;
                connectionMember.EmailAddress = string.IsNullOrEmpty(user.Email) ? null : user.Email;
                connectionMember.Gender = user.Gender;
                connectionMember.DateOfBirth = shouldHideDateOfBirth ? null : user.DateOfBirth;
                connectionMember.MobileCountryCode = user.CountryCode;
                connectionMember.MobileNumber = string.IsNullOrEmpty(user.Mobile) ? null : user.Mobile;
                
            }
            return connectionMember;
        }

        public static List<ConnectionDto> BuildConnectionDtos(List<UGMemberEntity> ugMemberEntities, List<UserGroupEntity> userGroups, Dictionary<int, int> countMemberGroupByUserGroup,  bool filterOnMember, bool countOnMember, bool includeConnectionHasNoMember)
        {
            var result = new List<ConnectionDto>();
            var ugmemberGroupByUserGroupId = ugMemberEntities == null ? null :
                ugMemberEntities.GroupBy(t => t.UserGroupId).ToDictionary(u => u.Key, u => u.ToList());
            foreach (var userGroupEntity in userGroups)
            {
                int? memberCount = null;
                List<UGMemberEntity> memberEntitiesInUserGroup = null;
                if (ugmemberGroupByUserGroupId != null)
                {
                    if (ugmemberGroupByUserGroupId.ContainsKey(userGroupEntity.UserGroupId))
                    {
                        memberEntitiesInUserGroup = ugmemberGroupByUserGroupId[userGroupEntity.UserGroupId];
                        memberCount = memberEntitiesInUserGroup.Count;
                    }
                }
                else if (countMemberGroupByUserGroup != null)
                {
                    if (countMemberGroupByUserGroup.ContainsKey(userGroupEntity.UserGroupId))
                    {
                        memberCount = countMemberGroupByUserGroup[userGroupEntity.UserGroupId];
                    }
                    
                }
                //If there is filter on member and connection has no any matched member, we skip this connection in result when consumer don't want to include 
                if (!includeConnectionHasNoMember && filterOnMember && (!memberCount.HasValue|| memberCount.Value==0))
                {
                    continue;
                }
                if (countOnMember && memberCount == null)
                {
                    memberCount = 0;
                }
                var connectionDto = ConnectionBuilder.GenerateConnectionDto(userGroupEntity, memberEntitiesInUserGroup, memberCount);
                result.Add(connectionDto);
            }
            return result;
        }
    }
}
