using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Business.Common;
using cxOrganization.Business.Exceptions;
using cxOrganization.Business.Extensions;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateDepartment
{
    public class DeactivateDepartmentService : IDeactivateDepartmentService
    {
        private readonly ILogger _logger;

        private readonly IAdvancedWorkContext _workContext;
        private readonly OrganizationDbContext _organizationUnitOfWork;
        private readonly IUserService _userService;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        private readonly IDepartmentService _departmentService;

        private readonly DeactivateDepartmentConfig _deactivateDepartmentConfig;
        public DeactivateDepartmentService(IAdvancedWorkContext workContext,
            ILoggerFactory loggerFactory,
            Func<ArchetypeEnum, IUserService> userService,
            OrganizationDbContext organizationUnitOfWork,
            IHierarchyDepartmentService hierarchyDepartmentService,
            IDepartmentService departmentService,
            IOptions<DeactivateDepartmentConfig> deactivateDepartmentConfigOption)
        {
            _workContext = workContext;
            _userService = userService(ArchetypeEnum.Unknown);
            _organizationUnitOfWork = organizationUnitOfWork;
            _hierarchyDepartmentService = hierarchyDepartmentService;
            _deactivateDepartmentConfig = deactivateDepartmentConfigOption.Value;
            _departmentService = departmentService;
            _logger = loggerFactory.CreateLogger<DeactivateDepartmentService>();
        }

        public DeactivateDepartmentsResultDto DeactivateDepartments(DeactivateDepartmentsDto deactivateDepartmentDto)
        {
            ValidateDeactivateDepartmentsDto(deactivateDepartmentDto);

            var updatedByUser = GetValidUpdatedByUser(deactivateDepartmentDto);

            var deactivateDepartmentResultDtos = DeactivateDepartmentsInternal(deactivateDepartmentDto, updatedByUser);

            return new DeactivateDepartmentsResultDto()
            {
                UpdatedByIdentity = updatedByUser.Identity,
                DepartmentResults = deactivateDepartmentResultDtos
            };
        }

        private List<DeactivateDepartmentResultDto> DeactivateDepartmentsInternal(DeactivateDepartmentsDto deactivateDepartmentDto, UserGenericDto updatedByUser)
        {
            var existingHierarchyDepartments = GetExistingHierarchyDepartments(deactivateDepartmentDto.Identities);

            var updatedById = (int)updatedByUser.Identity.Id;

            var deactivateDepartmentResultDtos = new List<DeactivateDepartmentResultDto>();

            //The given list of deactivating can have  hierarchy relationship,
            //we define the list to keep all deactivated department id (even descendant deactivated department)
            //to check and make sure it will not be processed deactivate multiple time

            var allDeactivatedDeparmentAndSubIds = new List<int>();
            var deactivateIfContainingUsers = deactivateDepartmentDto.DeactivateIfContainingUser.HasValue
                ? deactivateDepartmentDto.DeactivateIfContainingUser.Value
                : _deactivateDepartmentConfig.DeactivateIfContainingUsers.Enable;

            var userEntityStatusForChecking = deactivateDepartmentDto.UserEntityStatusesForChecking.IsNullOrEmpty()
                ? _deactivateDepartmentConfig.DeactivateIfContainingUsers.UserEntityStatuses
                : deactivateDepartmentDto.UserEntityStatusesForChecking;

            var deactivateIfContainingChildDepartment = deactivateDepartmentDto.DeactivateIfContainingChildDepartment == true;

            foreach (var deactivatingIdentity in deactivateDepartmentDto.Identities)
            {
                deactivateDepartmentResultDtos.Add(DeactivateDepartment(deactivateIfContainingUsers, deactivateIfContainingChildDepartment, deactivatingIdentity,
                    existingHierarchyDepartments, updatedById, allDeactivatedDeparmentAndSubIds, userEntityStatusForChecking));
            }
            return deactivateDepartmentResultDtos;
        }

        private DeactivateDepartmentResultDto DeactivateDepartment(bool deactivateIfContainingUser, bool deactivateIfContainingChildDepartment, IdentityDto deactivatingIdentity,
            List<HierarchyDepartmentEntity> existingHierarchyDepartments, int updatedById, List<int> allDeactivatedDeparmentIds, List<EntityStatusEnum> userEntityStatusForChecking)
        {
            var existingHierarchyDepartment = GetExistingHierarchyDepartment(deactivatingIdentity, existingHierarchyDepartments);
            if (existingHierarchyDepartment == null)
            {
                return DeactivateDepartmentResultDto.CreateNotFound(deactivatingIdentity);
            }
            deactivatingIdentity.Id = existingHierarchyDepartment.DepartmentId;
            deactivatingIdentity.ExtId = existingHierarchyDepartment.Department.ExtId;

            if (allDeactivatedDeparmentIds.Contains(existingHierarchyDepartment.DepartmentId))
            {
                //already deactivated
                return DeactivateDepartmentResultDto.CreateSuccess(deactivatingIdentity, "Deactivated department");
            }


            var descendantHierarchyDepartmentsOrderedByPath = GetDescendantHierarchyDepartmentsOrderByPath(existingHierarchyDepartment.HierarchyId, existingHierarchyDepartment.HDId);

            if (!deactivateIfContainingChildDepartment
                && descendantHierarchyDepartmentsOrderedByPath.Count > 0)
            {
                return DeactivateDepartmentResultDto.Create(deactivatingIdentity,
                      MessageStatus.CreateNoContent("Not performed due to containing child department"));
            }

            var deactivatingDepartmentAndSubIds = new List<int>(descendantHierarchyDepartmentsOrderedByPath.Select(h => h.DepartmentId)) { existingHierarchyDepartment.DepartmentId };

            if (!deactivateIfContainingUser)
            {
                if (ContainAnyUser(deactivatingDepartmentAndSubIds, userEntityStatusForChecking))
                {
                    return DeactivateDepartmentResultDto.Create(deactivatingIdentity,
                        MessageStatus.CreateNoContent("Not performed due to containing user"));
                }
            }
            else
            {
                //TODO: handle user, deactivate user?
            }

            var messageStatus = ExecuteDeactivateDepartmentAndDescendantDepartments(existingHierarchyDepartment,
                descendantHierarchyDepartmentsOrderedByPath, updatedById);
            var descendantIdenities = descendantHierarchyDepartmentsOrderedByPath.Select(d => d.Department.ToIdentityDto()).ToList();

            if (messageStatus.IsOk())
            {
                allDeactivatedDeparmentIds.AddRange(deactivatingDepartmentAndSubIds);
            }

            return DeactivateDepartmentResultDto.Create(deactivatingIdentity, messageStatus, descendantIdenities);
        }

        private MessageStatus ExecuteDeactivateDepartmentAndDescendantDepartments(
            HierarchyDepartmentEntity hierarchyDepartment,
            List<HierarchyDepartmentEntity> descendantHierarchyDepartments,
            int updatedById)
        {
            try
            {

                foreach (var descendantHierarchyDepartment in descendantHierarchyDepartments)
                {
                    ExecuteDeactivateDepartment(descendantHierarchyDepartment, updatedById);
                }

                ExecuteDeactivateDepartment(hierarchyDepartment, updatedById);
                _organizationUnitOfWork.SaveChanges();

                return MessageStatus.CreateSuccess(string.Format("Deactivated department and its {0} descendant(s)", descendantHierarchyDepartments.Count));

            }
            catch (CXValidationException e)
            {
                _logger.LogError(e.Message, e);
                return MessageStatus.CreateInvalid(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return MessageStatus.CreateError(e.Message);

            }
        }

        private bool ContainAnyUser(List<int> departmentIds, List<EntityStatusEnum> userEntityStatusForChecking)
        {
            var countOfUserInDeactivatingDepartmentAndSub = _userService.CountUsers(
                _workContext.CurrentOwnerId,
                parentDepartmentIds: departmentIds,
                statusIds: userEntityStatusForChecking,
                filterOnParentHd: false);

            return countOfUserInDeactivatingDepartmentAndSub > 0;

        }

        private List<HierarchyDepartmentEntity> GetDescendantHierarchyDepartmentsOrderByPath(int hierarchyId, int parentHdId)
        {
            var descendantHierarchyDepartmentIds =
                _hierarchyDepartmentService
                    .GetAllHDIdsFromAHierachyDepartmentToBelowByHdId(parentHdId)
                    .Where(id => id != parentHdId)
                    .ToList();

            if (descendantHierarchyDepartmentIds.Count == 0)
            {
                return new List<HierarchyDepartmentEntity>();
            }

            //We get descendant hierarchy departments with sorting path descending to process deactivating hierarchy from the leaf to root
            var descendantHierarchyDepartments =
                _hierarchyDepartmentService.GetHierarchyDepartmentEntities(_workContext.CurrentOwnerId,
                    hierarchyId,
                    hdIds: descendantHierarchyDepartmentIds, includeDepartment: true, departmentStatuses: _deactivateDepartmentConfig.AcceptedStatusesForDeactivating, orderBy: "PATH desc");
            return descendantHierarchyDepartments;
        }

        private void ExecuteDeactivateDepartment(HierarchyDepartmentEntity hierarchyDepartment, int updatedByIdId)
        {

            hierarchyDepartment.Deleted = 1;
            var updatedHierarchyDepartment = _hierarchyDepartmentService.UpdateHierarchyDepartment(hierarchyDepartment);

            var departmentEnity = updatedHierarchyDepartment.Department;
            _departmentService.UpdateDepartmentStatus(departmentEnity, EntityStatusEnum.Deactive, EntityStatusReasonEnum.Deactive_SynchronizedFromSource, updatedByIdId);


        }


        private HierarchyDepartmentEntity GetExistingHierarchyDepartment(IdentityDto givenDepartmentIdentity,
            List<HierarchyDepartmentEntity> existingHierarchyDepartments)
        {
            if (givenDepartmentIdentity.Id > 0)
            {
                return existingHierarchyDepartments
                    .FirstOrDefault(h => h.Department.ArchetypeId == (int)givenDepartmentIdentity.Archetype && h.DepartmentId == givenDepartmentIdentity.Id);
            }
            else if (!string.IsNullOrEmpty(givenDepartmentIdentity.ExtId))
            {
                return existingHierarchyDepartments
                    .FirstOrDefault(h => h.Department.ArchetypeId == (int)givenDepartmentIdentity.Archetype && h.Department.ExtId == givenDepartmentIdentity.ExtId);
            }
            return null;
        }


        private List<HierarchyDepartmentEntity> GetExistingHierarchyDepartments(List<IdentityDto> givenDepartmentIdentities)
        {
            var currentHierarchyDepartment = _hierarchyDepartmentService.GetCurrentHierarchyDepartment();
            if (currentHierarchyDepartment == null)
            {
                throw new NotFoundException("Current hierarchy department is not found");
            }

            var customerIds = new List<int?> { _workContext.CurrentCustomerId };
            var departmentIdentitiesHaveId = givenDepartmentIdentities.Where(d => d.Id > 0).ToList();
            var departmentIdentitiesHaveExtId = givenDepartmentIdentities
                .Except(departmentIdentitiesHaveId)
                .Where(d => !string.IsNullOrEmpty(d.ExtId)).ToList();

            var existingHierarchyDepartmentEntities = GetHierachyByDepartmentDepartmentByIds(departmentIdentitiesHaveId, currentHierarchyDepartment.HierarchyId, customerIds);
            existingHierarchyDepartmentEntities.AddRange(GetHierachyByDepartmentByDepartmentExtIds(departmentIdentitiesHaveExtId, currentHierarchyDepartment.HierarchyId, customerIds));

            return existingHierarchyDepartmentEntities;
        }

        private List<HierarchyDepartmentEntity> GetHierachyByDepartmentByDepartmentExtIds(List<IdentityDto> departmentIdentitiesHaveExtId, int hierachyId, List<int?> customerIds)
        {
            if (departmentIdentitiesHaveExtId.Count > 0)
            {
                var departmentExtIds = departmentIdentitiesHaveExtId.Select(d => d.ExtId).ToList();
                var departmentArchetypes = departmentIdentitiesHaveExtId.Select(d => d.Archetype).Distinct().ToList();

                return _hierarchyDepartmentService.GetHierarchyDepartmentEntities(_workContext.CurrentOwnerId,
                    hierachyId, customerIds: customerIds, departmentExtIds: departmentExtIds, departmentArchetypes: departmentArchetypes, includeDepartment: true, departmentStatuses: _deactivateDepartmentConfig.AcceptedStatusesForDeactivating);
            }
            return new List<HierarchyDepartmentEntity>();
        }

        private List<HierarchyDepartmentEntity> GetHierachyByDepartmentDepartmentByIds(List<IdentityDto> departmentIdentitiesHaveId, int hierachyId, List<int?> customerIds)
        {
            if (departmentIdentitiesHaveId.Count > 0)
            {
                var departmentIds = departmentIdentitiesHaveId.Select(d => (int)d.Id).ToList();
                var departmentArchetypes = departmentIdentitiesHaveId.Select(d => d.Archetype).Distinct().ToList();

                return _hierarchyDepartmentService.GetHierarchyDepartmentEntities(_workContext.CurrentOwnerId,
                    hierachyId, customerIds: customerIds, departmentIds: departmentIds, departmentArchetypes: departmentArchetypes, includeDepartment: true, departmentStatuses: _deactivateDepartmentConfig.AcceptedStatusesForDeactivating);
            }

            return new List<HierarchyDepartmentEntity>();
        }

        private void ValidateDeactivateDepartmentsDto(DeactivateDepartmentsDto deactivateDepartmentDto)
        {
            if (deactivateDepartmentDto.Identities.IsNullOrEmpty())
                throw new InvalidException("Identities is null or empty");

        }

        private UserGenericDto GetValidUpdatedByUser(DeactivateDepartmentsDto deactivateDepartmentsDto)
        {
            UserGenericDto updatedByUser = null;
            if (deactivateDepartmentsDto.UpdatedByIdentity != null)
            {
                updatedByUser = GetExistingUser(deactivateDepartmentsDto.UpdatedByIdentity, null);
                if (updatedByUser == null)
                {
                    throw new NotFoundException($"Unable to get executor info from UpdatedByIdentity {deactivateDepartmentsDto.UpdatedByIdentity.ToStringInfo()}");
                }
            }
            else
            {
                updatedByUser = DomainHelper.GetUserFromWorkContext(_workContext, _userService);
                if (updatedByUser == null)
                {
                    throw new NotFoundException($"Unable to get executor info from current context for sub '{_workContext.Sub}'");

                }
            }

            if (_deactivateDepartmentConfig.ExecutiveRoles.IsNotNullOrEmpty())
            {
                var userRoles = updatedByUser.GetAllRoleUserTypes();
                var matchExecutiveRole = userRoles.Any(r => _deactivateDepartmentConfig.ExecutiveRoles.Contains(r.Identity.ExtId, StringComparer.CurrentCultureIgnoreCase));
                if (!matchExecutiveRole)
                {
                    var allowExecutingRoles = string.Join(",", _deactivateDepartmentConfig.ExecutiveRoles);
                    throw new InvalidException($"{updatedByUser.ToStringInfo()} is required to have role in list '{allowExecutingRoles}' to execute deactivating department");
                }
            }
            return updatedByUser;
        }

        private UserGenericDto GetExistingUser(IdentityDto userIdentity, List<EntityStatusEnum> statusEnums) 
        {
            if (userIdentity.Id > 0)
            {
                return _userService.GetUsers<UserGenericDto>(userIdentity.OwnerId,
                        new List<int> { (int)userIdentity.CustomerId },
                        userIds: new List<int> { (int)userIdentity.Id.Value },
                        archetypeIds: new List<ArchetypeEnum> { userIdentity.Archetype },
                        statusIds: statusEnums,
                        getRoles: true)
                    .Items.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(userIdentity.ExtId))
            {
                return _userService.GetUsers<UserGenericDto>(_workContext.CurrentOwnerId,
                        new List<int> { (int)userIdentity.CustomerId },
                        extIds: new List<string> { userIdentity.ExtId },
                        archetypeIds: new List<ArchetypeEnum> { userIdentity.Archetype },
                        statusIds: statusEnums,
                        getRoles: true)
                    .Items.FirstOrDefault();
            }
            return null;
        }

    }
}