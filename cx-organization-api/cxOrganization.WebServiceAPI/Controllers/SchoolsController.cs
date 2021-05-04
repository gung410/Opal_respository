using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{

    /// <summary>
    /// Schools API 
    /// </summary>
    [Authorize]
    public class SchoolsController : ApiControllerBase
    {
        private readonly IDepartmentService _schooldepartmentService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IUserService _userService;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        private readonly Func<ArchetypeEnum, IUserService> _userServiceDelegate;
        private readonly Func<ArchetypeEnum, IDepartmentService> _departmentServiceDelegate;

        /// <summary>
        /// Schools API
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="userService"></param>
        /// <param name="hierarchyDepartmentService"></param>
        public SchoolsController(Func<ArchetypeEnum, IDepartmentService> departmentService,
            Func<ArchetypeEnum, IUserService> userService, 
            IHierarchyDepartmentService hierarchyDepartmentService,
            IAdvancedWorkContext workContext)
        {
            _schooldepartmentService = departmentService(ArchetypeEnum.School);
            _workContext = workContext;
            _userServiceDelegate = userService;
            _userService = userService(ArchetypeEnum.Employee);
            _departmentServiceDelegate = departmentService;
            _hierarchyDepartmentService = hierarchyDepartmentService;

        }
        /// <summary>
        /// get all schools
        /// </summary>
        /// <param name="parentDepartmentId"></param>
        /// <param name="queryOptions"></param>
        /// <param name="schoolIds"></param>
        /// <param name="statusEnums">School StatusEnums: Unknown = 0, Active = 1, Inactive = 2, Deactive = 3, All = 99, Default is Active</param>
        /// <param name="extIds">School ExtIds</param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectIdentity"></param>
        /// <returns></returns>
        [Route("schools")]
        [HttpGet]
        [ProducesResponseType(typeof(List<SchoolDto>),200)]
        public IActionResult GetSchools(int parentDepartmentId = 0,
            [FromQuery] List<int> schoolIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<string> extIds = null,
            DateTime? lastUpdatedBefore = null, 
            DateTime? lastUpdatedAfter = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _schooldepartmentService.GetDepartments<SchoolDto>(departmentIds: schoolIds, 
                parentDepartmentId: parentDepartmentId,
                statusIds: statusEnums, 
                archetypeIds: new List<int>() { (int)ArchetypeEnum.School }, 
                extIds:extIds,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items,
                
                pagingDto.PageIndex, 
                pagingDto.PageSize,
                pagingDto.HasMoreData,
                selectIdentity);
        }
        /// <summary>
        /// Insert school
        /// </summary>
        /// <param name="schoolDto"></param>
        /// <returns></returns>
        [Route("schools")]
        [HttpPost]
        [ProducesResponseType(typeof(SchoolDto),200)]
        public IActionResult InsertSchool([FromBody]SchoolDto schoolDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolDto.ParentDepartmentId ?? 0, ArchetypeEnum.SchoolOwner)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            schoolDto.Identity.Id = 0;
            var departmentBaseDto = _schooldepartmentService.InsertDepartment(validationSpecification, schoolDto);
            return StatusCode((int)HttpStatusCode.Created, departmentBaseDto);
        }
        /// <summary>
        /// get school by School Id 
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}")]
        [HttpGet]
        [ProducesResponseType(typeof(SchoolDto),200)]
        public IActionResult GetSchool(int schoolId)
        {
            var schoolDto = _schooldepartmentService.GetDepartments<SchoolDto>(departmentIds: new List<int>() { schoolId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (schoolDto == null)
                return CreateNotFoundResponse<SchoolDto>();
            return CreateResponse<SchoolDto>(schoolDto);
        }

        /// <summary>
        /// Update school by schoolId
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="school"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}")]
        [HttpPut]
        [ProducesResponseType(typeof(SchoolDto),200)]
        public IActionResult UpdateSchool(int schoolId, [FromBody]SchoolDto school)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(school.ParentDepartmentId ?? 0, ArchetypeEnum.SchoolOwner)
                .SkipCheckingArchetype()
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();
            school.Identity.Id = schoolId;
            var schoolResponse = _schooldepartmentService.UpdateDepartment(validationSpecification, school);
            if (schoolResponse == null)
            {
                return CreateNotFoundResponse(String.Format("School not found, (schoolId:{0})", schoolId));

            }
            else
            {
                return CreateResponse(schoolResponse);
            }
        }
        /// <summary>
        /// get employees 
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="statusEnums">employee status enums</param>
        /// <param name="queryOptions"></param>
        /// <param name="selectIdentity"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}/employees")]
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeDto>),200)]
        public IActionResult GetEmployeesInSchool(int schoolId,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _userService.GetUsers<EmployeeDto>(parentDepartmentIds: new List<int>() { schoolId }, 
                statusIds: statusEnums,
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Employee });

            return CreatePagingResponse(pagingDto.Items,
                
                pagingDto.PageIndex,
                pagingDto.PageSize,
                pagingDto.HasMoreData,
                selectIdentity);
        }
        /// <summary>
        /// get employee
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}/employees/{employeeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeDto),200)]
        public IActionResult GetEmployeeInSchool(int schoolId, 
            int employeeId)
        {
            var employee = _userService.GetUsers<EmployeeDto>(parentDepartmentIds: new List<int>() { schoolId },
                userIds: new List<int>() { employeeId }, 
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All}).Items.FirstOrDefault();
            if (employee == null)
                return CreateNotFoundResponse<EmployeeDto>();
            return CreateResponse<EmployeeDto>(employee);
        }
        /// <summary>
        /// get learners
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="statusEnums"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="selectIdentity"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}/learners")]
        [HttpGet]
        [ProducesResponseType(typeof(List<LearnerDto>),200)]
        public IActionResult GetLeanersInSchool(int schoolId,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _userServiceDelegate(ArchetypeEnum.Learner).GetUsers<LearnerDto>(parentDepartmentIds: new List<int>() { schoolId },
                statusIds: statusEnums,
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Learner },
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
        /// get learner
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="learnerid"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}/learners/{learnerid}")]
        [HttpGet]
        [ProducesResponseType(typeof(LearnerDto),200)]
        public IActionResult GetLeanerInSchool(int schoolId,
            int learnerid)
        {
            var learner = _userServiceDelegate(ArchetypeEnum.Learner)
                .GetUsers<LearnerDto>(parentDepartmentIds: new List<int>() { schoolId }, 
                userIds: new List<int>() { learnerid },
                statusIds:new List<EntityStatusEnum> { EntityStatusEnum.All}).Items.FirstOrDefault();
            return CreateResponse<LearnerDto>(learner);
        }

        /// <summary>
        /// Get school's classes
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="statusEnums">class status enums</param>
        /// <param name="queryOptions"></param>
        /// <param name="orderBy">
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}/classes")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ClassDto>),200)]
        public IActionResult GetClassesOnSchool(int schoolId,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool selectIdentity = false)
        {
            var departmentIds = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(schoolId);
            var classDtoPaging = _departmentServiceDelegate(ArchetypeEnum.Class)
                .GetDepartments<ClassDto>(departmentIds: departmentIds,
                                          archetypeIds: new List<int>() { (int)ArchetypeEnum.Class },
                                          pageIndex: pageIndex,
                                          pageSize: pageSize,
                                          orderBy: orderBy,
                                          statusIds: statusEnums);

            return CreatePagingResponse(classDtoPaging.Items,
                                                        classDtoPaging.PageIndex,
                                                        classDtoPaging.PageSize, 
                                                        classDtoPaging.HasMoreData,
                                                        selectIdentity);

        }
        /// <summary>
        /// Get school's class
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="classid"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}/classes/{classid}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ClassDto>),200)]
        public IActionResult GetClassOnSchool(int schoolId, 
            int classid)
        {
            var departmentId = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(schoolId).FirstOrDefault(x => x == classid);
            var classDto = _departmentServiceDelegate(ArchetypeEnum.Class)
                .GetDepartments<ClassDto>(departmentIds: new List<int>() { departmentId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            return CreateResponse<ClassDto>(classDto);
        }
        /// <summary>
        /// Get school's Schoolowner
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        [Route("schools/{schoolId}/schoolowners")]
        [HttpGet]
        [ProducesResponseType(typeof(SchoolOwnerDto),200)]
        public IActionResult GetSchoolSchoolowner(int schoolId)
        {
            var schoolownerdto = _departmentServiceDelegate(ArchetypeEnum.SchoolOwner)
                .GetDepartments<SchoolOwnerDto>(childDepartmentId: schoolId,
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All },
                archetypeIds : new List<int> { (int)ArchetypeEnum.SchoolOwner}).Items.FirstOrDefault();
            return CreateResponse<SchoolOwnerDto>(schoolownerdto);
        }
    }
}
