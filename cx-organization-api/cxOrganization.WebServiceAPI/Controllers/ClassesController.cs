using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{

    /// <summary> 
    /// Classes API 
    /// </summary>
    [Authorize]
    public class ClassesController : ApiControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IClassMemberService _classMemberService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IUserService _userService;
        private readonly Func<ArchetypeEnum, IDepartmentService> _departmentServiceDelegate;

        /// <summary>
        /// Classes API 
        /// </summary>
        /// <param name="departmentService"></param>
        /// <param name="workContext"></param>
        /// <param name="classMemberService"></param>
        /// <param name="userService"></param>
        public ClassesController(Func<ArchetypeEnum, IDepartmentService> departmentService, Func<ArchetypeEnum, IUserService> userService,
            IAdvancedWorkContext workContext, IClassMemberService classMemberService)
        {
            _departmentService = departmentService(ArchetypeEnum.Class);
            _workContext = workContext;
            _classMemberService = classMemberService;
            _userService = userService(ArchetypeEnum.Learner);
            _departmentServiceDelegate = departmentService;
        }

        /// <summary>
        /// get classes
        /// </summary>
        /// <param name="userIds">Get class that have the userId</param>
        /// <param name="classIds"></param>
        /// <param name="statusEnums">Class StatusEnums: Default is Active</param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="parentDepartmentId">school department in HD department</param>
        /// <param name="extIds"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectIdentity"></param>
        /// <returns></returns>
        [Route("classes", Name = "classes:find_class_by_parameters")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ClassDto>), 200)]
        public IActionResult GetClasses(
            int parentDepartmentId = 0,
            [FromQuery] List<int> userIds = null,
            [FromQuery] List<int> classIds = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool selectIdentity = false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _departmentService.GetDepartments<ClassDto>(userIds: userIds,
                parentDepartmentId: parentDepartmentId,
                departmentIds: classIds,
                extIds: extIds,
                statusIds: statusEnums,
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Class },
                lastUpdatedBefore: lastUpdatedBefore,
                lastUpdatedAfter: lastUpdatedAfter,
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
        /// insert class
        /// </summary>
        /// <param name="classItem"></param>
        /// <returns></returns>
        [Route("classes", Name = "classes:insert_class")]
        [HttpPost]
        [ProducesResponseType(typeof(ClassDto), 200)]
        public IActionResult InsertClass([FromBody]ClassDto classItem)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(classItem.ParentDepartmentId ?? 0, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            classItem.Identity.Id = 0;
            classItem = (ClassDto)_departmentService.InsertDepartment(validationSpecification, classItem);
            return StatusCode((int)HttpStatusCode.Created, classItem);
        }

        /// <summary>
        /// get class by identifier
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        [Route("classes/{classid}", Name = "classes:find_class")]
        [HttpGet]
        [ProducesResponseType(typeof(ClassDto), 200)]
        public IActionResult GetClass(int classid)
        {
            var department = _departmentService.GetDepartments<ClassDto>(departmentIds: new List<int>() { classid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (department == null)
                return CreateNotFoundResponse<ClassDto>();
            return CreateResponse<ClassDto>(department);
        }

        /// <summary>
        /// Update class
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="classDto"></param>
        /// <returns></returns>
        [Route("classes/{classid}", Name = "classes:update_class")]
        [HttpPut]
        [ProducesResponseType(typeof(ClassDto), 200)]
        public IActionResult UpdateClass(int classId, [FromBody]ClassDto classDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(classDto.ParentDepartmentId ?? 0, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            classDto.Identity.Id = classId;
            var classResponse = (ClassDto)_departmentService.UpdateDepartment(validationSpecification, classDto);
            return CreateResponse(classResponse);
        }

        /// <summary>
        /// get learners in class
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="statusEnums">learner status enums</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("classes/{classid}/learners", Name = "classes:findallleaners_class")]
        [HttpGet]
        [ProducesResponseType(typeof(List<LearnerDto>), 200)]
        public IActionResult GetLearnersInClass(int classid,
            [FromQuery] List<EntityStatusEnum> statusEnums = null)
        {
            var leaners = _userService.GetUsers<LearnerDto>(parentDepartmentIds: new List<int>() { classid },
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Learner },
                statusIds: statusEnums).Items;
            return CreateResponse(leaners);
        }

        /// <summary>
        /// Get learner in class
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="learnerid"></param>
        /// <returns></returns>
        [Route("classes/{classid}/learners/{learnerid}", Name = "classes:findleaner_class")]
        [HttpGet]
        [ProducesResponseType(typeof(LearnerDto), 200)]
        public IActionResult GetLearnerInClass(int classid,
            int learnerid)
        {
            var learner = _userService.GetUsers<LearnerDto>(parentDepartmentIds: new List<int>() { classid },
                userIds: new List<int>() { learnerid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            return CreateResponse<LearnerDto>(learner);
        }

        /// <summary>
        /// Get schools that the class belongs to
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        [Route("classes/{classid}/schools", Name = "classes:findschool_class")]
        [HttpGet]
        [ProducesResponseType(typeof(List<SchoolDto>), 200)]
        public IActionResult GetSchools(int classid)
        {
            var schools = _departmentServiceDelegate(ArchetypeEnum.School).GetDepartments<SchoolDto>(childDepartmentId: classid).Items;
            return CreateResponse(schools);
        }

        /// <summary>
        /// Get class's level
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        [Route("classes/{classid}/level")]
        [HttpGet]
        [ProducesResponseType(typeof(LevelDto), 200)]
        public IActionResult GetLevel(int classid)
        {
            var level = _departmentService.GetLevels(archytypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Level }, departmentIds: new List<int>() { classid }).FirstOrDefault();
            return CreateResponse(level);
        }

        /// <summary>
        /// Get class's memberships
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [Route("classes/{classid}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>), 200)]
        public IActionResult GetClassMemberships(int classId)
        {
            var result = _classMemberService.GetClassMemberships(classId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Update class level membership
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="levelDto"></param>
        /// <returns></returns>
        [Route("classes/{classid}/memberships/levels")]
        [HttpPost]
        [ProducesResponseType(typeof(LevelDto), 200)]
        public IActionResult UpdateClasslevel(int classId, MemberDto levelDto)
        {
            var result = _classMemberService.AddTypeToClass(classId, levelDto, true);
            return CreateResponse(result);
        }

        /// <summary>
        /// remove class's level
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="levelDto"></param>
        /// <returns></returns>
        [Route("classes/{classid}/memberships/levels")]
        [HttpPut]
        [ProducesResponseType(typeof(LevelDto), 200)]
        public IActionResult RemoveClasslevel(int classId, [FromBody]MemberDto levelDto)
        {
            var result = _classMemberService.RemoveDepartmentTypeClass(classId, levelDto);
            return CreateResponse(result);
        }
    }
}
