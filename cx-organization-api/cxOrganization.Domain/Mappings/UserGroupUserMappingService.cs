using System;
using cxOrganization.Client;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Mappings
{
    /// <summary>
    /// User group user mapping service
    /// </summary>
    public class UserGroupUserMappingService : IUserGroupUserMappingService
    {
        private readonly IAdvancedWorkContext _workContext;
        public UserGroupUserMappingService(IAdvancedWorkContext workContext)
        {
            _workContext = workContext;
        }
        /// <summary>
        /// To user group user entity 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberDto"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public UGMemberEntity ToUGMemberEntity(int? userId, MemberDto memberDto, UGMemberEntity entity)
        {
            UGMemberEntity userGroupUserEntity = new UGMemberEntity(); 
            if (entity != null)
                userGroupUserEntity = entity;
            if (userGroupUserEntity.UGMemberId <= 0) //for creating new entity
            {
                userGroupUserEntity.Created = memberDto.Created ?? DateTime.Now;
                userGroupUserEntity.CreatedBy = memberDto.CreatedBy.HasValue ? memberDto.CreatedBy : _workContext.CurrentUserId;
            }
            userGroupUserEntity.CustomerId = memberDto.Identity.CustomerId != default(int) ? memberDto.Identity.CustomerId : _workContext.CurrentCustomerId;
            userGroupUserEntity.EntityStatusId = (int?)memberDto.EntityStatus.StatusId;
            userGroupUserEntity.EntityStatusReasonId = (int?)memberDto.EntityStatus.StatusReasonId;
            userGroupUserEntity.MemberRoleId = memberDto.MemberRoleId;
            userGroupUserEntity.LastUpdated = memberDto.EntityStatus.LastUpdated != default(DateTime) ? memberDto.EntityStatus.LastUpdated : DateTime.Now;
            userGroupUserEntity.LastUpdatedBy = memberDto.EntityStatus.LastUpdatedBy != default(int) ? memberDto.EntityStatus.LastUpdatedBy : _workContext.CurrentUserId;
            userGroupUserEntity.UserGroupId = (int)memberDto.Identity.Id;
            userGroupUserEntity.UserId = userId;
            userGroupUserEntity.validFrom = memberDto.validFrom;
            userGroupUserEntity.ValidTo = memberDto.ValidTo;
            userGroupUserEntity.ExtId = memberDto.Identity.ExtId ?? string.Empty;
            userGroupUserEntity.ReferrerArchetypeId = memberDto.ReferrerArchetypeId;
            userGroupUserEntity.ReferrerResource = memberDto.ReferrerResource ?? string.Empty;
            userGroupUserEntity.ReferrerToken = memberDto.ReferrerToken ?? string.Empty;
            userGroupUserEntity.LastSynchronized = memberDto.EntityStatus.LastExternallySynchronized.HasValue ? memberDto.EntityStatus.LastExternallySynchronized.Value : DateTime.Now;
            userGroupUserEntity.PeriodId = memberDto.PeriodId;
            userGroupUserEntity.DisplayName = memberDto.DisplayName;

            return userGroupUserEntity;
        }
        /// <summary>
        /// Map to memberDto
        /// </summary>
        /// <param name="ugMemberEntity"></param>
        /// <param name="archetype"></param>
        /// <returns></returns>
        public MemberDto ToMemberDto(UGMemberEntity ugMemberEntity, ArchetypeEnum archetype)
        {
            MemberDto memberDto = new MemberDto();
            memberDto.Created = ugMemberEntity.Created;
            memberDto.CreatedBy = ugMemberEntity.CreatedBy;
            memberDto.validFrom = ugMemberEntity.validFrom;
            memberDto.ValidTo = ugMemberEntity.ValidTo;
            memberDto.MemberRoleId = ugMemberEntity.MemberRoleId;
            memberDto.ReferrerToken = ugMemberEntity.ReferrerToken;
            memberDto.ReferrerResource = ugMemberEntity.ReferrerResource;
            memberDto.ReferrerArchetypeId = ugMemberEntity.ReferrerArchetypeId;
            memberDto.DisplayName = ugMemberEntity.DisplayName;
            memberDto.PeriodId = ugMemberEntity.PeriodId;
            memberDto.UserGroupMemberId = ugMemberEntity.UGMemberId;
            memberDto.Identity = new IdentityDto()
            {
                CustomerId = ugMemberEntity.CustomerId.HasValue ? ugMemberEntity.CustomerId.Value : _workContext.CurrentCustomerId,
                Id = ugMemberEntity.UserGroupId,
                OwnerId = _workContext.CurrentOwnerId,
                ExtId = ugMemberEntity.ExtId,
                Archetype = archetype
            };
            memberDto.EntityStatus = new EntityStatusDto()
            {
                EntityVersion = ugMemberEntity.EntityVersion,
                LastUpdated = ugMemberEntity.LastUpdated.HasValue ? ugMemberEntity.LastUpdated.Value : default(DateTime),
                LastUpdatedBy = ugMemberEntity.LastUpdatedBy.HasValue ? (int)ugMemberEntity.LastUpdatedBy : default(int),
                StatusId = ugMemberEntity.EntityStatusId.HasValue ? (EntityStatusEnum)ugMemberEntity.EntityStatusId : EntityStatusEnum.Unknown,
                StatusReasonId = ugMemberEntity.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)ugMemberEntity.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                LastExternallySynchronized = ugMemberEntity.LastSynchronized
            };
            return memberDto;
        }

        /// <summary>
        /// map to entity
        /// </summary>
        /// <param name="membershipDto"></param>
        /// <param name="ugMemberEntity"></param>
        /// <returns></returns>
        public UGMemberEntity ToUGMemberEntity(MembershipDto membershipDto, UGMemberEntity ugMemberEntity = null)
        {
            if (ugMemberEntity == null)
                ugMemberEntity = new UGMemberEntity();

            if (ugMemberEntity.UGMemberId <= 0) //for creating new entity
            {
                ugMemberEntity.Created = membershipDto.Created ?? DateTime.Now;
                ugMemberEntity.CreatedBy = membershipDto.CreatedBy.HasValue ? membershipDto.CreatedBy : _workContext.CurrentUserId;
            }
            ugMemberEntity.CustomerId = membershipDto.Identity.CustomerId > 0 ? membershipDto.Identity.CustomerId : _workContext.CurrentCustomerId;
            ugMemberEntity.EntityStatusId = (int)membershipDto.EntityStatus.StatusId;
            ugMemberEntity.EntityStatusReasonId = (int)membershipDto.EntityStatus.StatusReasonId;
            ugMemberEntity.MemberRoleId = membershipDto.MemberRoleId;
            ugMemberEntity.LastUpdated = membershipDto.EntityStatus.LastUpdated != default(DateTime) ? membershipDto.EntityStatus.LastUpdated : DateTime.Now;
            ugMemberEntity.LastUpdatedBy = membershipDto.EntityStatus.LastUpdatedBy > 0 
                ? (int?)membershipDto.EntityStatus.LastUpdatedBy : null;
            ugMemberEntity.UserGroupId = membershipDto.GroupId;
            ugMemberEntity.UserId = membershipDto.MemberId;
            ugMemberEntity.validFrom = membershipDto.validFrom;
            ugMemberEntity.ValidTo = membershipDto.ValidTo;
            ugMemberEntity.LastSynchronized = membershipDto.EntityStatus.LastExternallySynchronized.HasValue 
                ? membershipDto.EntityStatus.LastExternallySynchronized.Value : DateTime.Now;
            ugMemberEntity.ExtId = membershipDto.Identity.ExtId ?? string.Empty;
            ugMemberEntity.ReferrerArchetypeId = membershipDto.ReferrerArchetypeId;
            ugMemberEntity.ReferrerResource = membershipDto.ReferrerResource ?? string.Empty;
            ugMemberEntity.ReferrerToken = membershipDto.ReferrerToken ?? string.Empty;
            ugMemberEntity.DisplayName = membershipDto.DisplayName ?? string.Empty;
            return ugMemberEntity;
        }
        /// <summary>
        /// Map to membership dto
        /// </summary>
        /// <param name="userGroupUserEntity"></param>
        /// <returns></returns>
        public MembershipDto ToMembershipDto(UGMemberEntity userGroupUserEntity)
        {
            MembershipDto membershipDto = new MembershipDto();
            membershipDto.Created = userGroupUserEntity.Created;
            membershipDto.CreatedBy = userGroupUserEntity.CreatedBy;
            membershipDto.validFrom = userGroupUserEntity.validFrom;
            membershipDto.ValidTo = userGroupUserEntity.ValidTo;
            membershipDto.MemberId = userGroupUserEntity.UserId;
            membershipDto.GroupId = userGroupUserEntity.UserGroupId;
            membershipDto.MemberRoleId = userGroupUserEntity.MemberRoleId;
            membershipDto.ReferrerToken = userGroupUserEntity.ReferrerToken;
            membershipDto.ReferrerResource = userGroupUserEntity.ReferrerResource;
            membershipDto.ReferrerArchetypeId = userGroupUserEntity.ReferrerArchetypeId;
            membershipDto.DisplayName = GetDisplayName(userGroupUserEntity);
            membershipDto.Email = GetEmail(userGroupUserEntity.User);
            membershipDto.PeriodId = userGroupUserEntity.PeriodId;
            membershipDto.Identity = new IdentityDto()
            {
                CustomerId = userGroupUserEntity.CustomerId.HasValue ? userGroupUserEntity.CustomerId.Value : _workContext.CurrentCustomerId,
                Id = userGroupUserEntity.UGMemberId,
                OwnerId = _workContext.CurrentOwnerId,
                ExtId = !string.IsNullOrEmpty(userGroupUserEntity.ExtId)
                    ? userGroupUserEntity.ExtId
                    : userGroupUserEntity.User.ExtId
            };
            membershipDto.EntityStatus = new EntityStatusDto()
            {
                EntityVersion = userGroupUserEntity.EntityVersion,
                Deleted = userGroupUserEntity.Deleted.HasValue ? true : false,
                LastUpdated = userGroupUserEntity.LastUpdated.HasValue ? userGroupUserEntity.LastUpdated.Value : default(DateTime),
                LastUpdatedBy = userGroupUserEntity.LastUpdatedBy.HasValue ? (int)userGroupUserEntity.LastUpdatedBy : default(int),
                StatusId = userGroupUserEntity.EntityStatusId.HasValue ? (EntityStatusEnum)userGroupUserEntity.EntityStatusId : EntityStatusEnum.Unknown,
                StatusReasonId = userGroupUserEntity.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)userGroupUserEntity.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                LastExternallySynchronized = userGroupUserEntity.LastSynchronized
            };
            return membershipDto;
        }

        private string GetDisplayName(UGMemberEntity userGroupUserEntity)
        {
            return !string.IsNullOrEmpty(userGroupUserEntity.DisplayName)
                ? userGroupUserEntity.DisplayName
                : userGroupUserEntity.User != null
                    ? (userGroupUserEntity.User.FirstName + " " + userGroupUserEntity.User.LastName).Trim()
                    : string.Empty;
        }

        private string GetEmail(UserEntity userEntity)
        {
            return userEntity != null
                    ? userEntity.Email
                    : string.Empty;
        }
    }
}
