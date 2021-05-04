using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using cxEvent.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.ApiClient;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.DatahubLog;

namespace cxOrganization.Domain.Services
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IAdvancedWorkContext _workContext;
        //private readonly ISecurityHandler _securityHandler;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IUserGroupMappingService _userGroupMappingService;
        private readonly IUserGroupValidator _userGroupValidator;
        private readonly IDepartmentValidator _departmentValidator;
        private readonly IDatahubLogger _datahubLogger;

        public UserGroupService(IUserGroupRepository userGroupRepository,
            IAdvancedWorkContext workContext,
            //ISecurityHandler securityHandler,
            OrganizationDbContext organizationDbContext,
            IUserGroupMappingService userGroupMappingService,
            IUserGroupValidator userGroupValidator,
            IDepartmentValidator departmentValidator,
            IDatahubLogger datahubLogger)
        {
            _userGroupRepository = userGroupRepository;
            _workContext = workContext;
            //_securityHandler = securityHandler;
            _organizationDbContext = organizationDbContext;
            _userGroupMappingService = userGroupMappingService;
            _userGroupValidator = userGroupValidator;
            _departmentValidator = departmentValidator;
            _datahubLogger = datahubLogger;
        }

        public ConexusBaseDto InsertUserGroup(HierarchyDepartmentValidationSpecification validationSpecification, UserGroupDtoBase usergroup, IAdvancedWorkContext workContext = null)
        {
            if (validationSpecification != null)
            {
                _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            }
            //Do the validation
            var entity = _userGroupValidator.Validate(usergroup, workContext ?? _workContext);

            //Map to User Entity
            var userGroupEntity = _userGroupMappingService.ToUserGroupEntity(entity, usergroup, workContext ?? _workContext);

            //Check security
            //_securityHandler.AllowAccess(departmentEntity, true);

            //Call insert department Entity
            userGroupEntity = _userGroupRepository.Insert(userGroupEntity);
            _organizationDbContext.SaveChanges();

            var reponsingDto = _userGroupMappingService.ToUserGroupDto(userGroupEntity);

            InsertEvent(userGroupEntity, reponsingDto, EventType.CREATED);

            //Remap to userdtoBase
            return reponsingDto;
        }
        public ConexusBaseDto UpdateUserGroup(HierarchyDepartmentValidationSpecification validationSpecification, UserGroupDtoBase usergroup)
        {
            if (validationSpecification != null)
            {
                _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            }

            //Do the validation
            var entity = _userGroupValidator.Validate(usergroup);

            var oldEntityStatusId = entity.EntityStatusId;
            var oldEntityStatusReasonId = entity.EntityStatusReasonId;


            //Map to Department Entity
            var userGroupEntity = _userGroupMappingService.ToUserGroupEntity(entity, usergroup);

            if (userGroupEntity == null) return null;

            var isChangedStatus = userGroupEntity.EntityStatusId != oldEntityStatusId;

            //Check security
            //if (_securityHandler.AllowAccess(userGroupEntity, AccessBinaryValues.Update, false).AceType == AccessControlEntryType.AccessDenied)
            //{
            //    throw new CXValidationException(new List<ValidationResult> { new ValidationResult(string.Format("Access denied: UserGroupId({0})", userGroupEntity.UserGroupId)) });
            //}

            //Until now, we only need to get modified property when entity status is changed. This method should be call before updating entity
            var modifiedProperties = isChangedStatus ? _userGroupRepository.GetModifiedProperties(userGroupEntity) : null;

            //Call insert user Entity
            userGroupEntity = _userGroupRepository.Update(userGroupEntity);
            _organizationDbContext.SaveChanges();
            var reponsingDto = _userGroupMappingService.ToUserGroupDto(userGroupEntity);

            //Insert domain event
            if (isChangedStatus)
            {
                //If status is changed we write an event with type <archetype>_ENTITYSTATUS_CHANGED, 
                var statusChangedInfo = DomainHelper.BuildEntityStatusChangedEventInfo(oldEntityStatusId, oldEntityStatusReasonId, userGroupEntity.EntityStatusId, userGroupEntity.EntityStatusReasonId);

                InsertEvent(userGroupEntity, statusChangedInfo, EventType.ENTITYSTATUS_CHANGED);

                //Check if info are also changed, we write more an event with type <archetype>_UPDATED

                if (DomainHelper.IsChangedInfo(modifiedProperties, nameof(UserEntity.LastUpdated), nameof(UserEntity.LastUpdatedBy), nameof(UserEntity.EntityStatusId)))
                {
                    InsertEvent(userGroupEntity, reponsingDto, EventType.UPDATED);
                }

            }
            else
            {
                InsertEvent(userGroupEntity, reponsingDto, EventType.UPDATED);
            }


            //Remap to userdtoBase
            return reponsingDto;
        }

        private void InsertEvent(UserGroupEntity userGroupEntity, object additionalInformation, EventType eventType)
        {
            dynamic body = new ExpandoObject();
            body.UserGroupData = additionalInformation;
            body.UserGroupId = userGroupEntity.UserGroupId;
            body.UserGroupArcheTypeId = userGroupEntity.ArchetypeId;

            var objectType = userGroupEntity.ArchetypeId == null ||
                             userGroupEntity.ArchetypeId == (int) ArchetypeEnum.Unknown
                ? "unknown_archetype_usergroup"
                : ((ArchetypeEnum) userGroupEntity.ArchetypeId).ToString();

            var eventMessage = new LogEventMessage(eventType.ToEventName(objectType), _workContext)
                .EntityId(userGroupEntity.UserGroupId.ToString())
                .Entity("domain", "usergroup")
                .WithBody(body);

            _datahubLogger.WriteEventLog(eventMessage);
        }


        /// <summary>
        /// Get User Group.
        /// </summary>
        /// <param name="userGroupId">The user group identifier.</param>
        /// <returns>UserGroup</returns>
        public ConexusBaseDto GetUserGroup(HierarchyDepartmentValidationSpecification validationSpecification, int userGroupId)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            var usergroup = _userGroupRepository.GetById(userGroupId);

            //Check security
            //if (usergroup != null && _securityHandler.AllowAccess(usergroup, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _userGroupMappingService.ToUserGroupDto(usergroup);
            //}

            //return null;
        }
        /// <summary>
        /// Get User Group.
        /// </summary>
        /// <param name="userGroupId">The user group identifier.</param>
        /// <returns>UserGroup</returns>
        public ConexusBaseDto GetUserGroup(int userGroupId)
        {
            var usergroup = _userGroupRepository.GetById(userGroupId);

            //Check security
            //if (usergroup != null && _securityHandler.AllowAccess(usergroup, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _userGroupMappingService.ToUserGroupDto(usergroup);
            //}

            //return null;
        }
        /// <summary>
        /// Creates the User Group _ User.
        /// </summary>
        /// <param name="departmentId">The Department identifier.</param>
        /// <returns>UserGroup</returns>
        public List<ConexusBaseDto> GetUserGroups(HierarchyDepartmentValidationSpecification validationSpecification, int departmentId)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            var usergroups = _userGroupRepository.GetUserGroupsByDepartmentId(departmentId, false, EntityStatusEnum.All);

            //Check security
            //usergroups = _securityHandler.AllowAccess(usergroups, AccessBinaryValues.Read, false);

            var results = new List<ConexusBaseDto>();
            foreach (var item in usergroups)
            {
                var teachingGroupDto = _userGroupMappingService.ToUserGroupDto(item);
                if (teachingGroupDto != null)
                {
                    results.Add(teachingGroupDto);
                }
            }

            return results;
        }
        public PaginatedList<T> GetUserGroups<T>(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userGroupIds = null,
            List<int> memberUserIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<EntityStatusEnum> userStatusIds = null,
            List<GrouptypeEnum> groupTypes = null,
            List<int> archetypeIds = null,
            List<string> extIds = null,
            List<int> groupUserIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            string searchKey = null,
            bool includeDepartment = false,
            bool includeUser = false) where T : ConexusBaseDto
        {
            customerIds = customerIds ?? new List<int>();
            if (_workContext.CurrentCustomerId != 0 && !customerIds.Contains(_workContext.CurrentCustomerId))
            {
                customerIds.Add(_workContext.CurrentCustomerId);
            }

            List<ConexusBaseDto> usergroupDtos = new List<ConexusBaseDto>();
            var pagingEntity = _userGroupRepository.GetUserGroups(ownerId: ownerId,
                customerIds: customerIds,
                userGroupIds: userGroupIds,
                memberUserIds: memberUserIds,
                parentDepartmentIds: departmentIds,
                statusIds: statusIds,
                userStatusIds: userStatusIds,
                groupTypes: groupTypes,
                archetypeIds: archetypeIds,
                extIds: extIds,
                parentUserIds: groupUserIds,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                searchKey: searchKey,
                includeDepartment: includeDepartment,
                includeUser: includeUser);
            //declare delegate mapping function
            Func<UserGroupEntity, bool?, T> mapEntityToDtoFunc = (entity, getDynamicPropertiesFlag)
                => (T)_userGroupMappingService.ToUserGroupDto(entity, getDynamicPropertiesFlag);

            return pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties);
        }

        /// <summary>
        /// DeleteUserGroupById
        /// </summary>
        /// <param name="userGroupId"></param>
        public void DeleteUserGroupById(HierarchyDepartmentValidationSpecification validationSpecification, int userGroupId)
        {
            _departmentValidator.ValidateHierarchyDepartment(validationSpecification);
            _userGroupRepository.Delete(userGroupId);
        }
        /// <summary>
        /// Creates the User Group _ User.
        /// </summary>
        /// <param name="departmentIds">The Department identifiers</param>
        /// <returns>UserGroup</returns>
        public List<IdentityStatusDto> GetUserGroupIdentifiers(List<int> departmentIds)
        {
            var usergroups = _userGroupRepository.GetUserGroupByDepartmentIds(departmentIds, null, EntityStatusEnum.All);

            //Check security
            //usergroups = _securityHandler.AllowAccess(usergroups, AccessBinaryValues.Read, false);

            var results = new List<IdentityStatusDto>();
            foreach (var item in usergroups)
            {
                if (item != null)
                {
                    var identityStatus = new IdentityStatusDto();
                    identityStatus.Identity = new IdentityDto
                    {
                        Archetype = item.ArchetypeId.HasValue ? (ArchetypeEnum)item.ArchetypeId : ArchetypeEnum.Unknown,
                        ExtId = item.ExtId,
                        Id = item.UserGroupId,
                        CustomerId = item.Department != null ? 0 : (item.Department.CustomerId ?? 0),
                        OwnerId = item.OwnerId
                    };
                    identityStatus.EntityStatus = new EntityStatusDto
                    {
                        EntityVersion = item.EntityVersion,
                        LastUpdated = item.LastUpdated,
                        LastUpdatedBy = item.LastUpdatedBy ?? 0,
                        StatusId = item.EntityStatusId.HasValue ? (EntityStatusEnum)item.EntityStatusId : EntityStatusEnum.Unknown,
                        StatusReasonId = item.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)item.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                        LastExternallySynchronized = item.LastSynchronized,
                        //ExternallyMastered = groupEntity.Locked == 1,
                        Deleted = item.Deleted.HasValue
                    };
                    results.Add(identityStatus);
                }
            }

            return results;
        }

        public List<IdentityStatusDto> UpdateUserGroupIdentifiers(List<IdentityStatusDto> userGroupIdentities, List<int> allowArchetypeIds, string hdPath, params EntityStatusEnum[] filters)
        {
            var userGroupIds = userGroupIdentities.Select(x => (int)x.Identity.Id).ToList();
            var includeProperties = Extensions.QueryExtension.CreateIncludeProperties<UserGroupEntity>(x => x.Department.H_D);
            var userGroups = _userGroupRepository.GetUserGroupByIds(userGroupIds, allowArchetypeIds, includeProperties, filters);
            var results = new List<IdentityStatusDto>();

            foreach (var userGroup in userGroups)
            {
                if (!string.IsNullOrEmpty(hdPath) && userGroup.Department.H_D.FirstOrDefault().Path.StartsWith(hdPath))
                {
                    var info = userGroupIdentities.FirstOrDefault(x => x.Identity.Id == userGroup.UserGroupId);
                    if (info == null)
                        continue;
                    userGroup.LastSynchronized = info.EntityStatus.LastExternallySynchronized ?? DateTime.Now;
                    userGroup.EntityStatusId = (int)info.EntityStatus.StatusId;
                    userGroup.EntityStatusReasonId = (int)info.EntityStatus.StatusReasonId;
                    _userGroupRepository.Update(userGroup);
                    var identityStatus = new IdentityStatusDto();
                    identityStatus.Identity = new IdentityDto
                    {
                        Archetype = userGroup.ArchetypeId.HasValue ? (ArchetypeEnum)userGroup.ArchetypeId : ArchetypeEnum.Unknown,
                        ExtId = userGroup.ExtId,
                        Id = userGroup.UserGroupId,
                        CustomerId = userGroup.Department != null ? 0 : (userGroup.Department.CustomerId ?? 0),
                        OwnerId = userGroup.OwnerId
                    };
                    identityStatus.EntityStatus = new EntityStatusDto
                    {
                        EntityVersion = userGroup.EntityVersion,
                        LastUpdated = userGroup.LastUpdated,
                        LastUpdatedBy = userGroup.LastUpdatedBy ?? 0,
                        StatusId = (EntityStatusEnum)userGroup.EntityStatusId,
                        StatusReasonId = (EntityStatusReasonEnum)userGroup.EntityStatusReasonId,
                        LastExternallySynchronized = userGroup.LastSynchronized,
                        //ExternallyMastered = groupEntity.Locked == 1,
                        Deleted = userGroup.Deleted.HasValue
                    };
                    results.Add(identityStatus);
                }
            }
            _organizationDbContext.SaveChanges();
            return results;
        }

        public ConexusBaseDto GetUserGroupIdentityStatusByExtId(string extId, int customerId)
        {
            var userGroup = _userGroupRepository.GetUserGroupByExtId(extId, null, customerId);
            if (userGroup == null)
            {
                return null;
            }
            //Check security
            //if (_securityHandler.AllowAccess(userGroup, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _userGroupMappingService.ToUserGroupDto(userGroup);
            //}
            //return null;
        }

        public List<ConexusBaseDto> GetListUserGroupIdentityStatusByExtId(string extId)
        {
            var usergroupEntities = _userGroupRepository.GetUserGroups(extIds: new List<string> { extId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;

            //var allowAccessedUserGroup = _securityHandler.AllowAccess(usergroupEntities, AccessBinaryValues.Read, false);

            List<ConexusBaseDto> result = new List<ConexusBaseDto>();
            foreach (var usergroupEntity in usergroupEntities)
            {
                var userGroupDto = _userGroupMappingService.ToUserGroupDto(usergroupEntity);
                if (userGroupDto != null)
                {
                    result.Add(userGroupDto);
                }
            }
            return result;
        }
        public List<UserGroupEntity> GetUserGroupByDepartmentId(int departmentId, int userGroupTypeId = 0,
            bool isIncludeUsers = true,
            int departmentTypeId = 0,
            params EntityStatusEnum[] filters)
        {
            return _userGroupRepository.GetuserGroupsByDepartmentIdAndUserGroupType(departmentId,
                userGroupTypeId, isIncludeUsers,
                departmentTypeId, filters);
        }

        public UserGroupEntity GetUserGroupByExtId(string extId)
        {
            return _userGroupRepository.GetUserGroupByExtId(extId);
        }
        public UserGroupEntity UpdateUserGroupDepartmentType(UserGroupEntity userGroup, DepartmentTypeEntity addingDepartmentType, bool isUniqueDepartmentType = false)
        {
            if (isUniqueDepartmentType)
            {
                //removing same department type with addingDepartmentType if it's existed
                for (int i = 0; i < userGroup.DT_UGs.Count; i++)
                {
                    DTUGEntity dtUg = userGroup.DT_UGs.ElementAt(i);
                    if (dtUg.DepartmentType != null && dtUg.DepartmentType.ArchetypeId == addingDepartmentType.ArchetypeId)
                    {
                        userGroup.DT_UGs.Remove(dtUg);
                        i--;
                    }
                }
            }
            userGroup.DT_UGs.Add(new DTUGEntity { DepartmentTypeId = addingDepartmentType.DepartmentTypeId, UserGroupId = userGroup.UserGroupId });
            _organizationDbContext.SaveChanges();
            return userGroup;
        }

        public List<TeachingSubjectDto> GetUserGroupsByArchetypes(List<ArchetypeEnum> archetypeIds)
        {
            var userGroupEntities = _userGroupRepository.GetUserGroupsByArchetypeIds(archetypeIds);

            List<TeachingSubjectDto> result = new List<TeachingSubjectDto>();

            foreach (var item in userGroupEntities)
            {
                var teachingGroupDto = _userGroupMappingService.ToTeachingSubjectDto(item);
                if(teachingGroupDto != null)
                {
                    result.Add(teachingGroupDto);
                }
            }

            return result;
        }

        public PaginatedList<UserGroupEntity> GetUserGroupEntities(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userGroupIds = null,
            List<int> memberUserIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<EntityStatusEnum> userStatusIds = null,
            List<GrouptypeEnum> groupTypes = null,
            List<int> archetypeIds = null,
            List<string> extIds = null,
            List<int> groupUserIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            string searchKey = null,
            bool includeDepartment = false,
            bool includeUser = false)
        {
            customerIds = customerIds ?? new List<int>();
            if (_workContext.CurrentCustomerId != 0 && !customerIds.Contains(_workContext.CurrentCustomerId))
            {
                customerIds.Add(_workContext.CurrentCustomerId);
            }

            var pagingEntity = _userGroupRepository.GetUserGroups(ownerId: ownerId,
                customerIds: customerIds,
                userGroupIds: userGroupIds,
                memberUserIds: memberUserIds,
                parentDepartmentIds: departmentIds,
                statusIds: statusIds,
                userStatusIds: userStatusIds,
                groupTypes: groupTypes,
                archetypeIds: archetypeIds,
                extIds: extIds,
                parentUserIds: groupUserIds,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                searchKey: searchKey,
                includeDepartment: includeDepartment,
                includeUser: includeUser);

            return pagingEntity;
        }

        public ConexusBaseDto MapToUserGroupService(UserGroupEntity userGroupEntity, bool? getDynamicProperties = null)

        {
            return _userGroupMappingService.ToUserGroupDto(userGroupEntity, getDynamicProperties);
        }
    }
}
