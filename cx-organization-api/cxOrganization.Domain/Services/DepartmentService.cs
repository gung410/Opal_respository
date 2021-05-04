using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentTypeRepository _departmentTypeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IDepartmentValidator _validator;
        private readonly IDepartmentMappingService _departmentMappingService;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly ICommonService _commonService;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        private readonly IDatahubLogger _datahubLogger;

        public DepartmentService(IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IDepartmentRepository departmentRepository,
            IAdvancedWorkContext workContext,
            OrganizationDbContext organizationDbContext,
            //ISecurityHandler securityHandler,
            IUserRepository userRepository,
            IDepartmentTypeRepository departmentTypeRepository,
            ICustomerRepository customerRepository,
            IDepartmentValidator validator,
            IDepartmentMappingService departmentMappingService,
            ICommonService commonService,
            IHierarchyDepartmentService hyHierarchyDepartmentService,
            IDatahubLogger datahubLogger)
        {
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _departmentRepository = departmentRepository;
            _workContext = workContext;
            _userRepository = userRepository;
            _departmentTypeRepository = departmentTypeRepository;
            _customerRepository = customerRepository;
            _validator = validator;
            _departmentMappingService = departmentMappingService;
            _organizationDbContext = organizationDbContext;
            //_securityHandler = securityHandler;
            _commonService = commonService;
            _hierarchyDepartmentService = hyHierarchyDepartmentService;
            _datahubLogger = datahubLogger;
        }

        public EntityStatusDto UpdateDepartmentStatus(HierarchyDepartmentValidationSpecification validationSpecification, int departmentId, EntityStatusDto departmentStatus)
        {
            return new EntityStatusDto();
        }

        public List<IdentityStatusDto> UpdateDepartmentIdentifiers(List<IdentityStatusDto> departmentidentites, List<int> allowArchetypeIds, string hdPath)
        {
            var departmentIds = departmentidentites.Select(x => x.Identity.Id).ToList();
            var departments = _departmentRepository.GetDepartmentByIdsAndArchetypeId(departmentIds, allowArchetypeIds);
            var results = new List<IdentityStatusDto>();

            foreach (var department in departments)
            {
                if (!string.IsNullOrEmpty(hdPath) && department.H_D.FirstOrDefault().Path.StartsWith(hdPath))
                {
                    var info = departmentidentites.FirstOrDefault(x => x.Identity.Id == department.DepartmentId);
                    if (info == null)
                        continue;
                    department.LastSynchronized = info.EntityStatus.LastExternallySynchronized ?? DateTime.Now;
                    department.EntityStatusId = (int)info.EntityStatus.StatusId;
                    department.EntityStatusReasonId = (int)info.EntityStatus.StatusReasonId;
                    results.Add(_departmentMappingService
                        .ToIdentityStatusDto
                        (_departmentRepository.UpdatePartial(department, x => x.Name)));
                }
            }
            _organizationDbContext.SaveChanges();
            return results;
        }

        public List<HierachyDepartmentIdentityDto> GetDepartmentHierachyDepartmentIdentities(int departmentId, bool includeParentHDs = true, bool includeChildrenHDs = false,
            int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, int? maxChildrenLevel = null, bool countChildren = false,
            List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false, bool getParentNode = false,
            bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null, List<string> jsonDynamicData = null,
            bool checkPermission = false)
        {
            var department = _departmentRepository.GetById(departmentId);
            if (department == null)
            {
                return null;
            }
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);

            return _hierarchyDepartmentService.GetHierarchyDepartmentIdentities(currentHD.HierarchyId, department.DepartmentId, includeParentHDs,
                includeChildrenHDs, ownerId, customerIds, departmentEntityStatuses, maxChildrenLevel, countChildren, departmentTypeIds, departmentName,
                includeDepartmentType: includeDepartmentType, getParentNode: getParentNode, countUser: countUser, 
                countUserEntityStatuses: countUserEntityStatuses, jsonDynamicData: jsonDynamicData,
                checkPermission: checkPermission);
        }
        public async Task<List<HierachyDepartmentIdentityDto>> GetDepartmentHierachyDepartmentIdentitiesAsync(int departmentId, bool includeParentHDs = true, bool includeChildrenHDs = false,
            int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, int? maxChildrenLevel = null, bool countChildren = false,
            List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false, bool getParentNode = false,
            bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null, List<string> jsonDynamicData = null,
            bool checkPermission = false)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
            {
                return null;
            }
            var currentHD = await _hierarchyDepartmentRepository.GetByIdAsync(_workContext.CurrentHdId);

            return await _hierarchyDepartmentService.GetHierarchyDepartmentIdentitiesAsync(currentHD.HierarchyId, department.DepartmentId, includeParentHDs,
                includeChildrenHDs, ownerId, customerIds, departmentEntityStatuses, maxChildrenLevel, countChildren, departmentTypeIds, departmentName,
                includeDepartmentType: includeDepartmentType, getParentNode: getParentNode, countUser: countUser,
                countUserEntityStatuses: countUserEntityStatuses, jsonDynamicData: jsonDynamicData,
                checkPermission: checkPermission);
        }

        public List<HierachyDepartmentIdentityDto> GetDepartmentHierachyDepartmentIdentities(string departmentExtId, bool includeParentHDs = true, bool includeChildrenHDs = false,
            int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, int? maxChildrenLevel = null, bool countChildren = false,
           List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false,
           bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null)
        {
            //to keep backward compatible with old consumer
            var fiterDepartmentStatuses = departmentEntityStatuses.IsNullOrEmpty() ?
            new List<EntityStatusEnum> { EntityStatusEnum.All } : departmentEntityStatuses;

            var department = _departmentRepository.GetDepartments(ownerId, null, customerIds: customerIds,
                departmentIds: null, statusIds: fiterDepartmentStatuses,
                archetypeIds: null, parentDepartmentId: 0,
                childrenDepartmentId: 0,
                departmetTypeExtIds: null, extIds: new List<string> { departmentExtId }).Items.FirstOrDefault();
            if (department == null)
            {
                return null;
            }
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            return _hierarchyDepartmentService.GetHierarchyDepartmentIdentities(currentHD.HierarchyId, department.DepartmentId, includeParentHDs,
                includeChildrenHDs, ownerId, customerIds, departmentEntityStatuses, maxChildrenLevel, countChildren, departmentTypeIds: departmentTypeIds, departmentName: departmentName,
                includeDepartmentType: includeDepartmentType, countUser: countUser, countUserEntityStatuses: countUserEntityStatuses);
        }
        public async Task<List<HierachyDepartmentIdentityDto>> GetDepartmentHierachyDepartmentIdentitiesAsync(string departmentExtId, bool includeParentHDs = true, bool includeChildrenHDs = false,
            int ownerId = 0, List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null, int? maxChildrenLevel = null, bool countChildren = false,
           List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false,
           bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null)
        {
            //to keep backward compatible with old consumer
            var fiterDepartmentStatuses = departmentEntityStatuses.IsNullOrEmpty() ?
            new List<EntityStatusEnum> { EntityStatusEnum.All } : departmentEntityStatuses;

            var department = (await _departmentRepository.GetDepartmentsAsync(ownerId, null, customerIds: customerIds,
                departmentIds: null, statusIds: fiterDepartmentStatuses,
                archetypeIds: null, parentDepartmentId: 0,
                childrenDepartmentId: 0,
                departmetTypeExtIds: null, extIds: new List<string> { departmentExtId })).Items.FirstOrDefault();
            if (department == null)
            {
                return null;
            }
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            return await _hierarchyDepartmentService.GetHierarchyDepartmentIdentitiesAsync(currentHD.HierarchyId, department.DepartmentId, includeParentHDs,
                includeChildrenHDs, ownerId, customerIds, departmentEntityStatuses, maxChildrenLevel, countChildren, departmentTypeIds: departmentTypeIds, departmentName: departmentName,
                includeDepartmentType: includeDepartmentType, countUser: countUser, countUserEntityStatuses: countUserEntityStatuses);
        }

        public IdentityStatusDto GetDepartmentIdentityStatusByExtId(string extId, int customerId)
        {
            var department = _departmentRepository.GetDepartmentByExtId(extId, customerId, true);
            if (department == null)
            {
                return null;
            }
            return _departmentMappingService.ToIdentityStatusDto(department);
        }

        public List<IdentityStatusDto> GetListDepartmentIdentityStatusByExtId(string extId)
        {
            var departmentEntities = _departmentRepository.GetDepartments(_workContext.CurrentOwnerId,
                null, null, null, null, null, 0, 0, null, new List<string> { extId }).Items;

            //departmentEntities = _securityHandler.AllowAccess(departmentEntities, AccessBinaryValues.Read, false);

            List<IdentityStatusDto> result = new List<IdentityStatusDto>();
            foreach (var departmentEntity in departmentEntities)
            {
                var identityStatusDto = _departmentMappingService.ToIdentityStatusDto(departmentEntity);
                if (identityStatusDto != null)
                {
                    result.Add(identityStatusDto);
                }
            }
            return result;
        }

        public IdentityStatusDto GetDepartmentIdentityStatusById(int departmentId)
        {
            var departmentEntity = _departmentRepository.GetById(departmentId);
            if (departmentEntity == null)
            {
                return null;
            }
            //Check security
            //if (_securityHandler.AllowAccess(departmentEntity, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessAllowed)
            //{
            return _departmentMappingService.ToIdentityStatusDto(departmentEntity);
            //}
            //return null;
        }

        public PaginatedList<T> GetDepartments<T>(int ownerId = 0,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<int> customerIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<int> archetypeIds = null,
            int parentDepartmentId = 0,
            int childDepartmentId = 0,
            List<string> departmentTypeExtIds = null,
            List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "") where T : ConexusBaseDto
        {
            var pagingEntity = _departmentRepository.GetDepartments(ownerId,
                userIds,
                customerIds,
                departmentIds,
                statusIds,
                archetypeIds,
                parentDepartmentId,
                childDepartmentId,
                departmentTypeExtIds,
                extIds,
                lastUpdatedBefore,
                lastUpdatedAfter,
                pageIndex,
                pageSize,
                orderBy);

            //Check security
            //Conflicting with paging feature
            //departments = _securityHandler.AllowAccess(departments, AccessBinaryValues.Read, false);

            Func<DepartmentEntity, bool?, T> mapEntityToDtoFunc = (entity, getDynamicPropertiesFlag)
                => (T)_departmentMappingService.ToDepartmentDto(entity, getDynamicPropertiesFlag);

            return pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties);
        }

        public async Task<PaginatedList<T>> GetDepartmentsAsync<T>(int ownerId = 0,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<int> customerIds = null,
            List<int> departmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<int> archetypeIds = null,
            int parentDepartmentId = 0,
            List<int> parentDepartmentIds = null,
            string parentDepartmentExtId = null,
            int childDepartmentId = 0,
            List<string> departmentTypeExtIds = null,
            List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? externallyMastered = null,
            bool? getDynamicProperties = null,
            bool? includeDepartmentType = true,
            string searchText = "",
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "") where T : ConexusBaseDto
        {
            if (parentDepartmentId == 0 && !string.IsNullOrWhiteSpace(parentDepartmentExtId))
            {
                var parentDepartment = await _departmentRepository.GetDepartmentByExtIdAsync(parentDepartmentExtId, customerIds);
                if (parentDepartment == null)
                {
                    throw new CXValidationException(cxExceptionCodes.ERROR_DEPARTMENT_NOT_FOUND, $"Could not found parent department with extId {parentDepartmentExtId}.");
                }
                parentDepartmentId = parentDepartment.DepartmentId;
            }

            var pagingEntity = await _departmentRepository.GetDepartmentsAsync(ownerId: ownerId,
                userIds: userIds,
                customerIds: customerIds,
                departmentIds: departmentIds,
                statusIds: statusIds,
                archetypeIds: archetypeIds,
                parentDepartmentId: parentDepartmentId,
                childrenDepartmentId: childDepartmentId,
                departmetTypeExtIds: departmentTypeExtIds,
                extIds: extIds,
                parentDepartmentIds: parentDepartmentIds,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                externallyMastered: externallyMastered,
                includeDepartmentType: includeDepartmentType,
                searchText: searchText,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy);

            //Check security
            //Conflicting with paging feature
            //departments = _securityHandler.AllowAccess(departments, AccessBinaryValues.Read, false);

            Func<DepartmentEntity, bool?, T> mapEntityToDtoFunc = (entity, getDynamicPropertiesFlag)
                => (T)_departmentMappingService.ToDepartmentDto(entity, getDynamicPropertiesFlag);

            return pagingEntity.ToPaginatedListDto(mapEntityToDtoFunc, getDynamicProperties);
        }
        public List<ConexusBaseDto> GetParentDepartments(int departmentId)
        {
            var departments = _departmentRepository.GetParentDepartment(departmentId);
            //Check security
            //departments = _securityHandler.AllowAccess(departments, AccessBinaryValues.Read, false);
            return MappingResultToDto(departments);

        }
        private List<ConexusBaseDto> MappingResultToDto(List<DepartmentEntity> departments)
        {
            List<ConexusBaseDto> result = new List<ConexusBaseDto>();
            foreach (var item in departments)
            {
                int parentDepartmentId = 0;
                var parentDepartment = item.H_D.Select(t => t.Parent).FirstOrDefault();
                if (parentDepartment != null)
                {
                    parentDepartmentId = parentDepartment.DepartmentId;
                }
                var userDto = _departmentMappingService.ToDepartmentDto(item, parentDepartmentId);
                if (userDto != null)
                {
                    result.Add(userDto);
                }
            }
            return result;
        }
        public List<LevelDto> GetLevels(List<ArchetypeEnum> archytypeIds = null, List<int> departmentIds = null, List<int> departmentTypeIds = null)
        {
            var levels = _departmentTypeRepository.GetDepartmentTypes(0, archetypeIds: archytypeIds,
                departmentIds: departmentIds, departmentTypeIds: departmentTypeIds);
            List<LevelDto> result = new List<LevelDto>();
            foreach (var item in levels)
            {
                result.Add(new LevelDto
                {
                    Identity = new IdentityDto
                    {
                        Id = item.DepartmentTypeId,
                        Archetype = (ArchetypeEnum)Enum.ToObject(typeof(ArchetypeEnum), (int)item.ArchetypeId),
                        ExtId = item.ExtId,
                        OwnerId = item.OwnerId
                    },
                    EntityStatus = new EntityStatusDto { }
                });
            }
            return result;
        }
        public MemberDto AddUser(int departmentId, MemberDto user)
        {

            var userEntity = _validator.ValidateMemberDtoForUpdating(departmentId, user);
            userEntity.DepartmentId = departmentId;

            _userRepository.Update(userEntity);
            _organizationDbContext.SaveChanges();

            return user;
        }
        public MemberDto RemoveUser(int departmentId, MemberDto user)
        {
            int schoolId = 0;
            var userEntity = _validator.ValidateMemberDtoForRemoving(departmentId, user, ref schoolId);
            userEntity.DepartmentId = schoolId;

            _userRepository.Update(userEntity);
            _organizationDbContext.SaveChanges();

            return user;
        }

        /// <summary>
        /// Inserts the department.
        /// </summary>
        /// <param name="parentHd"></param>
        /// <param name="departmentDto">The DepartmentDto.</param>
        /// <returns>Department.</returns>
        public ConexusBaseDto InsertDepartment(HierarchyDepartmentValidationSpecification validationSpecification, DepartmentDtoBase departmentDto)
        {
            var parentHd = _validator.ValidateHierarchyDepartment(validationSpecification);
            //Do the validation
            var entity = _validator.Validate(departmentDto);


            //Map to Department Entity
            var departmentEntity = _departmentMappingService.ToDepartmentEntity(parentHd, entity, departmentDto);

            //Check security
            //if (_securityHandler.AllowAccess(new DepartmentEntity { DepartmentId = parentHd.DepartmentID }, AccessBinaryValues.Create, false).AceType == AccessControlEntryType.AccessDenied)
            //{
            //    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_DEPARTMENT);
            //}


            //Call insert department Entity
            departmentEntity = _departmentRepository.Insert(departmentEntity);
            _organizationDbContext.SaveChanges();

            //Map entity to Dto for return later
            var reponsingDto = _departmentMappingService.ToDepartmentDto(departmentEntity, parentHd.DepartmentId);

            InsertEvent(departmentEntity, reponsingDto, EventType.CREATED);

            //Remap to DepartmentBaseDto
            return reponsingDto;
        }

        /// <summary>
        /// Inserts the department.
        /// </summary>
        /// <param name="departmentDto">The DepartmentDto.</param>
        /// <returns>Department.</returns>
        public ConexusBaseDto UpdateDepartment(HierarchyDepartmentValidationSpecification validationSpecification, DepartmentDtoBase departmentDto)
        {
            var parentHd = _validator.ValidateHierarchyDepartment(validationSpecification);
            //Do the validation
            var entity = _validator.Validate(departmentDto);
           
            var oldEntityStatusId = entity.EntityStatusId;
            var oldEntityStatusReasonId = entity.EntityStatusReasonId;

            //Map to Department Entity
            //Don't need to map parent department id for updating
            var departmentEntity = _departmentMappingService.ToDepartmentEntity(new HierarchyDepartmentEntity(), entity, departmentDto);

            if (departmentEntity == null) return null;

            var isChangedStatus = departmentEntity.EntityStatusId != oldEntityStatusId;


            //Check security
            //if (_securityHandler.AllowAccess(departmentEntity, AccessBinaryValues.Update, false).AceType == AccessControlEntryType.AccessDenied)
            //{
            //    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_DEPARTMENT);
            //}


            //Until now, we only need to get modified property when entity status is changed. This method should be call before updating entity
            var modifiedProperties = isChangedStatus ? _departmentRepository.GetModifiedProperties(departmentEntity) : null;

            //Call insert department Entity
            departmentEntity = _departmentRepository.Update(departmentEntity);
            _organizationDbContext.SaveChanges();
            //Map entity to Dto for return later
            var reponsingDto = _departmentMappingService.ToDepartmentDto(departmentEntity, parentHd.DepartmentId);


            if (isChangedStatus)
            {
                //If status is changed we write an event with type <archetype>_ENTITYSTATUS_CHANGED, 
                var statusChangedInfo = DomainHelper.BuildEntityStatusChangedEventInfo(oldEntityStatusId, oldEntityStatusReasonId, departmentEntity.EntityStatusId, departmentEntity.EntityStatusReasonId);

                InsertEvent(departmentEntity, statusChangedInfo, EventType.ENTITYSTATUS_CHANGED);

                //Check if info are also changed, we write more an event with type <archetype>_UPDATED

                if (DomainHelper.IsChangedInfo(modifiedProperties, nameof(UserEntity.LastUpdated), nameof(UserEntity.LastUpdatedBy), nameof(UserEntity.EntityStatusId)))
                {
                    InsertEvent(departmentEntity, reponsingDto, EventType.UPDATED);
                }

            }
            else
            {
                InsertEvent(departmentEntity, reponsingDto, EventType.UPDATED);
            }

            //Remap to DepartmentBaseDto
            return reponsingDto;
        }

        public void UpdateObjectMappingsEmployee(List<string> classExtIds, long employeeId, int customerId)
        {
            const int relationTypeId = (int)RelationTypes.H_DAccess;
            var customer = _customerRepository.GetById(customerId);
            foreach (var classExtId in classExtIds)
            {
                var department = GetDepartmentByExtIdIncludeHd(classExtId, customer.OwnerId);
                var currentHd = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                var hierachyDepartment = _hierarchyDepartmentService.GetHierachyDepartment(currentHd.HierarchyId, department.DepartmentId);
                if (!CheckExistObjectMapping(hierachyDepartment.HDId, employeeId))
                {
                    var newObjMapping = new ObjectMappingEntity
                    {
                        OwnerId = _workContext.CurrentOwnerId,
                        FromTableTypeId = (int)TableTypes.User,
                        FromId = (int)employeeId,
                        ToTableTypeId = (int)TableTypes.H_D,
                        ToId = (int)hierachyDepartment.HDId,
                        RelationTypeId = relationTypeId
                    };
                    _commonService.AddObjectMapping(newObjMapping);
                }
            }
        }
        private bool CheckExistObjectMapping(int hdId, long userId)
        {
            const int relationTypeId = (int)RelationTypes.H_DAccess;
            var objectMappings = _commonService.FindObjectMappings(
                _workContext.CurrentOwnerId,
                (int)TableTypes.User,
                new List<int> { (int)userId },
                (int)TableTypes.H_D,
                relationTypeId).Where(s => s.ToId == hdId).ToList();
            return objectMappings.Count > 0;
        }
        public DepartmentEntity UpdateDepartmentStatus(DepartmentEntity departmentEntity, EntityStatusEnum newEntityStatus, EntityStatusReasonEnum newEntitySatusReason, int? updatedById)
        {
            var oldEntityStatusId = departmentEntity.EntityStatusId;
            var oldEntityStatusReasonId = departmentEntity.EntityStatusReasonId;

            //Staus is not changed
            if (oldEntityStatusId == (int)newEntityStatus
                && oldEntityStatusReasonId == (int)newEntitySatusReason)
            {
                return departmentEntity;
            }

            //Check security
            //if (_securityHandler.AllowAccess(departmentEntity, AccessBinaryValues.Update, false).AceType == AccessControlEntryType.AccessDenied)
            //{
            //    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_DEPARTMENT);
            //}

            departmentEntity.EntityStatusId = (int)newEntityStatus;
            departmentEntity.EntityStatusReasonId = (int)newEntitySatusReason;
            departmentEntity.LastUpdatedBy = updatedById ?? _workContext.CurrentUserId;
            departmentEntity.LastUpdated = DateTime.Now;

            var updatedEntity = _departmentRepository.Update(departmentEntity);

            //If status is changed we write an event with type <archetype>_ENTITYSTATUS_CHANGED, 
            var statusChangedInfo = DomainHelper.BuildEntityStatusChangedEventInfo(oldEntityStatusId, oldEntityStatusReasonId,
                departmentEntity.EntityStatusId, departmentEntity.EntityStatusReasonId);

            InsertEvent(departmentEntity, statusChangedInfo, EventType.ENTITYSTATUS_CHANGED);

            return updatedEntity;
        }

        public DepartmentEntity UpdateDepartmentStatus(int departmentId, EntityStatusEnum newEntityStatus, EntityStatusReasonEnum newEntitySatusReason, int? updatedById)
        {
            var departmentEntity = _departmentRepository.GetById(departmentId);
            if (departmentEntity == null)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("Id : {0}", departmentId));
            }
            return UpdateDepartmentStatus(departmentEntity, newEntityStatus, newEntitySatusReason, updatedById);
        }

        private void InsertEvent(DepartmentEntity departmentEntity, object additionalInformation, EventType eventType)
        {
            var hierarchyInfo = _hierarchyDepartmentRepository.GetHierarchyInfo(_workContext.CurrentHdId,
                departmentEntity.DepartmentId, departmentEntity?.H_D.FirstOrDefault(), true);

            dynamic body = new ExpandoObject();
            body.DepartmentData = additionalInformation;
            body.DepartmentId = departmentEntity.DepartmentId;
            body.DepartmentArcheTypeId = departmentEntity.ArchetypeId;
            body.HierarchyInfo = hierarchyInfo;
       

            var objectType = departmentEntity.ArchetypeId == null ||
                             departmentEntity.ArchetypeId == (int) ArchetypeEnum.Unknown
                ? "unknown_archetype_department"
                : ((ArchetypeEnum) departmentEntity.ArchetypeId).ToString();

            var eventMessage = new LogEventMessage(eventType.ToEventName(objectType), _workContext)
                .EntityId(departmentEntity.DepartmentId.ToString())
                .Entity("domain", "department")
                .WithBody(body);

            _datahubLogger.WriteEventLog(eventMessage);
        }
   
        /// <summary>
        /// Get department.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns>Department.</returns>
        public ConexusBaseDto GetDepartment(HierarchyDepartmentValidationSpecification validationSpecification, int departmentId)
        {
            var parentHd = _validator.ValidateHierarchyDepartment(validationSpecification);

            var departmentEntity = _departmentRepository.GetById(departmentId);
            if (departmentEntity == null) return null;

            //Check security
            //if (_securityHandler.AllowAccess(departmentEntity, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessDenied)
            //{
            //    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_DEPARTMENT);
            //}

            return _departmentMappingService.ToDepartmentDto(departmentEntity, parentHd.DepartmentId);
        }

        /// <summary>
        /// Get department.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns>Department.</returns>
        public ConexusBaseDto GetDepartment(int departmentId)
        {
            var departmentEntity = _departmentRepository.GetById(departmentId);
            var parentDepartmentId = departmentEntity.H_D.Select(t => t.Parent.DepartmentId).FirstOrDefault();
            if (departmentEntity == null) return null;

            //Check security
            //if (_securityHandler.AllowAccess(departmentEntity, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessDenied)
            //{
            //    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_DEPARTMENT);
            //}

            return _departmentMappingService.ToDepartmentDto(departmentEntity, parentDepartmentId);
        }
        /// <summary>
        /// Get the child departments by their parent department.
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns>Department.</returns>
        public List<ConexusBaseDto> GetDepartments(HierarchyDepartmentValidationSpecification validationSpecification, int parentId)
        {
            var hierarchyDepartment = _validator.ValidateHierarchyDepartment(validationSpecification);
            var childHierarchyDepartments = _hierarchyDepartmentRepository.GetChildHds(hierarchyDepartment.Path, null,
                true, true, _workContext.CurrentOwnerId, _workContext.CurrentCustomerId);
            var departments = _departmentRepository.GetDepartmentsByDepartmentIds(childHierarchyDepartments.Select(x => x.DepartmentId).ToList(), true);

            //Check security
            //departments = _securityHandler.AllowAccess(departments, AccessBinaryValues.Read, false);

            List<ConexusBaseDto> result = new List<ConexusBaseDto>();
            foreach (var item in departments)
            {
                var departmentDto = _departmentMappingService.ToDepartmentDto(item, hierarchyDepartment.DepartmentId);
                if (departmentDto != null)
                {
                    result.Add(departmentDto);
                }
            }
            return result;
        }

        /// <summary>
        /// Inserts the department.
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="customerId"></param>
        /// <returns>Department.</returns>
        public ConexusBaseDto GetDepartment(string extId, int customerId)
        {
            var departmentEntity = _departmentRepository.GetDepartmentByExtId(extId, customerId);
            var parentDepartmentId = departmentEntity.H_D.Select(t => t.Parent.DepartmentId).FirstOrDefault();
            if (departmentEntity == null) return null;

            //Check security
            //if (_securityHandler.AllowAccess(departmentEntity, AccessBinaryValues.Read, false).AceType == AccessControlEntryType.AccessDenied)
            //{
            //    throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_DEPARTMENT);
            //}

            return _departmentMappingService.ToDepartmentDto(departmentEntity, parentDepartmentId);
        }

        public DepartmentEntity GetDepartmentById(int Id)
        {
            return _departmentRepository.GetById(Id);
        }

        public DepartmentEntity GetDepartment(int departmentId, int ownerId, bool includeInActiveStatus = false)
        {
            return _departmentRepository.GetDepartment(departmentId, ownerId, includeInActiveStatus); ;
        }

        public DepartmentEntity GetDepartmentByExtIdIncludeHd(string extId, int ownerId, bool includeInActiveStatus = false)
        {
            return _departmentRepository.GetDepartmentByExtIdIncludeHd(extId, ownerId, includeInActiveStatus);
        }

        public DepartmentEntity GetDepartmentByExtId(string departmentExtId, string customerExtId, bool includeInActiveStatus = false)
        {
            var customer = _customerRepository.GetCustomerByExtId(customerExtId);
            if (customer == null) return null;
            return _departmentRepository.GetDepartmentByExtId(departmentExtId, customer.CustomerId, includeInActiveStatus);
        }
        public List<DepartmentDto> GetListChildDepartmentByDepartmentId(int departmentId)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var result = new List<DepartmentDto>();
            var hirerachyDepartment = _hierarchyDepartmentRepository.GetHdByHierarchyIdAndDepartmentId(currentHD.HierarchyId, departmentId, true);
            var childHds = _hierarchyDepartmentRepository.GetChildrenHDsByHDID(hirerachyDepartment.HDId);

            result.AddRange(childHds.Select(c => new DepartmentDto
            {
                DepartmentId = c.DepartmentId,
                Name = c.Department.Name,
                ParentId = departmentId,
                HdId = c.HDId
            }));
            return result;
        }
        public DepartmentEntity UpdateDepartmentType(DepartmentEntity department, DepartmentTypeEntity addingDepartmentType, bool isUniqueDepartmentType = false)
        {

            if (isUniqueDepartmentType)
            {
                //removing same department type with addingDepartmentType if it's existed
                for (int i = 0; i < department.DT_Ds.Count; i++)
                {
                    DTDEntity dtd = department.DT_Ds.ElementAt(i);
                    if (dtd.DepartmentType != null && dtd.DepartmentType.ArchetypeId == addingDepartmentType.ArchetypeId)
                    {
                        department.DT_Ds.Remove(dtd);
                        i--;
                    }
                }
            }
            department.DT_Ds.Add(new DTDEntity { DepartmentId = department.DepartmentId, DepartmentTypeId = addingDepartmentType.DepartmentTypeId });
            _organizationDbContext.SaveChanges();
            return department;

        }

        public List<DepartmentEntity> GetDepartmentByNames(List<string> departmentNames)
        {
            return _departmentRepository.GetDepartmentByNames(departmentNames);
        }

        public List<IdentityStatusDto> UpdateDepartmentLastSyncDate(List<IdentityStatusDto> departments)
        {
            var departmentIds = departments.Select(x => (int)x.Identity.Id).ToList();
            var departmentsDb = _departmentRepository.GetDepartmentsByDepartmentIds(departmentIds);
            var results = new List<IdentityStatusDto>();

            foreach (var department in departmentsDb)
            {
                var info = departments.FirstOrDefault(x => x.Identity.Id == department.DepartmentId);
                if (info == null)
                    continue;
                department.LastSynchronized = DateTime.Now;
                results.Add(_departmentMappingService.ToIdentityStatusDto(_departmentRepository.UpdatePartial(department, x => x.Name)));
            }
            _organizationDbContext.SaveChanges();
            return results;
        }

        public DepartmentEntity GetDepartmentByIdIncludeHd(int departmentId, int ownerId, int customerId)
        {
            return _departmentRepository.GetDepartmentByIdIncludeHd(departmentId, ownerId, customerId);
        }
    }
}
