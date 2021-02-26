using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using cxOrganization.Client;
using cxOrganization.Client.Account;
using cxOrganization.Client.Departments;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.ApiClient;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Validators;

using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.Exceptions;
using cxPlatform.Core.Extentions;
using cxPlatform.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace cxOrganization.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private IWorkContext _workContext;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        //private readonly ISecurityHandler _securityHandler;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICommonService _commonService;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IUserValidator _validator;
        private readonly IUserMappingService _userMappingService;
        private readonly IDepartmentMappingService _departmentMappingService;
        private readonly IUserGroupMappingService _userGroupMappingService;
        private readonly IEventLogDomainApiClient _eventClientService;
        private readonly Func<string, ICryptographyService> _cryptographyService;
        private readonly IOwnerRepository _ownerRepository;
        private readonly Repositories.IObjectMappingRepository _objectMappingRepository;
        private IUserGroupUserMappingService _userGroupUserMappingService;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        private readonly IDatahubLogger _datahubLogger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserWithIdpInfoMappingService _userWithIdpInfoMappingService;
        private readonly ILogger<UserService> _logger;
        private readonly EmailTemplates _emailTemplates;
        private readonly AppSettings _appSettings;
        private readonly EntityStatusReasonTexts _entityStatusReasonTexts;
        private List<UserTypeEntity> _userTypeEntities;
        private readonly IUserAccessService _userAccessService;
        private readonly Func<ArchetypeEnum, IUserGroupService> _funcUserGroupService;
        private readonly IDTDEntityRepository _dtdEntityRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUGMemberRepository _ugMemberRepository;
        private readonly IHierarchyDepartmentPermissionService _hierarchyDepartmentPermissionService;
        private readonly IIdentityServerClientService _identityServerClientService;
        private readonly IUserGroupService _userGroupService;
        private readonly IUGMemberService _uGMemberService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IInternalHttpClientRequestService _internalHttpClientRequestService;

        public UserService(IUserRepository userRepository,
            IWorkContext workContext,
            OrganizationDbContext organizationDbContext,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IUserTypeRepository userTypeRepository,
            //ISecurityHandler securityHandler,
            IDepartmentRepository departmentRepository,
            ICommonService commonService,
            IUserValidator validator,
            IUserMappingService userMappingService,
            IDepartmentMappingService departmentMappingService,
            IUserGroupMappingService userGroupMappingService,
            IEventLogDomainApiClient eventClientService,
            Func<string, ICryptographyService> cryptographyService,
            IOwnerRepository ownerRepository,
            IUserGroupUserMappingService userGroupUserMappingService,
            IObjectMappingRepository objectMappingRepository,
            IHierarchyDepartmentService hierarchyDepartmentService,
            IDatahubLogger datahubLogger,
            IUserWithIdpInfoMappingService userWithIdpInfoMappingService,
            ILogger<UserService> logger,
            IOptions<EmailTemplates> emailTemplatesOptions,
            IOptions<AppSettings> appSettingOptions,
            IOptions<EntityStatusReasonTexts> entityStatusReasonTextsOption,
            IUserAccessService userAccessService,
            Func<ArchetypeEnum, IUserGroupService> funcUserGroupService,
            IDTDEntityRepository dtdEntityRepository,
            IUserGroupRepository userGroupRepository,
            IUGMemberRepository ugMemberRepository,
            IHierarchyDepartmentPermissionService hierarchyDepartmentPermissionService,
            IIdentityServerClientService identityServerClientService,
            IUserGroupService userGroupService,
            IUGMemberService uGMemberService,
            IServiceScopeFactory serviceScopeFactory,
            IInternalHttpClientRequestService internalHttpClientRequestService
            )
        {
            _userRepository = userRepository;
            _workContext = workContext;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _userTypeRepository = userTypeRepository;
            _departmentRepository = departmentRepository;
            _commonService = commonService;
            _organizationDbContext = organizationDbContext;
            _validator = validator;
            _userMappingService = userMappingService;
            _departmentMappingService = departmentMappingService;
            _userGroupMappingService = userGroupMappingService;
            _eventClientService = eventClientService;
            _cryptographyService = cryptographyService;
            _ownerRepository = ownerRepository;
            _objectMappingRepository = objectMappingRepository;
            _userGroupUserMappingService = userGroupUserMappingService;
            _hierarchyDepartmentService = hierarchyDepartmentService;
            _datahubLogger = datahubLogger;
            _userWithIdpInfoMappingService = userWithIdpInfoMappingService;
            _logger = logger;
            _emailTemplates = emailTemplatesOptions.Value;
            _appSettings = appSettingOptions.Value;
            _entityStatusReasonTexts = entityStatusReasonTextsOption.Value;
            _userAccessService = userAccessService;
            _funcUserGroupService = funcUserGroupService;
            _dtdEntityRepository = dtdEntityRepository;
            _userGroupRepository = userGroupRepository;
            _ugMemberRepository = ugMemberRepository;
            _hierarchyDepartmentPermissionService = hierarchyDepartmentPermissionService;
            _identityServerClientService = identityServerClientService;
            _userGroupService = userGroupService;
            _uGMemberService = uGMemberService;
            _serviceScopeFactory = serviceScopeFactory;
            _internalHttpClientRequestService = internalHttpClientRequestService;
        }

        public ConexusBaseDto InsertUser(
            HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto,
            UserDtoBase userDto,
            IWorkContext workContext = null,
            bool isInsertedByImport = false)
        {
            //Pre-checking: make sure the path of hierarchy department is correct
            var parentHd = _validator.ValidateHierarchyDepartment(hierarchyDepartmentValidationDto);

            var parentDepartment = _departmentRepository.GetById(userDto.GetParentDepartmentId());

            //Do the validation
            var entity = _validator.Validate(
                parentDepartment,
                userDto,
                workContext is object
                    ? workContext.CurrentOwnerId
                    : _workContext.CurrentOwnerId);

            //Map to User Entity
            var userEntity = _userMappingService.ToUserEntity(
                parentDepartment,
                parentHd,
                entity,
                userDto,
                false,
                workContext is object
                    ? workContext.CurrentOwnerId
                    : _workContext.CurrentOwnerId);

            var currentOwner = _ownerRepository.GetById(workContext is object ? workContext.CurrentOwnerId : _workContext.CurrentOwnerId);

            //Insert to DB
            userEntity = _userRepository.Insert(userEntity, currentOwner.UseHashPassword, currentOwner.UseOTP, currentOwner.DefaultHashMethod);
            _organizationDbContext.SaveChanges();

            userEntity.Department = userEntity.Department ?? parentDepartment;

            userEntity = SendCommunicationMessagesWhenInsertOrUpdateUser(null, userEntity, false, userDto.OtpValue, userDto.OtpExpiration);

            SetUserGroupOwnerUser(userEntity);

            //Map entity to Dto for return later
            //We keep original ssn from UserEntity for logging
            var reponsingUserDto = _userMappingService.ToUserDto(userEntity, keepEncryptedSsn: true);
            var insertedUserDto = reponsingUserDto as UserDtoBase;

            if (insertedUserDto != null)
            {
                insertedUserDto.OtpValue = userDto.OtpValue;
            }

            var hierarchyInfo = GetEventHierarchyInfo(userEntity, parentDepartment);

            InsertUserEvent(userEntity, parentDepartment, reponsingUserDto, hierarchyInfo, EventType.CREATED, workContext ?? _workContext, isInsertedByImport);

            if (insertedUserDto != null)
            {
                _userMappingService.HideOrDecryptSSN(insertedUserDto);
            }


            //Remap to userdtoBase
            return reponsingUserDto;
        }

        /// <summary>
        /// Inserts the user.
        /// </summary>
        /// <param name="hierarchyDepartmentValidationDto"></param>
        /// <param name="userDto">The userDto.</param>
        /// <returns>Department.</returns>
        public ConexusBaseDto UpdateUser(
            HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto,
            UserDtoBase userDto,
            bool skipCheckingEntityVersion = false,
            IWorkContext workContext = null,
            bool? isAutoArchived = null)
        {
            _workContext = workContext ?? _workContext;
            //Pre-checking: make sure the path of hierarchy department is correct
            var parentHd = _validator.ValidateHierarchyDepartment(hierarchyDepartmentValidationDto);

            var parentDepartment = _departmentRepository.GetDepartmentByIdIncludeHd(userDto.GetParentDepartmentId(), userDto.Identity.OwnerId, userDto.Identity.CustomerId);

            //Do the validation
            var entity = _validator.Validate(parentDepartment, userDto, _workContext.CurrentOwnerId, _workContext.CurrentCustomerId);

            /* In case we are rejecting current user from pending level,
             * we need to update Username to dummy value so that both username and email can be reused */
            if (userDto.EntityStatus.StatusId == EntityStatusEnum.Rejected)
            {
                entity.UserName = userDto.EmailAddress;
            }

            //We only need to clone entity for checking info change when entity status is changed
            var oldEntity = CloneUserEntityForCheckingChange(entity);

            //Map to Department Entity
            var userEntity = _userMappingService.ToUserEntity(parentDepartment, null, entity, userDto, skipCheckingEntityVersion);

            if (userEntity == null) return null;

            //in case publish user data to datahub only, no changes of data.
            userEntity.LastUpdated = _workContext.CorrelationId == Guid.Empty.ToString() && _workContext.RequestId == Guid.Empty.ToString()
                ? oldEntity.LastUpdated
                : userEntity.LastUpdated;
            userEntity.LastUpdatedBy = _workContext.CorrelationId == Guid.Empty.ToString() && _workContext.RequestId == Guid.Empty.ToString()
                ? oldEntity.LastUpdatedBy
                : userEntity.LastUpdatedBy;
            var isChangedStatus = userEntity.EntityStatusId != oldEntity.EntityStatusId;
            var isResettingOtp = !isChangedStatus && userDto.ResetOtp == true;

            //Post-checking: check if user.Department belong to parent
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(parentHd.DepartmentId, ArchetypeEnum.Unknown)
                .ValidateDepartment(entity.DepartmentId, ArchetypeEnum.Unknown)
                .WithStatus(EntityStatusEnum.All)
                .SkipCheckingArchetype()
                .IsNotDirectParent()
                .Create();
            _validator.ValidateHierarchyDepartment(validationSpecification);

            //Until now, we only need to get modified property when entity status is changed. This method should be call before updating entity
            var modifiedProperties = isChangedStatus ? _userRepository.GetModifiedProperties(userEntity) : null;

            if (!(_workContext.CorrelationId == Guid.Empty.ToString() && _workContext.RequestId == Guid.Empty.ToString()))
            {
                //Call update user Entity
                userEntity = UpdateUser(userEntity);
            }
            var userIsMoved = userEntity.DepartmentId != oldEntity.DepartmentId;
            userEntity.Department = userEntity.Department ?? parentDepartment;
            oldEntity.Department = userIsMoved ? _departmentRepository.GetById(oldEntity.DepartmentId) : userEntity.Department;

            if (!(_workContext.CorrelationId == Guid.Empty.ToString() && _workContext.RequestId == Guid.Empty.ToString()))
            {
                userEntity = SendCommunicationMessagesWhenInsertOrUpdateUser(oldEntity, userEntity, isResettingOtp, userDto.OtpValue, userDto.OtpExpiration);
                SetUserGroupOwnerUser(userEntity);
            }

            //Map entity to Dto for return later
            //We keep original ssn from UserEntity for logging
            var repondingUserDto = _userMappingService.ToUserDto(userEntity, keepEncryptedSsn: true);

            if (!(_workContext.CorrelationId == Guid.Empty.ToString() && _workContext.RequestId == Guid.Empty.ToString()))
            {
                //Insert domain event
                var updatedUserDto = repondingUserDto as UserDtoBase;
                if (updatedUserDto != null)
                {
                    updatedUserDto.OtpValue = userDto.OtpValue;
                    _userMappingService.HideOrDecryptSSN(updatedUserDto);
                }
            }

            var hierarchyInfo = GetEventHierarchyInfo(userEntity, parentDepartment);
            if (isChangedStatus)
            {
                //If status is changed we write an event with type <archetype>_ENTITYSTATUS_CHANGED, 
                var statusChangedInfo = DomainHelper.BuildEntityStatusChangedEventInfo(oldEntity.EntityStatusId, oldEntity.EntityStatusReasonId, userEntity.EntityStatusId, userEntity.EntityStatusReasonId);

                InsertUserEvent(userEntity,
                                parentDepartment,
                                statusChangedInfo,
                                hierarchyInfo,
                                EventType.ENTITYSTATUS_CHANGED,
                                null,
                                false,
                                isAutoArchived);

                //Check if info are also changed, we write more an event with type <archetype>_UPDATED

                if (DomainHelper.IsChangedInfo(modifiedProperties, nameof(UserEntity.LastUpdated), nameof(UserEntity.LastUpdatedBy), nameof(UserEntity.EntityStatusId)))
                {
                    InsertUserEvent(userEntity, parentDepartment, repondingUserDto, hierarchyInfo, EventType.UPDATED);
                }
            }
            else
            {

                //If status is NOT changed we always write an event with type <archetype>_UPDATED
                InsertUserEvent(
                    userEntity,
                    parentDepartment,
                    repondingUserDto,
                    hierarchyInfo,
                    EventType.UPDATED,
                    null,
                    false,
                    null,
                    _workContext.CorrelationId == Guid.Empty.ToString()
                        && _workContext.RequestId == Guid.Empty.ToString());
            }

            if (userIsMoved)
            {
                //TODO: should we skip event 'UPDATED' if user is only change department?

                var fromDepartmentEntity = oldEntity.Department ?? new DepartmentEntity { DepartmentId = oldEntity.DepartmentId };
                var toDepartmentEntity = userEntity.Department ?? new DepartmentEntity { DepartmentId = userEntity.DepartmentId };

                var movedUserGroupIds = MoveUserGroupOfUserToNewDepartment(oldEntity.UserId, fromDepartmentEntity.DepartmentId, toDepartmentEntity.DepartmentId);

                var moveDepartmentInfo = DomainHelper.BuildMoveDepartmentInfo(_workContext, fromDepartmentEntity, toDepartmentEntity, userEntity.LastUpdatedBy, movedUserGroupIds);
                InsertUserEvent(userEntity, parentDepartment, moveDepartmentInfo, hierarchyInfo, EventType.USER_MOVED);
            }
            return repondingUserDto;
        }

        private void SetUserGroupOwnerUser(UserEntity userEntity)
        {
            if (userEntity.UGMembers != null && userEntity.UGMembers.Count > 0)
            {
                var ugMembersNeedToFindOwnerUser = userEntity.UGMembers
                    .Where(ugm => ugm.UserGroup?.UserId > 0 && ugm.UserGroup.User == null).ToList();
                if (ugMembersNeedToFindOwnerUser.Count > 0)
                {
                    var userGroupOwnerUserIds = ugMembersNeedToFindOwnerUser.Select(ugm => ugm.UserGroup.UserId.Value)
                        .Distinct().ToList();

                    var userGroupOwnerUsers = _userRepository.GetUserByIds(userGroupOwnerUserIds);

                    foreach (var userEntityUgMember in ugMembersNeedToFindOwnerUser)
                    {
                        var ownerUserOfUserGroup =
                            userGroupOwnerUsers.FirstOrDefault(u => u.UserId == userEntityUgMember.UserGroup.UserId);
                        userEntityUgMember.UserGroup.User = ownerUserOfUserGroup;
                    }
                }
            }
        }

        private List<int> MoveUserGroupOfUserToNewDepartment(int userId, int oldDepartmentId, int newDepartmentId)
        {
            var movedUserGroupIds = new List<int>();
            try
            {
                var customerIds = new List<int> { _workContext.CurrentCustomerId };
                var oldDepartmentIds = new List<int> { oldDepartmentId };
                var userIds = new List<int> { userId };

                var genericUserGroupService = _funcUserGroupService(ArchetypeEnum.Unknown);

                var ownedUserGroupEntitiesGroupedByArchetype = genericUserGroupService.GetUserGroupEntities(
                    ownerId: _workContext.CurrentOwnerId,
                    customerIds: customerIds,
                    departmentIds: oldDepartmentIds,
                    groupUserIds: userIds,
                    userStatusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.GroupBy(a => a.ArchetypeId);

                foreach (var userGroupEntityGroupByArchetype in ownedUserGroupEntitiesGroupedByArchetype)
                {
                    var userGroupArchetype = (ArchetypeEnum)(userGroupEntityGroupByArchetype.Key ?? 0);
                    if (userGroupArchetype == ArchetypeEnum.Unknown)
                    {
                        continue;
                    }

                    var userGroupService = _funcUserGroupService(userGroupArchetype);
                    foreach (var userGroupEntity in userGroupEntityGroupByArchetype)
                    {
                        var userGroupDto = userGroupService.MapToUserGroupService(userGroupEntity) as UserGroupDtoBase;
                        if (userGroupDto == null) continue;

                        userGroupDto.SetParentDepartmentId(newDepartmentId);
                        userGroupService.UpdateUserGroup(null, userGroupDto);
                        movedUserGroupIds.Add(userGroupEntity.UserGroupId);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected error occurs while changing department of user group when moving department of user '{userId}'.");
            }

            return movedUserGroupIds;
        }


        private void InsertUserEvent(UserEntity userEntity,
            DepartmentEntity parentDepartment,
            object additionalInformation,
            object hierarchyInfo,
            EventType eventType,
            IWorkContext workContext = null,
            bool isInsertedByImport = false,
            bool? isAutoArchived = null,
            bool isSyncDataOnly = false)
        {
            dynamic body = new ExpandoObject();
            body.UserData = additionalInformation;
            body.DepartmentId = userEntity.DepartmentId;
            body.DepartmentArcheTypeId = parentDepartment?.ArchetypeId;
            body.UserId = userEntity.UserId;
            body.UserCxId = userEntity.ExtId;
            body.UserArcheTypeId = userEntity.ArchetypeId;

            if (isInsertedByImport)
            {
                body.isInsertedByImport = true;
            }

            if (isAutoArchived.HasValue)
            {
                body.isAutoArchived = isAutoArchived;
            }

            if (hierarchyInfo != null)
            {
                body.HierarchyInfo = hierarchyInfo;
            }

            var objectType = userEntity.ArchetypeId == null || userEntity.ArchetypeId == (int)ArchetypeEnum.Unknown
                ? "unknown_archetype_user"
                : ((ArchetypeEnum)userEntity.ArchetypeId).ToString();
            if (isSyncDataOnly)
            {
                Task.Run(async delegate
                {
                    var logEventMessage = new LogEventMessage($"{eventType.ToEventName(objectType)}-syncdata", workContext ?? _workContext)
                        .EntityId(userEntity.UserId.ToString())
                        .Entity("domain", "user")
                        .WithBody(body);
                    _datahubLogger.WriteEventLog(logEventMessage);
                    await Task.Delay(100);
                });
            }
            else
            {
                Task.Run(async delegate
                {
                    var logEventMessage = new LogEventMessage($"{eventType.ToEventName(objectType)}", workContext ?? _workContext)
                        .EntityId(userEntity.UserId.ToString())
                        .Entity("domain", "user")
                        .WithBody(body);
                    _datahubLogger.WriteEventLog(logEventMessage);
                    await Task.Delay(100);
                });
            }
        }

        private dynamic GetEventHierarchyInfo(UserEntity userEntity, DepartmentEntity parentDepartment)
        {
            return _hierarchyDepartmentRepository.GetHierarchyInfo(_workContext.CurrentHdId,
                userEntity.DepartmentId, parentDepartment?.H_D.FirstOrDefault(), true);
        }

        private UserEntity CloneUserEntityForCheckingChange(UserEntity userEntity)
        {
            return new UserEntity()
            {
                UserId = userEntity.UserId,
                ArchetypeId = userEntity.ArchetypeId,
                OwnerId = userEntity.OwnerId,
                CustomerId = userEntity.CustomerId,
                ExtId = userEntity.ExtId,
                DepartmentId = userEntity.DepartmentId,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
                Gender = userEntity.Gender,
                SSN = userEntity.SSN,
                DateOfBirth = userEntity.DateOfBirth,
                CountryCode = userEntity.CountryCode,
                ChangePassword = userEntity.ChangePassword,
                LanguageId = userEntity.LanguageId,
                Locked = userEntity.Locked,
                LastSynchronized = userEntity.LastSynchronized,
                Mobile = userEntity.Mobile,
                RoleId = userEntity.RoleId,
                ForceUserLoginAgain = userEntity.ForceUserLoginAgain,
                Tag = userEntity.Tag,
                EntityStatusId = userEntity.EntityStatusId,
                EntityStatusReasonId = userEntity.EntityStatusReasonId,
                EntityExpirationDate = userEntity.EntityExpirationDate,
                EntityActiveDate = userEntity.EntityActiveDate,
                DynamicAttributes = userEntity.DynamicAttributes
            };
        }
        public List<MemberDto> GetUserMemberships(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto, int userId, ArchetypeEnum userArcheType, ArchetypeEnum useTypeArcheType, ArchetypeEnum departmentTypeArcheType)
        {
            _validator.ValidateHierarchyDepartment(hierarchyDepartmentValidationDto);

            return GetUserMemberships(userId, userArcheType, new List<ArchetypeEnum> { useTypeArcheType, departmentTypeArcheType });
        }

        public Dictionary<string, List<MemberDto>> GetUsersMemberships(List<string> userExtIds, ArchetypeEnum userArcheType)
        {
            var result = new Dictionary<string, List<MemberDto>>();

            var members = new List<MemberDto>();
            var usersEntity = _userRepository.GetUsersByExtIds(_workContext.CurrentCustomerId,
                                                               _workContext.CurrentOwnerId,
                                                               userExtIds,
                                                               includeDepartment: true,
                                                               includeUgMember: true,
                                                               includeUserGroupFromUgMember: true,
                                                               includeUserType: true).Where(x => x.ArchetypeId == (int)userArcheType);

            foreach (var userEntity in usersEntity)
            {
                members = new List<MemberDto>();
                foreach (var usertypeUser in userEntity.UT_Us)
                {
                    var item = usertypeUser?.UserType;
                    if (item != null)
                    {
                        int identityId;
                        members.Add(new MemberDto
                        {

                            Identity = new IdentityDto
                            {
                                Id = int.TryParse(item.ExtId, out identityId) ? identityId : item.UserTypeId,
                                Archetype = item.ArchetypeId == null ? ArchetypeEnum.Unknown : (ArchetypeEnum)item.ArchetypeId,
                                CustomerId = _workContext.CurrentCustomerId,
                                ExtId = item.ExtId,
                                OwnerId = item.OwnerId
                            },
                            EntityStatus = new EntityStatusDto()
                            {
                                StatusId = EntityStatusEnum.Active
                            },
                            Role = string.Empty
                        });
                    }
                }

                foreach (var item in userEntity.UGMembers)
                {
                    if (item.EntityStatusId == (int)EntityStatusEnum.Active)
                    {
                        var memberDto = _userGroupUserMappingService.ToMemberDto(item, ArchetypeEnum.TeachingGroup);
                        if (memberDto != null)
                        {
                            memberDto.Identity.ExtId = item.UserGroup.ExtId;
                            members.Add(memberDto);
                        }
                    }

                }
                members.Add(_departmentMappingService.ToMemberDto(userEntity.Department));
                result.Add(userEntity.ExtId, members);
            }

            return result;
        }

        public List<MemberDto> GetUserMemberships(int userId,
            ArchetypeEnum userArcheType,
            List<ArchetypeEnum> membershipsArchetypeIds = null,
            List<EntityStatusEnum> membershipStatusIds = null,
            List<int> membershipIds = null,
            List<string> membershipExtIds = null)
        {
            //TODO : Need to optimate performence again
            membershipsArchetypeIds = membershipsArchetypeIds ?? new List<ArchetypeEnum>();
            membershipStatusIds = membershipStatusIds ?? new List<EntityStatusEnum>();
            membershipIds = membershipIds ?? new List<int>();
            membershipExtIds = membershipExtIds ?? new List<string>();

            var result = new List<MemberDto>();
            var (includeUserType, includeUserGroup, includeDepartment) = GenerateIncludeProperties(membershipsArchetypeIds);

            var userEntity = _userRepository.GetUserByIds(new List<int> { userId },
                includeDepartment: includeDepartment,
                includeUserGroup: includeUserGroup,
                includeUserType: includeUserType).FirstOrDefault();

            if (userEntity == null || userEntity.ArchetypeId != (int)userArcheType)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }

            //filtering function
            Func<int, int?, string, int?, bool> IsSatisfyFilterConditions = GetCheckingFunction(membershipsArchetypeIds,
                membershipStatusIds,
                membershipIds,
                membershipExtIds);

            foreach (var utu in userEntity.UT_Us)
            {
                var item = utu.UserType;
                if (item == null)
                    continue;
                bool isAdded = IsSatisfyFilterConditions(item.UserTypeId, item.ArchetypeId, item.ExtId, null);
                if (isAdded)
                {
                    int identityId;
                    result.Add(new MemberDto
                    {
                        Identity = new IdentityDto
                        {
                            Id = int.TryParse(item.ExtId, out identityId) ? identityId : item.UserTypeId,
                            Archetype = (ArchetypeEnum)item.ArchetypeId,
                            CustomerId = _workContext.CurrentCustomerId,
                            ExtId = item.ExtId,
                            OwnerId = item.OwnerId
                        },
                        EntityStatus = new EntityStatusDto()
                        {
                            StatusId = EntityStatusEnum.Active
                        },
                        Role = string.Empty
                    });
                }
            }

            foreach (var item in userEntity.UGMembers)
            {
                bool isAdded = IsSatisfyFilterConditions(item.UserGroup.UserGroupId, item.UserGroup.ArchetypeId, item.UserGroup.ExtId, item.EntityStatusId);
                if (isAdded)
                {
                    var memberDto = _userGroupUserMappingService.ToMemberDto(item, (ArchetypeEnum)item.UserGroup.ArchetypeId);
                    if (memberDto != null)
                    {
                        result.Add(memberDto);
                    }
                }

            }
            if (!membershipIds.Any()
                || membershipIds.Contains(userEntity.DepartmentId))
            {
                var departmentEntity = userEntity.Department;

                bool isAdded = departmentEntity == null ? false : IsSatisfyFilterConditions(departmentEntity.DepartmentId, departmentEntity.ArchetypeId, departmentEntity.ExtId, departmentEntity.EntityStatusId);
                if (isAdded)
                {
                    result.Add(_departmentMappingService.ToMemberDto(departmentEntity));
                }
            }

            return result;
        }

        private static Func<int, int?, string, int?, bool> GetCheckingFunction(List<ArchetypeEnum> membershipsArchetypeIds,
            List<EntityStatusEnum> membershipStatusIds,
            List<int> membershipIds,
            List<string> membershipExtIds)
        {
            return (Id, archetypeId, extId, entityStatusId) =>
            {
                bool doesMatchConditions = true;
                if (entityStatusId != null)
                {
                    if (!membershipStatusIds.Any())
                    {
                        doesMatchConditions = entityStatusId == (int)EntityStatusEnum.Active;
                    }
                    else
                    {
                        doesMatchConditions = membershipStatusIds.Contains((EntityStatusEnum)entityStatusId) || membershipStatusIds.Contains(EntityStatusEnum.All);
                    }
                }
                if (!archetypeId.HasValue)
                    return false;
                if (membershipIds.Any() && !membershipIds.Contains(Id))
                    doesMatchConditions = false;
                if (membershipsArchetypeIds.Any() && !membershipsArchetypeIds.Contains((ArchetypeEnum)archetypeId))
                    doesMatchConditions = false;
                if (membershipExtIds.Any() && !membershipExtIds.Contains(extId))
                    doesMatchConditions = false;

                return doesMatchConditions;
            };
        }

        private (bool, bool, bool) GenerateIncludeProperties(List<ArchetypeEnum> membershipsArchetypeIds)
        {

            if (membershipsArchetypeIds.Any())
            {
                bool includeUserType = false;
                bool includeUserGroup = false;
                bool includeDepartment = false;
                foreach (var archetype in membershipsArchetypeIds)
                {
                    switch (archetype)
                    {
                        case (ArchetypeEnum.Level):
                        case (ArchetypeEnum.Role):
                            includeUserType = true;
                            break;
                        case (ArchetypeEnum.CandidatePool):
                        case (ArchetypeEnum.TeachingGroup):
                        case (ArchetypeEnum.Team):
                            includeUserGroup = true;
                            break;
                        case (ArchetypeEnum.CandidateDepartment):
                        case (ArchetypeEnum.Company):
                        case (ArchetypeEnum.Country):
                        case (ArchetypeEnum.OrganizationalUnit):
                        case (ArchetypeEnum.School):
                        case (ArchetypeEnum.Class):
                        case (ArchetypeEnum.SchoolOwner):
                        case (ArchetypeEnum.DataOwner):
                        case (ArchetypeEnum.Partner):
                            includeDepartment = true;
                            break;

                    }
                }
                return (includeUserType, includeUserGroup, includeDepartment);

            }
            return (true, true, true);

        }

        public List<LevelDto> GetUserLevel(int userId, ArchetypeEnum userArcheType, ArchetypeEnum useTypeArcheType)
        {
            var result = new List<LevelDto>();
            var userEntity = _userRepository.GetUserById(userId);
            if (userEntity == null || (userEntity.ArchetypeId.HasValue && (userEntity.ArchetypeId.Value != (int)userArcheType)))
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_IDENTITY_NOT_FOUND);
            }
            foreach (var utu in userEntity.UT_Us)
            {
                var item = utu.UserType;
                if (item.ArchetypeId.HasValue && item.ArchetypeId == (int)useTypeArcheType)
                {
                    result.Add(new LevelDto
                    {
                        Identity = new IdentityDto
                        {
                            Id = item.UserTypeId,
                            Archetype = useTypeArcheType,
                            //CustomerId = item.CustomerId ?? item.CustomerId.Value,
                            ExtId = item.ExtId,
                            OwnerId = item.OwnerId
                        },
                        EntityStatus = new EntityStatusDto
                        { }

                    });
                }
            }

            return result;
        }
        public List<UserDtoBase> GetUsers(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto, bool includeSubUsers = false)
        {
            //Pre-checking: make sure the path of hierarchy department is correct
            var parentHd = _validator.ValidateHierarchyDepartment(hierarchyDepartmentValidationDto);

            List<UserEntity> users;
            if (includeSubUsers)
            {
                var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                var departmentHd = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(currentHD.HierarchyId, parentHd.DepartmentId, true);
                var childHds = _hierarchyDepartmentRepository.GetChildHds(departmentHd.Path, includeInActiveStatus: true);
                var departmentIds = childHds.Select(x => x.DepartmentId).ToList();
                departmentIds.Insert(0, parentHd.DepartmentId);
                users = _userRepository.GetUsersByDepartmentIds(departmentIds, EntityStatusEnum.All);
            }
            else
            {
                users = _userRepository.GetUsersByDepartmentIdIncludeUserTypesUserGroup(parentHd.DepartmentId, _workContext.CurrentOwnerId, EntityStatusEnum.All);
            }

            //Check security
            //users = _securityHandler.AllowAccess(users, AccessBinaryValues.Read, false);

            List<UserDtoBase> result = new List<UserDtoBase>();
            foreach (var item in users)
            {
                var userDto = (UserDtoBase)_userMappingService.ToUserDto(item);
                if (userDto != null)
                {
                    result.Add(userDto);
                }
            }
            return result;
        }

        public List<ConexusBaseDto> GetUsersByUserGroups(List<int> userGroupIds, List<int> usertypeIds, List<int> gender = null, List<string> ages = null, string countries = "")
        {
            List<UserEntity> users = _userRepository.GetUsersByUserGroupIds(userGroupIds, usertypeIds, gender, ages, countries);

            //Check security
            //users = _securityHandler.AllowAccess(users, AccessBinaryValues.Read, false);

            List<ConexusBaseDto> result = new List<ConexusBaseDto>();
            foreach (var item in users)
            {
                var userDto = _userMappingService.ToUserDto(item);
                if (userDto != null)
                {
                    result.Add(userDto);
                }
            }
            return result;
        }

        public ConexusBaseDto GetUser(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto, int userId)
        {
            //Pre-checking: make sure the path of hierarchy department is correct
            var parentHd = _validator.ValidateHierarchyDepartment(hierarchyDepartmentValidationDto);

            var user = _userRepository.GetUserById(userId);

            if (user == null)
            {
                return null;
            }

            //Post-checking: check if user.Department belong to parent
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(parentHd.DepartmentId, ArchetypeEnum.Unknown)
                .ValidateDepartment(user.DepartmentId, ArchetypeEnum.Unknown)
                .WithStatus(EntityStatusEnum.All)
                .SkipCheckingArchetype()
                .IsNotDirectParent()
                .Create();
            _validator.ValidateHierarchyDepartment(validationSpecification);

            //Check security
            //if (_securityHandler.AllowAccess(user, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _userMappingService.ToUserDto(user);
            //}
            //return null;
        }

        public ConexusBaseDto GetUser(int userId)
        {
            var user = _userRepository.GetUserById(userId);

            if (user == null)
            {
                return null;
            }

            //Check security
            //if (_securityHandler.AllowAccess(user, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _userMappingService.ToUserDto(user);
            //}
            //return null;
        }

        public List<IdentityStatusDto> UpdateUserIdentifiers(List<IdentityStatusDto> userIdentities, List<int> allowArchetypeIds, string hdPath)
        {
            var userIds = userIdentities.Select(x => x.Identity.Id).ToList();
            var users = _userRepository.GetUsersByUserIdsAndArchetypeIds(userIds, allowArchetypeIds);
            var results = new List<IdentityStatusDto>();
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(hdPath) && user.Department.H_D.FirstOrDefault().Path.StartsWith(hdPath))
                {
                    var info = userIdentities.FirstOrDefault(x => x.Identity.Id == user.UserId);
                    if (info == null)
                        continue;
                    user.LastSynchronized = info.EntityStatus.LastExternallySynchronized ?? DateTime.Now;
                    user.EntityStatusId = (int)info.EntityStatus.StatusId;
                    user.EntityStatusReasonId = (int)info.EntityStatus.StatusReasonId;
                    results.Add(_userMappingService.ToIdentityStatusDto(_userRepository.Update(user)));
                }
            }
            _organizationDbContext.SaveChanges();
            return results;
        }

        public async Task<(List<HierachyDepartmentIdentityDto> HierachyDepartmentIdentities, bool AccessDenied)> GetUserHierarchyDepartmentIdentitiesAsync(int ownerId,
                int? userId, string userExtId, bool includeParentHDs = true, bool includeChildrenHDs = false,
                List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null,
                int? maxChildrenLevel = null, bool countChildren = false,
                List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false,
                bool getParentNode = false,
                bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null,
                List<string> jsonDynamicData = null)
        {
            var user = await _userRepository.GetUserAsync(ownerId, userId, userExtId);
            if (user == null)
            {
                return (null, false);
            }

            var (accessDeniedOnRootDepartment, keepTheRootDepartment) = await CheckDepartmentIdParameterAsync(user.DepartmentId);
            if (accessDeniedOnRootDepartment)
            {
                _logger.LogWarning(
                    $"Logged-in user with sub '{_workContext.Sub}' does not have access on the root department id '{user.DepartmentId}'.");
                return (null, true);

            }

            var checkPermission = !(await _hierarchyDepartmentPermissionService.IgnoreSecurityCheckAsync());
            var currentHD = await _hierarchyDepartmentRepository.GetByIdAsync(_workContext.CurrentHdId);

            var hierarchyDepartmentIdentities = await _hierarchyDepartmentService.GetHierarchyDepartmentIdentitiesAsync(
                currentHD.HierarchyId,
                user.DepartmentId, includeParentHDs,
                includeChildrenHDs, ownerId, customerIds, departmentEntityStatuses, maxChildrenLevel, countChildren,
                departmentTypeIds, departmentName,
                includeDepartmentType: includeDepartmentType, getParentNode: getParentNode, countUser: countUser,
                countUserEntityStatuses: countUserEntityStatuses, jsonDynamicData: jsonDynamicData,
                checkPermission: checkPermission);
            if (!keepTheRootDepartment)
                hierarchyDepartmentIdentities =
                    _hierarchyDepartmentPermissionService.ProcessRemovingTheRootDepartment(
                        hierarchyDepartmentIdentities);
            return (hierarchyDepartmentIdentities, false);

        }

        public Dictionary<string, List<HierachyDepartmentIdentityDto>> GetUserHierachyDepartmentIdentitiesByExtIds(List<string> extIds)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var users = _userRepository.GetUsersByExtIds(_workContext.CurrentCustomerId, _workContext.CurrentOwnerId, extIds, includeDepartment: true);
            var departmentids = users.Select(x => x.DepartmentId).Distinct().ToList();
            var departmentInfos = new Dictionary<int, List<HierachyDepartmentIdentityDto>>();
            foreach (var departmentId in departmentids)
            {
                departmentInfos.Add(departmentId, _hierarchyDepartmentService.GetHierarchyDepartmentIdentities(currentHD.HierarchyId, departmentId));
            }
            var result = new Dictionary<string, List<HierachyDepartmentIdentityDto>>();
            foreach (var user in users)
            {
                var entry = departmentInfos[user.DepartmentId];
                result.Add(user.ExtId, entry);
            }

            return result;

        }

        public List<HierachyDepartmentIdentityDto> GetUserHierachyDepartmentIdentitiesByExtId(string extId, int customerId = 0)
        {
            var user = _userRepository.GetUserByUserExtId(extId, customerId);
            if (user == null)
            {
                return null;
            }
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            return _hierarchyDepartmentService.GetHierarchyDepartmentIdentities(currentHD.HierarchyId, user.DepartmentId);
        }

        public List<HierachyDepartmentIdentityDto> GetUserHierachyDepartmentIdentitiesBySSN(string ssn, int customerId = 0)
        {
            var user = _userRepository.GetUserBySSN(ssn, false, customerId);
            if (user == null)
            {
                return null;
            }
            return null;
            //_workContext.CurrentHd.HierarchyId the old code will through exception because CurrentHd is not implemented
            //return _hierarchyDepartmentRepository.GetHierarchyDepartmentIdentities(_workContext.CurrentHd.HierarchyId, user.DepartmentId);
        }

        public IdentityStatusDto GetUserIdentityStatusByExtId(string extId, int customerId = 0)
        {
            var user = _userRepository.GetUserByUserExtId(extId, customerId);
            if (user == null)
            {
                return null;
            }
            //Check security
            //if (_securityHandler.AllowAccess(user, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _userMappingService.ToIdentityStatusDto(user);
            //}
            //return null;
        }

        public List<IdentityStatusDto> GetListUserIdentityStatusByExtId(string extId)
        {
            var userEntities = _userRepository.GetUsersByUserExtId(extId);

            //userEntities = _securityHandler.AllowAccess(userEntities, AccessBinaryValues.Read, false);

            List<IdentityStatusDto> result = new List<IdentityStatusDto>();
            foreach (var userEntity in userEntities)
            {
                var identityStatusDto = _userMappingService.ToIdentityStatusDto(userEntity);
                if (identityStatusDto != null)
                {
                    result.Add(identityStatusDto);
                }
            }
            return result;
        }

        public IdentityStatusDto GetUserIdentityStatusBySsn(string ssn, int customerId = 0)
        {
            var user = _userRepository.GetUserBySSN(ssn, false, customerId);
            if (user == null)
            {
                return null;
            }
            //Check security
            //if (_securityHandler.AllowAccess(user, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _userMappingService.ToIdentityStatusDto(user);
            //}
            //return null;
        }

        public IdentityStatusDto GetUserIdentityStatusById(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return null;
            }
            //Check security
            //if (_securityHandler.AllowAccess(user, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _userMappingService.ToIdentityStatusDto(user);
            //}
            //return null;
        }
        public UserEntity GetDefaultUserByCustomer(int customerId)
        {
            return _userRepository.GetDefaultUserByCustomer(customerId);
        }

        /// <summary>
        /// Get users.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <param name="customerIds"></param>
        /// <param name="userIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="statusIds"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="userTypeIds"></param>
        /// <param name="userTypeExtIds"></param>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="extIds"></param>
        /// <param name="ssnList"></param>
        /// <param name="userNames"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="ageRanges"></param>
        /// <param name="genders"></param>
        /// <param name="memberStatuses"></param>
        /// <param name="memberValidFromBefore"></param>
        /// <param name="memberValidFromAfter"></param>
        /// <param name="memberValidToBefore"></param>
        /// <param name="memberValidToAfter"></param>
        /// <param name="searchKey"></param>
        /// <param name="loginServiceClaims"></param>
        /// <param name="loginServiceClaimTypes"></param>
        /// <param name="loginServiceIds"></param>
        /// <param name="getDynamicProperties"></param>
        /// <param name="getLoginServiceClaims"></param>
        /// <param name="getRoles"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="filterOnParentHd"></param>
        /// <param name="includeUGMembers"></param>
        /// <param name="includeDepartment"></param>
        /// <param name="jsonDynamicData"></param>
        /// <param name="externallyMastered"></param>
        /// <param name="skipPaging"></param>
        /// <param name="createdAfter"></param>
        /// <param name="createdBefore"></param>
        /// <param name="expirationDateAfter"></param>
        /// <param name="expirationDateBefore"></param>
        /// <param name="orgUnittypeIds"></param>
        /// <param name="multiUserTypefilters"></param>
        /// <param name="filterOnSubDepartment"></param>
        /// <param name="multiUserGroupFilters"></param>
        /// <param name="multiUserTypeExtIdFilters"></param>
        /// <param name="currentWorkContext"></param>
        /// <param name="checkDepartmentPermission"></param>
        /// <param name="emails"></param>
        /// <param name="ignoreCheckReadUserAccess">NOTE: This flag for internal use only and should never be sent as the parameter in the controller action.</param>
        /// <param name="activeDateBefore"></param>
        /// <param name="activeDateAfter"></param>
        /// <returns></returns>
        public PaginatedList<T> GetUsers<T>(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool? filterOnParentHd = true,
            bool includeUGMembers = false,
            bool includeDepartment = false,
            List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            bool skipPaging = false,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            List<int> orgUnittypeIds = null,
            List<List<int>> multiUserTypefilters = null,
            bool? filterOnSubDepartment = null,
            List<List<int>> multiUserGroupFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            IWorkContext currentWorkContext = null,
            bool checkDepartmentPermission = false,
            List<string> emails = null,
            bool ignoreCheckReadUserAccess = false,
            bool includeOwnUserGroups = false,
            DateTime? activeDateBefore = null,
            DateTime? activeDateAfter = null,
            List<int> exceptUserIds = null) where T : ConexusBaseDto
        {
            if (currentWorkContext != null)
                _workContext = currentWorkContext;
            if (filterOnSubDepartment == true && !parentDepartmentIds.IsNullOrEmpty())
            {
                _logger.LogDebug($"Start retrieving departmentIds for filtering on sub-departments.'");
                var currentHd = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);

                parentDepartmentIds = currentHd == null
                    ? new List<int>()
                    : _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(currentHd.HierarchyId, parentDepartmentIds);
                _logger.LogDebug($"End retrieving departmentIds for filtering on sub-departments. {parentDepartmentIds.Count} departmentIds has been retrieved.");

                if (parentDepartmentIds.IsNullOrEmpty())
                    return new PaginatedList<T>();
            }

            if (!ignoreCheckReadUserAccess)
            {
                var userAccessChecking = _userAccessService.CheckReadUserAccess(workContext: _workContext,
                    ownerId: ownerId,
                    customerIds: customerIds,
                    userExtIds: extIds,
                    loginServiceClaims: loginServiceClaims,
                    userIds: userIds,
                    userGroupIds: userGroupIds,
                    parentDepartmentIds: parentDepartmentIds,
                    multiUserGroupFilters: multiUserGroupFilters,
                    userTypeIdsFilter: userTypeIds,
                    userTypeExtIdsFilter: userTypeExtIds,
                    multipleUserTypeIdsFilter: multiUserTypefilters,
                    multipleUserTypeExtIdsFilter: multiUserTypeExtIdFilters);

                if (!userAccessChecking.IsAllowedAccess)
                {
                    return new PaginatedList<T>();
                }

                userIds = userAccessChecking.UserIds;
                userGroupIds = userAccessChecking.UserGroupIds;
                parentDepartmentIds = userAccessChecking.ParentDepartmentIds;
                multiUserGroupFilters = userAccessChecking.MultiUserGroupFilters;
                multiUserTypefilters = userAccessChecking.MultiUserTypeFilters;

                //We already union filter userTypeIds,  userTypeExtIds, multiUserTypeExtIdFilters into multiUserTypefilters. so we don't need filter on this.
                userTypeIds = null;
                userTypeExtIds = null;
                multiUserTypeExtIdFilters = null;
            }

            _logger.LogDebug($"Start retrieving UserEntities with page index {pageIndex},  page size {pageSize}.");

            var includeDepartmentOption =
                includeDepartment
                    ? IncludeDepartmentOption.Department
                    : IncludeDepartmentOption.None;

            var includeUserTypeOption = getRoles == true
                ? IncludeUserTypeOption.UtUs
                : IncludeUserTypeOption.None;

            var includeUGMemberOption = IncludeUgMemberOption.None;

            var pagingEntity = _userRepository.GetUsers(ownerId: ownerId,
                customerIds: customerIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds,
                parentDepartmentIds: parentDepartmentIds,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                searchKey: searchKey,
                loginServiceClaimTypes: loginServiceClaimTypes,
                loginServiceIds: loginServiceIds,
                loginServiceClaims: loginServiceClaims,
                ageRanges: ageRanges,
                genders: genders,
                memberStatuses: memberStatuses,
                memberValidFromBefore: memberValidToBefore,
                memberValidFromAfter: memberValidFromAfter,
                memberValidToBefore: memberValidToBefore,
                memberValidToAfter: memberValidToAfter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                includeDepartment: includeDepartmentOption,
                includeLoginServiceUsers: getLoginServiceClaims == true,
                includeUserTypes: includeUserTypeOption,
                filterOnParentHd: filterOnParentHd == true,
                includeUGMembers: includeUGMemberOption,
                jsonDynamicData: jsonDynamicData,
                externallyMastered: externallyMastered,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                orgUnittypeIds: orgUnittypeIds,
                skipPaging: skipPaging,
                multiUserTypeFilters: multiUserTypefilters,
                multiUserGroupFilters: multiUserGroupFilters,
                multiUserTypeExtIdFilters: multiUserTypeExtIdFilters,
                emails: emails,
                includeOwnUserGroups: includeOwnUserGroups,
                entityActiveDateAfter: activeDateAfter,
                entityActiveDateBefore: activeDateBefore,
                exceptUserIds: exceptUserIds);

            _logger.LogDebug($"End retrieving UserEntities with page index {pagingEntity.PageIndex}, page size {pagingEntity.PageSize}. {pagingEntity.Items.Count} of {pagingEntity.TotalItems} has been returned.");

            Func<UserEntity, bool?, List<UGMemberEntity>, T> mapEntityToDtoFunc =
                (entity, getDynamicPropertiesFlag, ugMemberEntities)
                    => (T)_userMappingService.ToUserDto(entity, getDynamicProperties: getDynamicPropertiesFlag,
                        ugMemberEntities: ugMemberEntities);

            PaginatedList<T> paginatedDtos;
            var dtoTypeName = typeof(T).Name;

            List<UGMemberEntity> ugMembersOfUsers = null;
            if (includeUGMembers)
            {
                var existingUserIds = pagingEntity.Items
                    .Select(u => u.UserId)
                    .ToList();
                if (existingUserIds.Count > 0)
                {
                    _logger.LogDebug($"Start retrieving UGMember for finding parent user group of {pagingEntity.Items.Count} UserEntities");

                    ugMembersOfUsers =
                        _ugMemberRepository.GetUGMembers(userIds: existingUserIds, includeUserGroupUser: true, disableTracker: true);

                    _logger.LogDebug($"Start retrieving UGMember for finding parent user group  of {pagingEntity.Items.Count} UserEntities");
                }
            }
            _logger.LogDebug($"Start mapping {pagingEntity.Items.Count} UserEntities to {dtoTypeName}.");

            //Checking accessing entities if don't use paging feature 
            if (pageIndex == 0 && pageSize == 0)
            {
                // Check security
                // Comment out because of Conflicting with Paging Feature
                //var items = _securityHandler.AllowAccess(pagingEntity.Items, AccessBinaryValues.Read, false);
                var paging = new PaginatedList<UserEntity>(pagingEntity.Items, pagingEntity.PageIndex, pagingEntity.PageSize, pagingEntity.HasMoreData) { TotalItems = pagingEntity.TotalItems };
                paginatedDtos = paging.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties, ugMembersOfUsers);
            }
            else
            {
                paginatedDtos = pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties, ugMembersOfUsers);

            }
            _logger.LogDebug($"End mapping {pagingEntity.Items.Count} UserEntities to {dtoTypeName}.");

            return paginatedDtos;
        }

        /// <summary>
        /// Get users async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <param name="customerIds"></param>
        /// <param name="userIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="statusIds"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="userTypeIds"></param>
        /// <param name="userTypeExtIds"></param>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="extIds"></param>
        /// <param name="ssnList"></param>
        /// <param name="userNames"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="ageRanges"></param>
        /// <param name="genders"></param>
        /// <param name="memberStatuses"></param>
        /// <param name="memberValidFromBefore"></param>
        /// <param name="memberValidFromAfter"></param>
        /// <param name="memberValidToBefore"></param>
        /// <param name="memberValidToAfter"></param>
        /// <param name="searchKey"></param>
        /// <param name="loginServiceClaims"></param>
        /// <param name="loginServiceClaimTypes"></param>
        /// <param name="loginServiceIds"></param>
        /// <param name="getDynamicProperties"></param>
        /// <param name="getLoginServiceClaims"></param>
        /// <param name="getRoles"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="filterOnParentHd"></param>
        /// <param name="includeUGMembers"></param>
        /// <param name="includeDepartment"></param>
        /// <param name="jsonDynamicData"></param>
        /// <param name="externallyMastered"></param>
        /// <param name="skipPaging"></param>
        /// <param name="createdAfter"></param>
        /// <param name="createdBefore"></param>
        /// <param name="expirationDateAfter"></param>
        /// <param name="expirationDateBefore"></param>
        /// <param name="orgUnittypeIds"></param>
        /// <param name="multiUserTypefilters"></param>
        /// <param name="filterOnSubDepartment"></param>
        /// <param name="multiUserGroupFilters"></param>
        /// <param name="multiUserTypeExtIdFilters"></param>
        /// <param name="currentWorkContext"></param>
        /// <param name="checkDepartmentPermission"></param>
        /// <param name="emails"></param>
        /// <param name="ignoreCheckReadUserAccess">NOTE: This flag for internal use only and should never be sent as the parameter in the controller action.</param>
        /// <param name="activeDateBefore"></param>
        /// <param name="activeDateAfter"></param>
        /// <returns></returns>
        public async Task<PaginatedList<T>> GetUsersAsync<T>(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool? filterOnParentHd = true,
            bool includeUGMembers = false,
            bool includeDepartment = false,
            List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            bool skipPaging = false,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            List<int> orgUnittypeIds = null,
            List<List<int>> multiUserTypefilters = null,
            bool? filterOnSubDepartment = null,
            List<string> departmentExtIds = null,
            List<List<int>> multiUserGroupFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            IWorkContext currentWorkContext = null,
            List<string> emails = null,
            bool ignoreCheckReadUserAccess = false,
            bool includeOwnUserGroups = false,
            DateTime? activeDateBefore = null,
            DateTime? activeDateAfter = null,
            List<int> exceptUserIds = null,
            bool isCrossOrganizationalUnit = false,
            List<string> systemRolePermissions = null,
            string token = null) where T : ConexusBaseDto
        {
            if (currentWorkContext != null)
                _workContext = currentWorkContext;

            if (filterOnSubDepartment == true && !parentDepartmentIds.IsNullOrEmpty())
            {
                _logger.LogDebug($"Start retrieving departmentIds for filtering on sub-departments.'");
                var currentHd = await _hierarchyDepartmentRepository.GetByIdAsync(_workContext.CurrentHdId);

                parentDepartmentIds = currentHd == null
                    ? new List<int>()
                    : await _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToBelowAsync(currentHd.HierarchyId, parentDepartmentIds);
                _logger.LogDebug($"End retrieving departmentIds for filtering on sub-departments. {parentDepartmentIds.Count} departmentIds has been retrieved.");

                if (parentDepartmentIds.IsNullOrEmpty())
                    return new PaginatedList<T>();
            }
            if (!ignoreCheckReadUserAccess && !isCrossOrganizationalUnit)
            {
                var userAccessChecking = await _userAccessService.CheckReadUserAccessAsync(workContext: _workContext,
                    ownerId: ownerId,
                    customerIds: customerIds,
                    userExtIds: extIds,
                    loginServiceClaims: loginServiceClaims,
                    userIds: userIds,
                    userGroupIds: userGroupIds,
                    parentDepartmentIds: parentDepartmentIds,
                    multiUserGroupFilters: multiUserGroupFilters,
                    userTypeIdsFilter: userTypeIds,
                    userTypeExtIdsFilter: userTypeExtIds,
                    multipleUserTypeIdsFilter: multiUserTypefilters,
                    multipleUserTypeExtIdsFilter: multiUserTypeExtIdFilters);

                if (!userAccessChecking.IsAllowedAccess)
                {
                    return new PaginatedList<T>();
                }

                userIds = userAccessChecking.UserIds;
                userGroupIds = userAccessChecking.UserGroupIds;
                parentDepartmentIds = userAccessChecking.ParentDepartmentIds;
                multiUserGroupFilters = userAccessChecking.MultiUserGroupFilters;
                multiUserTypefilters = userAccessChecking.MultiUserTypeFilters;

                //We already union filter userTypeIds,  userTypeExtIds, multiUserTypeExtIdFilters into multiUserTypefilters. so we don't need filter on this.
                userTypeIds = null;
                userTypeExtIds = null;
                multiUserTypeExtIdFilters = null;

            }

            _logger.LogDebug($"Start retrieving UserEntities with page index {pageIndex},  page size {pageSize}.");

            var includeDepartmentOption =
                includeDepartment
                    ? IncludeDepartmentOption.Department
                    : IncludeDepartmentOption.None;

            var includeUserTypeOption = getRoles == true
                ? IncludeUserTypeOption.UtUs
                : IncludeUserTypeOption.None;

            var includeUGMemberOption = IncludeUgMemberOption.None;

            if (isCrossOrganizationalUnit)
            {
                // 15813 - Testing only, should be replaced by ExtId
                var toBelowDepartmentIds = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(15813);
                parentDepartmentIds.AddRange(toBelowDepartmentIds);
            }

            if (systemRolePermissions is object)
            {

                var baseUrl = _appSettings.PortalAPI + "/SystemRoles/FindByPermissionKeys";
                var systemRoleIdsBasedOnPermissions = await _internalHttpClientRequestService.GetAsync<List<int>>(token,
                                                                                       baseUrl,
                                                                                       ("PermissionKeys", systemRolePermissions),
                                                                                       ("LogicalOperator", new List<string>() { "AND" }));
                if (multiUserTypefilters is null)
                {
                    multiUserTypefilters = new List<List<int>>();
                }

                if (!multiUserTypefilters.Any())
                {
                    multiUserTypefilters.Add(systemRoleIdsBasedOnPermissions);
                }
                else
                {
                    multiUserTypefilters[0] = multiUserTypefilters[0].Intersect(systemRoleIdsBasedOnPermissions).ToList();

                    if (!multiUserTypefilters[0].Any())
                    {
                        multiUserTypefilters[0].Add(-99);
                    }
                }

            }

            var pagingEntity = await _userRepository.GetUsersAsync(ownerId: ownerId,
                customerIds: customerIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds,
                parentDepartmentIds: parentDepartmentIds,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                searchKey: searchKey,
                loginServiceClaimTypes: loginServiceClaimTypes,
                loginServiceIds: loginServiceIds,
                loginServiceClaims: loginServiceClaims,
                ageRanges: ageRanges,
                genders: genders,
                memberStatuses: memberStatuses,
                memberValidFromBefore: memberValidToBefore,
                memberValidFromAfter: memberValidFromAfter,
                memberValidToBefore: memberValidToBefore,
                memberValidToAfter: memberValidToAfter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                includeDepartment: includeDepartmentOption,
                departmentExtIds: departmentExtIds,
                includeLoginServiceUsers: getLoginServiceClaims == true,
                includeUserTypes: includeUserTypeOption,
                filterOnParentHd: filterOnParentHd == true,
                includeUGMembers: includeUGMemberOption,
                jsonDynamicData: jsonDynamicData,
                externallyMastered: externallyMastered,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                orgUnittypeIds: orgUnittypeIds,
                skipPaging: skipPaging,
                multiUserTypeFilters: multiUserTypefilters,
                multiUserGroupFilters: multiUserGroupFilters,
                multiUserTypeExtIdFilters: multiUserTypeExtIdFilters,
                emails: emails,
                includeOwnUserGroups: includeOwnUserGroups,
                entityActiveDateAfter: activeDateAfter,
                entityActiveDateBefore: activeDateBefore,
                exceptUserIds: exceptUserIds);

            _logger.LogDebug($"End retrieving UserEntities with page index {pagingEntity.PageIndex}, page size {pagingEntity.PageSize}. {pagingEntity.Items.Count} of {pagingEntity.TotalItems} has been returned.");

            Func<UserEntity, bool?, List<UGMemberEntity>, T> mapEntityToDtoFunc =
                (entity, getDynamicPropertiesFlag, ugMemberEntities)
                    => (T)_userMappingService.ToUserDto(entity, getDynamicProperties: getDynamicPropertiesFlag,
                        ugMemberEntities: ugMemberEntities);

            PaginatedList<T> paginatedDtos;
            var dtoTypeName = typeof(T).Name;

            List<UGMemberEntity> ugMembersOfUsers = null;
            if (includeUGMembers)
            {
                var existingUserIds = pagingEntity.Items
                    .Select(u => u.UserId)
                    .ToList();
                if (existingUserIds.Count > 0)
                {
                    _logger.LogDebug($"Start retrieving UGMember for finding parent user group of {pagingEntity.Items.Count} UserEntities");

                    const int maxItemPerTime = 2000;
                    if (existingUserIds.Count <= maxItemPerTime)
                    {
                        ugMembersOfUsers = await _ugMemberRepository.GetUGMembersAsync(userIds: existingUserIds, includeUserGroupUser: true, disableTracker: true);
                    }
                    else
                    {
                        //To avoid exception of query with to many parameters, we slit userIds to smaller list.
                        ugMembersOfUsers = new List<UGMemberEntity>();
                        var splittedUserIds = existingUserIds.Split(maxItemPerTime);

                        foreach (var splittingUserIds in splittedUserIds)
                        {
                            var currentUgMembersOfUsers = await _ugMemberRepository.GetUGMembersAsync(userIds: splittingUserIds, includeUserGroupUser: true, disableTracker: true);
                            ugMembersOfUsers.AddRange(currentUgMembersOfUsers);
                        }
                    }

                    _logger.LogDebug($"Start retrieving UGMember for finding parent user group  of {pagingEntity.Items.Count} UserEntities");
                }
            }
            _logger.LogDebug($"Start mapping {pagingEntity.Items.Count} UserEntities to {dtoTypeName}.");

            //Checking accessing entities if don't use paging feature 
            if (pageIndex == 0 && pageSize == 0)
            {
                // Check security
                // Comment out because of Conflicting with Paging Feature
                //var items = _securityHandler.AllowAccess(pagingEntity.Items, AccessBinaryValues.Read, false);
                var paging = new PaginatedList<UserEntity>(pagingEntity.Items, pagingEntity.PageIndex, pagingEntity.PageSize, pagingEntity.HasMoreData) { TotalItems = pagingEntity.TotalItems };
                paginatedDtos = paging.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties, ugMembersOfUsers);
            }
            else
            {
                paginatedDtos = pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties, ugMembersOfUsers);

            }
            _logger.LogDebug($"End mapping {pagingEntity.Items.Count} UserEntities to {dtoTypeName}.");

            return paginatedDtos;
        }

        public async Task<PaginatedList<UserEntity>> GetAllUsers(int pageIndex)
        {
            return await _userRepository.GetAllUsers(pageIndex);
        }

        public PaginatedList<T> SearchActors<T>(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            List<int> exceptUserIds = null) where T : ConexusBaseDto
        {
            var pagingEntity = _userRepository.SearchActors(ownerId: ownerId,
                customerIds: customerIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds,
                parentDepartmentIds: parentDepartmentIds,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                searchKey: searchKey,
                loginServiceClaimTypes: loginServiceClaimTypes,
                loginServiceIds: loginServiceIds,
                loginServiceClaims: loginServiceClaims,
                ageRanges: ageRanges,
                genders: genders,
                memberStatuses: memberStatuses,
                memberValidFromBefore: memberValidToBefore,
                memberValidFromAfter: memberValidFromAfter,
                memberValidToBefore: memberValidToBefore,
                memberValidToAfter: memberValidToAfter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                includeLoginServiceUsers: getLoginServiceClaims == true,
                includeUserTypes: getRoles == true,
                exceptUserIds: exceptUserIds);

            Func<UserEntity, bool?, List<UserGroupEntity>, T> mapEntityToDtoFunc =
                (entity, getDynamicPropertiesFlag, parentUserGroupEntities)
                    => (T)_userMappingService.ToUserDto(entity, getDynamicPropertiesFlag);

            //Checking accessing entities if don't use paging feature 
            if (pageIndex == 0 && pageSize == 0)
            {
                // Check security
                // Comment out because of Conflicting with Paging Feature
                //var items = _securityHandler.AllowAccess(pagingEntity.Items, AccessBinaryValues.Read, false);
                var paging = new PaginatedList<UserEntity>(pagingEntity.Items, pagingEntity.PageIndex, pagingEntity.PageSize, pagingEntity.HasMoreData);
                return paging.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties);
            }
            return pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties);
        }
        public List<ConexusBaseDto> SearchUsers(string searchKey, int departmentId, int hdId = 0, bool deepSearch = false, int maxTake = 0)
        {
            List<UserEntity> usersSearched = new List<UserEntity>();
            var allKeys = searchKey.Split(' ');
            if (allKeys.Length > 1)
            {
                var firstSearchKey = allKeys[0];
                allKeys[0] = "";
                var secondSearchKey = string.Join(" ", allKeys).TrimStart();
                usersSearched = _userRepository.SearchUser(firstSearchKey, secondSearchKey, departmentId, hdId, deepSearch, maxTake);
            }
            else
            {
                usersSearched = _userRepository.SearchUser(searchKey, departmentId, hdId, deepSearch, maxTake);
            }

            var results = new List<ConexusBaseDto>();
            foreach (var item in usersSearched)
            {
                var userDto = _userMappingService.ToUserDto(item);
                if (userDto != null)
                    results.Add(userDto);
            }
            return results;
        }
        public ConexusBaseDto InsertUser(UserDtoBase userDto)
        {
            var parentDepartment = _departmentRepository.GetById(userDto.GetParentDepartmentId());

            //Do the validation
            var entity = _validator.Validate(parentDepartment, userDto);

            //Map to User Entity
            var userEntity = _userMappingService.ToUserEntity(parentDepartment, entity, userDto);

            //Check security
            //_securityHandler.AllowAccess(departmentEntity, true);

            var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
            //Insert to DB
            userEntity = _userRepository.Insert(userEntity, currentOwner.UseHashPassword, currentOwner.UseOTP, currentOwner.DefaultHashMethod);
            _organizationDbContext.SaveChanges();

            //Remap to userdtoBase
            return _userMappingService.ToUserDto(userEntity);
        }
        public ConexusBaseDto UpdateUser(UserDtoBase userDto)
        {
            var parentDepartment = _departmentRepository.GetById(userDto.GetParentDepartmentId());

            //Do the validation
            var entity = _validator.ValidateForUpdating(parentDepartment, userDto);
            //Map to Department Entity
            var userEntity = _userMappingService.ToUserEntity(parentDepartment, entity, userDto);

            if (userEntity == null) return null;

            //Check security
            //if (_securityHandler.AllowAccess(newUserEntity, AccessBinaryValues.Update, false).AceType == AccessControlEntryType.AccessDenied)
            //{
            //    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_USER);
            //}

            userEntity = UpdateUser(userEntity);

            return _userMappingService.ToUserDto(userEntity);
        }

        private UserEntity UpdateUser(UserEntity userEntity)
        {
            var saved = false;
            while (!saved)
            {
                try
                {
                    // Attempt to save changes to the database
                    userEntity = _userRepository.Update(userEntity);
                    _organizationDbContext.SaveChanges();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        _logger.LogWarning("DbUpdateConcurrencyException happened, trying to overwrite database values: " + entry.Metadata.Name);
                        if (entry.Entity is UserEntity)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();
                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = databaseValues[property];
                                // TODO: decide which value should be written to database
                                proposedValues[property] = proposedValue;
                                _logger.LogWarning($"DbUpdateConcurrencyException-override value>> {property.Name}: {proposedValue}");
                                //Currently we try to overwrite db values when currency happened.
                            }
                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                            entry.CurrentValues.SetValues(proposedValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }

            return userEntity;
        }

        public void MoveLearnersToSchool(int schoolId,
            List<int> learnerIds,
            int graduateUsertypeId,
            int newStudentUserTypeId,
            int receivingDepartmentPropertyId,
            int currentPeriodId,
            int toPeriodId,
            int fromPeriodId)
        {
            ValidateMoveLearnsToSchool(schoolId);

            List<UserEntity> users = _userRepository.GetUsersIncludeUserTypes(learnerIds.ToList(), false);
            var allObjectMappingsForNewSchoolYearUT = _commonService.GetObjectMappingsByRelationTypeId((int)RelationTypes.NewSchoolYearUT);

            foreach (UserEntity user in users)
            {
                if (user == null || user.EntityStatusId != (short)EntityStatusEnum.Active)//not Exist or Deleted
                {
                    //return errorMsg;
                }
                //Move user to new department
                user.DepartmentId = schoolId;

                List<int> userTypesNeedToRemove = new List<int>();
                List<int> userTypesNeedToAdd = new List<int>();
                HandleNewSchoolYearForMoveLearnersToSchool(currentPeriodId, toPeriodId, fromPeriodId, allObjectMappingsForNewSchoolYearUT, user, ref userTypesNeedToRemove, ref userTypesNeedToAdd);
                user.UT_Us = user.UT_Us.Where(p => userTypesNeedToRemove.Contains(p.UserTypeId)).ToList();
                foreach (var userTypeId in userTypesNeedToAdd)
                {
                    var userType = _userTypeRepository.GetById(userTypeId);
                    if (userType != null)
                    {
                        user.UT_Us.Add(new UTUEntity { UserTypeId = userType.UserTypeId, UserId = user.UserId });
                    }
                }

                //Remove graduate user type
                var graduateUsertype = user.UT_Us.FirstOrDefault(ut => ut.UserTypeId == graduateUsertypeId);
                if (graduateUsertype != null)
                {
                    user.UT_Us.Remove(graduateUsertype);
                }
                //Add new student user type
                var fresherUserType = _userTypeRepository.GetById(newStudentUserTypeId);
                if (fresherUserType != null)
                {
                    user.UT_Us.Add(new UTUEntity { UserTypeId = fresherUserType.UserTypeId, UserId = user.UserId });
                }
                var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
                _userRepository.Update(user, currentOwner.UseHashPassword, currentOwner.UseOTP, currentOwner.DefaultHashMethod);

                ////Save the info of source school for later references
                //PropValue propReceivingDepartment = _propertyService.FindPropValueForInsertUpdate(user.UserId, receivingDepartmentPropertyId);
                //if (propReceivingDepartment != null)
                //{
                //    propReceivingDepartment.Value = schoolid.ToString();
                //    propReceivingDepartment.Updated = DateTime.Now;
                //    _propertyService.UpdatePropValue(propReceivingDepartment);
                //}
                //else
                //{
                //    propReceivingDepartment = new PropValue
                //    {
                //        PropertyId = receivingDepartmentPropertyId,
                //        ItemId = user.UserId,
                //        Value = schoolid.ToString(),
                //        CreatedBy = _workContext.CurrentUserId,
                //        Updated = DateTime.Now,
                //        UpdatedBy = _workContext.CurrentUserId
                //    };
                //    _propertyService.InsertPropValue(propReceivingDepartment);
                //}
            }
            _organizationDbContext.SaveChanges();
        }

        private void HandleNewSchoolYearForMoveLearnersToSchool(int currentPeriodId,
            int toPeriodId,
            int fromPeriodId,
            List<ObjectMappingEntity> allObjectMappingsForNewSchoolYearUT,
            UserEntity user,
            ref List<int> userTypesNeedToRemove,
            ref List<int> userTypesNeedToAdd)
        {
            if (fromPeriodId != toPeriodId)
            {
                //If receiving department has done NSY and source department has not, upgrade moving student to higher level
                if (fromPeriodId == currentPeriodId)
                {
                    var userTypeIds = user.UT_Us.Select(p => p.UserTypeId).ToList();
                    var objectMappings = allObjectMappingsForNewSchoolYearUT.Where(p => p.ToTableTypeId == (int)TableTypes.User &&
                    p.FromTableTypeId == (int)TableTypes.User &&
                    userTypeIds.Any(u => u == p.FromId));

                    userTypesNeedToRemove = objectMappings.Select(p => p.FromId).ToList();
                    userTypesNeedToAdd = objectMappings.Select(p => p.ToId).ToList();
                }
                //If source department has done NSY and receiving department has not, downgrade moving student to lower level
                else if (toPeriodId == currentPeriodId)
                {
                    var userTypeIds = user.UT_Us.Select(p => p.UserTypeId).ToList();
                    var objectMappings = allObjectMappingsForNewSchoolYearUT.Where(p => p.ToTableTypeId == (int)TableTypes.User &&
                    p.FromTableTypeId == (int)TableTypes.User &&
                    userTypeIds.Any(u => u == p.ToId));

                    userTypesNeedToRemove = objectMappings.Select(p => p.ToId).ToList();
                    userTypesNeedToAdd = objectMappings.Select(p => p.FromId).ToList();
                }
            }
        }

        private void ValidateMoveLearnsToSchool(int schoolId)
        {
            var toDepartment = _departmentRepository.GetById(schoolId);
            if (toDepartment == null || toDepartment.EntityStatusId != (short)EntityStatusEnum.Active)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_SCHOOL_NOT_FOUND, schoolId);
            }
            if (toDepartment.ArchetypeId != (int)ArchetypeEnum.School)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            if (toDepartment.CustomerId == null || toDepartment.CustomerId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_REQUIRED, string.Format("Missing config for CustomerID of the department " + toDepartment.Name + "(" + toDepartment.DepartmentId + ")"));
            }
        }

        public void MoveLearnersToClass(int classId, List<int> learnerIds, int newStudentUserTypeId)
        {
            List<UserEntity> users = new List<UserEntity>();
            ValidateMoveLearnsToClass(classId, learnerIds, ref users);

            foreach (var user in users)
            {
                if (user.EntityStatusId != (short)EntityStatusEnum.Active)//not Exist or Deleted
                {
                    //will write warning
                    //return errorMsg;
                }
                //Move user to new class
                user.DepartmentId = classId;
                //Remove New Student user type
                var newStudentUserType = user.UT_Us.FirstOrDefault(ut => ut.UserTypeId == newStudentUserTypeId);
                if (newStudentUserType != null)
                {
                    user.UT_Us.Remove(newStudentUserType);
                }
                var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
                _userRepository.Update(user, currentOwner.UseHashPassword, currentOwner.UseOTP, currentOwner.DefaultHashMethod);
            }
            _organizationDbContext.SaveChanges();
        }

        private void ValidateMoveLearnsToClass(int classId, List<int> learnerIds, ref List<UserEntity> users)
        {
            var toDepartment = _departmentRepository.GetDepartmentIncludeDepartmentTypes(classId, _workContext.CurrentOwnerId);
            if (toDepartment == null || toDepartment.EntityStatusId != (short)EntityStatusEnum.Active)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CLASS_NOT_FOUND);
            }
            if (toDepartment.ArchetypeId != (int)ArchetypeEnum.Class)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ARCHETYPE_DB_NOT_MATCH);
            }
            if (toDepartment.CustomerId == null || toDepartment.CustomerId == 0)
            {
                throw new CXValidationException(cxExceptionCodes.VALIDATION_CUSTOMERID_REQUIRED, string.Format("Missing config for CustomerID of the department " + toDepartment.Name + "(" + toDepartment.DepartmentId + ")"));
            }

            users = _userRepository.GetUsersIncludeUserTypes(learnerIds.ToList(), false);
            var departmentTypes = GetDepartmentTypesFromUserTypes(users);
            var allowToMove = departmentTypes.All(depTypeId => toDepartment.DT_Ds.Any(dt => dt.DepartmentTypeId == depTypeId));
            if (!allowToMove)
            {
                //throw new CXValidationException(cxExceptionCodes.VALIDATION_DO_NOT_ALLOW_TO_MOVE, "learner", "DepartmentTypes of receive class do not match");
            }
        }

        private List<int> GetDepartmentTypesFromUserTypes(List<UserEntity> users)
        {
            var departmentTypes = new List<int>();
            List<ObjectMappingEntity> allMappings = _commonService.GetObjectMappingsByOwnerId(_workContext.CurrentOwnerId);
            List<ObjectMappingEntity> objectMappingsForNewSchoolYearUT = _commonService.FindObjectByMapping(allMappings, (int)TableTypes.UserType, 0, 0, (int)RelationTypes.NewSchoolYearUT);

            foreach (var user in users)
            {
                foreach (UTUEntity userTypeUser in user.UT_Us)
                {
                    if (objectMappingsForNewSchoolYearUT.Any(objectMapping =>
                    userTypeUser.UserTypeId == objectMapping.FromId ||
                    userTypeUser.UserTypeId == objectMapping.ToId))
                    {
                        List<ObjectMappingEntity> objectMappings = _commonService.FindObjectByMapping(allMappings,
                            (int)TableTypes.UserType,
                            userTypeUser.UserTypeId,
                            (int)TableTypes.DepartmentType, (int)RelationTypes.Standard);
                        if (objectMappings.Count > 0)
                        {
                            if (!departmentTypes.Contains(objectMappings[0].ToId))
                            {
                                departmentTypes.Add(objectMappings[0].ToId);
                            }
                        }
                    }
                }
            }

            return departmentTypes;
        }

        public List<UserEntity> GetListUsersByDepartmentIds(IEnumerable<int> departmentIds, bool includeLinkedUsers = false)
        {
            return _userRepository.GetListUserByDepartmentIds(departmentIds, includeLinkedUsers);
        }

        public bool CheckUsername(int ownerId, int userId, string userName)
        {
            return _userRepository.CheckUsername(ownerId, userId, userName);
        }
        public UserEntity GetUserForUpdateInsert(int userId)
        {
            return _userRepository.GetUserForUpdateInsert(userId);
        }
        public UserEntity UpdateUser(UserEntity user, bool changePassword = false, bool generateRandomPassword = true, string updatedPassword = "")
        {
            var useHashPassword = 0;
            var useOTP = false;
            var defaultHashMethod = 0;
            if (changePassword)
            {
                var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
                useHashPassword = currentOwner.UseHashPassword;
                useOTP = currentOwner.UseOTP;
                defaultHashMethod = currentOwner.DefaultHashMethod;
            }
            return _userRepository.Update(user, useHashPassword, useOTP, defaultHashMethod, changePassword, generateRandomPassword, updatedPassword);
        }
        public UserEntity InsertUser(UserEntity user)
        {
            var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
            return _userRepository.Insert(user, currentOwner.UseHashPassword, currentOwner.UseOTP, currentOwner.DefaultHashMethod);
        }
        public List<UserEntity> GetUsersByUsernameForUpdate(int ownerId, string username, params EntityStatusEnum[] filters)
        {
            return _userRepository.GetUsersByUsernameForUpdate(ownerId, username, filters);
        }
        public UserEntity GetUserIncludeDepartmentIncludeUserTypesIncludeUserGroups(int userId, bool putToCache = true)
        {
            return _userRepository.GetUserIncludeDepartmentIncludeUserTypesIncludeUserGroups(userId, putToCache);
        }

        public LogonResponseDto NativeLogin(string userName, string password)
        {
            var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
            var logonResponseDto = new LogonResponseDto { SignInStatus = SignInStatus.Failure };
            var useHashPassword = (UseHashPasswordStatus)currentOwner.UseHashPassword;
            var user = GetUsersByUsernameForUpdate(_workContext.CurrentOwnerId, userName, EntityStatusEnum.Active, EntityStatusEnum.Pending).FirstOrDefault();
            if (user != null)
            {
                logonResponseDto.Identity = new IdentityDto
                {
                    Id = user.UserId,
                    Archetype = (ArchetypeEnum)user.ArchetypeId,
                    CustomerId = user.CustomerId ?? 0,
                    ExtId = user.ExtId,
                    OwnerId = user.OwnerId
                };
                bool validPassword;
                switch (useHashPassword)
                {
                    case UseHashPasswordStatus.Use:
                        validPassword = _cryptographyService(currentOwner.DefaultHashMethod.ToString(CultureInfo.InvariantCulture))
                            .VerifyPassword(password, user.SaltPassword, user.HashPassword);
                        if (validPassword)
                        {
                            logonResponseDto.SignInStatus = SignInStatus.Success;
                            SetLogonDynamicProperties(user, logonResponseDto);
                        }
                        break;
                    case UseHashPasswordStatus.None:
                        LoginUsingEncryptPassword(password, user, logonResponseDto);
                        break;
                    case UseHashPasswordStatus.Both:
                        validPassword = _cryptographyService(currentOwner.DefaultHashMethod.ToString(CultureInfo.InvariantCulture))
                            .VerifyPassword(password, user.SaltPassword, user.HashPassword);
                        if (validPassword)
                        {
                            logonResponseDto.SignInStatus = SignInStatus.Success;
                            SetLogonDynamicProperties(user, logonResponseDto);
                        }
                        else
                        {
                            LoginUsingEncryptPassword(password, user, logonResponseDto);
                        }
                        break;
                }
            }
            else
            {
                logonResponseDto.SignInStatus = SignInStatus.InvalidUsername;
            }
            if (user.Locked == 1)
            {
                logonResponseDto.SignInStatus = SignInStatus.LockedOut;
            }
            return logonResponseDto;
        }

        private void SetLogonDynamicProperties(UserEntity user, LogonResponseDto logonResponseDto)
        {
            logonResponseDto.DynamicAttributes = new List<EntityKeyValueDto>();
            foreach (var utu in user.UT_Us)
            {
                if (!string.IsNullOrEmpty(utu?.UserType?.ExtId))
                {
                    logonResponseDto.DynamicAttributes.Add(new EntityKeyValueDto
                    {
                        Key = "UserType",
                        Value = utu?.UserType?.ExtId
                    });
                }
            }
        }

        private void LoginUsingEncryptPassword(string password, UserEntity user, LogonResponseDto logonResponseDto)
        {
            bool validPassword;
            var encryptPassword = Encryption.Encrypt(password);
            var currentOwner = _ownerRepository.GetById(_workContext.CurrentOwnerId);
            if (currentOwner.UseOTPCaseSensitive)
            {
                bool descyptSuccess = true;
                var decryptOtpPassword = Encryption.Decrypt(user.OneTimePassword, ref descyptSuccess);
                if (user.EntityStatusId == (int)EntityStatusEnum.Pending)
                {
                    // Pending user is only allowed log-in using one time password.
                    validPassword = (string.Equals(decryptOtpPassword, password, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    validPassword = (user.Password == encryptPassword ||
                                  string.Equals(decryptOtpPassword, password, StringComparison.OrdinalIgnoreCase))
                                && (user.EntityStatusId == (int)EntityStatusEnum.Active);
                }
            }
            else
            {
                bool descyptSuccess = true;
                var decryptOtpPassword = Encryption.Decrypt(user.OneTimePassword, ref descyptSuccess);
                if (user.EntityStatusId == (int)EntityStatusEnum.Pending)
                {
                    // Pending user is only allowed log-in using one time password.
                    validPassword = (decryptOtpPassword == password);
                }
                else
                {
                    validPassword = (user.Password == encryptPassword || decryptOtpPassword == password)
                                && (user.EntityStatusId == (int)EntityStatusEnum.Active);
                }
            }

            if (validPassword)
            {
                logonResponseDto.SignInStatus = SignInStatus.Success;
                SetLogonDynamicProperties(user, logonResponseDto);
            }
        }

        public List<IdentityStatusDto> UpdateUserLastSyncDate(List<IdentityStatusDto> users)
        {
            var userIds = users.Select(x => (int)x.Identity.Id).ToList();
            var usersDb = _userRepository.GetUserByIds(userIds, null);
            var results = new List<IdentityStatusDto>();

            foreach (var user in usersDb)
            {

                var info = users.FirstOrDefault(x => x.Identity.Id == user.UserId);
                if (info == null)
                    continue;
                user.LastSynchronized = DateTime.Now;
                results.Add(_userMappingService.ToIdentityStatusDto(_userRepository.Update(user)));

            }
            _organizationDbContext.SaveChanges();
            return results;
        }

        public List<IdentityStatusDto> GetUserIdentitiesByObjectMapping(List<int> userIds)
        {
            var result = new List<IdentityStatusDto>();
            var objectMapping = _objectMappingRepository.GetObjectMappings(_workContext.CurrentOwnerId, TableTypes.User, userIds, TableTypes.User, relationTypeId: 5);
            var userIdsInDb = objectMapping.Select(x => x.ToId).ToList();
            if (userIdsInDb.Any())
            {
                var userEntities = _userRepository.GetUserByIds(userIdsInDb, null);
                foreach (var userEntity in userEntities)
                {
                    var identityStatusDto = _userMappingService.ToIdentityStatusDto(userEntity);
                    if (identityStatusDto != null)
                    {
                        result.Add(identityStatusDto);
                    }
                }
            }
            return result;
        }

        public int CountUsers(int ownerId = 0,
         List<int> customerIds = null,
         List<int> userIds = null,
         List<int> userGroupIds = null,
         List<EntityStatusEnum> statusIds = null,
         List<ArchetypeEnum> archetypeIds = null,
         List<int> userTypeIds = null,
         List<string> userTypeExtIds = null,
         List<int> parentDepartmentIds = null,
         List<string> extIds = null,
         List<string> ssnList = null,
         List<string> userNames = null,
         DateTime? lastUpdatedBefore = null,
         DateTime? lastUpdatedAfter = null,
         List<AgeRange> ageRanges = null,
         List<Gender> genders = null,
         bool? filterOnParentHd = true,
         List<string> jsonDynamicData = null,
         DateTime? createdAfter = null,
         DateTime? createdBefore = null,
         bool? externallyMastered = null,
         List<int> exceptUserIds = null)
        {
            return _userRepository.CountUsers(ownerId: ownerId,
                customerIds: customerIds, userIds: userIds, userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds,
                parentDepartmentIds: parentDepartmentIds,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                ageRanges: ageRanges,
                genders: genders,
                filterOnParentHd: filterOnParentHd == true,
                jsonDynamicData: jsonDynamicData,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                externallyMastered: externallyMastered,
                exceptUserIds: exceptUserIds);
        }

        public async Task<int> CountUsersAsync(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            bool? filterOnParentHd = true,
            List<string> jsonDynamicData = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            bool? externallyMastered = null,
            List<int> exceptUserIds = null)
        {
            return await _userRepository.CountUsersAsync(ownerId: ownerId,
                customerIds: customerIds, userIds: userIds, userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                userTypeExtIds: userTypeExtIds,
                parentDepartmentIds: parentDepartmentIds,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                ageRanges: ageRanges,
                genders: genders,
                filterOnParentHd: filterOnParentHd == true,
                jsonDynamicData: jsonDynamicData,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                externallyMastered: externallyMastered,
                exceptUserIds: exceptUserIds);
        }

        public async Task<CountUserResultDto> CountUserGroupByAsync(int ownerId,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,

            List<int> departmentIds = null,
            List<string> extIds = null,
            List<string> jsonDynamicData = null,
            List<int> exceptUserIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<string>> multiUserTypeExIdFilters = null,

            List<List<int>> multiUserGroupFilters = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            UserGroupByField groupByField = UserGroupByField.None)
        {

            var userAccessChecking = await _userAccessService.CheckReadUserAccessAsync(workContext: _workContext,
                ownerId: ownerId,
                customerIds: customerIds,
                userExtIds: extIds,
                loginServiceClaims: null,
                userIds: userIds,
                userGroupIds: userGroupIds,
                parentDepartmentIds: departmentIds,
                multiUserGroupFilters: multiUserGroupFilters,
                userTypeIdsFilter: userTypeIds,
                userTypeExtIdsFilter: userTypeExtIds,
                multipleUserTypeIdsFilter: multiUserTypeFilters,
                multipleUserTypeExtIdsFilter: multiUserTypeExIdFilters);

            if (!userAccessChecking.IsAllowedAccess)
            {
                return new CountUserResultDto
                {
                    GroupByField = groupByField,
                    CountValues = new List<CountUserValueDto>()
                };
            }

            userIds = userAccessChecking.UserIds;
            departmentIds = userAccessChecking.ParentDepartmentIds;
            multiUserGroupFilters = userAccessChecking.MultiUserGroupFilters;
            multiUserTypeFilters = userAccessChecking.MultiUserTypeFilters;
            userGroupIds = userAccessChecking.UserGroupIds;

            var userCountingData = await _userRepository.CountUserGroupByAsync(ownerId: ownerId,
                customerIds: customerIds,
                userIds: userIds,
                userGroupIds: userGroupIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                userTypeIds: userTypeIds,
                departmentIds: departmentIds,
                extIds: extIds,
                jsonDynamicData: jsonDynamicData,
                exceptUserIds: exceptUserIds,
                multiUserTypeFilters: multiUserTypeFilters,
                multiUserGroupFilters: multiUserGroupFilters,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                createdBefore: createdBefore,
                createdAfter: createdAfter,
                groupByField: groupByField);

            return new CountUserResultDto
            {
                TotalUser = userCountingData.TotalUser,
                GroupByField = groupByField,
                CountValues = userCountingData.UserCountValues.Select(a => new CountUserValueDto
                {
                    GroupValue = a.GroupValue,
                    UserCount = a.UserCount
                }).ToList()

            };
        }

        public async Task<PaginatedList<UserWithIdpInfoDto>> GetUsersWithIdpInfoAsync(string searchKey, int pageIndex, int pageSize, bool includeUGMembers, bool includeDepartment, bool getRoles, List<string> loginServiceClaims, string orderBy, List<string> jsonDynamicData, bool? externallyMastered)
        {

            var userAccessChecking = await _userAccessService.CheckReadUserAccessAsync(workContext: _workContext,
                ownerId: _workContext.CurrentOwnerId, customerIds: new List<int> { _workContext.CurrentCustomerId },
                userExtIds: null,
                loginServiceClaims: loginServiceClaims,
                userIds: null,
                userGroupIds: null,
                parentDepartmentIds: null,
                multiUserGroupFilters: null,
                userTypeIdsFilter: null,
                userTypeExtIdsFilter: null,
                multipleUserTypeIdsFilter: null,
                multipleUserTypeExtIdsFilter: null);

            if (!userAccessChecking.IsAllowedAccess)
            {
                return new PaginatedList<UserWithIdpInfoDto>();
            }

            var userIds = userAccessChecking.UserIds;
            var parentDepartmentIds = userAccessChecking.ParentDepartmentIds;
            var multiUserGroupFilters = userAccessChecking.MultiUserGroupFilters;
            var multiUserTypeFilters = userAccessChecking.MultiUserTypeFilters;
            var includeDepartmentOption = includeDepartment ? IncludeDepartmentOption.Department : IncludeDepartmentOption.None;

            var includeUserTypeOption = getRoles
                ? IncludeUserTypeOption.UtUs
                : IncludeUserTypeOption.None;


            var includeUgMemberOption = IncludeUgMemberOption.None;

            var pagingEntity = await _userRepository.GetUsersAsync(
                searchKey: searchKey,
                loginServiceClaims: loginServiceClaims,
                pageIndex: pageIndex,
                pageSize: pageSize, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All },
                orderBy: orderBy,
                includeDepartment: includeDepartmentOption,
                includeLoginServiceUsers: true,
                includeUserTypes: includeUserTypeOption,
                includeUGMembers: includeUgMemberOption,
                jsonDynamicData: jsonDynamicData,
                externallyMastered: externallyMastered,
                userIds: userIds,
                parentDepartmentIds: parentDepartmentIds,
                multiUserGroupFilters: multiUserGroupFilters,
                multiUserTypeFilters: multiUserTypeFilters,
                filterOnParentHd: false);

            Func<UserEntity, List<DTDEntity>, List<UGMemberEntity>, UserWithIdpInfoDto> mapEntityToDtoFunc = (entity, dtdEntities, ugMemberEntities)
                => _userWithIdpInfoMappingService.ToUserDto(entity, dtdEntities, ugMemberEntities);

            List<DTDEntity> dtdEntitiesOfUsers = null;

            List<UGMemberEntity> ugMembersOfUsers = null;
            if (includeUGMembers)
            {
                var existingUserIds = pagingEntity.Items
                    .Select(u => u.UserId)
                    .ToList();
                if (existingUserIds.Count > 0)
                {
                    ugMembersOfUsers = await _ugMemberRepository.GetUGMembersAsync(userIds: existingUserIds, includeUserGroupUser: true, disableTracker: true);
                }
            }
            if (includeDepartment)
            {
                var departmentIds = pagingEntity
                    .Items
                    .Select(a => a.DepartmentId).Distinct().ToList();

                if (departmentIds.Count > 0)
                {
                    dtdEntitiesOfUsers = await _dtdEntityRepository.GetDepartmentDepartmentTypesByDepartmentIdsAsync(departmentIds);
                }
            }
            //Checking accessing entities if don't use paging feature 
            if (pageIndex == 0 && pageSize == 0)
            {
                // Check security
                // Comment out because of Conflicting with Paging Feature
                //var items = _securityHandler.AllowAccess(pagingEntity.Items, AccessBinaryValues.Read, false);
                var paging =
                    new PaginatedList<UserEntity>(pagingEntity.Items, pagingEntity.PageIndex, pagingEntity.PageSize,
                        pagingEntity.HasMoreData)
                    { TotalItems = pagingEntity.TotalItems };
                return paging.ToPaginatedListDto(mapEntityToDtoFunc, dtdEntitiesOfUsers, ugMembersOfUsers);
            }
            return pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, dtdEntitiesOfUsers, ugMembersOfUsers);
        }

        public void ManuallySendWelcomeEmail(IWorkContext workContext,
            List<int> userIds = null,
            List<string> userExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> emails = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            int pageSize = 0,
            int pageIndex = 0,
            string orderBy = "",
            List<bool> externallyMasteredValues = null,
            List<EntityStatusEnum> userEntityStatuses = null)

        {
            if (workContext != null)
                _workContext = workContext;
            if (_emailTemplates.UserEmailTemplates.IsNullOrEmpty()) return;
            var manualEmailTemplates = _emailTemplates.UserEmailTemplates
                .Values.Where(e => !e.Disabled && e.IsWelcomeEmail && e.ApplyWhen?.ManuallyExecute != null).ToList();
            if (manualEmailTemplates.Count == 0) return;
            bool ignoreCheckManualUserStatus = false;
            if (userEntityStatuses.IsNullOrEmpty())
            {
                userEntityStatuses = manualEmailTemplates
                    .SelectMany(m => m.ApplyWhen.ManuallyExecute.EntityStatuses)
                    .Distinct()
                    .ToList();
            }
            else
            {
                ignoreCheckManualUserStatus = true;
            }


            bool? externallyMastered = null;
            bool ignoreCheckManualExternallyMasteredValue = false;
            if (!externallyMasteredValues.IsNullOrEmpty())
            {
                if (externallyMasteredValues.Count == 1)
                    externallyMastered = externallyMasteredValues.First();
                ignoreCheckManualExternallyMasteredValue = true;
            }
            else if (manualEmailTemplates.All(
                a => !a.ApplyWhen.ManuallyExecute.ExternallyMasteredValues.IsNullOrEmpty()))
            {
                //if all email template apply for same one externallyMastered value, we get user base on this, otherwise we do not filter this value

                var configuredExternallyMasteredValues = manualEmailTemplates
                    .SelectMany(m => m.ApplyWhen.ManuallyExecute.ExternallyMasteredValues)
                    .Distinct()
                    .ToList();
                if (configuredExternallyMasteredValues.Count == 1)
                    externallyMastered = configuredExternallyMasteredValues.First();

            }

            var executorUser =
                DomainHelper.GetUserFromWorkContext(_workContext, this, getRoles: false, getLoginServiceClaims: true);
            var hasPagination = pageSize > 0;
            var paginatedUserEntities = GetUserEntitiesForSendingWelcomeEmail(userEntityStatuses,
                externallyMastered,
                userIds: userIds,
                userExtIds: userExtIds,
                parentDepartmentIds: parentDepartmentIds,
                emails: emails,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            var totalEmailCount = 0;
            var userEntities = paginatedUserEntities.Items ?? new List<UserEntity>();

            var oldValueAutoDetectChangesEnabled = _organizationDbContext.ChangeTracker.AutoDetectChangesEnabled;
            _organizationDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            int count = 0;
            foreach (var userEntity in userEntities)
            {
                var emailCount = ExecuteSendingCommunicationMessages(
                    executorUser: executorUser,
                    userEmailTemplates: manualEmailTemplates,
                    oldUserEntity: userEntity,
                    newUserEntity: userEntity,
                    isResettingOtp: false,
                    otp: null,
                    otpExpiration: null,
                    fromEntityStatus: (EntityStatusEnum?)userEntity.EntityStatusId,
                    isCreating: false,
                    isChangingEntityStatus: false,
                    isChangingEmail: false,
                    isMovingUser: false,
                    isManualExecuting: true,
                    ignoreCheckManualExternallyMasteredValue: ignoreCheckManualExternallyMasteredValue,
                    ignoreCheckManualUserStatus: ignoreCheckManualUserStatus,
                    isJobExecuting: false,
                    sentWelcomeMessage: out bool sendSentWelcomeEmail);


                _logger.LogInformation(
                    $"{emailCount} command(s) of manual sending email have been published for user '{userEntity.ExtId}' ({userEntity.UserId}), user {count} of {paginatedUserEntities.TotalItems}.");


                if (sendSentWelcomeEmail)
                {
                    AddOrUpdateJsonDynamicAttributes(userEntity, UserJsonDynamicAttributeName.SentWelcomeEmail, true);
                    AddOrUpdateJsonDynamicAttributes(userEntity, UserJsonDynamicAttributeName.SentWelcomeEmailDate, DateTime.UtcNow.ConvertToISO8601());
                    AddOrUpdateJsonDynamicAttributes(userEntity, UserJsonDynamicAttributeName.SentWelcomeEmailAction, SendWelcomeActions.Manual);

                    userEntity.LastUpdated = DateTime.Now;

                    UpdateUser(userEntity);
                }

                totalEmailCount += emailCount;
            }

            _organizationDbContext.ChangeTracker.AutoDetectChangesEnabled = oldValueAutoDetectChangesEnabled;

            var logInfoBuilder =
                new StringBuilder(
                    $"Total {totalEmailCount} command(s) of manual sending email have been published for {userEntities.Count} of {paginatedUserEntities.TotalItems} user(s)");
            if (hasPagination)
            {
                logInfoBuilder.Append(
                    $" (pageSize {paginatedUserEntities.PageSize}, pageIndex {paginatedUserEntities.PageIndex}, order by '{orderBy}')");
            }

            logInfoBuilder.Append(".");
            _logger.LogInformation(logInfoBuilder.ToString());

        }
        public void SchedulySendWelcomeEmail(IWorkContext workContext, DateTime? entityActiveDateBefore = null, DateTime? entityActiveDateAfter = null)

        {
            if (workContext != null)
                _workContext = workContext;
            if (_emailTemplates.UserEmailTemplates.IsNullOrEmpty()) return;
            var scheduledEmailTemplates = _emailTemplates.UserEmailTemplates
                .Values.Where(e => !e.Disabled && e.IsWelcomeEmail && e.ApplyWhen?.SchedulyExecute != null).ToList();
            if (scheduledEmailTemplates.Count == 0) return;
            List<EntityStatusEnum> userEntityStatuses = null;
            bool? externallyMastered = null;

            if (scheduledEmailTemplates.All(
                a => !a.ApplyWhen.SchedulyExecute.EntityStatuses.IsNullOrEmpty()))
            {
                //if all email template apply for same one externallyMastered value, we get user base on this, otherwise we do not filter this value

                userEntityStatuses = scheduledEmailTemplates
                    .SelectMany(m => m.ApplyWhen.SchedulyExecute.EntityStatuses)
                    .Distinct()
                    .ToList();

            }

            if (scheduledEmailTemplates.All(
                a => !a.ApplyWhen.SchedulyExecute.ExternallyMasteredValues.IsNullOrEmpty()))
            {
                //if all email template apply for same one externallyMastered value, we get user base on this, otherwise we do not filter this value

                var configuredExternallyMasteredValues = scheduledEmailTemplates
                    .SelectMany(m => m.ApplyWhen.SchedulyExecute.ExternallyMasteredValues)
                    .Distinct()
                    .ToList();
                if (configuredExternallyMasteredValues.Count == 1)
                    externallyMastered = configuredExternallyMasteredValues.First();

            }

            var executorUser =
                DomainHelper.GetUserFromWorkContext(_workContext, this, getRoles: false, getLoginServiceClaims: true);

            var paginatedUserEntities = GetUserEntitiesForSendingWelcomeEmail(userEntityStatuses: userEntityStatuses,
                externallyMastered: externallyMastered,
                entityActiveDateAfter: entityActiveDateAfter,
                entityActiveDateBefore: entityActiveDateBefore);

            var totalEmailCount = 0;
            var userEntities = paginatedUserEntities.Items ?? new List<UserEntity>();

            var oldValueAutoDetectChangesEnabled = _organizationDbContext.ChangeTracker.AutoDetectChangesEnabled;
            _organizationDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            int count = 0;
            foreach (var userEntity in userEntities)
            {
                count++;
                var emailCount = ExecuteSendingCommunicationMessages(
                    executorUser: executorUser,
                    userEmailTemplates: scheduledEmailTemplates,
                    oldUserEntity: userEntity,
                    newUserEntity: userEntity,
                    isResettingOtp: false,
                    otp: null,
                    otpExpiration: null,
                    fromEntityStatus: (EntityStatusEnum?)userEntity.EntityStatusId,
                    isCreating: false,
                    isChangingEntityStatus: false,
                    isChangingEmail: false,
                    isMovingUser: false,
                    isManualExecuting: false,
                    ignoreCheckManualExternallyMasteredValue: false,
                    ignoreCheckManualUserStatus: false,
                    isJobExecuting: true,
                    sentWelcomeMessage: out bool sendSentWelcomeEmail);


                _logger.LogInformation(
                    $"{emailCount} command(s) of scheduled sending email have been published for user '{userEntity.ExtId}' ({userEntity.UserId} ), user {count} of {paginatedUserEntities.TotalItems}.");


                if (sendSentWelcomeEmail)
                {
                    AddOrUpdateJsonDynamicAttributes(userEntity, UserJsonDynamicAttributeName.SentWelcomeEmail, true);
                    AddOrUpdateJsonDynamicAttributes(userEntity, UserJsonDynamicAttributeName.SentWelcomeEmailDate, DateTime.UtcNow.ConvertToISO8601());
                    AddOrUpdateJsonDynamicAttributes(userEntity, UserJsonDynamicAttributeName.SentWelcomeEmailAction, SendWelcomeActions.Schedule);

                    userEntity.LastUpdated = DateTime.Now;

                    UpdateUser(userEntity);
                }

                totalEmailCount += emailCount;
            }

            _organizationDbContext.ChangeTracker.AutoDetectChangesEnabled = oldValueAutoDetectChangesEnabled;

            var logInfoBuilder =
                new StringBuilder(
                    $"Total {totalEmailCount} command(s) of scheduled sending email have been published for {userEntities.Count} of {paginatedUserEntities.TotalItems} user(s)");


            logInfoBuilder.Append(".");
            _logger.LogInformation(logInfoBuilder.ToString());

        }

        public async Task<ConexusBaseDto> ArchiveUserByIdAsync(int userId,
                                                               bool syncToIdp = true,
                                                               EntityStatusReasonEnum? entityStatusReason = null,
                                                               bool? isAutoArchived = null)
        {
            var userDto = GetUsers<UserGenericDto>(userIds: new List<int> { userId },
                                                   statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items
                                                                                                                  .FirstOrDefault();
            if (userDto == null)
            {
                return null;
            }
            if (syncToIdp)
            {
                try
                {
                    await ArchiveUserInIDP(userId);
                }
                catch (Exception ex)
                {
                    if (ex is cxHttpResponseException && (ex as cxHttpResponseException).StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning($"ArchiveUser failed for user with email: {userDto.EmailAddress}, reason: {ex.Message}");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
           .ValidateDepartment(userDto.DepartmentId, ArchetypeEnum.Unknown)
           .SkipCheckingArchetype()
           .WithStatus(EntityStatusEnum.All)
           .IsDirectParent()
           .Create();

            userDto.EntityStatus.StatusId = EntityStatusEnum.Archived;

            if (entityStatusReason.HasValue)
            {
                userDto.EntityStatus.StatusReasonId = entityStatusReason.Value;
            }

            var updatedUser = UpdateUser(validationSpecification, userDto, true, null, isAutoArchived);

            UpdateGroupByUserId(validationSpecification, (int)userDto.Identity.Id.Value, EntityStatusEnum.Archived);

            return updatedUser;
        }

        public async Task<ConexusBaseDto> ArchiveUserAsync(
            UserDtoBase userDtoBase,
            int departmentId,
            bool syncToIdp = true,
            EntityStatusReasonEnum? entityStatusReason = null,
            IWorkContext workContext = null,
            bool? isAutoArchived = null)
        {
            _workContext = workContext ?? _workContext;

            if (userDtoBase == null)
            {
                return null;
            }

            if (syncToIdp)
            {
                try
                {
                    await ArchiveUserInIDP((int)userDtoBase.Identity.Id.Value);
                }
                catch (Exception ex)
                {
                    if (ex is cxHttpResponseException && (ex as cxHttpResponseException).StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning($"ArchiveUser failed for user with email: {userDtoBase.EmailAddress}, reason: {ex.Message}");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
           .ValidateDepartment(departmentId, ArchetypeEnum.Unknown)
           .SkipCheckingArchetype()
           .WithStatus(EntityStatusEnum.All)
           .IsDirectParent()
           .Create();

            userDtoBase.EntityStatus.StatusId = EntityStatusEnum.Archived;

            if (entityStatusReason.HasValue)
            {
                userDtoBase.EntityStatus.StatusReasonId = entityStatusReason.Value;
            }

            var updatedUser = UpdateUser(validationSpecification, userDtoBase, false, _workContext, isAutoArchived);

            UpdateGroupByUserId(validationSpecification, (int)userDtoBase.Identity.Id.Value, EntityStatusEnum.Archived);

            return updatedUser;
        }

        public async Task ProcessAutoArchiveUser(IWorkContext workContext)
        {
            if (workContext != null)
            {
                _workContext = workContext;
            }

            var usersPaging = await _userRepository.GetUsersAsync(expirationDateBefore: DateTime.UtcNow,
                                                                   statusIds: new List<EntityStatusEnum>() { EntityStatusEnum.Active,
                                                                                                             EntityStatusEnum.Inactive,
                                                                                                             EntityStatusEnum.New,
                                                                                                             EntityStatusEnum.IdentityServerLocked });
            if (usersPaging is null || !usersPaging.Items.Any())
            {
                return;
            }

            usersPaging.Items.ForEach(async userEntity =>
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(ArchetypeEnum.Unknown);

                        var userDto = _userMappingService.ToUserDto(userEntity) as UserGenericDto;
                        if (userDto is object)
                        {
                            var archivedUser = await userService.ArchiveUserAsync(userDto, userEntity.DepartmentId, true, null, _workContext, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"ProcessAutoArchiveUser failed for user with email: {userEntity.Email}, error '{ex.Message}'");
                }
            });
        }

        private void UpdateGroupByUserId(HierarchyDepartmentValidationSpecification validationSpecification, int userId, EntityStatusEnum entityStatus)
        {
            var userGroups = _userGroupService.GetUserGroups<ConexusBaseDto>(_workContext.CurrentOwnerId,
                                                                                        customerIds: new List<int> { _workContext.CurrentCustomerId },
                                                                                        groupUserIds: new List<int> { userId },
                                                                                        statusIds: new List<EntityStatusEnum> { EntityStatusEnum.Active }
                                                                                        ).Items.ToList();

            foreach (var group in userGroups)
            {
                group.EntityStatus.StatusId = entityStatus;
                _userGroupService.UpdateUserGroup(validationSpecification, new UserGroupDto
                {
                    Identity = group.Identity,
                    EntityStatus = group.EntityStatus
                });
            }
            if (userGroups.Any())
            {
                var ugMembers = _uGMemberService.GetMemberships(_workContext.CurrentOwnerId,
                                                                customerIds: new List<int> { _workContext.CurrentCustomerId },
                                                                userGroupIds: userGroups.Select(x => (int)x.Identity.Id.Value).ToList(),
                                                                ugMemberStatuses: new List<EntityStatusEnum> { EntityStatusEnum.Active },
                                                                userGroupStatus: new List<EntityStatusEnum> { entityStatus });
                foreach (var ugMember in ugMembers)
                {
                    ugMember.EntityStatus.StatusId = entityStatus;
                    _uGMemberService.Update(ugMember);
                }
            }

            //set ugmember of usergroup deactive
            var ugMemberValues = _uGMemberService.GetMemberships(_workContext.CurrentOwnerId,
                                                                 customerIds: new List<int> { _workContext.CurrentCustomerId },
                                                                 userIds: new List<int> { userId });
            if (ugMemberValues != null)
            {
                foreach (var ugMemberValue in ugMemberValues)
                {
                    if (ugMemberValue.EntityStatus.StatusId == EntityStatusEnum.Active)
                    {
                        ugMemberValue.EntityStatus.StatusId = entityStatus;
                        _uGMemberService.Update(ugMemberValue);
                    }
                }
            }
        }

        public async Task<ConexusBaseDto> UnarchiveAsync(int userId, bool syncToIdp = true, EntityStatusReasonEnum? entityStatusReason = null)
        {
            var userDto = GetUsers<UserGenericDto>(userIds: new List<int> { userId },
                                                   statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items
                                                                                                                  .FirstOrDefault();

            var lastEntityStatusId = userDto.GetJsonPropertyValue(UserJsonDynamicAttributeName.LastEntityStatusId);
            var lastEntityStatusReasonId = userDto.GetJsonPropertyValue(UserJsonDynamicAttributeName.LastEntityStatusReasonId);

            if (userDto == null || userDto.EntityStatus.StatusId != EntityStatusEnum.Archived)
            {
                return null;
            }


            if (syncToIdp)
            {
                try
                {
                    await ArchiveUserInIDP(userId, (EntityStatusEnum)lastEntityStatusId);
                }
                catch (Exception ex)
                {
                    if (ex is cxHttpResponseException && (ex as cxHttpResponseException).StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning($"ArchiveUser failed for user with email: {userDto.EmailAddress}, reason: {ex.Message}");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
           .ValidateDepartment(userDto.DepartmentId, ArchetypeEnum.Unknown)
           .SkipCheckingArchetype()
           .WithStatus(EntityStatusEnum.All)
           .IsDirectParent()
           .Create();

            userDto.EntityStatus.StatusId = (EntityStatusEnum)lastEntityStatusId;

            userDto.EntityStatus.StatusReasonId = lastEntityStatusReasonId is object
                                                  ? (EntityStatusReasonEnum)lastEntityStatusReasonId
                                                  : EntityStatusReasonEnum.Active_None;

            var updatedUser = UpdateUser(validationSpecification, userDto);

            UpdateGroupByUserId(validationSpecification, (int)userDto.Identity.Id.Value, (EntityStatusEnum)lastEntityStatusId);

            return updatedUser;

        }

        private async Task ArchiveUserInIDP(int userId, EntityStatusEnum? lastEntityStatusId = null)
        {
            try
            {
                await _identityServerClientService.ChangeArchiveUserStatusAsync(userId, lastEntityStatusId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PaginatedList<UserEntity> GetUserEntitiesForSendingWelcomeEmail(
            List<EntityStatusEnum> userEntityStatuses = null,
            bool? externallyMastered = null,
            List<int> userIds = null,
            List<string> userExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> emails = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            int pageSize = 0,
            int pageIndex = 0,
            string orderBy = "")
        {
            var userEntities = new List<UserEntity>();
            var hasPagination = pageSize > 0;
            bool hasMoreData;
            pageIndex = !hasPagination || pageIndex <= 0 ? 1 : pageIndex;

            var jsonFilter = $"$.{UserJsonDynamicAttributeName.SentWelcomeEmail}=null,false";
            do
            {
                var paginatedUserEntities = _userRepository.GetUsers(ownerId: _workContext.CurrentOwnerId,
                    customerIds: new List<int> { _workContext.CurrentCustomerId },
                    statusIds: userEntityStatuses,
                    userIds: userIds,
                    emails: emails,
                    parentDepartmentIds: parentDepartmentIds,
                    extIds: userExtIds,
                    externallyMastered: externallyMastered,
                    createdAfter: createdAfter,
                    createdBefore: createdBefore,
                    lastUpdatedBefore: lastUpdatedBefore,
                    lastUpdatedAfter: lastUpdatedAfter,
                    expirationDateBefore: expirationDateBefore,
                    expirationDateAfter: expirationDateAfter,
                    entityActiveDateBefore: entityActiveDateBefore,
                    entityActiveDateAfter: entityActiveDateAfter,
                    includeUserTypes: IncludeUserTypeOption.UtUs,
                    includeDepartment: IncludeDepartmentOption.Department,
                    filterOnParentHd: false,
                    pageIndex: pageIndex,
                    pageSize: pageSize,
                    orderBy: orderBy,
                    forUpdating: true,
                    jsonDynamicData: new List<string> { jsonFilter });

                hasMoreData = paginatedUserEntities.HasMoreData;
                pageIndex++;

                if (hasPagination)
                    return paginatedUserEntities;

                if (paginatedUserEntities.Items.Count > 0)
                {
                    userEntities.AddRange(paginatedUserEntities.Items);
                }
            } while (hasMoreData);
            return new PaginatedList<UserEntity>(userEntities, 0, 0, false)
            {
                TotalItems = userEntities.Count
            };
        }

        private UserEntity SendCommunicationMessagesWhenInsertOrUpdateUser(
            UserEntity oldEntity,
            UserEntity newUserEntity,
            bool isResettingOtp,
            string otp,
            DateTime? otpExpiration)
        {
            var isCreating = false;
            var isChangingEmail = false;
            var isChangingEntityStatus = false;
            var isMovingUser = false;
            var newEmail = newUserEntity.Email ?? "";
            var actionBuilder = new StringBuilder();

            EntityStatusEnum? fromEntityStatus = null;
            if (oldEntity == null)
            {
                isCreating = true;
                isResettingOtp = false;
                actionBuilder.Append("creating, ");

            }
            else
            {
                var oldEmail = oldEntity.Email;
                fromEntityStatus = (EntityStatusEnum?)oldEntity.EntityStatusId;

                isChangingEntityStatus = fromEntityStatus != (EntityStatusEnum?)newUserEntity.EntityStatusId;

                //When newUserDto.EntityStatus.Deleted, email might be anonymized, this is not meaning email is changed
                isChangingEmail = !newUserEntity.Deleted.HasValue && !string.IsNullOrEmpty(oldEmail) && !string.Equals(oldEmail, newEmail);

                isMovingUser = newUserEntity.DepartmentId != oldEntity.DepartmentId;

                if (isChangingEmail)
                    actionBuilder.Append($"changing email from '{oldEmail}' to '{newEmail}', ");
                if (isResettingOtp)
                    actionBuilder.Append("resetting OTP, ");
                if (isChangingEntityStatus)
                    actionBuilder.Append($"changing entity status from '{fromEntityStatus}' to '{(EntityStatusEnum?)newUserEntity.EntityStatusId}'");

                if (isMovingUser)
                    actionBuilder.Append(
                        $"moving user from department '{oldEntity.Department.Name}' ({oldEntity.DepartmentId}) to '{newUserEntity.Department.Name}' ({newUserEntity.DepartmentId})");
            }

            var actionText = actionBuilder.ToString().Trim(',', ' ');
            if (string.IsNullOrEmpty(actionText)) actionText = "updating";

            if (_emailTemplates.UserEmailTemplates == null ||
                _emailTemplates.UserEmailTemplates.Count == 0)
            {
                _logger.LogWarning($"Skip sending email when {actionText} for user with id {newUserEntity.UserId} since missing configuration of InsertOrUpdateUserTemplates");
                return newUserEntity;

            }
            var executorUser = DomainHelper.GetUserFromWorkContext(_workContext, this, getRoles: false, getLoginServiceClaims: true);

            var emailCount = ExecuteSendingCommunicationMessages(
                executorUser: executorUser,
                userEmailTemplates: _emailTemplates.UserEmailTemplates.Values.ToList(),
                oldUserEntity: oldEntity,
                newUserEntity: newUserEntity,
                isResettingOtp: isResettingOtp,
                otp: otp,
                otpExpiration: otpExpiration,
                fromEntityStatus: fromEntityStatus,
                isCreating: isCreating,
                isChangingEntityStatus: isChangingEntityStatus,
                isChangingEmail: isChangingEmail,
                isMovingUser: isMovingUser,
                isManualExecuting: false,
                ignoreCheckManualExternallyMasteredValue: false,
                ignoreCheckManualUserStatus: false,
                isJobExecuting: false,
                sentWelcomeMessage: out bool sentWelcomeEmail);
            _logger.LogInformation($"{emailCount} command(s) of sending email have been published when {actionText} for user {newUserEntity.UserId}");

            if (sentWelcomeEmail)
            {
                AddOrUpdateJsonDynamicAttributes(newUserEntity, UserJsonDynamicAttributeName.SentWelcomeEmail, true);
                AddOrUpdateJsonDynamicAttributes(newUserEntity, UserJsonDynamicAttributeName.SentWelcomeEmailDate, DateTime.UtcNow.ConvertToISO8601());
                AddOrUpdateJsonDynamicAttributes(newUserEntity, UserJsonDynamicAttributeName.SentWelcomeEmailAction, SendWelcomeActions.Auto);


                newUserEntity = UpdateUser(newUserEntity);
            }

            return newUserEntity;
        }

        private static void AddOrUpdateJsonDynamicAttributes(UserEntity userEntity, string propertyName, object value)
        {
            var jsonDynamicAttributes = string.IsNullOrEmpty(userEntity.DynamicAttributes)
                ? new Dictionary<string, dynamic>()
                : JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(userEntity.DynamicAttributes);

            if (jsonDynamicAttributes.ContainsKey(propertyName))
            {
                jsonDynamicAttributes[propertyName] = value;
            }
            else
            {
                jsonDynamicAttributes.Add(propertyName, value);
            }

            userEntity.DynamicAttributes = JsonConvert.SerializeObject(jsonDynamicAttributes);
        }

        private int ExecuteSendingCommunicationMessages(UserGenericDto executorUser, List<UserEmailTemplate> userEmailTemplates,
            UserEntity oldUserEntity, UserEntity newUserEntity,
            bool isResettingOtp, string otp, DateTime? otpExpiration, EntityStatusEnum? fromEntityStatus, bool isCreating,
            bool isChangingEntityStatus, bool isChangingEmail, bool isMovingUser,
            bool isManualExecuting,
            bool ignoreCheckManualExternallyMasteredValue, bool ignoreCheckManualUserStatus, bool isJobExecuting, out bool sentWelcomeMessage)
        {
            sentWelcomeMessage = false;
            var userRoleExtIds = GetUserRoleExtIds(newUserEntity);
            var executorName = executorUser?.GetFullName();
            var emailCount = 0;
            var oldEmail = oldUserEntity?.Email;
            var senderIdentity = GetSenderIdentity(newUserEntity, executorUser);
            ////Todo: for MOE R1: disable for now because it's to risky if we still can send some email to real user
            //if (newUserEntity.Locked != 0 && !isManualExecuting)
            //{
            //    return 0;
            //}
            foreach (var communicationTemplateConfiguration in userEmailTemplates)
            {
                if (communicationTemplateConfiguration.Disabled) continue;

                if (ShouldSkipCommunicationTemplate(communicationTemplateConfiguration, oldUserEntity, newUserEntity, fromEntityStatus,
                    userRoleExtIds, isCreating: isCreating,
                    isChangingStatus: isChangingEntityStatus,
                    isResettingOtp: isResettingOtp,
                    isChangingEmail: isChangingEmail,
                    isMovingUser: isMovingUser,
                    isManualExecuting: isManualExecuting,
                    ignoreCheckManualExternallyMasteredValue: ignoreCheckManualExternallyMasteredValue,
                    ignoreCheckManualUserStatus: ignoreCheckManualUserStatus,
                    isJobExecuting: isJobExecuting))
                {
                    continue;
                }

                var communicationSubject = communicationTemplateConfiguration.GetSubject(_workContext.CurrentLocales, _appSettings.FallBackLanguageCode);
                var recipients = BuildRecipientInfos(oldUserEntity, executorUser: executorUser, objectiveUserEntity: newUserEntity,
                    sendEmailToDto: communicationTemplateConfiguration.SendTo);

                foreach (dynamic recipient in recipients)
                {
                    try
                    {
                        //We need to separate email for each recipient to be able to build recipient context in case we send same email to multiple e
                        var communicationTemplate = communicationTemplateConfiguration.CommunicationApiTemplate.DeepClone();
                        var dataKeys = communicationTemplate.Data.Keys.ToList();
                        foreach (var dataProperty in dataKeys)
                        {
                            communicationTemplate.Data[dataProperty] = ReplaceWithExecutingData(communicationTemplate.Data[dataProperty],
                                oldUserEntity,
                                newUserEntity, otp, otpExpiration, newUserEntity.Department, oldEmail, (string)recipient.Name,
                                executorName);
                        }

                        var sendCommunicationCommand = DomainHelper.GenerateCommunicationCommand(
                            correlationId: _workContext.CorrelationId,
                            customerId: senderIdentity.CustomerId,
                            senderIdentityId: senderIdentity.Identity,
                            emailSubject: communicationSubject,
                            communicationApiTemplate: communicationTemplate,
                            routingAction: _appSettings.EmailMessageRoutingAction,
                            recipientCxToken: recipient.CxToken,
                            recipientEmail: recipient.Email,
                            receipientExtId: recipient.UserExtId);
                        var jsonSerializerSettings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            Converters = new List<JsonConverter>
                            {
                                new StringEnumConverter()
                            }
                        };

                        var message = _datahubLogger.WriteCommandLog(sendCommunicationCommand, useMessageRoutingActionAsRoutingKey: true, jsonSerializerSettings: jsonSerializerSettings);
                        if (!string.IsNullOrEmpty(message))
                        {
                            emailCount++;
                            if (communicationTemplateConfiguration.IsWelcomeEmail)
                            {
                                sentWelcomeMessage = true;
                            }

                            _logger.LogInformation($"A command message of sending email '{communicationSubject}' to '{recipient.Email}' (user {recipient.UserId}) has been published: {message}");
                        }
                        else
                        {
                            var json = JsonConvert.SerializeObject(sendCommunicationCommand, jsonSerializerSettings);
                            _logger.LogWarning($"Datahub logger has not write command message of sending email '{communicationSubject}' to '{recipient.Email}' (user {recipient.UserId}): {json}");

                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Unexpected error occurs when publishing command message of sending email '{communicationSubject}' to '{recipient.Email}' (user {recipient.UserId}):");
                    }

                }
            }

            return emailCount;
        }

        private List<UserTypeEntity> GetGetAllUserTypesInCache()
        {
            return _userTypeEntities = _userTypeEntities ?? _userTypeRepository.GetAllUserTypesInCache();

        }
        private List<string> GetUserRoleExtIds(UserEntity userEntity)
        {
            if (userEntity?.UT_Us != null && userEntity.UT_Us.Count > 0)
            {
                var hasIncludedUserType = userEntity.UT_Us.First().UserType != null;
                if (hasIncludedUserType)
                {
                    return userEntity.UT_Us
                        .Where(u => u.UserType != null && (u.UserType.ArchetypeId == (int)ArchetypeEnum.Role ||
                                                           u.UserType.ArchetypeId == (int)ArchetypeEnum.SystemRole))
                        .Select(ut => ut.UserType.ExtId).ToList();
                }

                return userEntity.GetAllRoleUserTypeEntities(GetGetAllUserTypesInCache()).Select(u => u.ExtId).ToList();
            }

            return new List<string>();
        }

        private static (int CustomerId, string Identity) GetSenderIdentity(UserEntity newUserEntity, UserGenericDto executorUser)
        {
            int customerId;
            string senderIdentity;
            if (executorUser != null)
            {
                customerId = executorUser.Identity.CustomerId;
                var executorUserClaims = executorUser.LoginServiceClaims?.Select(l => l.Value).ToList();
                senderIdentity = !executorUserClaims.IsNullOrEmpty()
                    ? executorUserClaims.FirstOrDefault()
                    : executorUser.Identity.ExtId;
            }
            else
            {
                customerId = newUserEntity.UserId;
                var loginServiceClaims = newUserEntity.LoginServiceUsers?.Select(l => l.PrimaryClaimValue).ToList();
                senderIdentity = !loginServiceClaims.IsNullOrEmpty()
                    ? loginServiceClaims.FirstOrDefault()
                    : newUserEntity.ExtId;
            }

            return (customerId, senderIdentity);
        }


        private bool ShouldSkipCommunicationTemplate(UserEmailTemplate emailTemplate, UserEntity oldUserEntity, UserEntity newUserEntity, EntityStatusEnum? fromEntityStatus, List<string> userRoleExtIds,
            bool isCreating, bool isChangingStatus, bool isResettingOtp, bool isChangingEmail, bool isMovingUser, bool isManualExecuting, bool ignoreCheckManualExternallyMasteredValue, bool ignoreCheckManualUserStatus, bool isJobExecuting)
        {
            var shouldApply = MatchWithApplyWhen(
                applyWhen: emailTemplate.ApplyWhen,
                oldUserEntity: oldUserEntity,
                newUserEntity: newUserEntity,
                fromEntityStatus: fromEntityStatus,
                isCreating: isCreating,
                isChangingStatus: isChangingStatus,
                isResettingOtp: isResettingOtp,
                isChangingEmail: isChangingEmail,
                isMovingUser: isMovingUser,
                isManualExecuting: isManualExecuting,
                ignoreCheckManualExternallyMasteredValue: ignoreCheckManualExternallyMasteredValue,
                ignoreCheckManualUserStatus: ignoreCheckManualUserStatus,
                isJobExecuting: isJobExecuting);

            if (!shouldApply)
                return true;

            if (!emailTemplate.ApplyForObjectiveUserEntityStatuses.IsNullOrEmpty()
                && !emailTemplate.ApplyForObjectiveUserEntityStatuses.Contains(EntityStatusEnum.All)
                && !emailTemplate.ApplyForObjectiveUserEntityStatuses.Contains((EntityStatusEnum)(newUserEntity.EntityStatusId ?? 0)))
                return true;

            if (!emailTemplate.ApplyForObjectiveUserArchetypes.IsNullOrEmpty() &&
                !emailTemplate.ApplyForObjectiveUserArchetypes.Contains((ArchetypeEnum)(newUserEntity.ArchetypeId ?? 0)))
                return true;


            if (!emailTemplate.ApplyForObjectiveUserRoles.IsNullOrEmpty() &&
                !userRoleExtIds.Any(extId => emailTemplate.ApplyForObjectiveUserRoles.Contains(extId, StringComparer.CurrentCultureIgnoreCase)))
                return true;


            if (!emailTemplate.DoNotApplyForObjectiveUserRoles.IsNullOrEmpty() &&
                userRoleExtIds.Any(extId => emailTemplate.DoNotApplyForObjectiveUserRoles.Contains(extId, StringComparer.CurrentCultureIgnoreCase)))
                return true;

            if (!string.IsNullOrEmpty(emailTemplate.ApplyWithUserEntityExpression))
            {
                if (!MatchUserEntityExpression(emailTemplate.ApplyWithUserEntityExpression, newUserEntity)) return true;
            }

            return false;
        }

        private bool MatchUserEntityExpression(string expression, UserEntity userEntity)
        {
            try
            {
                var queryableUserEntities = new List<UserEntity> { userEntity }.AsQueryable();
                var userFoundByExpression = queryableUserEntities.Where(expression);
                if (!userFoundByExpression.Any()) return false;
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurs when checking user entity expression {expression}");
                return false;
            }

        }

        private static bool MatchWithApplyWhen(ApplyWhenDto applyWhen, UserEntity oldUserEntity, UserEntity newUserEntity,
            EntityStatusEnum? fromEntityStatus, bool isCreating, bool isChangingStatus, bool isResettingOtp,
            bool isChangingEmail, bool isMovingUser, bool isManualExecuting, bool ignoreCheckManualExternallyMasteredValue, bool ignoreCheckManualUserStatus, bool isJobExecuting)
        {
            if (applyWhen == null) return false;
            var entityStatus = (EntityStatusEnum)(newUserEntity.EntityStatusId ?? 0);
            var externallyMastered = newUserEntity.Locked == 1;

            if (isJobExecuting && applyWhen.SchedulyExecute?.ShouldApply(entityStatus, externallyMastered) == true)
            {
                return true;
            }

            if (isCreating && applyWhen.CreateUser?.ShouldApply(entityStatus, externallyMastered, newUserEntity.Department?.ExtId) == true)
            {
                return true;
            }

            if (isMovingUser && applyWhen.MoveUser?.ShouldApply(entityStatus, externallyMastered,
                    oldUserEntity.Department?.ExtId, newUserEntity.Department?.ExtId) == true)
            {
                return true;
            }

            if (isChangingStatus
                && fromEntityStatus != null
                && applyWhen.ChangeEntityStatus?.ShouldApply(fromEntityStatus.Value, entityStatus) == true)
            {
                return true;
            }

            if (isResettingOtp && applyWhen.ResetOtp)
            {
                return true;
            }

            if (isChangingEmail && applyWhen.ChangeEmail?.ShouldApply(entityStatus, externallyMastered) == true)
            {
                return true;
            }

            if (isManualExecuting && applyWhen.ManuallyExecute != null)
            {

                var matchEntityStatus = ignoreCheckManualUserStatus ||
                                        applyWhen.ManuallyExecute.EntityStatuses.IsNullOrEmpty() ||
                                        applyWhen.ManuallyExecute.EntityStatuses.Contains(EntityStatusEnum.All) ||
                                        applyWhen.ManuallyExecute.EntityStatuses.Contains(entityStatus);

                var matchExternallyMasteredValue = ignoreCheckManualExternallyMasteredValue ||
                                                   applyWhen.ManuallyExecute.ExternallyMasteredValues.IsNullOrEmpty() ||
                                                   applyWhen.ManuallyExecute.ExternallyMasteredValues.Contains(externallyMastered);

                if (matchEntityStatus && matchExternallyMasteredValue)
                {
                    return true;
                }
            }


            return false;
        }
        public dynamic BuildCommunicationCommandRecipient(UserDtoBase executorUser, UserDtoBase objectiveUser, SendEmailToDto sendEmailToDto)
        {
            dynamic recipient = new ExpandoObject();
            recipient.CxToken =
                executorUser != null ? $"{executorUser.Identity.OwnerId}:{executorUser.Identity.CustomerId}"
                : objectiveUser != null ? $"{objectiveUser.Identity.OwnerId}:{objectiveUser.Identity.CustomerId}"
                : $"{_workContext.CurrentOwnerId}:{_workContext.CurrentCustomerId}";
            var recipientInfos = BuildRecipientInfos(null, executorUser, objectiveUser, sendEmailToDto);
            //ExtId is UserCXID
            recipient.UserIds = recipientInfos.Select(a => (string)a.UserExtId).Distinct().ToList();
            return recipient;
        }
        private List<dynamic> BuildRecipientInfos(UserEntity oldUserEntity, UserDtoBase executorUser, UserDtoBase objectiveUser, SendEmailToDto sendEmailToDto)
        {
            List<dynamic> recipients = new List<dynamic>();

            if (sendEmailToDto.ExecutorUser && executorUser != null && !string.IsNullOrEmpty(executorUser.EmailAddress))
            {
                recipients.Add(GenerateRecipient(executorUser));
            }

            if (sendEmailToDto.ObjectiveUser && objectiveUser != null)
            {
                if (objectiveUser.EntityStatus.Deleted)
                {
                    if (oldUserEntity != null && !string.IsNullOrEmpty(oldUserEntity.Email))
                    {
                        //When user is deleted, data might be anonymized, we must use old data to build recipient
                        recipients.Add(GenerateRecipient(oldUserEntity));
                    }
                }
                else if (!string.IsNullOrEmpty(objectiveUser.EmailAddress))
                {
                    recipients.Add(GenerateRecipient(objectiveUser));

                }
            }

            if (sendEmailToDto.ObjectiveUserOldEmail && objectiveUser != null && !string.IsNullOrEmpty(oldUserEntity?.Email))
            {
                recipients.Add(GenerateRecipient(objectiveUser, oldUserEntity.Email));
            }

            if (!sendEmailToDto.OtherUsers.IsNullOrEmpty())
            {
                var user = objectiveUser ?? executorUser;
                if (user != null)
                {
                    recipients.AddRange(GetListRecipientsFromOtherUsers(user.Identity.OwnerId, user.GetParentDepartmentId(), sendEmailToDto.OtherUsers));
                }
            }

            recipients = recipients.DistinctBy(r => r.UserId).ToList();
            return recipients;
        }

        private List<dynamic> BuildRecipientInfos(UserEntity oldUserEntity, UserDtoBase executorUser, UserEntity objectiveUserEntity, SendEmailToDto sendEmailToDto)
        {
            List<dynamic> recipients = new List<dynamic>();

            if (sendEmailToDto.ExecutorUser && executorUser != null && !string.IsNullOrEmpty(executorUser.EmailAddress))
            {
                recipients.Add(GenerateRecipient(executorUser));
            }

            if (sendEmailToDto.ObjectiveUser && objectiveUserEntity != null)
            {
                if (objectiveUserEntity.Deleted.HasValue)
                {
                    if (oldUserEntity != null && !string.IsNullOrEmpty(oldUserEntity.Email))
                    {
                        //When user is deleted, data might be anonymized, we must use old data to build recipient
                        recipients.Add(GenerateRecipient(oldUserEntity));
                    }
                }
                else if (!string.IsNullOrEmpty(objectiveUserEntity.Email))
                {
                    recipients.Add(GenerateRecipient(objectiveUserEntity));

                }
            }

            if (sendEmailToDto.ObjectiveUserOldEmail && objectiveUserEntity != null && !string.IsNullOrEmpty(oldUserEntity?.Email))
            {
                recipients.Add(GenerateRecipient(objectiveUserEntity, oldUserEntity.Email));
            }

            if (!sendEmailToDto.OtherUsers.IsNullOrEmpty())
            {
                var departmentId = 0;
                var ownerId = 0;
                if (objectiveUserEntity != null)
                {
                    departmentId = objectiveUserEntity.DepartmentId;
                    ownerId = objectiveUserEntity.OwnerId;
                }
                else if (executorUser != null)
                {
                    departmentId = executorUser.GetParentDepartmentId();
                    ownerId = executorUser.Identity.OwnerId;
                }

                if (departmentId > 0)
                {
                    recipients.AddRange(GetListRecipientsFromOtherUsers(ownerId, departmentId, sendEmailToDto.OtherUsers));

                }
            }

            recipients = recipients.DistinctBy(r => r.UserId).ToList();
            return recipients;
        }

        private dynamic GenerateRecipient(UserDtoBase executorUser, string email = null)
        {
            if (executorUser == null) return null;
            dynamic recipient = new ExpandoObject();
            recipient.CxToken = $"{executorUser.Identity.OwnerId}:{executorUser.Identity.CustomerId}";
            recipient.Name = executorUser.GetFullName();
            recipient.Email = email ?? executorUser.EmailAddress;
            recipient.UserId = executorUser.Identity.Id.Value;
            recipient.UserExtId = executorUser.Identity.ExtId;
            return recipient;
        }

        private dynamic GenerateRecipient(UserEntity userEntity, string email = null)
        {
            if (userEntity == null) return null;
            dynamic recipient = new ExpandoObject();
            recipient.CxToken = $"{userEntity.OwnerId}:{userEntity.CustomerId}";
            recipient.Name = $"{userEntity.FirstName} {userEntity.LastName}".Trim();
            recipient.Email = email ?? userEntity.Email;
            recipient.UserId = (long)userEntity.UserId;
            recipient.UserExtId = userEntity.ExtId;
            return recipient;
        }

        private List<dynamic> GetListRecipientsFromOtherUsers(int ownerId, int departmentId, List<SendToUserDto> sendToUserDtos)
        {
            List<UserEntity> allUserEntities = new List<UserEntity>();
            foreach (var sendToUserDto in sendToUserDtos)
            {
                var userEntities = GetUserEntitiesAsRecipients(ownerId, departmentId, sendToUserDto);
                allUserEntities.AddRange(userEntities);
            }

            return BuildRecipientsFromUserEntities(allUserEntities);

        }

        private List<UserEntity> GetUserEntitiesAsRecipients(int ownerId, int departmentId, SendToUserDto sendToUser)
        {
            //For now we don't have use case that get list recipients without user type.
            //For safety, we need to require configuration has UserTypeExtIds, to make sure we don't send email to any user

            if (sendToUser == null || sendToUser.UserTypeExtIds.IsNullOrEmpty())
                return new List<UserEntity>();

            var doNotSendToUserInAnyDepartment = !sendToUser.InFullHierarchyDepartment &&
                                                 !sendToUser.InSameDepartment &&
                                                 !sendToUser.InAncestorDepartment &&
                                                 !sendToUser.InDescendantDepartment;

            if (doNotSendToUserInAnyDepartment)
                return new List<UserEntity>();


            var userTypes = _userTypeRepository.GetUserTypes(ownerId,
                extIds: sendToUser.UserTypeExtIds);
            var notFoundUserTypeExtIds = sendToUser.UserTypeExtIds.Where(u => !userTypes.Any(a =>
                    string.Equals(u, a.ExtId, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();
            if (notFoundUserTypeExtIds.Count > 0)
            {
                _logger.LogWarning(
                    $"Unable to find user type for with extId(s) {string.Join(",", notFoundUserTypeExtIds.Select(e => $"'{e}'"))} for sending email");
            }

            if (userTypes.Count > 0)
            {
                var userTypeIds = userTypes.Select(u => u.UserTypeId).ToList();
                List<int> departmentIds = null;

                var sendToUserInAllDepartment = sendToUser.InFullHierarchyDepartment;

                if (!sendToUserInAllDepartment)
                {
                    departmentIds = new List<int>();

                    if (sendToUser.InAncestorDepartment)
                    {
                        departmentIds.AddRange(_hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(departmentId));
                    }

                    if (sendToUser.InDescendantDepartment)
                    {
                        departmentIds.AddRange(_hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(departmentId));
                    }

                    departmentIds = departmentIds.Where(id => id != departmentId).ToList();

                    if (sendToUser.InSameDepartment)
                    {
                        departmentIds.Add(departmentId);
                    }

                    if (departmentIds.Count == 0)
                    {
                        return new List<UserEntity>();
                    }
                }
                return GetUserEntitiesAsRecipients(ownerId, departmentIds, userTypeIds);

            }

            return new List<UserEntity>();
        }
        private List<UserEntity> GetUserEntitiesAsRecipients(int ownerId, List<int> departmentIds, List<int> userTypeIds)
        {
            var userEntities = new List<UserEntity>();
            var hasMoreData = true;
            int pageIndex = 1;
            do
            {
                var paginatedUserEntities = _userRepository.GetUsers(ownerId: ownerId,
                    parentDepartmentIds: departmentIds, userTypeIds: userTypeIds, pageIndex: pageIndex,
                    filterOnParentHd: false);
                hasMoreData = paginatedUserEntities.HasMoreData;
                pageIndex++;
                if (paginatedUserEntities.Items.Count > 0)
                {
                    userEntities.AddRange(paginatedUserEntities.Items);
                }

            } while (hasMoreData);


            return userEntities;
        }

        private List<dynamic> BuildRecipientsFromUserEntities(List<UserEntity> userEntities)
        {
            var recipients = new List<dynamic>();
            foreach (var user in userEntities)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var allowToSendEmailToUser = false;
                    if (string.IsNullOrEmpty(user.DynamicAttributes))
                    {
                        allowToSendEmailToUser = true;
                    }
                    else
                    {
                        var jsonDynamic = JsonConvert.DeserializeObject<dynamic>(user.DynamicAttributes);
                        string notificationReference =
                            Convert.ToString(jsonDynamic?.NotificationReference ?? jsonDynamic?.notificationReference);
                        if (string.IsNullOrEmpty(notificationReference) || string.Equals(notificationReference, "Email",
                                StringComparison.CurrentCultureIgnoreCase) || string.Equals(notificationReference,
                                "Default",
                                StringComparison.CurrentCultureIgnoreCase))
                        {
                            allowToSendEmailToUser = true;
                        }
                    }

                    if (allowToSendEmailToUser)
                    {
                        recipients.Add(GenerateRecipient(user));
                    }
                }
            }

            return recipients;
        }

        private string ReplaceWithExecutingData(string propertyValue, UserEntity oldUserEntity, UserEntity newUserEntity, string otp, DateTime? otpExpiration,
            DepartmentEntity parentDepartment, string oldEmail, string recipientName, string executorName)
        {
            string fullName;
            string email;
            var entityStatusReason = (EntityStatusReasonEnum)(newUserEntity.EntityStatusReasonId ?? 0);
            var activeDate = newUserEntity.EntityActiveDate;
            if (newUserEntity.Deleted.HasValue)
            {
                // When user is deleted, data might be anonymized, we must use old data 
                fullName = $"{oldUserEntity.FirstName} {oldUserEntity.LastName}".Trim();
                email = oldUserEntity.Email;
            }
            else
            {
                fullName = $"{newUserEntity.FirstName} {newUserEntity.LastName}".Trim();
                email = newUserEntity.Email;
            }
            string value = ReplaceWithExecutingData(propertyValue, parentDepartment, oldEmail, recipientName, fullName, email, otp, otpExpiration, entityStatusReason, activeDate, executorName);

            return value;
        }

        private string ReplaceWithExecutingData(string propertyValue, DepartmentEntity parentDepartment,
            string oldEmail, string recipientName, string fullName, string email, string otp, DateTime? otpExpiration,
            EntityStatusReasonEnum entityStatusReason, DateTime? activeDate, string executorName)
        {
            var reasonText = _entityStatusReasonTexts.GetText(entityStatusReason, _workContext.CurrentLocales);
            var value = propertyValue
                .Replace("{FullName}", fullName)
                .Replace("{ExecutorName}", executorName)
                .Replace("{RecipientName}", recipientName)
                .Replace("{DepartmentName}", parentDepartment?.Name)
                .Replace("{OtpCode}", otp)
                .Replace("{OPALMainPageLink}", _appSettings.OPALMainPageLink)
                .Replace("{PDPMLink}", _appSettings.PDPMLink)
                .Replace("{SAMLink}", _appSettings.SAMLink)
                .Replace("{LearnerWebAppLink}", _appSettings.LearnerWebAppLink)
                .Replace("{LearnerAndroidAppLink}", _appSettings.LearnerAndroidAppLink)
                .Replace("{LearnerIOSAppLink}", _appSettings.LearnerIOSAppLink)
                .Replace("{LogoPath}", _appSettings.LogoPath)
                .Replace("{Email}", email)
                .Replace("{HintEmail}", HintEmail(email))
                .Replace("{OldEmail}", oldEmail)
                .Replace("{HintOldEmail}", HintEmail(oldEmail))
                .Replace("{EntityStatusReasonText}", reasonText);


            value = ReplaceDateTimeValue(value, "CurrentDateTime", () => DateTime.Now);
            value = ReplaceDateTimeValue(value, "CurrentDateTimeUtc", () => DateTime.UtcNow);
            value = ReplaceDateTimeValue(value, "TimeZoneDateTime", GetCurrentDateTimeForTimezoneOffset);
            value = ReplaceDateTimeValue(value, "ActiveDate", () => activeDate);
            value = ReplaceDateTimeValue(value, "OtpExpiration", () => GetOtpExpirationForTimezoneOffset(otpExpiration));
            return value;
        }

        private DateTime? GetCurrentDateTimeForTimezoneOffset()
        {
            if (_appSettings.TimeZoneOffset == null)
                return DateTime.Now;
            return DateTime.UtcNow.AddHours(_appSettings.TimeZoneOffset.Value);
        }
        private DateTime? GetOtpExpirationForTimezoneOffset(DateTime? otpExpiration)
        {
            if (otpExpiration == null) return null;
            if (_appSettings.TimeZoneOffset == null)
                return otpExpiration;
            return otpExpiration.Value.ToUniversalTime().AddHours(_appSettings.TimeZoneOffset.Value);

        }
        private static string ReplaceDateTimeValue(string value, string parameter, Func<DateTime?> dateTimeFunc)
        {
            var keyword = "{" + parameter;

            var startIndex = value.IndexOf(keyword, StringComparison.Ordinal);
            if (startIndex < 0) return value;
            var endIndex = value.IndexOf("}", startIndex, StringComparison.Ordinal);
            if (endIndex < 0) return value;

            var fullParameter = value.Substring(startIndex, endIndex - startIndex + 1);


            string format = null;
            var startFormatIndex = fullParameter.IndexOf(":") + 1;//+1 for excluding ':' at start
            if (startFormatIndex > 0 && fullParameter.Length > startFormatIndex)
            {

                format = fullParameter.Substring(startFormatIndex, fullParameter.Length - startFormatIndex - 1); //-1 for excluding '}' at the end
            }

            var date = dateTimeFunc();
            var datetimeAsString = date == null
                ? null
                : string.IsNullOrEmpty(format)
                    ? date.Value.ToString(CultureInfo.CurrentCulture)
                    : date.Value.ToString(format);

            return value.Replace(fullParameter, datetimeAsString);
        }

        private string HintEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return email;
            var emailParts = email.Split('@');
            var domain = emailParts.Length > 1 ? emailParts[1] : null;
            var hintUserName = $"{emailParts[0].Substring(0, 2)}****";
            return $"{hintUserName}@{domain}";
        }
        private async Task<(bool AccessDeniedOnRootDepartment, bool KeepTheRootDepartment)> CheckDepartmentIdParameterAsync(int retrievingDepartmentId)
        {
            // Set default value.
            var keepTheRootDepartment = false;
            var accessDeniedOnRootDepartment = false;

            if (!_hierarchyDepartmentPermissionService.UserIsAuthenticatedByToken())
            {
                keepTheRootDepartment = true;
                return (accessDeniedOnRootDepartment, keepTheRootDepartment);
            }

            // User is authenticated by token.
            if (retrievingDepartmentId == _hierarchyDepartmentPermissionService.GetRootDepartmentId())
            {
                var authenticatedUser = await _hierarchyDepartmentPermissionService.GetWorkContextUserAsync();
                if (authenticatedUser.DepartmentId == retrievingDepartmentId)
                {
                    keepTheRootDepartment = true;
                }
                else
                {
                    accessDeniedOnRootDepartment = !(await _hierarchyDepartmentPermissionService.HasFullAccessOnHierarchyDepartmentAsync());
                }
            }

            return (accessDeniedOnRootDepartment, keepTheRootDepartment);
        }
    }

    public static class SendWelcomeActions
    {
        public const string Auto = "Auto";
        public const string Manual = "Manual";
        public const string Schedule = "Schedule";

    }
}
