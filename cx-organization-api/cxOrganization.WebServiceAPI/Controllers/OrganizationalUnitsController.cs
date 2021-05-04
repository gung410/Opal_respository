using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using cxOrganization.Business.Extensions;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxOrganization.WebServiceAPI.ActionFilters;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    public class OrganizationalUnitsController : ApiControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IUserService _userService;
        /// <summary>
        /// Controller constructor
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="userService"></param>
        public OrganizationalUnitsController(Func<ArchetypeEnum, IDepartmentService> departmentService,
            IAdvancedWorkContext workContext,
            Func<ArchetypeEnum, IUserService> userService)
        {
            _departmentService = departmentService(ArchetypeEnum.OrganizationalUnit);
            _workContext = workContext;
            _userService = userService(ArchetypeEnum.Employee);
        }

        /// <summary>
        /// Get organizationalunits
        /// </summary>
        /// <param name="parentDepartmentId"></param>
        /// <param name="searchText"></param>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="parentDepartmentExtId"></param>
        /// <param name="userIds"></param>
        /// <param name="organizationalUnitIds"></param>
        /// <param name="organizationalUnitExtIds"></param>
        /// <param name="statusEnums"></param>
        /// <param name="departmentTypeExtIds"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="selectIdentity"></param>
        /// <param name="externallyMastered"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [Obsolete("This method is obsolete, we need to implement POST method. Call organizationalunits/v2 instead.", false)]
        [Route("organizationalunits", Name = "organizationalunits:find_organizationalunit_by_parameters")]
        [HttpGet]
        [ProducesResponseType(typeof(List<OrganizationalUnitDto>), 200)]
        public async Task<IActionResult> GetOrganizationalUnits(
            int parentDepartmentId = 0,
            [FromQuery] string searchText = "",
            [FromQuery] List<int> parentDepartmentIds = null,
            [FromQuery] string parentDepartmentExtId = null,
            [FromQuery] List<int> userIds = null,
            [FromQuery] List<int> organizationalUnitIds = null,
            [FromQuery] List<string> organizationalUnitExtIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<string> departmentTypeExtIds = null,
            [FromQuery] bool isByPassFilter = false,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool selectIdentity = false,
            bool? externallyMastered = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            if (!ValidateMinimalFilter(parentDepartmentId,
                                       parentDepartmentIds,
                                       parentDepartmentExtId,
                                       organizationalUnitIds,
                                       organizationalUnitExtIds,
                                       departmentTypeExtIds,
                                       externallyMastered) && !isByPassFilter)
            {
                return CreateNoContentResponse();
            }

            var pagingDto = await _departmentService.GetDepartmentsAsync<OrganizationalUnitDto>(userIds: userIds,
                parentDepartmentId: parentDepartmentId,
                parentDepartmentIds: parentDepartmentIds,
                parentDepartmentExtId: parentDepartmentExtId,
                departmentIds: organizationalUnitIds,
                statusIds: statusEnums,
                extIds: organizationalUnitExtIds,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.OrganizationalUnit },
                departmentTypeExtIds: departmentTypeExtIds,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                externallyMastered: externallyMastered,
                includeDepartmentType: false,   // Set to False to not include in the query into database but still map into DTOs.
                searchText: searchText,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items,
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData,
                selectIdentity);
        }

        /// <summary>
        /// Get organizationalunits by POST method to avoid the query parameters's length limitation.
        /// </summary>
        /// <param name="organizationalUnitDtoV2"></param>
        /// <returns>List<OrganizationalUnitDto></returns>
        [Route("organizationalunits/v2", Name = "organizationalunitsV2:find_organizationalunit_by_payload")]
        [HttpPost]
        [ProducesResponseType(typeof(List<OrganizationalUnitDto>), 200)]
        public async Task<IActionResult> GetOrganizationalUnitsV2(OrganizationalUnitDtoV2 organizationalUnitDtoV2)
        {
            if (!ValidateMinimalFilter(organizationalUnitDtoV2.ParentDepartmentId,
                                       organizationalUnitDtoV2.ParentDepartmentIds,
                                       organizationalUnitDtoV2.ParentDepartmentExtId,
                                       organizationalUnitDtoV2.OrganizationalUnitIds,
                                       organizationalUnitDtoV2.OrganizationalUnitExtIds,
                                       organizationalUnitDtoV2.DepartmentTypeExtIds,
                                       organizationalUnitDtoV2.ExternallyMastered) && !organizationalUnitDtoV2.IsByPassFilter)
            {
                return CreateNoContentResponse();
            }

            var pagingDto = await _departmentService.GetDepartmentsAsync<OrganizationalUnitDto>(userIds: organizationalUnitDtoV2.UserIds,
                parentDepartmentId: organizationalUnitDtoV2.ParentDepartmentId,
                parentDepartmentIds: organizationalUnitDtoV2.ParentDepartmentIds,
                parentDepartmentExtId: organizationalUnitDtoV2.ParentDepartmentExtId,
                departmentIds: organizationalUnitDtoV2.OrganizationalUnitIds,
                statusIds: organizationalUnitDtoV2.StatusEnums,
                extIds: organizationalUnitDtoV2.OrganizationalUnitExtIds,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.OrganizationalUnit },
                departmentTypeExtIds: organizationalUnitDtoV2.DepartmentTypeExtIds,
                lastUpdatedBefore: organizationalUnitDtoV2.LastUpdatedBefore,
                lastUpdatedAfter: organizationalUnitDtoV2.LastUpdatedAfter,
                externallyMastered: organizationalUnitDtoV2.ExternallyMastered,
                includeDepartmentType: false,   // Set to False to not include in the query into database but still map into DTOs.
                searchText: organizationalUnitDtoV2.SearchText,
                pageSize: organizationalUnitDtoV2.PageSize,
                pageIndex: organizationalUnitDtoV2.PageIndex,
                orderBy: organizationalUnitDtoV2.OrderBy);

            return CreatePagingResponse(pagingDto.Items,
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData,
                organizationalUnitDtoV2.SelectIdentity);
        }

        private bool ValidateMinimalFilter(int parentDepartmentId,
            List<int> parentDepartmentIds,
            string parentDepartmentExtId,
            List<int> organizationalUnitIds,
            List<string> organizationalUnitExtIds,
            List<string> departmentTypeExtIds,
            bool? externallyMastered = null)
        {
            //Only validate when authorized by user token
            if (!string.IsNullOrEmpty(_workContext.Sub))
            {
                return parentDepartmentId > 0
                || !parentDepartmentIds.IsNullOrEmpty()
                || !string.IsNullOrEmpty(parentDepartmentExtId)
                || !departmentTypeExtIds.IsNullOrEmpty()
                || !organizationalUnitIds.IsNullOrEmpty()
                || !organizationalUnitExtIds.IsNullOrEmpty()
                || externallyMastered.HasValue;
            }
            return true;
        }

        /// <summary>
        /// Insert organizationalunit
        /// </summary>
        /// <param name="organizationalUnitDto"></param>
        /// <returns></returns>
        [Route("organizationalunits", Name = "organizationalunits:insert_organizationalunit")]
        [HttpPost]
        [ProducesResponseType(typeof(OrganizationalUnitDto), 200)]
        [TypeFilter(typeof(PreventXSSFilter))]
        public IActionResult InsertOrganizationalUnit([FromBody] OrganizationalUnitDto organizationalUnitDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(organizationalUnitDto.ParentDepartmentId ?? 0, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            organizationalUnitDto.Identity.Id = 0;
            organizationalUnitDto = (OrganizationalUnitDto)_departmentService.InsertDepartment(validationSpecification, organizationalUnitDto);
            return StatusCode((int)HttpStatusCode.Created, organizationalUnitDto);

        }
        /// <summary>
        /// Get organizationalunit by identifier
        /// </summary>
        /// <param name="organizationalunitid"></param>
        /// <returns></returns>
        [Route("organizationalunits/{organizationalunitid}", Name = "organizationalunits:find_organizationalunit")]
        [HttpGet]
        [ProducesResponseType(typeof(OrganizationalUnitDto), 200)]
        public async Task<IActionResult> GetOrganizationalUnit(int organizationalunitid)
        {
            var department = (await _departmentService.GetDepartmentsAsync<OrganizationalUnitDto>(
                departmentIds: new List<int>() { organizationalunitid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.OrganizationalUnit })).Items.FirstOrDefault();

            if (department == null)
                return CreateNoContentResponse<OrganizationalUnitDto>();
            return CreateResponse(department);
        }
        /// <summary>
        /// Update organizationalunit
        /// </summary>
        /// <param name="organizationalunitid"></param>
        /// <param name="organizationalUnitDto"></param>
        /// <returns></returns>
        [Route("organizationalunits/{organizationalunitid}", Name = "organizationalunits:update_organizationalunit")]
        [HttpPut]
        [ProducesResponseType(typeof(OrganizationalUnitDto), 200)]
        [TypeFilter(typeof(PreventXSSFilter))]
        public IActionResult UpdateOrganizationalUnit(int organizationalunitid, [FromBody] OrganizationalUnitDto organizationalUnitDto)
        {
            // Special code for testing the error handler on the client side.
            if (organizationalUnitDto != null && !string.IsNullOrEmpty(organizationalUnitDto.Name) && organizationalUnitDto.Name.StartsWith("testingerror-code"))
            {
                var statusCodeString = organizationalUnitDto.Name.Split("-").Last();
                if (int.TryParse(statusCodeString, out var statusCode))
                    return new StatusCodeResult(statusCode);
            }

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(organizationalUnitDto.ParentDepartmentId ?? 0, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            organizationalUnitDto.Identity.Id = organizationalunitid;
            var departmentResponse = (OrganizationalUnitDto)_departmentService.UpdateDepartment(validationSpecification, organizationalUnitDto);
            if (departmentResponse == null)
                return CreateNotFoundResponse(string.Format("OrganizationalUnit not found: OrganizationalUnitDtoId({0}))", organizationalunitid));
            else
                return CreateResponse(departmentResponse);
        }
        /// <summary>
        /// Get organizational unit's employees
        /// </summary>
        /// <param name="organizationalunitid"></param>
        /// <param name="statusEnums">employee status enums</param>
        /// <returns></returns>
        [Route("organizationalunits/{organizationalunitid}/employees")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        public IActionResult GetOrganizationalUnitEmployees(int organizationalunitid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var employees = _userService.GetUsers<EmployeeDto>(parentDepartmentIds: new List<int>() { organizationalunitid },
                statusIds: statusEnums,
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Employee }).Items;

            return base.CreateResponse(employees);
        }
        /// <summary>
        /// Get organizationalunit's employee by identifier
        /// </summary>
        /// <param name="organizationalunitid"></param>
        /// <param name="employeeid"></param>
        /// <returns></returns>
        [Route("organizationalunits/{organizationalunitid}/employees/{employeeid}")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        public async Task<IActionResult> GetOrganizationalUnitEmployee(int organizationalunitid,
            int employeeid)
        {
            var employee = (await _userService.GetUsersAsync<EmployeeDto>(
                parentDepartmentIds: new List<int>() { organizationalunitid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All },
                userIds: new List<int> { employeeid },
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Employee })).Items.FirstOrDefault();

            if (employee == null)
                return CreateNoContentResponse<EmployeeDto>();
            return CreateResponse(employee);
        }
    }
}
