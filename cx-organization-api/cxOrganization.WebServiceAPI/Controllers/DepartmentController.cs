using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using cxOrganization.Business.DeactivateOrganization.DeactivateDepartment;
using cxOrganization.Business.MoveOrganization.MoveDepartment;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Dtos.Departments;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxOrganization.Domain.Services;
using cxOrganization.WebServiceAPI.Models;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Department API controller
    /// </summary>
    [Authorize]
    public class DepartmentController : ApiControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;
        private readonly IDepartmentMappingService _departmentMappingService;
        private readonly IUserMappingService _userMappingService;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        private readonly IWorkContext _workContext;
        private readonly IDepartmentTypeService _departmentTypeService;
        private readonly IUserGroupService _userGroupService;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMoveDepartmentService _moveDepartmentService;
        private readonly IDeactivateDepartmentService _deactivateDepartmentService;
        private readonly IHierarchyDepartmentPermissionService _hierarchyDepartmentPermissionService;
        private readonly IDepartmentAccessService _departmentAccessService;

        /// <summary>
        /// Default controller
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="applicationService"></param>
        /// <param name="departmentTypeService"></param>
        /// <param name="hierarchyDepartmentService"></param>
        /// <param name="userService"></param>
        /// <param name="departmentMappingService"></param>
        /// <param name="userMappingService"></param>
        /// <param name="usergroupService"></param>
        /// <param name="hierarchyDepartmentRepository"></param>
        /// <param name="moveDepartmentService"></param>
        /// <param name="deactivateDepartmentService"></param>
        public DepartmentController(
            ILogger<DepartmentController> logger,
            IDepartmentService departmentService,
            IWorkContext workContext,
            IDepartmentTypeService departmentTypeService,
            IHierarchyDepartmentService hierarchyDepartmentService,
            IUserService userService,
            IDepartmentMappingService departmentMappingService,
            IUserMappingService userMappingService,
            IUserGroupService usergroupService,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IUserRepository userRepository,
            IMoveDepartmentService moveDepartmentService,
            IDeactivateDepartmentService deactivateDepartmentService,
            IHierarchyDepartmentPermissionService hierarchyDepartmentPermissionService,
            IDepartmentAccessService departmentAccessService)
        {
            _logger = logger;
            _departmentService = departmentService;
            _workContext = workContext;
            _departmentTypeService = departmentTypeService;
            _hierarchyDepartmentService = hierarchyDepartmentService;
            _userService = userService;
            _departmentMappingService = departmentMappingService;
            _userMappingService = userMappingService;
            _userGroupService = usergroupService;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _userRepository = userRepository;
            _moveDepartmentService = moveDepartmentService;
            _deactivateDepartmentService = deactivateDepartmentService;
            _hierarchyDepartmentPermissionService = hierarchyDepartmentPermissionService;
            _departmentAccessService = departmentAccessService;
        }
        /// <summary>
        /// Get departments by ownerId without filtering CXToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="customerIds"></param>
        /// <param name="departmentIds"></param>
        /// <param name="departmentStatusEnums"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="userIds"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public async Task<IActionResult> GetDepartmentsByOwnerIdWithoutFilterCXToken(int ownerId,
            [FromQuery] List<int> customerIds,
            [FromQuery] List<int> departmentIds,
            [FromQuery] List<EntityStatusEnum> departmentStatusEnums,
            [FromQuery] List<int> archetypeIds,
            [FromQuery] List<int> userIds,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = await _departmentService.GetDepartmentsAsync<IdentityStatusDto>(ownerId: ownerId,
                customerIds: customerIds,
                departmentIds: departmentIds,
                statusIds: departmentStatusEnums,
                archetypeIds: archetypeIds,
                userIds: userIds,
                extIds: extIds,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);
        }
        /// <summary>
        /// Get department by ownerId without filtering CXToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/{departmentId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto), 200)]
        public IActionResult GetDepartmentByOwnerIdWithoutFilterCXToken(int ownerId,
            int departmentId)
        {
            var departmentIdentityStatusDto = _departmentService.GetDepartments<IdentityStatusDto>(ownerId: ownerId,
                        departmentIds: new List<int> { departmentId },
                        statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<IdentityStatusDto>(departmentIdentityStatusDto);
        }
        /// <summary>
        /// Get users by owner and department id without filtering CXToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="departmentId"></param>
        /// <param name="customerIds"></param>
        /// <param name="userStatusEnums"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="userIds"></param>
        /// <param name="ssnList"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/{departmentId}/users")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUsersByOwnerAndDepIdWithoutFilterCXToken(int ownerId,
            int departmentId,
            [FromQuery] List<int> customerIds,
            [FromQuery] List<EntityStatusEnum> userStatusEnums,
            [FromQuery] List<ArchetypeEnum> archetypeIds,
            [FromQuery] List<int> userIds,
            [FromQuery] List<string> ssnList = null,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _userService.GetUsers<IdentityStatusDto>(ownerId: ownerId,
                parentDepartmentIds: new List<int> { departmentId },
                customerIds: customerIds,
                statusIds: userStatusEnums,
                archetypeIds: archetypeIds,
                ssnList: ssnList,
                extIds: extIds,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                userIds: userIds,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);
        }
        /// <summary>
        /// Get user by owner and department Id without filtering CxToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/{departmentId}/users/{userid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto), 200)]
        public IActionResult GetUserByOwnerAndDepIdWithoutFilterCXToken(int ownerId,
            int departmentId,
            int userId)
        {
            var userIdentityStatusDtos = _userService.GetUsers<IdentityStatusDto>(ownerId: ownerId,
                parentDepartmentIds: new List<int> { departmentId },
                userIds: new List<int> { userId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<IdentityStatusDto>(userIdentityStatusDtos);
        }

        /// <summary>
        /// Get user groups by owner and department id without filtering CXToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="departmentId"></param>
        /// <param name="customerIds"></param>
        /// <param name="userIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="userGroupStatusEnums"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/{departmentId}/usergroups")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUserGroupsByOwnerAndDepIdWithoutFilterCXToken(int ownerId,
           int departmentId,
           [FromQuery] List<int> customerIds,
           [FromQuery] List<int> userIds,
           [FromQuery] List<int> userGroupIds,
           [FromQuery] List<EntityStatusEnum> userGroupStatusEnums,
           [FromQuery] List<int> archetypeIds,
           [FromQuery] List<string> extIds = null,
           DateTime? lastUpdatedBefore = null,
           DateTime? lastUpdatedAfter = null,
           int pageIndex = 0,
           int pageSize = 0,
           string orderBy = "")
        {
            var pagingDto = _userGroupService.GetUserGroups<IdentityStatusDto>(ownerId: ownerId,
                customerIds: customerIds,
                departmentIds: new List<int> { departmentId },
                statusIds: userGroupStatusEnums,
                archetypeIds: archetypeIds,
                extIds: extIds,
                userGroupIds: userGroupIds,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                memberUserIds: userIds,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);
        }
        /// <summary>
        /// Get user group by owner and department id without filtering CXToken
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="departmentId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/{departmentId}/usergroups/{usergroupId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto), 200)]
        public IActionResult GetUserGroupByOwnerAndDepIdWithoutFilterCXToken(int ownerId,
            int departmentId, int userGroupId)
        {
            var userGroup = _userGroupService.GetUserGroups<IdentityStatusDto>(ownerId: ownerId,
               departmentIds: new List<int> { departmentId },
               userGroupIds: new List<int> { userGroupId },
               statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<IdentityStatusDto>(userGroup);
        }
        /// <summary>
        /// Get department types of department
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="departmentId"></param>
        /// <param name="departmentTypeArchetypeIds"></param>
        /// <param name="departmentTypeIds"></param>
        /// <param name="extIds"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/{departmentId}/departmenttypes")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetDepartmentTypesOfDepartment(int ownerId,
            int departmentId,
            [FromQuery] List<ArchetypeEnum> departmentTypeArchetypeIds = null,
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery] List<string> extIds = null)
        {
            var departmentTypeIdentityStatusDtos = _departmentTypeService.GetDepartmentTypes(ownerId: ownerId,
                departmentIds: new List<int> { departmentId },
                archetypeIds: departmentTypeArchetypeIds,
                departmentTypeIds: departmentTypeIds,
                extIds: extIds);
            if (departmentTypeIdentityStatusDtos.Any())
            {
                return CreateResponse(departmentTypeIdentityStatusDtos);
            }
            else
            {
                return CreateNotFoundResponse<IdentityStatusDto>();
            }
        }

        /// <summary>
        /// Get Department Type of department
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="departmentId"></param>
        /// <param name="departmentTypeId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/{departmentId}/departmenttypes/{departmenttypeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto), 200)]
        public IActionResult GetDepartmentTypeOfDepartment(int ownerId,
           int departmentId, int departmentTypeId)
        {
            var departmentTypeIdentityStatusDto = _departmentTypeService.GetDepartmentTypes(ownerId: ownerId,
                departmentIds: new List<int> { departmentId },
                departmentTypeIds: new List<int> { departmentTypeId }).FirstOrDefault();
            return CreateResponse<IdentityStatusDto>(departmentTypeIdentityStatusDto);
        }

        /// <summary>
        /// Returns all class identifiers for a school id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="status">if status parameter is provided, result will be filtered on status. Default only active classes will be returned if no status is provided. For example: status=active, status=inactive, status=all</param>
        /// <param name="customer">if customer parameter is provided, result will be filtered on customer</param>
        /// <returns></returns>
        [Route("departments/id/{departmentId}/departments/identifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetDepartmentsById(int departmentId, string status = "all", int? customer = null)
        {
            try
            {
                bool includeInActiveStatus = !string.IsNullOrEmpty(status) && (status.ToLower() == DepartmentStatus.All || status.ToLower() == DepartmentStatus.InActive);
                var department = _departmentService.GetDepartment(departmentId, _workContext.CurrentOwnerId, includeInActiveStatus);
                if (department == null)
                {
                    return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: departmentId({0})) ownerId({1})", departmentId, _workContext.CurrentOwnerId));
                }
                if (customer.HasValue && department.CustomerId != customer)
                {
                    return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: departmentId({0})) customerId({1})", departmentId, customer));
                }
                var currentHd = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                var hierachyDepartment = _hierarchyDepartmentService.GetHierachyDepartment(currentHd.HierarchyId, department.DepartmentId);
                if (hierachyDepartment == null)
                {
                    return CreateResponse(HttpStatusCode.NotFound, string.Format("H_D not found: HierarchyId({0})) DepartmentId({1})", currentHd.HierarchyId, department.DepartmentId));
                }
                var responseDtos = new List<IdentityStatusDto>();
                var hierachydepartments = _hierarchyDepartmentService.GetChildHds(hierachyDepartment.Path, true, includeInActiveStatus);
                foreach (var hierachydepartment in hierachydepartments)
                {
                    if (!string.IsNullOrEmpty(status) && status.ToLower() == DepartmentStatus.InActive && hierachydepartment.Department.EntityStatusId >= (int)EntityStatusEnum.Active)
                    {
                        continue;
                    }
                    var identityStatusDto = _departmentMappingService.ToIdentityStatusDto(hierachydepartment.Department);
                    if (identityStatusDto == null) continue;
                    responseDtos.Add(identityStatusDto);
                }
                return CreateResponse(responseDtos);
            }
            catch (Exception ex)
            {
                return CreateBadRequestResponse(ex.Message);
            }
        }

        /// <summary>
        /// Returns identifiers for all users by department id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="customer"></param>
        /// <param name="includesubdepartments">including users in all sub-departments if parameter includesubdepartments is sent</param>
        /// <param name="includelinkedusers">include users having department (hierarchy) as linked department through U_D if includelinkedusers is sent</param>
        /// <returns></returns>
        [Route("departments/id/{departmentId}/users/identifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUsersByDepartmentId(int departmentId, int? customer = null, bool includesubdepartments = true, bool includelinkedusers = true)
        {
            try
            {
                var department = _departmentService.GetDepartment(departmentId, _workContext.CurrentOwnerId, true);
                if (department == null)
                {
                    return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: departmentId({0})) ownerId({1})", departmentId, _workContext.CurrentOwnerId));
                }
                if (customer.HasValue && department.CustomerId != customer)
                {
                    return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: departmentId({0})) customerId({1})", departmentId, customer));
                }
                var departmentIds = new List<int> { departmentId };
                if (includesubdepartments)
                {
                    var currentHd = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                    var hierachyDepartment = _hierarchyDepartmentService.GetHierachyDepartment(currentHd.HierarchyId, department.DepartmentId);
                    if (hierachyDepartment == null)
                    {
                        return CreateResponse(HttpStatusCode.NotFound, string.Format("H_D not found: HierarchyId({0})) DepartmentId({1})", currentHd.HierarchyId, department.DepartmentId));
                    }
                    var hierachydepartments = _hierarchyDepartmentService.GetChildHds(hierachyDepartment.Path, false, true);
                    foreach (var hierachydepartment in hierachydepartments)
                    {
                        departmentIds.Add(hierachydepartment.DepartmentId);
                    }

                }
                var responseDtos = new List<IdentityStatusDto>();
                var users = _userService.GetListUsersByDepartmentIds(departmentIds, includelinkedusers);
                foreach (var user in users)
                {
                    var identityStatusDto = _userMappingService.ToIdentityStatusDto(user);
                    if (identityStatusDto == null) continue;
                    responseDtos.Add(identityStatusDto);
                }
                return CreateResponse(responseDtos);
            }
            catch (Exception ex)
            {
                return CreateBadRequestResponse(ex.Message);
            }
        }

        /// <summary>
        /// Returns identifiers for all users by department external id
        /// </summary>
        /// <param name="departmentExtId"></param>
        /// <param name="customer"></param>
        /// <param name="includesubdepartments">including users in all sub-departments if parameter includesubdepartments is sent</param>
        /// <param name="includelinkedusers">include users having department (hierarchy) as linked department through U_D if includelinkedusers is sent</param>
        /// <returns></returns>
        [Route("departments/extid/{departmentExtId}/users/identifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUsersByDepartmentExtId(string departmentExtId, int? customer = null, bool includesubdepartments = true, bool includelinkedusers = true)
        {
            try
            {
                var department = _departmentService.GetDepartmentByExtIdIncludeHd(departmentExtId, _workContext.CurrentOwnerId, true);
                if (department == null)
                {
                    return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: departmentExtId({0})) ownerId({1})", departmentExtId, _workContext.CurrentOwnerId));
                }
                if (customer.HasValue && department.CustomerId != customer)
                {
                    return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: departmentExtId({0})) customerId({1})", departmentExtId, customer));
                }
                var departmentIds = new List<int> { department.DepartmentId };
                if (includesubdepartments)
                {
                    var currentHd = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
                    var hierachyDepartment = _hierarchyDepartmentService.GetHierachyDepartment(currentHd.HierarchyId, department.DepartmentId);
                    if (hierachyDepartment == null)
                    {
                        return CreateResponse(HttpStatusCode.NotFound, string.Format("H_D not found: HierarchyId({0})) DepartmentId({1})", currentHd.HierarchyId, department.DepartmentId));
                    }
                    var hierachydepartments = _hierarchyDepartmentService.GetChildHds(hierachyDepartment.Path, true);
                    foreach (var hierachydepartment in hierachydepartments)
                    {
                        departmentIds.Add(hierachydepartment.DepartmentId);
                    }

                }
                var responseDtos = new List<IdentityStatusDto>();
                var users = _userService.GetListUsersByDepartmentIds(departmentIds, includelinkedusers);
                foreach (var user in users)
                {
                    var identityStatusDto = _userMappingService.ToIdentityStatusDto(user);
                    if (identityStatusDto != null)
                    {
                        responseDtos.Add(identityStatusDto);
                    }
                }
                return CreateResponse(responseDtos);
            }
            catch (Exception ex)
            {
                return CreateBadRequestResponse(ex.Message);
            }
        }



        /// <summary>
        /// Returns all teaching groups identifiers for a school id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [Route("departments/id/{departmentId}/usergroups/identifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUserGroups(int departmentId)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hd = _hierarchyDepartmentService.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, departmentId);
            if (hd == null)
            {
                return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: HierarchyId({0})) DepartmentId({1})", currentHD.HierarchyId, departmentId));
            }
            var childHds = _hierarchyDepartmentService.GetChildHds(hd.Path);
            var departmentIds = new List<int> { departmentId };
            departmentIds.AddRange(childHds.Select(x => x.DepartmentId).ToList());

            return CreateResponse(_userGroupService.GetUserGroupIdentifiers(departmentIds));
        }

        /// <summary>
        /// Update department identifiers last sync date
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="departments"></param>
        /// <returns></returns>
        [Route("departments/id/{departmentId}/departments/identifiers")]
        [HttpPut]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult UpdateDepartmentLastSyncData(int departmentId, [FromBody]List<IdentityStatusDto> departments)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hd = _hierarchyDepartmentService.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, departmentId);
            if (hd == null)
            {
                return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: HierarchyId({0})) DepartmentId({1})", currentHD.HierarchyId, departmentId));
            }

            var allowArchetypeIds = new List<int> { (int)ArchetypeEnum.SchoolOwner, (int)ArchetypeEnum.School, (int)ArchetypeEnum.Class };

            return CreateResponse(_departmentService.UpdateDepartmentIdentifiers
                (departments.Where(x => x.Identity != null && x.Identity.Id.HasValue).ToList(),
                allowArchetypeIds, hd.Path));
        }

        /// <summary>
        /// Update department identifiers last sync date
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        [Route("departments/id/{departmentId}/users/identifiers")]
        [HttpPut]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult UpdateUsersLastSyncData(int departmentId, [FromBody]List<IdentityStatusDto> users)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hd = _hierarchyDepartmentService.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, departmentId);
            if (hd == null)
            {
                return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: HierarchyId({0})) DepartmentId({1})", currentHD.HierarchyId, departmentId));
            }
            var allowArchetypeIds = new List<int> { (int)ArchetypeEnum.Employee, (int)ArchetypeEnum.Learner };

            return CreateResponse(_userService.UpdateUserIdentifiers(
                users.Where(x => x.Identity != null && x.Identity.Id.HasValue).ToList(),
                allowArchetypeIds, hd.Path));

        }

        /// <summary>
        /// Update department identifiers last sync date
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="usergroups"></param>
        /// <returns></returns>
        [Route("departments/id/{departmentId}/usergroups/identifiers")]
        [HttpPut]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult UpdateUserGroupLastSyncData(int departmentId, List<IdentityStatusDto> usergroups)
        {
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);
            var hd = _hierarchyDepartmentService.GetHierachyDepartmentByHierachyIdAndDepartmentId(currentHD.HierarchyId, departmentId);
            if (hd == null)
            {
                return CreateResponse(HttpStatusCode.NotFound, string.Format("Department not found: HierarchyId({0})) DepartmentId({1})", currentHD.HierarchyId, departmentId));
            }
            var allowArchetypeIds = new List<int> { (int)ArchetypeEnum.TeachingGroup };

            return CreateResponse(_userGroupService.UpdateUserGroupIdentifiers(
                usergroups.Where(x => x.Identity != null && x.Identity.Id.HasValue).ToList(),
                allowArchetypeIds, hd.Path));
        }

        /// <summary>
        /// Get list hierarchy department by user
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="includeParent">Set true to exclude parent  hierarchy department identifiers. Default value is 'True' </param>
        /// <param name="includeChildren">Set true to include parent  hierarchy department identifiers. Default value is 'False' </param>
        /// <param name="departmentEntityStatuses">List entity status of department. If there is not value set, it will be get department with any status. </param>
        /// <param name="maxChildrenLevel"> If includeChildren=true and maxChildrenLevel >0, we will get children to level maxChildrenLevel, otherwise, we will get all level</param>
        /// <param name="countChildren">Set true to count children hierarchy departmentIdentity of each element </param>
        /// <param name="countUser">Set true to count user of each element</param>
        /// <param name="countUserEntityStatuses">List entity statuses that user will be counted when 'countUser' is set true</param> /// <returns></returns>
        [Route("departments/{departmentId}/hierarchydepartmentidentifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]
        public async Task<IActionResult> GetHierarchyDepartmentByDepartmentId(int departmentId,
            string departmentName = null,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null, 
            [FromQuery] bool includeDepartmentType = false,
            [FromQuery] bool countChildren = false, 
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery]bool getParentNode = false,
            [FromQuery] bool countUser = false, 
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null,
            [FromQuery] List<string> jsonDynamicData = null)
        {
            //Set includeParent is true as default to keep backward compatible with existing consumer
            

            var (AccessDeniedOnRootDepartment, KeepTheRootDepartment) = await CheckDepartmentIdParameterAsync(departmentId);

            // NOTE: As discussed with all sides before releasing R 3.0, we will consider the department hierarchy as public to see since people can easily go for googling it.
            /*
            if (AccessDeniedOnRootDepartment)
            {
                _logger.LogWarning($"Logged-in user with sub '{_workContext.Sub}' does not have access on the root department id '{departmentId}'.");
                return AccessDenied();

            }
            */

            var hierarchyDepartmentIdentities = await GetHierarchyDepartmentIdentitiesAsync(
                departmentId, departmentName,
                departmentEntityStatuses, maxChildrenLevel,
                departmentTypeIds,
                getParentNode,
                jsonDynamicData,
                includeParent,
                includeChildren,
                includeDepartmentType,
                countChildren,
                countUser,
                countUserEntityStatuses,
                KeepTheRootDepartment);

            return CreateResponse(hierarchyDepartmentIdentities);
        }

        /// <summary>
        /// Get list hierarchy department.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="departmentName"></param>
        /// <param name="departmentEntityStatuses">List entity status of department. If there is not value set, it will be get department with any status.</param>
        /// <param name="departmentTypeIds"></param>
        /// <param name="jsonDynamicData"></param>
        /// <param name="getDetailDepartment"></param>
        /// <param name="includeDepartmentType"></param>
        /// <param name="getParentDepartmentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [Route("departments/{departmentId}/hierarchydepartmentidentifiers/v2")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]

        public async Task<IActionResult> GetHierarchyDepartments(
            int departmentId,
            string departmentName = null,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery] List<string> jsonDynamicData = null,
            [FromQuery] bool getDetailDepartment = true,
            [FromQuery] bool includeDepartmentType = false,
            [FromQuery] bool getParentDepartmentId = false,
            [FromQuery] List<int> departmentIds = null,
            int pageIndex = 1,
            int pageSize = 100, 
            string orderBy = null)
        {
            //Set includeParent is true as default to keep backward compatible with existing consumer
            var currentHD = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);

            var department = await _hierarchyDepartmentService.GetHierachyDepartmentAsync(currentHD.HierarchyId, departmentId);
            if (department is null)
            {
                return CreateNotFoundResponse(string.Format("Department not found: DepartmentId({0})", departmentId));
            }

            var items = await _hierarchyDepartmentService.GetAllHdsByPathAsync(
                department.Path,
                departmentName,
                pageIndex,
                pageSize,
                orderBy,
                jsonDynamicData,
                getDetailDepartment,
                departmentEntityStatuses: departmentEntityStatuses,
                departmentTypeIds: departmentTypeIds,
                includeDepartmentType: includeDepartmentType,
                getParentDepartmentId: getParentDepartmentId,
                departmentIds: departmentIds);

            return CreateResponse(items);
        }

        /// <summary>
        /// Get list hierarchy department infos by department identity.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="includeParent">Set true to exclude parent  hierarchy department identifiers. Default value is 'True' </param>
        /// <param name="includeChildren">Set true to include parent  hierarchy department identifiers. Default value is 'False' </param>
        /// <param name="departmentEntityStatuses">List entity status of department. If there is not value set, it will be get department with any status. </param>
        /// <param name="maxChildrenLevel"> If includeChildren=true and maxChildrenLevel >0, we will get children to level maxChildrenLevel, otherwise, we will get all level</param>
        /// <param name="countChildren">Set true to count children hierarchy departmentIdentity of each element </param>
        /// <param name="countUser">Set true to count user of each element</param>
        /// <param name="countUserEntityStatuses">List entity statuses that user will be counted when 'countUser' is set true</param> /// <returns></returns>
        [Route("departments/{departmentId}/hierarchydepartmentinfos")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierarchyDepartmentInfo>), 200)]
        public async Task<IActionResult> GetHierarchyInfosByDepartmentId(int departmentId,
            string departmentName = null,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null,
            [FromQuery] bool includeDepartmentType = false,
            [FromQuery] bool countChildren = false,
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery]bool getParentNode = false,
            [FromQuery] bool countUser = false,
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null,
            [FromQuery] List<string> jsonDynamicData = null)
        {
            //Set includeParent is true as default to keep backward compatible with existing consumer

            var (AccessDeniedOnRootDepartment, KeepTheRootDepartment) = await CheckDepartmentIdParameterAsync(departmentId);

            if (AccessDeniedOnRootDepartment)
            {
                _logger.LogWarning($"Logged-in user with sub '{_workContext.Sub}' does not have access on the root department id '{departmentId}'.");
                return AccessDenied();
            }

            var hierarchyDepartmentIdentities = await GetHierarchyDepartmentIdentitiesAsync(
                departmentId, departmentName,
                departmentEntityStatuses, maxChildrenLevel,
                departmentTypeIds,
                getParentNode,
                jsonDynamicData,
                includeParent,
                includeChildren,
                includeDepartmentType,
                countChildren,
                countUser,
                countUserEntityStatuses,
                KeepTheRootDepartment);

            return CreateResponse(hierarchyDepartmentIdentities.Select(HierarchyDepartmentInfo.CreateFrom));
        }

        /// <summary>
        /// Get list hierarchy department by user
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="ownerId"></param>
        /// <param name="includeParent">Set true to exclude parent hierarchy department identifiers. Default value is 'True' </param>
        /// <param name="includeChildren">Set true to include parent hierarchy department identifiers. Default value is 'False' </param>
        /// <param name="customerIds">List customerId to filter</param>
        /// <param name="departmentEntityStatuses">List entity status of department. If there is not value set, it will be get department with any status. </param>
        /// <param name="maxChildrenLevel"> If includeChildren=true and maxChildrenLevel >0, we will get children to level maxChildrenLevel, otherwise, we will get all level</param>
        /// <param name="countChildren">Set true to count children hierarchy departmentIdentity of each element</param>
        /// <param name="countUser">Set true to count user of each element</param>
        /// <param name="countUserEntityStatuses">List entity statuses that user will be counted when 'countUser' is set true</param> /// <returns></returns>
        [Route("owners/{ownerId}/departments/{departmentId}/hierarchydepartmentidentifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]
        public IActionResult GetHierarchyDepartmentsByDepartmentIdWithoutFilterCXToken(int ownerId, int departmentId,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<int> customerIds = null,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null,
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery] bool countChildren = false,
            [FromQuery] bool countUser = false,
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null)
        {
            //Set includeParent is true as default to keep backward compatible with existing consumer

            _workContext.CurrentOwnerId = ownerId;
            var result = _departmentService.GetDepartmentHierachyDepartmentIdentities(departmentId, includeParent, includeChildren, ownerId, customerIds,
                departmentEntityStatuses, maxChildrenLevel, countChildren, departmentTypeIds, countUser: countUser, countUserEntityStatuses: countUserEntityStatuses);
            if (result == null)
                return CreateNotFoundResponse(string.Format("Department not found: DepartmentId({0})", departmentId));
            return CreateResponse(result);
        }

        /// <summary>
        /// Get list hierarchy department by user
        /// </summary>
        /// <param name="departmentextid"></param>
        /// <param name="includeParent">Set true to exclude parent hierarchy department identifiers. Default value is 'True' </param>
        /// <param name="includeChildren">Set true to include parent hierarchy department identifiers. Default value is 'False' </param>
        /// <param name="departmentEntityStatuses">List entity status of department. If there is not value set, it will be get department with any status. </param>
        /// <param name="maxChildrenLevel"> If includeChildren=true and maxChildrenLevel >0, we will get children to level maxChildrenLevel, otherwise, we will get all level</param>
        /// <param name="countChildren">Set true to count children hierarchy departmentIdentity of each element</param>
        /// <param name="countUser">Set true to count user of each element</param>
        /// <param name="countUserEntityStatuses">List entity statuses that user will be counted when 'countUser' is set true</param>
        /// <returns></returns>
        [Route("departments/extid/{departmentextid}/hierarchydepartmentidentifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]
        public async Task<IActionResult> GetHierarchyDepartmentByDepartmentExtId(string departmentextid,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null,
            [FromQuery] bool countChildren = false,
            [FromQuery] bool countUser = false,
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null)
        {
            //Set includeParent is true as default to keep backward compatible with existing consumer

            var customerids = _workContext.CurrentCustomerId > 0 ? new List<int> { _workContext.CurrentCustomerId } : null;
            var result = await _departmentService.GetDepartmentHierachyDepartmentIdentitiesAsync(departmentextid,
                includeParent,
                includeChildren,
                _workContext.CurrentOwnerId,
                customerids,
                departmentEntityStatuses,
                maxChildrenLevel,
                countChildren,
                countUser: countUser,
                countUserEntityStatuses: countUserEntityStatuses);
            if (result == null)
                return CreateNotFoundResponse(string.Format("Department not found: DepartmentExtId({0})", departmentextid));
            return CreateResponse(result);
        }

        /// <summary>
        /// Get list hierarchy department by user
        /// </summary>
        /// <param name="departmentextid"></param>
        /// <param name="ownerId"></param>
        /// <param name="includeParent">Set true to exclude parent hierarchy department identifiers. Default value is 'True' </param>
        /// <param name="includeChildren">Set true to include parent hierarchy department identifiers. Default value is 'False' </param>
        /// <param name="customerIds">List customerId to filter</param>
        /// <param name="departmentEntityStatuses">List entity status of department. If there is not value set, it will be get department with any status. </param>
        /// <param name="maxChildrenLevel"> If includeChildren=true and maxChildrenLevel >0, we will get children to level maxChildrenLevel, otherwise, we will get all level</param>
        /// <param name="countChildren">Set true to count children hierarchy departmentIdentity of each element</param>
        /// <param name="countUser">Set true to count user of each element</param>
        /// <param name="countUserEntityStatuses">List entity statuses that user will be counted when 'countUser' is set true</param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/extid/{departmentextid}/hierarchydepartmentidentifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]
        public IActionResult GetHierarchyDepartmentsByDepartmentExtIdWithoutFilterCXToken(int ownerId, string departmentextid,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<int> customerIds = null,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null,
            [FromQuery] bool countChildren = false,
            [FromQuery] bool countUser = false,
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null)
        {
            _workContext.CurrentOwnerId = ownerId;
            var result = _departmentService.GetDepartmentHierachyDepartmentIdentities(departmentextid, includeParent, includeChildren, ownerId, customerIds,
                departmentEntityStatuses, maxChildrenLevel, countChildren, countUser: countUser, countUserEntityStatuses: countUserEntityStatuses);
            if (result == null)
                return CreateNotFoundResponse(string.Format("Department not found: DepartmentExtId({0})", departmentextid));
            return CreateResponse(result);
        }

        /// <summary>
        /// Update department last sync 
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userid"></param>
        /// <param name="customerIds"></param>
        /// <param name="archetypeids"></param>
        /// <param name="extids"></param>
        /// <param name="departmentIds"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/departments/updatelastsync")]
        [HttpPut]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult UpdateDepartmentLastSyncDate(int ownerId, [FromQuery] List<int> customerIds = null, [FromQuery] List<int> archetypeids = null, [FromQuery] List<string> extids = null, [FromQuery] List<int> departmentIds = null)
        {
            var departments = _departmentService.GetDepartments<IdentityStatusDto>(ownerId, customerIds, departmentIds:
                departmentIds, extIds: extids, archetypeIds: archetypeids, statusIds:
                new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;
            _departmentService.UpdateDepartmentLastSyncDate(departments);
            return Ok();
        }


        /// <summary>
        /// Move the department into new target parent
        /// </summary>
        /// <returns></returns>
        [Route("move_departments")]
        [HttpPut]
        [ProducesResponseType(typeof(MoveDepartmentsResultDto), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult MoveDepartments([FromBody] MoveDepartmentsDto moveDepartmentDto)
        {
            var moveDepartmentsResultDto = _moveDepartmentService.MoveDepartments(moveDepartmentDto);

            var status = moveDepartmentsResultDto.MaxStatus();

            if (status == 0)
                return StatusCode((int)HttpStatusCode.NoContent);

            var statusCode = status == (int)HttpStatusCode.NoContent
                ? HttpStatusCode.OK : (HttpStatusCode)status;
            return StatusCode((int)statusCode, moveDepartmentsResultDto);
        }
        /// <summary>
        /// Deactivate the department and sub-department
        /// </summary>
        /// <returns></returns>
        [Route("deactivate_departments")]
        [HttpPut]
        [ProducesResponseType(typeof(DeactivateDepartmentsResultDto), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult DeactivateDepartments([FromBody] DeactivateDepartmentsDto deactivateDepartmentDto)
        {
            var departmentDepartmentsResultDto = _deactivateDepartmentService.DeactivateDepartments(deactivateDepartmentDto);

            var status = departmentDepartmentsResultDto.MaxStatus();

            if (status == 0)
                return StatusCode((int)HttpStatusCode.NoContent);

            var statusCode = status == (int)HttpStatusCode.NoContent
                ? HttpStatusCode.OK : (HttpStatusCode)status;
            return StatusCode((int)statusCode, departmentDepartmentsResultDto);
        }

        /// <summary>
        /// Gets the top hierarchy department which the currently logged-in user has access to.
        /// </summary>
        /// <returns></returns>
        [Route("mytophierarchydepartment")]
        [HttpGet]
        [ProducesResponseType(typeof(TopHierarchyDepartmentIdentityDto), 200)]
        public async Task<IActionResult> MyTopHierarchyDepartment()
        {
            var authenticatedUser = await _hierarchyDepartmentPermissionService.GetWorkContextUserAsync();
            if (authenticatedUser == null)
            {
                return CreateNoContentResponse();
            }

            var topHierarchyDepartmentInfo = (await _departmentAccessService.GetTopHierarchyDepartmentsByWorkContext(_workContext));
            var topHierarchyDepartmentIdentity = topHierarchyDepartmentInfo.TopHierachyDepartmentIdentity;

            if (topHierarchyDepartmentIdentity == null) return NotFound();

            BasicHierarchyInfo defaultHierarchyInfo = null;
            var rootDepartmentId = _hierarchyDepartmentPermissionService.GetRootDepartmentId();
            var topHierarchyInfo= topHierarchyDepartmentInfo.AccessibleHierarchyInfos.FirstOrDefault(h => h.DepartmentId == topHierarchyDepartmentIdentity.Identity.Id);

            if (topHierarchyDepartmentIdentity.Identity.Id == rootDepartmentId)
            {
                var keepTheRootDepartment = !_hierarchyDepartmentPermissionService.UserIsAuthenticatedByToken();
                if (!keepTheRootDepartment)
                {
                    defaultHierarchyInfo =
                        topHierarchyDepartmentInfo.AccessibleHierarchyInfos.FirstOrDefault(a =>
                            a.ParentHdId == topHierarchyInfo?.HdId);
                }
            }

            if (defaultHierarchyInfo == null) defaultHierarchyInfo = topHierarchyInfo;

            var topHierarchyDepartmentIdentityDto = topHierarchyDepartmentIdentity.Cast<TopHierarchyDepartmentIdentityDto>();

            topHierarchyDepartmentIdentityDto.DefaultHierarchyDepartment = defaultHierarchyInfo;

            return CreateResponse(topHierarchyDepartmentIdentityDto);
        }



        private async Task<List<HierachyDepartmentIdentityDto>> GetHierarchyDepartmentIdentitiesAsync(
            int departmentId, string departmentName,
            List<EntityStatusEnum> departmentEntityStatuses, int? maxChildrenLevel,
            List<int> departmentTypeIds, bool getParentNode, List<string> jsonDynamicData,
            bool includeParent, bool includeChildren, bool includeDepartmentType,
            bool countChildren, bool countUser,
            List<EntityStatusEnum> countUserEntityStatuses, bool keepTheRootDepartment)
        {
            var shouldCheckSecurity = !(await _hierarchyDepartmentPermissionService.IgnoreSecurityCheckAsync());

            var customerids = _workContext.CurrentCustomerId > 0 ? new List<int> { _workContext.CurrentCustomerId } : null;
            var hierarchyDepartmentIdentities = await _departmentService.GetDepartmentHierachyDepartmentIdentitiesAsync(
                departmentId,
                includeParent,
                includeChildren,
                _workContext.CurrentOwnerId,
                customerids,
                departmentEntityStatuses, maxChildrenLevel, countChildren, departmentTypeIds, departmentName, includeDepartmentType, getParentNode: getParentNode,
                countUser: countUser, countUserEntityStatuses: countUserEntityStatuses,
                jsonDynamicData: jsonDynamicData,
                checkPermission: shouldCheckSecurity);

            if (!keepTheRootDepartment) hierarchyDepartmentIdentities = _hierarchyDepartmentPermissionService.ProcessRemovingTheRootDepartment(hierarchyDepartmentIdentities);
            return hierarchyDepartmentIdentities;
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

    /// <summary>
    /// Department status text
    /// </summary>
    public class DepartmentStatus
    {
        /// <summary>
        /// Inactive department status
        /// </summary>
        public const string InActive = "inactive";
        /// <summary>
        /// All statuses
        /// </summary>
        public const string All = "all";
    }
}
