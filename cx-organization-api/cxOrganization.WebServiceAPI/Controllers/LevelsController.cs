using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Client;
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
    /// Level api: manage members of classes/schools
    /// </summary>
    [Authorize]
    public class LevelsController : ApiControllerBase
    {
        private readonly IAdvancedWorkContext _workContext;
        private readonly ILevelService _levelService;
        private readonly IUserTypeService _userTypeService;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="levelService"></param>
        /// <param name="workContext"></param>
        /// <param name="userTypeService"></param>
        /// <param name="userService"></param>
        /// <param name="departmentService"></param>
        public LevelsController(IAdvancedWorkContext workContext,
            ILevelService levelService, 
            IUserTypeService userTypeService,
            Func<ArchetypeEnum,IUserService> userService,
            Func<ArchetypeEnum, IDepartmentService> departmentService)
        {
            _workContext = workContext;
            _levelService = levelService;
            _userTypeService = userTypeService;
            _userService = userService(ArchetypeEnum.Learner);
            _departmentService = departmentService(ArchetypeEnum.Class);
        }
        /// <summary>
        /// Get level
        /// </summary>
        /// <param name="levelid"></param>
        /// <returns></returns>
        [Route("levels/{levelid}")]
        [HttpGet]
        [ProducesResponseType(typeof(LevelDto),200)]
        public IActionResult GetLevelByExtId(string levelid)
        {
            var data = _userTypeService.GetUserTypeByExtId(levelid);
            if (data == null || data.ArchetypeId != (int)ArchetypeEnum.Level)
                return CreateNotFoundResponse(string.Format("Level not found: LevelId({0})", levelid));
            return CreateResponse(new LevelDto
            {
                Identity = new IdentityDto
                {
                    Id = data.UserTypeId,
                    Archetype = ArchetypeEnum.Level,
                    //CustomerId = item.CustomerId ?? item.CustomerId.Value,
                    ExtId = data.ExtId,
                    OwnerId = data.OwnerId
                },
                EntityStatus = new EntityStatusDto
                { }

            });
        }
        /// <summary>
        /// Get all learners that have the entered level
        /// </summary>
        /// <param name="levelid"></param>
        /// <param name="learnerIds"></param>
        /// <param name="learnerExtIds"></param>
        /// <param name="learnerStatusEnums"></param>
        /// <param name="learnerSsnList"></param>
        /// <param name="learnerLastUpdatedBefore"></param>
        /// <param name="learnerLastUpdatedAfter"></param>
        /// <param name="learnerParentDepartmentIds"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="queryOptions"></param>
        /// <param name="selectIdentity"></param>
        /// <returns></returns>
        [Route("levels/{levelid}/learners")]
        [HttpGet]
        [ProducesResponseType(typeof(List<LearnerDto>),200)]
        public IActionResult GetLearnersByLevel(string levelid,
            [FromQuery] List<int> learnerParentDepartmentIds = null,
            [FromQuery] List<int> learnerIds=null,
            [FromQuery] List<string> learnerExtIds=null,
            [FromQuery] List<EntityStatusEnum> learnerStatusEnums = null,
            [FromQuery] List<string> learnerSsnList = null,
            DateTime? learnerLastUpdatedBefore = null,
            DateTime? learnerLastUpdatedAfter = null,
            bool selectIdentity =false,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "")
        {
            var pagingDto = _userService.GetUsers<LearnerDto>(userTypeExtIds: new List<string>() { levelid },
                parentDepartmentIds:learnerParentDepartmentIds,
                userIds:learnerIds,
                extIds:learnerExtIds,
                statusIds:learnerStatusEnums,
                ssnList:learnerSsnList,
                lastUpdatedAfter:learnerLastUpdatedAfter,
                lastUpdatedBefore:learnerLastUpdatedBefore,
                archetypeIds:new List<ArchetypeEnum>() { ArchetypeEnum.Learner},
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
        /// Get the learner that have the entered level and identifier
        /// </summary>
        /// <param name="levelid"></param>
        /// <param name="learnerid"></param>
        /// <returns></returns>
        [Route("levels/{levelid}/learners/{learnerid}")]
        [HttpGet]
        [ProducesResponseType(typeof(LearnerDto),200)]
        public IActionResult GetLearnersByLevel(string levelid,int learnerid)
        {
            var learner = _userService.GetUsers<LearnerDto>(userIds: new List<int>() { learnerid }
            , userTypeExtIds: new List<string>() { levelid }
            , archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Learner }
            , statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            return CreateResponse<LearnerDto>(learner);
        }
        /// <summary>
        /// Get all classes that have the entered level
        /// </summary>
        /// <param name="levelid"></param>
        /// <returns></returns>
        [Route("levels/{levelid}/classes")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ClassDto>),200)]
        public IActionResult GetClassesByLevel(string levelid)
        {
            var classes = _departmentService.GetDepartments<ClassDto>(departmentTypeExtIds: new List<string>() { levelid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Class }).Items;

            return CreateResponse<ClassDto>(classes);
        }
        /// <summary>
        /// Get all classes that have the entered level
        /// </summary>
        /// <param name="levelid"></param>
        /// <param name="classid"></param>
        /// <returns></returns>
        [Route("levels/{levelid}/classes/{classid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ClassDto),200)]
        public IActionResult GetClassByLevel(string levelid,int classid)
        {
            var department = _departmentService.GetDepartments<ClassDto>(departmentIds:new List<int>() { classid},
                departmentTypeExtIds: new List<string>() { levelid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Class }).Items.FirstOrDefault();

            return CreateResponse<ClassDto>(department);
        }

        #region will be removed
        /// <summary>
        /// Add Level member (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerid"></param>
        /// <param name="schoolid"></param>
        /// <param name="levelid"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerid}/schools/{schoolid}/levels/{levelid}/members")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult AddMembers(int schoolownerid, int schoolid, string levelid, [FromBody]MemberDto memberDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerid, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolid, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();

            return CreateResponse(_levelService.AddOrRemoveMember(validationSpecification, levelid, memberDto));
        }

        /// <summary>
        /// Add Level member (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerid"></param>
        /// <param name="schoolid"></param>
        /// <param name="levelextid"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerid}/schools/{schoolid}/levels/{levelextid}/memberships")]
        [HttpGet]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult GetMemberships(int schoolownerid, int schoolid, string levelextid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerid, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolid, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            return CreateResponse(_levelService.GetMemberships(validationSpecification, levelextid));
        }

        /// <summary>
        /// Get members as class (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerid"></param>
        /// <param name="schoolid"></param>
        /// <param name="levelid"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerid}/schools/{schoolid}/levels/{levelid}/members")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MemberDto>),200)]
        public IActionResult GetMembers(int schoolownerid, int schoolid, string levelid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
           .ValidateDepartment(schoolownerid, ArchetypeEnum.SchoolOwner)
           .ValidateDepartment(schoolid, ArchetypeEnum.School)
           .WithStatus(EntityStatusEnum.All)
           .IsDirectParent()
           .Create();
            return CreateResponse(_levelService.GetMembersAsUser(validationSpecification, levelid));
        }

        /// <summary>
        ///  Get Learner as a level member (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerid"></param>
        /// <param name="schoolid"></param>
        /// <param name="levelid"></param>
        /// <param name="learnerid"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerid}/schools/{schoolid}/levels/{levelid}/members/learners/{learnerid}")]
        [HttpGet]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult GetMemberAsUser(int schoolownerid, int schoolid, string levelid, int learnerid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerid, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolid, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var learner = _levelService.GetMemberAsUser(validationSpecification, levelid, learnerid);
            if (learner != null)
            {
                return CreateResponse(learner);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("Learner not found: LearnerId ({0})", learnerid));
            }
        }

        /// <summary>
        ///  Get Class as a level member (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerid"></param>
        /// <param name="schoolid"></param>
        /// <param name="classid"></param>
        /// <param name="levelid"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerid}/schools/{schoolid}/levels/{levelid}/members/classes/{classid}")]
        [HttpGet]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult GetMemberAsClass(int schoolownerid, int schoolid, string levelid, int classid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerid, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolid, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var learner = _levelService.GetMemberAsDepartment(validationSpecification, levelid, classid);
            if (learner != null)
            {
                return CreateResponse(learner);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("Class not found: ClassId ({0})", classid));
            }
        }

        /// <summary>
        ///  Get Teachinggroup as a level member (This endpoint is obsolete)
        /// </summary>
        /// <param name="schoolownerid"></param>
        /// <param name="schoolid"></param>
        /// <param name="teachinggroupid"></param>
        /// <param name="levelid"></param>
        /// <returns></returns>
        [Route("schoolowners/{schoolownerid}/schools/{schoolid}/levels/{levelid}/members/teachinggroups/{teachinggroupid}")]
        [HttpGet]
        [ProducesResponseType(typeof(MemberDto),200)]
        public IActionResult GetMemberAsTeachingGroup(int schoolownerid, int schoolid, string levelid, int teachinggroupid)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(schoolownerid, ArchetypeEnum.SchoolOwner)
            .ValidateDepartment(schoolid, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var teachingGroup = _levelService.GetMemberAsTeachingGroup(validationSpecification, levelid, teachinggroupid);
            if (teachingGroup != null)
            {
                return CreateResponse(teachingGroup);
            }
            else
            {
                return CreateNotFoundResponse(string.Format("Class not found: ClassId ({0})", teachinggroupid));
            }
        }  
        #endregion
    }
}
