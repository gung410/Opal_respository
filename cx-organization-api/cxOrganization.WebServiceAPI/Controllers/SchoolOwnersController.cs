using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using cxOrganization.Client.Departments;
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
    /// Schools owner API controller 
    /// </summary>
    [Authorize]
    public class SchoolOwnersController : ApiControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IWorkContext _workContext;
        private readonly IHierarchyDepartmentService _hierarchyDepartmentService;
        private readonly Func<ArchetypeEnum, IDepartmentService> _departmentServiceDelegate;
        private readonly Func<ArchetypeEnum, IUserService> _userServiceDelegate;

        /// <summary>
        /// Schools controller api
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="hierarchyDepartmentService"></param>
        /// <param name="userService"></param>
        public SchoolOwnersController(Func<ArchetypeEnum, IDepartmentService> departmentService, Func<ArchetypeEnum, IUserService> userService,
            IWorkContext workContext, IHierarchyDepartmentService hierarchyDepartmentService)
        {
            _userServiceDelegate = userService;
            _departmentService = departmentService(ArchetypeEnum.SchoolOwner);
            _workContext = workContext;
            _hierarchyDepartmentService = hierarchyDepartmentService;
            _departmentServiceDelegate = departmentService;

        }
        /// <summary>
        /// Get school owners 
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="schoolownerIds"></param>
        /// <param name="statusEnums">schoolowner StatusEnums</param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <param name="parentDepartmentId"></param>
        /// <param name="selectIdentity"></param>
        /// <param name="extIds"></param>
        /// <returns></returns>
        [Route("schoolowners", Name = "classes:find_schoolowners_by_parameters")]
        [HttpGet]
        [ProducesResponseType(typeof(List<SchoolOwnerDto>),200)]
        public IActionResult GetSchoolOwners(
            int parentDepartmentId = 0,
            [FromQuery] List<int> userIds = null,
            [FromQuery] List<int> schoolownerIds = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            statusEnums = statusEnums ?? new List<EntityStatusEnum> { EntityStatusEnum.All };

            var pagingDto = _departmentService.GetDepartments<SchoolOwnerDto>(userIds: userIds,
                extIds : extIds,
                departmentIds: schoolownerIds,
                statusIds: statusEnums,
                parentDepartmentId: parentDepartmentId,
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.SchoolOwner },
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
        /// Insert schools owner
        /// </summary>
        /// <param name="schoolOwnerDto"></param>
        /// <returns></returns>
        [Route("schoolowners")]
        [HttpPost]
        [ProducesResponseType(typeof(SchoolOwnerDto),200)]
        public IActionResult InsertSchoolOwner([FromBody]SchoolOwnerDto schoolOwnerDto)
        {
            schoolOwnerDto.Identity.Id = 0;
            //Set default parent department where the new school owner belong to
            schoolOwnerDto.ParentDepartmentId = schoolOwnerDto.ParentDepartmentId > 0 ? schoolOwnerDto.ParentDepartmentId : 18448;
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)schoolOwnerDto.ParentDepartmentId, ArchetypeEnum.Unknown)
            .IsNotInArchetypes(new List<ArchetypeEnum> { ArchetypeEnum.Class, ArchetypeEnum.School })
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .Create();
            var departmentDtoBase = (SchoolOwnerDto)_departmentService.InsertDepartment(validationSpecification, schoolOwnerDto);
            departmentDtoBase.ParentDepartmentId = schoolOwnerDto.ParentDepartmentId;
            return StatusCode((int)HttpStatusCode.Created, departmentDtoBase);
        }

        /// <summary>
        /// Get school owner
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}")]
        [HttpGet]
        [ProducesResponseType(typeof(SchoolOwnerDto),200)]
        public IActionResult GetSchoolOwner(int schoolownerId)
        {
            var department = _departmentService.GetDepartments<SchoolOwnerDto>(departmentIds: new List<int>() { schoolownerId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (department == null)
                CreateNotFoundResponse<SchoolOwnerDto>();
            return CreateResponse(department);
        }
        /// <summary>
        /// Update school owner
        /// </summary>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}", Name = "schoolowners:update")]
        [HttpPut]
        [ProducesResponseType(typeof(SchoolOwnerDto),200)]
        public IActionResult UpdateSchoolOwner(int schoolownerId, [FromBody]SchoolOwnerDto schoolOwner)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolOwner.ParentDepartmentId ?? 18448, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .IsNullArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            schoolOwner.Identity.Id = schoolownerId;
            var schoolResponse = _departmentService.UpdateDepartment(validationSpecification, schoolOwner);
            return CreateResponse(schoolResponse);
        }

        /// <summary>
        /// Get school owner's learner
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="statusEnums"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/learners")]
        [HttpGet]
        [ProducesResponseType(typeof(List<LearnerDto>),200)]
        public IActionResult GetLearnersOnSchoolOwner(int schoolownerId,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var departmentIds = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(schoolownerId);

            departmentIds.Add(schoolownerId);

            var pagingDto = _userServiceDelegate(ArchetypeEnum.Learner).GetUsers<LearnerDto>(parentDepartmentIds: departmentIds,
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Learner },
                statusIds: statusEnums,
                pageSize:pageSize,
                pageIndex:pageIndex,
                orderBy : orderBy);

            return CreatePagingResponse(pagingDto.Items,  pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);

        }
        /// <summary>
        /// Get school owner's learner
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="learnerId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/learners/{learnerId}")]
        [HttpGet]
        [ProducesResponseType(typeof(LearnerDto),200)]
        public IActionResult GetLearnerOnSchoolOwner(int schoolownerId, int learnerId)
        {
            var departmentIds = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(schoolownerId);
            departmentIds.Add(schoolownerId);
            var learners = _userServiceDelegate(ArchetypeEnum.Learner)
                            .GetUsers<LearnerDto>(parentDepartmentIds: departmentIds,
                                      userIds: new List<int>() { learnerId },
                                      archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Learner },
                                      statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            return CreateResponse(learners);
        }

        /// <summary>
        /// Get school owner's classes
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="statusEnums"> Class status enums,Default is Active</param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/classes")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ClassDto>),200)]
        public IActionResult GetClassesOnSchoolOwner(int schoolownerId,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var departmentIds = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(schoolownerId);

            var pagingDto = _departmentServiceDelegate(ArchetypeEnum.Class)
                            .GetDepartments<ClassDto>(departmentIds: departmentIds,
                                                      archetypeIds: new List<int>() { (int)ArchetypeEnum.Class },
                                                      statusIds: statusEnums,
                                                      pageSize: pageSize,
                                                      pageIndex: pageIndex,
                                                      orderBy: orderBy);

            return CreatePagingResponse(pagingDto.Items,  pagingDto.PageIndex, pagingDto.PageSize, pagingDto.HasMoreData);

        }
        /// <summary>
        /// Get school owner's class
        /// </summary>
        /// <param name="schoolownerId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerId}/classes/{classId}")]
        [HttpGet]
        [ProducesResponseType(typeof(ClassDto),200)]
        public IActionResult GetClassOnSchoolOwner(int schoolownerId,
            int classId)
        {
            var departmentId = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(schoolownerId).FirstOrDefault(x => x == classId);
            var classDto = _departmentServiceDelegate(ArchetypeEnum.Class)
                                    .GetDepartments<ClassDto>(departmentIds: new List<int>() { departmentId },
                                                            statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            return CreateResponse<ClassDto>(classDto);
        }
    }
}
