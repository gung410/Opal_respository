using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Domain.Services
{
    public class ApprovalGroupMemberService : IApprovalGroupMemberService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IUGMemberRepository _uGMemberRepository;
        private readonly IUserValidator _userValidator;
        private readonly IUserGroupValidator _userGroupValidator;
        private readonly IUserGroupUserMappingService _userGroupUserMappingService;
        private readonly IWorkContext _workContext;
        private readonly IDatahubLogger _datahubLogger;
        private readonly ILogger _logger;

        public ApprovalGroupMemberService(IUserGroupRepository userGroupRepository,
            OrganizationDbContext organizationDbContext, IUGMemberRepository uGMemberRepository, IUserGroupUserMappingService userGroupUserMappingService, IServiceProvider serviceProvider,
            IDatahubLogger datahubLogger, IWorkContext workContext, ILoggerFactory loggerFactory)
        {
            _userGroupRepository = userGroupRepository;
            _organizationDbContext = organizationDbContext;
            _uGMemberRepository = uGMemberRepository;
            _userValidator = serviceProvider.GetService<EmployeeValidator>();
            _userGroupValidator = serviceProvider.GetService<ApprovalGroupValidator>();
            _userGroupUserMappingService = userGroupUserMappingService;
            _datahubLogger = datahubLogger;
            _workContext = workContext;
            _logger = loggerFactory.CreateLogger<ApprovalGroupMemberService>();
        }


        public List<MemberDto> GetApprovalGroupMemberships(int approvalGroupId)
        {
            var result = new List<MemberDto>();
            var approvalGroupEntity = _userGroupRepository.GetUserGroupById(approvalGroupId, true, false);
            if (approvalGroupEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }
            foreach (var member in approvalGroupEntity.UGMembers)
            {
                var item = member.User;
                if (item != null && item.ArchetypeId == (int)ArchetypeEnum.Employee)
                {
                    result.Add(new MemberDto
                    {
                        Identity = new IdentityDto
                        {
                            Id = item.UserId,
                            Archetype = item.ArchetypeId.HasValue ? (ArchetypeEnum)item.ArchetypeId : ArchetypeEnum.Unknown,
                            ExtId = item.ExtId,
                            OwnerId = item.OwnerId
                        },
                        DisplayName = (item.FirstName + " " + item.LastName).Trim(),
                        EntityStatus = new EntityStatusDto(),
                        Role = string.Empty
                    });
                }
            }
            return result;
        }

        public List<MembershipDto> ProcessAddUserGroupUserMembership(List<int> employeeIds, MemberDto memberDto)
        {
            var usergroup = _userGroupValidator.ValidateMembership(memberDto);
            var existingUGMembers = _uGMemberRepository
           .GetUGMembers(userIds: employeeIds,
               ugmemberIds: memberDto.UserGroupMemberId.HasValue
                   ? new List<long> { memberDto.UserGroupMemberId.Value }
                   : new List<long>(), includeUserGroup: true);
            var existingUGMemberUserIds = existingUGMembers.Select(x => x.UserId).ToList();
            var result = new List<MembershipDto>();
            var resultEntity = new List<UGMemberEntity>();
            //hack to delete ug member with current mapping code
            var currentStatus = memberDto.EntityStatus.StatusId;
            foreach (var item in existingUGMembers)
            {
                if (item.UserGroup.UserGroupTypeId == usergroup.UserGroupTypeId)
                {
                    memberDto.EntityStatus.StatusId = EntityStatusEnum.Deactive;
                    item.EntityStatusId = (int)EntityStatusEnum.Deactive;
                    var userGroupUserEntity = _uGMemberRepository.Update(item);
                    // Write event log to Datahub
                    InsertEvent(new RequestContext(_workContext), userGroupUserEntity, memberDto, EventType.USER_MEMBERSHIP_DELETED);
                }
            }
            memberDto.EntityStatus.StatusId = currentStatus;
            foreach (var userId in employeeIds)
            {
                _userValidator.ValidateMember(userId);
                UGMemberEntity userGroupUserEntity = new UGMemberEntity();
                userGroupUserEntity = _userGroupUserMappingService.ToUGMemberEntity(userId, memberDto, userGroupUserEntity);
                var insertedMemberDto = _uGMemberRepository.Insert(userGroupUserEntity);
                resultEntity.Add(userGroupUserEntity);
                //result.Add(insertedMembershipDto);
                //Insert domain event
                InsertEvent(new RequestContext(_workContext), insertedMemberDto, memberDto, EventType.USER_MEMBERSHIP_CREATED);
            }
            _organizationDbContext.SaveChanges();
            foreach (var item in resultEntity)
            {
                result.Add(_userGroupUserMappingService.ToMembershipDto(item));
            }
            return result;
        }

        public List<MembershipDto> ProcessRemoveUserGroupUserMembership(List<int> employeeIds, MemberDto memberDto)
        {
            var usergroup = _userGroupValidator.ValidateMembership(memberDto);
            var existingUGMembers = _uGMemberRepository
           .GetUGMembers(userIds: employeeIds, userGroupIds: new List<int> { (int)memberDto.Identity.Id },
               ugmemberIds: memberDto.UserGroupMemberId.HasValue
                   ? new List<long> { memberDto.UserGroupMemberId.Value }
                   : new List<long>());
            var existingUGMemberUserIds = existingUGMembers.Select(x => x.UserId).ToList();
            var result = new List<MembershipDto>();
            var resultEntity = new List<UGMemberEntity>();
            //hack to delete ug member with current mapping code
            var currentStatus = memberDto.EntityStatus.StatusId;
            foreach (var item in existingUGMembers)
            {
                item.EntityStatusId = (int)EntityStatusEnum.Deactive;
                memberDto.EntityStatus.StatusId = EntityStatusEnum.Deactive;
                var userGroupUserEntity = _userGroupUserMappingService.ToUGMemberEntity(item.UserId, memberDto, item);
                _uGMemberRepository.Update(userGroupUserEntity);
                // Write event log to Datahub
                InsertEvent(new RequestContext(_workContext), item, memberDto, EventType.USER_MEMBERSHIP_DELETED);
            }
            _organizationDbContext.SaveChanges();
            memberDto.EntityStatus.StatusId = currentStatus;
            foreach (var item in existingUGMembers)
            {
                result.Add(_userGroupUserMappingService.ToMembershipDto(item));
            }
            return result;
        }

        private void InsertEvent(RequestContext requestContext, UGMemberEntity userGroupMember, MemberDto memberDto, EventType eventType)
        {
            dynamic body = new ExpandoObject();
            body.UserGroup = new
            {
                UserGroupMemberDisplayName = userGroupMember.DisplayName,
                UserGroupMemberExtID = userGroupMember.ExtId,
                userGroupMember.UserGroup?.Name,
                userGroupMember.UserGroup?.ExtId,
                userGroupMember.UserGroup?.OwnerId,
                userGroupMember.UserGroup?.ReferrerToken,
                userGroupMember.UserGroup?.ReferrerResource,
                userGroupMember.UserGroup?.ArchetypeId,
                userGroupMember.UserGroup?.ReferrerArchetypeId
            };

            body.MemberData = memberDto;
            body.UserGroupId = userGroupMember.UserGroupId;
            var eventLogMessage = new LogEventMessage($"{eventType.ToString().ToLower()}.{memberDto.Identity.Archetype.ToString().ToLower()}", _workContext)
                                                        .EntityId(userGroupMember.UserId?.ToString())
                                                        .WithBody(body);
            _datahubLogger.WriteEventLog(eventLogMessage);
        }
    }
}
