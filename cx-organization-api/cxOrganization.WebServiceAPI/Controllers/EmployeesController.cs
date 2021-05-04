using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using cxOrganization.Business.DeactivateOrganization.DeactivateUser;
using cxOrganization.Client;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Validators;
using cxOrganization.WebServiceAPI.Extensions;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Employee API 
    /// </summary>
    [Authorize]
    public class EmployeesController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IUserTypeService _userTypeService;
        private readonly ILoginServiceUserService _loginServiceUserService;
        private readonly IDeactivateUserService<EmployeeDto> _deactivateUserService;
        private readonly IUGMemberService _userGroupUserMemberService;
        private readonly IApprovalGroupMemberService _approvalGroupMemberService;
        private readonly AppSettings _appSettings;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="workContext"></param>
        /// <param name="userTypeService"></param>
        /// <param name="loginServiceUserService"></param>
        public EmployeesController(Func<ArchetypeEnum, IUserService> userService,
            IAdvancedWorkContext workContext,
            Func<ArchetypeEnum, IUserTypeService> userTypeService,
            ILoginServiceUserService loginServiceUserService,
            IDeactivateUserService<EmployeeDto> deactivateUserService,
            IUGMemberService ugMemberService,
            IApprovalGroupMemberService approvalGroupMemberService,
            IOptions<AppSettings> appSettingOptions)
        {
            _userService = userService(ArchetypeEnum.Employee);
            _workContext = workContext;
            _userTypeService = userTypeService(ArchetypeEnum.Employee);
            _loginServiceUserService = loginServiceUserService;
            _deactivateUserService = deactivateUserService;
            _userGroupUserMemberService = ugMemberService;
            _approvalGroupMemberService = approvalGroupMemberService;
            _appSettings = appSettingOptions.Value;
        }

        /// <summary>
        /// Get employees by parameters
        /// </summary>
        /// <param name="employeeIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="statusEnums">Employee status enum, default is active</param>
        /// <param name="extIds"></param>
        /// <param name="ageRanges"></param>
        /// <param name="genders"></param>
        /// <param name="ssnList"></param>
        /// <param name="getLoginServiceClaims"></param>
        /// <param name="getRoles"></param>
        /// <param name="ssn"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="getDynamicProperties"></param>
        /// <param name="orderBy"></param>
        /// <param name="filterOnParentHd"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="loginServiceIds"></param>
        /// <param name="selectIdentity"></param>
        /// <param name="userNames"></param>
        /// <param name="searchKey"></param>
        /// <param name="loginServiceClaims"></param>
        /// <param name="loginServiceClaimTypes"></param>
        /// <param name="roleIds">List of id of role. (Role id is id of UserType now)</param>
        /// <param name="roleExtIds">List of extId of role. (Role ExtId is extid of UserType now)</param>/// <returns></returns>
        [Route("employees", Name = "employees:getAll")]
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeDto>), 200)]
        public async Task<IActionResult> GetEmployees([FromQuery] List<int> employeeIds = null,
            [FromQuery]List<int> userGroupIds = null,
            [FromQuery] List<int> parentDepartmentIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<AgeRange> ageRanges = null,
            [FromQuery] List<Gender> genders = null,
            [FromQuery] List<string> ssnList = null,
            [FromQuery] List<string> userNames = null,
             string searchKey = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
            string ssn = null,
            [FromQuery] List<string> loginServiceClaims = null,
            [FromQuery] List<string> loginServiceClaimTypes = null,
            [FromQuery] List<int> loginServiceIds = null,
            [FromQuery] List<int> roleIds = null,
            [FromQuery] List<string> roleExtIds = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool? filterOnParentHd = true,
            bool? filterOnSubDepartment = null,
            [FromQuery] List<int> exceptEmployeeIds = null)
        {
            bool shouldFindByExtId = extIds == null ||
                !extIds.Any()
                && (loginServiceClaimTypes == null || !loginServiceClaimTypes.Any())
                && (loginServiceClaims != null && loginServiceClaims.Count == 1);
            if (shouldFindByExtId)
                extIds = loginServiceClaims;
            if (!string.IsNullOrEmpty(ssn))
            {
                ssnList = ssnList ?? new List<string>();
                ssnList.Add(ssn);
            }

            if (Request.ShouldLimitPageSize())
            {
                pageSize = _appSettings.LimitUserPageSize(pageSize);
            }

            var pagingDto = await _userService.GetUsersAsync<EmployeeDto>(userIds: employeeIds,
                userGroupIds: userGroupIds,
                customerIds: null,
                parentDepartmentIds: parentDepartmentIds,
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Employee },
                userTypeIds: roleIds,
                userTypeExtIds: roleExtIds,
                statusIds: statusEnums,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                getDynamicProperties: getDynamicProperties,
                getLoginServiceClaims: getLoginServiceClaims,
                getRoles: getRoles,
                loginServiceClaims: shouldFindByExtId ? null : loginServiceClaims,
                loginServiceClaimTypes: loginServiceClaimTypes,
                loginServiceIds: loginServiceIds,
                ageRanges: ageRanges,
                genders: genders,
                searchKey: searchKey,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy,
                filterOnParentHd: filterOnParentHd,
                externallyMastered: null,
                filterOnSubDepartment: filterOnSubDepartment,
                exceptUserIds: exceptEmployeeIds);

            return CreatePagingResponse(pagingDto.Items,

                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData,
                selectIdentity);
        }

        /// <summary>
        /// Insert Employee
        /// </summary>
        /// <param name="employeeDto"></param>
        /// <returns>A list of response Employee Dto </returns>
        [Route("employees")]
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        public IActionResult InsertEmployee([FromBody] EmployeeDto employeeDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(employeeDto.EmployerDepartmentId, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            employeeDto.Identity.Id = 0;
            var userDtoBase = _userService.InsertUser(validationSpecification, employeeDto);
            return StatusCode((int)HttpStatusCode.Created, userDtoBase);
        }
        /// <summary>
        ///  Get Employee
        /// </summary>
        /// <param name="employeeid"></param>
        /// <returns></returns>
        [Route("employees/{employeeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        public async Task<IActionResult> GetEmployeeDto(int employeeId)
        {
            var employeeDto = (await _userService.GetUsersAsync<EmployeeDto>(userIds: new List<int>() { employeeId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All })).Items.FirstOrDefault();
            if (employeeDto == null)
                return CreateNotFoundResponse<EmployeeDto>();
            return CreateResponse<EmployeeDto>(employeeDto);
        }
        /// <summary>
        /// Update employee info
        /// </summary>
        /// <param name="employeeid"></param>
        /// <param name="employeeDto"></param>
        /// <param name="skipCheckingEntityVersion"></param>
        /// <returns></returns>
        [Route("employees/{employeeId}")]
        [HttpPut]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        public IActionResult UpdateEmployeeInfo(int employeeId,
            [FromBody]EmployeeDto employeeDto,
            bool skipCheckingEntityVersion = false)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                 .ValidateDepartment(employeeDto.EmployerDepartmentId, ArchetypeEnum.Unknown)
                 .SkipCheckingArchetype()
                 .WithStatus(EntityStatusEnum.All)
                 .IsDirectParent()
                 .Create();

            employeeDto.Identity.Id = employeeId;
            var employee = _userService.UpdateUser(validationSpecification, employeeDto, skipCheckingEntityVersion);
            if (employee == null)
                return CreateNotFoundResponse(string.Format("Learner not found: LearnerId({0}))", employeeId));
            else
                return CreateResponse(employee);
        }

        /// <summary>
        /// Get Employee's memberships
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="archetypeIds">membership archetype enum</param>
        /// <param name="statusIds">membership status enum</param>
        /// <param name="membershipIds"></param>
        /// <param name="membershipExtIds"></param>
        /// <returns></returns>
        [Route("employees/{employeeId}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult GetEmployeeMemberships(int employeeId,
            [FromQuery] List<ArchetypeEnum> archetypeIds = null,
            [FromQuery] List<EntityStatusEnum> statusIds = null,
            [FromQuery] List<int> membershipIds = null,
            [FromQuery] List<string> membershipExtIds = null)
        {
            var memberships = _userService.GetUserMemberships(employeeId,
                ArchetypeEnum.Employee,
                membershipExtIds: membershipExtIds,
                membershipsArchetypeIds: archetypeIds,
                membershipIds: membershipIds,
                membershipStatusIds: statusIds);

            if (memberships.Any())
            {
                return CreateResponse(memberships);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("No membership found: EmployeeId({0})", employeeId));
            }
        }
        /// <summary>
        /// Add role to employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="memberDto">Holding role info</param>
        /// <returns></returns>
        [Route("employees/{employeeId}/memberships/roles")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult AddRoleToEmployee(int employeeId, [FromBody] MemberDto memberDto)
        {
            var responseMemberDto = _userTypeService.UpdateOrInsertUserTypeUser(employeeId,
                memberDto, isUnique: false);
            return CreateResponse(responseMemberDto);
        }
        /// <summary>
        /// Remove employee role
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="memberDto">Holding role info</param>
        /// <returns></returns>
        [Route("employees/{employeeId}/memberships/roles")]
        [HttpPut]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult RemoveEmployeeRole(int employeeId, [FromBody]MemberDto memberDto)
        {
            var responseMemberDto = _userTypeService.DeleteUserTypeUser(employeeId,
                memberDto);
            return CreateResponse(responseMemberDto);
        }
        /// <summary>
        /// Insert loginServiceUser that contains claim value for a employee in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("employees/loginservices")]
        [HttpPost]
        [ProducesResponseType(typeof(LoginServiceUserDto), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult InsertLoginServiceClaim([Required] [FromBody]LoginServiceUserDto loginServiceUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Employee)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var insertedLoginService = _loginServiceUserService.Insert(loginServiceUser);
            return StatusCode((int)HttpStatusCode.Created, insertedLoginService);
        }
        /// <summary>
        /// Update loginServiceUser that contains claim value for a employee in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("employees/loginservices")]
        [HttpPut]
        [ProducesResponseType(typeof(LoginServiceUserDto), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult UpdateLoginServiceClaim([Required][FromBody] LoginServiceUserDto loginServiceUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Employee)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var updatedLoginService = _loginServiceUserService.Update(loginServiceUser);
            return StatusCode((int)HttpStatusCode.OK, updatedLoginService);
        }
        /// <summary>
        /// Delete loginServiceUser that contains claim value for a employee in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("employees/loginservices")]
        [HttpDelete]
        [ProducesResponseType(typeof(LoginServiceUserDto), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult DeleteLoginServiceClaim([Required][FromBody] LoginServiceUserDto loginServiceUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Employee)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var deletedLoginService = _loginServiceUserService.Delete(loginServiceUser);
            return StatusCode((int)HttpStatusCode.OK, deletedLoginService);
        }
        /// <summary>
        /// Get list of loginServiceUser of employee based on given parameters
        /// </summary>
        /// <returns></returns>
        [Route("employees/loginservices")]
        [HttpGet]
        [ProducesResponseType(typeof(List<LoginServiceUserDto>), 200)]
        public IActionResult GetLoginServiceClaims(
            [FromQuery] List<int> employeeIds = null,
            [FromQuery] List<string> employeeExtIds = null,
            [FromQuery] List<int> loginServiceIds = null,
            [FromQuery] List<string> primaryClaimTypes = null,
            [FromQuery] List<EntityStatusEnum> employeeStatuses = null,
            [FromQuery] List<int> siteIds = null,
            [FromQuery] bool? includeLoginServiceHasNullSiteId = null,
            [FromQuery] List<string> claimValues = null,
            [FromQuery] DateTime? createdAfter = null,
            [FromQuery] DateTime? createdBefore = null)
        {

            var loginServiceUsers = _loginServiceUserService.Get(userIds: employeeIds,
                userExtIds: employeeExtIds,
                userArchetypes: new List<ArchetypeEnum> { ArchetypeEnum.Employee },
                loginServiceIds: loginServiceIds,
                primaryClaimTypes: primaryClaimTypes,
                userEntityStatuses: employeeStatuses,
                siteIds: siteIds,
                includeLoginServiceHasNullSiteId: includeLoginServiceHasNullSiteId,
                claimValues: claimValues,
                createdAfter: createdAfter,
                createdBefore: createdBefore);

            return CreateResponse(loginServiceUsers);
        }
        /// <summary>
        /// Deactivate employees
        /// </summary>
        /// <returns></returns>
        [Route("deactivate_employees")]
        [HttpPut]
        [ProducesResponseType(typeof(List<DeactivateUsersResultDto>), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult DeactivateEmployees([FromBody] DeactivateEmployeesDto deactivateEmployees)
        {
            var deactivateEmployeesResult = _deactivateUserService.Deactivate(deactivateEmployees);
            var status = deactivateEmployeesResult.MaxStatus();
            if (status == 0)
                return StatusCode((int)HttpStatusCode.NoContent);
            var statusCode = status == (int)HttpStatusCode.NoContent
                ? HttpStatusCode.OK : (HttpStatusCode)status;
            return StatusCode((int)statusCode, deactivateEmployeesResult);
        }

        /// <summary>
        /// Make leaner memberships for employee's teachinggroup
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("employees/{employeeId}/memberships/teachinggroups")]
        [HttpPost]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult AddLearnerToTeachingGroup(int employeeId, [FromBody] MemberDto memberDto)
        {
            memberDto.MemberRoleId = 2;
            var result = _userGroupUserMemberService.InsertUserGroupUserMembership(employeeId, memberDto);
            return CreateResponse(result);
        }

        /// <summary>
        /// Make employees memberships for approval's group
        /// </summary>
        /// <param name="employeeIds"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("employees/memberships/approvalgroups")]
        [HttpPost]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult AddUserToApprovalGroup([FromBody] MemberDto memberDto, [FromQuery] List<int> employeeIds)
        {
            memberDto.MemberRoleId = 2; // ??
            var result = _approvalGroupMemberService.ProcessAddUserGroupUserMembership(employeeIds, memberDto);
            return CreateResponse(result);
        }

        /// <summary>
        /// Make employees memberships for approval's group
        /// </summary>
        /// <param name="employeeIds"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("employees/memberships/approvalgroups")]
        [HttpDelete]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult RemoveUserFromApprovalGroup([FromBody] MemberDto memberDto, [FromQuery] List<int> employeeIds)
        {
            memberDto.MemberRoleId = 2; // ??
            var result = _approvalGroupMemberService.ProcessRemoveUserGroupUserMembership(employeeIds, memberDto);
            return CreateResponse(result);
        }
    }
}