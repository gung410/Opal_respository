using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using cxOrganization.Client.Departments;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Legacy controller to operate directly on users. Should use archetype controllers (Learner/Employee) instead if possible
    /// </summary>
    [Authorize]
    public class UsersController : ApiControllerBase
    {
        private readonly OrganizationDbContext _unitOfWorkForOrganization;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly IUserTypeService _userTypeService;
        private readonly IUserGroupService _userGroupService;
        private readonly IDepartmentService _departmentService;
        private readonly IOwnerService _ownerService;
        private readonly ICustomerService _customerService;
        private readonly ILoginServiceUserService _loginServiceUserService;
        /// <summary>
        /// Setup controller
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="organizationUnitOfWork"></param>
        /// <param name="userService"></param>
        /// <param name="workContext"></param>
        /// <param name="propertyService"></param>
        /// <param name="loggingService"></param>
        /// <param name="userTypeService"></param>
        /// <param name="userGroupService"></param>
        /// <param name="departmentService"></param>
        /// <param name="ownerService"></param>
        /// <param name="customerService"></param>
        public UsersController(
            OrganizationDbContext organizationUnitOfWork,
            IUserService userService,
            IWorkContext workContext,
            IUserTypeService userTypeService,
            IUserGroupService userGroupService,
            IDepartmentService departmentService,
            IOwnerService ownerService,
            ICustomerService customerService,
            ILoginServiceUserService loginServiceUserService,
            IHierarchyDepartmentPermissionService hierarchyDepartmentPermissionService)
        {
            _unitOfWorkForOrganization = organizationUnitOfWork;
            _userService = userService;
            _workContext = workContext;
            _userTypeService = userTypeService;
            _userGroupService = userGroupService;
            _departmentService = departmentService;
            _ownerService = ownerService;
            _customerService = customerService;
            _loginServiceUserService = loginServiceUserService;
        }


        /// <summary>
        /// Get users
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="customerIds"></param>
        /// <param name="userNames"></param>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="userStatusEnums">Default is Active</param>
        /// <param name="archetypeIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="userIds"></param>
        /// <param name="ssnList"></param>
        /// <param name="extIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="queryOptions"></param>
        /// <param name="getDynamicProperties"></param>
        /// <param name="orderBy"></param>
        /// <param name="ssn"></param>
        /// <param name="loginServiceIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="loginServiceClaims"></param>
        /// <param name="loginServiceClaimTypes"></param>
        /// <returns></returns>
        /// <response code="200">Returns list User's IdentityStatusDto</response>
        /// <response code="400">No content</response>
        [Route("owners/{ownerId}/users")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUsers(int ownerId,
            [FromQuery] List<int> customerIds = null,
            [FromQuery] List<int> userGroupIds = null,
            [FromQuery] List<int> parentDepartmentIds = null,
            [FromQuery] List<EntityStatusEnum> userStatusEnums = null,
            [FromQuery] List<ArchetypeEnum> archetypeIds = null,
            [FromQuery] List<int> userIds = null,
            [FromQuery] List<string> ssnList = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            string ssn = null,
            [FromQuery] List<string> loginServiceClaims = null,
            [FromQuery] List<string> loginServiceClaimTypes = null,
            [FromQuery]List<int> loginServiceIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            if (!string.IsNullOrEmpty(ssn))
            {
                ssnList = ssnList ?? new List<string>();
                ssnList.Add(ssn);
            }
            var pagingDto = _userService.GetUsers<IdentityStatusDto>(ownerId: ownerId,
                userIds: userIds,
                customerIds: customerIds,
                userGroupIds: userGroupIds,
                parentDepartmentIds: parentDepartmentIds,
                statusIds: userStatusEnums,
                archetypeIds: archetypeIds,
                ssnList: ssnList,
                userNames: userNames,
                extIds: extIds,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                getDynamicProperties: getDynamicProperties,
                loginServiceClaimTypes: loginServiceClaimTypes,
                loginServiceIds: loginServiceIds,
                loginServiceClaims: loginServiceClaims,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);
        }
        /// <summary>
        /// Get user belong to Owner
        /// </summary>
        /// <param name="ownerid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/{userId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto), 200)]
        public IActionResult GetUsersBelongToOwnerId(int ownerId,
            int userId)
        {
            var identityStatusDto = _userService.GetUsers<IdentityStatusDto>(ownerId: ownerId,
                userIds: new List<int> { userId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            return CreateResponse<IdentityStatusDto>(identityStatusDto);
        }

        /// <summary>
        /// Get UserGroups User
        /// </summary>
        /// <param name="ownerid"></param>
        /// <param name="userid"></param>
        /// <param name="customerIds"></param>
        /// <param name="departmentIds"></param>
        /// <param name="userGroupStatusEnums"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/{userId}/usergroups")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUserGroupsUser(int ownerid,
            int userid,
            [FromQuery] List<int> customerIds,
            [FromQuery] List<int> departmentIds,
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
            var pagingDto = _userGroupService.GetUserGroups<IdentityStatusDto>(ownerId: ownerid,
                customerIds: customerIds,
                departmentIds: departmentIds,
                statusIds: userGroupStatusEnums,
                archetypeIds: archetypeIds,
                extIds: extIds,
                userGroupIds: userGroupIds,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                memberUserIds: new List<int> { userid },
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);
        }


        /// <summary>
        /// Get usergroup of user
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/{userId}/usergroups/{usergroupid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto), 200)]
        public IActionResult GetUserGroupUser(int ownerId,
            int userId, int userGroupId)
        {
            var userGroup = _userGroupService.GetUserGroups<IdentityStatusDto>(ownerId: ownerId,
               memberUserIds: new List<int> { userId },
               userGroupIds: new List<int> { userGroupId },
               statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<IdentityStatusDto>(userGroup);
        }

        /// <summary>
        /// Get Department IdentityStatusDto of User
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <param name="customerIds"></param>
        /// <param name="departmentIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="departmentStatusEnums"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="extIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/{userId}/departments")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUserDepartments(int ownerId,
            int userId,
            [FromQuery] List<int> customerIds,
            [FromQuery] List<int> departmentIds,
            [FromQuery] List<int> userGroupIds,
            [FromQuery] List<EntityStatusEnum> departmentStatusEnums,
            [FromQuery] List<int> archetypeIds,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _departmentService.GetDepartments<IdentityStatusDto>(ownerId: ownerId,
                userIds: new List<int> { userId },
                customerIds: customerIds,
                departmentIds: departmentIds,
                userGroupIds: userGroupIds,
                statusIds: departmentStatusEnums,
                archetypeIds: archetypeIds,
                extIds: extIds,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items, pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);
        }
        /// <summary>
        /// Get Department IdentityStatusDto of User
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/{userId}/departments/{departmentid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto), 200)]
        public IActionResult GetUserDepartment(int ownerId,
            int userId, int departmentId)
        {
            var departmentIdentityStatusDto = _departmentService.GetDepartments<IdentityStatusDto>(ownerId: ownerId,
               userIds: new List<int> { userId },
               departmentIds: new List<int> { departmentId },
               statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            return CreateResponse<IdentityStatusDto>(departmentIdentityStatusDto);
        }

        /// <summary>
        /// Get Usertypes of user
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <param name="userTypeArchetypeIds"></param>
        /// <param name="userTypeIds"></param>
        /// <param name="extIds"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/{userId}/usertypes")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IdentityStatusDto>), 200)]
        public IActionResult GetUserTypesUser(int ownerId,
            int userId,
            [FromQuery] List<ArchetypeEnum> userTypeArchetypeIds = null,
            [FromQuery] List<int> userTypeIds = null,
            [FromQuery] List<string> extIds = null)
        {
            var userTypeIdentityStatusDtos = _userTypeService.GetUserTypes(ownerId: ownerId,
                userIds: new List<int> { userId },
                archetypeIds: userTypeArchetypeIds,
                userTypeIds: userTypeIds,
                extIds: extIds);
            if (userTypeIdentityStatusDtos.Any())
            {
                return CreateResponse(userTypeIdentityStatusDtos);
            }
            else
            {
                return CreateNotFoundResponse<IdentityStatusDto>();
            }
        }

        /// <summary>
        /// Get Usertype of user
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <param name="userTypeId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/{userId}/usertypes/{usertypeid}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<UserTypeDto>), 200)]
        public IActionResult GetUserTypeUser(int ownerId,
            int userId,
            int userTypeId)
        {
            var userTypeIdentityStatusDto = _userTypeService.GetUserTypes(ownerId: ownerId,
                userIds: new List<int> { userId },
                userTypeIds: new List<int> { userTypeId }, includeLocalizedData: false).FirstOrDefault();
            return CreateResponse<UserTypeDto>(userTypeIdentityStatusDto);
        }
        /// <summary>
        /// Get list hierarchy department by user id
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/{userId}/hierarchydepartmentidentifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]
        public async Task<IActionResult> GetHierarchyDepartmentsByUserIdWithoutCxToken([FromRoute]int ownerId,
            [FromRoute] int userId,
            [FromQuery] string departmentName = null,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null,
            [FromQuery] bool includeDepartmentType = false,
            [FromQuery] bool countChildren = false,
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery] bool getParentNode = false,
            [FromQuery] bool countUser = false,
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null,
            [FromQuery] List<string> jsonDynamicData = null)
        {


            var customerIds = _workContext.CurrentCustomerId > 0 ? new List<int> { _workContext.CurrentCustomerId } : null;
            var hierarchyDepartmentIdentitiesResult = await _userService.GetUserHierarchyDepartmentIdentitiesAsync(
                ownerId: ownerId,
                userId: userId,
                userExtId: null,
                includeParentHDs: includeParent,
                includeChildrenHDs: includeChildren,
                customerIds: customerIds,
                departmentEntityStatuses: departmentEntityStatuses,
                maxChildrenLevel: maxChildrenLevel,
                countChildren: countChildren,
                departmentTypeIds: departmentTypeIds,
                departmentName: departmentName,
                includeDepartmentType: includeDepartmentType,
                getParentNode: getParentNode,
                countUser: countUser,
                countUserEntityStatuses: countUserEntityStatuses,
                jsonDynamicData: jsonDynamicData);

            if (hierarchyDepartmentIdentitiesResult.HierachyDepartmentIdentities == null)
                return CreateNotFoundResponse(string.Format("User not found: UserId({0})", userId));

            if (hierarchyDepartmentIdentitiesResult.AccessDenied)
                return AccessDenied();

       
            return CreateResponse(hierarchyDepartmentIdentitiesResult.HierachyDepartmentIdentities);
        }

        /// <summary>
        /// Get list hierarchy department by user extId
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="extId"></param>
        /// <returns></returns>
        [Route("owners/{ownerId}/users/extId/{extId}/hierarchydepartmentidentifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]
        public async Task<IActionResult> GetHierarchyDepartmentsByUserExtIdWithoutCxToken([FromRoute] int ownerId,
            [FromRoute] string extId,
            [FromQuery] string departmentName = null,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null,
            [FromQuery] bool includeDepartmentType = false,
            [FromQuery] bool countChildren = false,
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery] bool getParentNode = false,
            [FromQuery] bool countUser = false,
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null,
            [FromQuery] List<string> jsonDynamicData = null)
        {


            var customerIds = _workContext.CurrentCustomerId > 0 ? new List<int> { _workContext.CurrentCustomerId } : null;
            var hierarchyDepartmentIdentitiesResult = await _userService.GetUserHierarchyDepartmentIdentitiesAsync(
                ownerId: ownerId,
                userId: null,
                userExtId: extId,
                includeParentHDs: includeParent,
                includeChildrenHDs: includeChildren,
                customerIds: customerIds,
                departmentEntityStatuses: departmentEntityStatuses,
                maxChildrenLevel: maxChildrenLevel,
                countChildren: countChildren,
                departmentTypeIds: departmentTypeIds,
                departmentName: departmentName,
                includeDepartmentType: includeDepartmentType,
                getParentNode: getParentNode,
                countUser: countUser,
                countUserEntityStatuses: countUserEntityStatuses,
                jsonDynamicData: jsonDynamicData);

            if (hierarchyDepartmentIdentitiesResult.HierachyDepartmentIdentities == null)
                return CreateNotFoundResponse(string.Format("User not found: ExtId({0})", extId));

            if (hierarchyDepartmentIdentitiesResult.AccessDenied)
                return AccessDenied();


            return CreateResponse(hierarchyDepartmentIdentitiesResult.HierachyDepartmentIdentities);
        }
        /// <summary>
        /// Get list hierarchy department by user extId
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        [Route("users/extId/{extId}/hierarchydepartmentidentifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]
        public async Task<IActionResult> GetHierarchyDepartmentsByUserId(
            [FromRoute] string extId,
            [FromQuery] string departmentName = null,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null,
            [FromQuery] bool includeDepartmentType = false,
            [FromQuery] bool countChildren = false,
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery] bool getParentNode = false,
            [FromQuery] bool countUser = false,
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null,
            [FromQuery] List<string> jsonDynamicData = null)
        {


            var customerIds = _workContext.CurrentCustomerId > 0 ? new List<int> { _workContext.CurrentCustomerId } : null;
            var hierarchyDepartmentIdentitiesResult = await _userService.GetUserHierarchyDepartmentIdentitiesAsync(
                ownerId: _workContext.CurrentOwnerId,
                userId: null,
                userExtId: extId,
                includeParentHDs: includeParent,
                includeChildrenHDs: includeChildren,
                customerIds: customerIds,
                departmentEntityStatuses: departmentEntityStatuses,
                maxChildrenLevel: maxChildrenLevel,
                countChildren: countChildren,
                departmentTypeIds: departmentTypeIds,
                departmentName: departmentName,
                includeDepartmentType: includeDepartmentType,
                getParentNode: getParentNode,
                countUser: countUser,
                countUserEntityStatuses: countUserEntityStatuses,
                jsonDynamicData: jsonDynamicData);

            if (hierarchyDepartmentIdentitiesResult.HierachyDepartmentIdentities == null)
                return CreateNotFoundResponse(string.Format("User not found: ExtId({0})", extId));

            if (hierarchyDepartmentIdentitiesResult.AccessDenied)
                return AccessDenied();


            return CreateResponse(hierarchyDepartmentIdentitiesResult.HierachyDepartmentIdentities);
        }

        /// <summary>
        /// Get list hierarchy department by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("users/{userId}/hierarchydepartmentidentifiers")]
        [HttpGet]
        [ProducesResponseType(typeof(List<HierachyDepartmentIdentityDto>), 200)]
        public async Task<IActionResult> GetHierarchyDepartmentsByUserExtId(
            [FromRoute] int userId,
            [FromQuery] string departmentName = null,
            [FromQuery] bool includeParent = true,
            [FromQuery] bool includeChildren = false,
            [FromQuery] List<EntityStatusEnum> departmentEntityStatuses = null,
            [FromQuery] int? maxChildrenLevel = null,
            [FromQuery] bool includeDepartmentType = false,
            [FromQuery] bool countChildren = false,
            [FromQuery] List<int> departmentTypeIds = null,
            [FromQuery] bool getParentNode = false,
            [FromQuery] bool countUser = false,
            [FromQuery] List<EntityStatusEnum> countUserEntityStatuses = null,
            [FromQuery] List<string> jsonDynamicData = null)
        {


            var customerIds = new List<int> { _workContext.CurrentCustomerId };
            var hierarchyDepartmentIdentitiesResult = await _userService.GetUserHierarchyDepartmentIdentitiesAsync(
                ownerId: _workContext.CurrentOwnerId,
                userId: userId,
                userExtId: null,
                includeParentHDs: includeParent,
                includeChildrenHDs: includeChildren,
                customerIds: customerIds,
                departmentEntityStatuses: departmentEntityStatuses,
                maxChildrenLevel: maxChildrenLevel,
                countChildren: countChildren,
                departmentTypeIds: departmentTypeIds,
                departmentName: departmentName,
                includeDepartmentType: includeDepartmentType,
                getParentNode: getParentNode,
                countUser: countUser,
                countUserEntityStatuses: countUserEntityStatuses,
                jsonDynamicData: jsonDynamicData);

            if (hierarchyDepartmentIdentitiesResult.HierachyDepartmentIdentities == null)
                return CreateNotFoundResponse(string.Format("User not found: UserId({0})", userId));

            if (hierarchyDepartmentIdentitiesResult.AccessDenied)
                return AccessDenied();


            return CreateResponse(hierarchyDepartmentIdentitiesResult.HierachyDepartmentIdentities);
        }
        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [Route("users/getbyobjectmappings")]
        [HttpGet]
        [ProducesResponseType(typeof(IdentityStatusDto), 200)]
        public IActionResult GetUserByObjectMappings([FromQuery] List<int> userIds)
        {
            var userDto = _userService.GetUserIdentitiesByObjectMapping(userIds);
            return CreateResponse(userDto);
        }

        /// <summary>
        /// Insert loginServiceUser that contains claim value for a user in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("users/loginservices")]
        [HttpPost]
        [ProducesResponseType(typeof(LoginServiceUserDto), 200)]
        public IActionResult InsertLoginServiceClaim([Required] LoginServiceUserDto loginServiceUser)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ValidateUserArchetype(loginServiceUser);

            var insertedLoginService = _loginServiceUserService.Insert(loginServiceUser);
            return CreateResponse(insertedLoginService);
        }


        /// <summary>
        /// Update loginServiceUser that contains claim value for a user in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("users/loginservices")]
        [HttpPut]
        [ProducesResponseType(typeof(LoginServiceUserDto), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult UpdateLoginServiceClaim([Required] LoginServiceUserDto loginServiceUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ValidateUserArchetype(loginServiceUser);

            var updatedLoginService = _loginServiceUserService.Update(loginServiceUser);
            return Ok(updatedLoginService);
        }

        /// <summary>
        /// Delete loginServiceUser that contains claim value for a user in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("users/loginservices")]
        [HttpDelete]
        [ProducesResponseType(typeof(LoginServiceUserDto), 200)]
        public IActionResult DeleteLoginServiceClaim([Required][FromBody] LoginServiceUserDto loginServiceUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ValidateUserArchetype(loginServiceUser);

            var deletedLoginService = _loginServiceUserService.Delete(loginServiceUser);
            return Ok(deletedLoginService);
        }
        private static void ValidateUserArchetype(LoginServiceUserDto loginServiceUser)
        {
            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Candidate &&
                loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Employee &&
                loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Learner)

            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            }
        }

        /// <summary>
        /// Get list of loginServiceUser of user based on given parameters
        /// </summary>
        /// <returns></returns>
        [Route("users/loginservices")]
        [HttpGet]
        [ProducesResponseType(typeof(List<LoginServiceUserDto>), 200)]
        public IActionResult GetLoginServiceClaims(
            [FromQuery] List<int> userIds = null,
            [FromQuery] List<string> userExtIds = null,
            [FromQuery] List<ArchetypeEnum> userArchetypes = null,
            [FromQuery] List<int> loginServiceIds = null,
            [FromQuery] List<string> primaryClaimTypes = null,
            [FromQuery] List<EntityStatusEnum> userEntityStatuses = null,
            [FromQuery] List<int> siteIds = null,
            [FromQuery] bool? includeLoginServiceHasNullSiteId = null,
            [FromQuery] List<string> claimValues = null,
            [FromQuery] DateTime? createdAfter = null,
            [FromQuery] DateTime? createdBefore = null)
        {
            var loginServiceUsers = _loginServiceUserService.Get(userIds: userIds,
                userExtIds: userExtIds,
                userArchetypes: userArchetypes,
                loginServiceIds: loginServiceIds,
                primaryClaimTypes: primaryClaimTypes,
                userEntityStatuses: userEntityStatuses,
                siteIds: siteIds,
                includeLoginServiceHasNullSiteId: includeLoginServiceHasNullSiteId,
                claimValues: claimValues,
                createdAfter: createdAfter,
                createdBefore: createdBefore);

            return CreateResponse(loginServiceUsers);
        }

        

    }
}
