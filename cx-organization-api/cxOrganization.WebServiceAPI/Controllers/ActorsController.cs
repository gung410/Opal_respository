using System;
using System.Collections.Generic;
using cxOrganization.Business.DeactivateOrganization.DeactivateUser;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Employee API 
    /// </summary>
    [Authorize]
    public class ActorsController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly IUserTypeService _userTypeService;
        private readonly ILoginServiceUserService _loginServiceUserService;
        private readonly IDeactivateUserService<EmployeeDto> _deactivateUserService;
        private readonly IUGMemberService _userGroupUserMemberService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="workContext"></param>
        /// <param name="userTypeService"></param>
        /// <param name="loginServiceUserService"></param>
        public ActorsController(Func<ArchetypeEnum, IUserService> userService,
            IWorkContext workContext,
            Func<ArchetypeEnum, IUserTypeService> userTypeService,
            ILoginServiceUserService loginServiceUserService,
            IDeactivateUserService<EmployeeDto> deactivateUserService,
            IUGMemberService ugMemberService)
        {
            _userService = userService(ArchetypeEnum.Actor);
            _workContext = workContext;
            _userTypeService = userTypeService(ArchetypeEnum.Employee);
            _loginServiceUserService = loginServiceUserService;
            _deactivateUserService = deactivateUserService;
            _userGroupUserMemberService = ugMemberService;
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
        [Route("actors", Name = "actors:getAll")]
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeDto>), 200)]
        public IActionResult GetActors([FromQuery] List<int> employeeIds = null,
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
            string orderBy = "")
        {
            if (!string.IsNullOrEmpty(ssn))
            {
                ssnList = ssnList ?? new List<string>();
                ssnList.Add(ssn);
            }
            var pagingDto = _userService.SearchActors<EmployeeDto>(userIds: employeeIds,
                userGroupIds: userGroupIds,
                customerIds: null,
                parentDepartmentIds: parentDepartmentIds,
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Employee, ArchetypeEnum.ExternalUser },
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
                loginServiceClaims: loginServiceClaims,
                loginServiceClaimTypes: loginServiceClaimTypes,
                loginServiceIds: loginServiceIds,
                ageRanges: ageRanges,
                genders: genders,
                searchKey: searchKey,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items,
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData);
        }

        
    }
}