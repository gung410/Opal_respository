using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using cxOrganization.Business.DeactivateOrganization.DeactivateUser;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    public class LearnersController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly IDepartmentService _departmentService;
        private readonly Func<ArchetypeEnum, IDepartmentService> _departmentServiceDelegate;
        private readonly IUGMemberService _userGroupUserMemberService;
        private readonly IUserTypeService _userTypeService;
        private readonly IClassMemberService _classMemberService;
        private readonly ILoginServiceUserService _loginServiceUserService;
        private readonly IDeactivateUserService<LearnerDto> _deactivateUserService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="workContext"></param>
        /// <param name="departmentService"></param>
        /// <param name="userTypeService"></param>
        /// <param name="userGroupUserMemberService"></param>
        /// <param name="classMemberService"></param>
        /// <param name="loginServiceUserService"></param>
        public LearnersController(Func<ArchetypeEnum, IUGMemberService> userGroupUserMemberService,
            Func<ArchetypeEnum, IUserService> userService,
            IWorkContext workContext,
            Func<ArchetypeEnum, IDepartmentService> departmentService,
            Func<ArchetypeEnum, IUserTypeService> userTypeService,
            IClassMemberService classMemberService,
            ILoginServiceUserService loginServiceUserService,
            IDeactivateUserService<LearnerDto> deactivateUserService)
        {
            _userService = userService(ArchetypeEnum.Learner);
            _workContext = workContext;
            _departmentService = departmentService(ArchetypeEnum.Class);
            _departmentServiceDelegate = departmentService;
            _userGroupUserMemberService = userGroupUserMemberService(ArchetypeEnum.TeachingGroup);
            _userTypeService = userTypeService(ArchetypeEnum.Learner);
            _classMemberService = classMemberService;
            _loginServiceUserService = loginServiceUserService;
            _deactivateUserService = deactivateUserService;

        }


        /// <summary>
        /// Get learners
        /// </summary>
        /// <param name="learnerIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="statusEnums"> Learner StatusEnums: Unknown = 0, Active = 1, Inactive = 2, Deactive = 3, All = 99, default is active</param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="queryOptions"></param>
        /// <param name="extIds"></param>
        /// <param name="ssnList"></param>
        /// <param name="getDynamicProperties"></param>
        /// <param name="loginServiceIds"></param>       
        /// <param name="selectIdentity"></param>
        /// <param name="orderBy"></param>
        /// <param name="filterOnParentHd"></param>
        /// <param name="pageIndex"></param>
        /// <param name="extId">Will be removed</param>
        /// <param name="ssn">Will be removed</param>
        /// <param name="pageSize"></param>
        /// <param name="userNames"></param>
        /// <param name="ageRanges"></param>
        /// <param name="genders"></param>
        /// <param name="searchKey"></param>
        /// <param name="getLoginServiceClaims"></param>
        /// <param name="getRoles"></param>
        /// <param name="loginServiceClaims"></param>
        /// <param name="loginServiceClaimTypes"></param>
        /// <param name="roleIds">List of id of role. (Role id is id of UserType now)</param>
        /// <param name="roleExtIds">List of extId of role. (Role ExtId is extid of UserType now)</param>
        /// <returns></returns>
        [Route("learners", Name = "learners:getAll")]
        [HttpGet]
        public IActionResult GetLearners([FromQuery] List<int> learnerIds = null,
            [FromQuery] List<int> userGroupIds = null,
            [FromQuery] List<int> parentDepartmentIds = null,
            [FromQuery] List<EntityStatusEnum> statusEnums = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<string> ssnList = null,
            [FromQuery] List<string> userNames = null,
            [FromQuery] List<AgeRange> ageRanges = null,
            [FromQuery] List<Gender> genders = null,
            string searchKey = null,
            string ssn = null,
            string extId = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
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
            bool? filterOnSubDepartment = null)
        {
            if (!string.IsNullOrEmpty(extId))
            {
                extIds = extIds ?? new List<string>();
                extIds.Add(extId);
            }
            if (!string.IsNullOrEmpty(ssn))
            {
                ssnList = ssnList ?? new List<string>();
                ssnList.Add(ssn);
            }
            var userDtoPaging = _userService.GetUsers<LearnerDto>(userIds: learnerIds,
                userGroupIds: userGroupIds,
                customerIds: null,
                parentDepartmentIds: parentDepartmentIds,
                archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.Learner },
                userTypeIds: roleIds,
                userTypeExtIds: roleExtIds,
                statusIds: statusEnums,
                extIds: extIds,
                ssnList: ssnList,
                userNames: userNames,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                loginServiceClaims: loginServiceClaims,
                loginServiceClaimTypes: loginServiceClaimTypes,
                loginServiceIds: loginServiceIds,
                ageRanges: ageRanges,
                genders: genders,
                searchKey: searchKey,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                getDynamicProperties: getDynamicProperties,
                getLoginServiceClaims: getLoginServiceClaims,
                getRoles: getRoles,
                filterOnParentHd: filterOnParentHd,
                filterOnSubDepartment: filterOnSubDepartment);

            return CreatePagingResponse(userDtoPaging.Items,
                userDtoPaging.PageIndex,
                userDtoPaging.PageSize,
                userDtoPaging.HasMoreData);
        }

        /// <summary>
        /// Insert learner to school owner
        /// </summary>
        /// <param name="learnerDto"></param>
        /// <returns></returns>
        [Route("learners")]
        [HttpPost]
        public IActionResult InsertLearners([FromBody]LearnerDto learnerDto)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(learnerDto.ParentDepartmentId, ArchetypeEnum.Unknown)
                .SkipCheckingArchetype()
                .WithStatus(EntityStatusEnum.All)
                .IsNotDirectParent()
                .Create();
            learnerDto.Identity.Id = 0;
            var userDtoBase = _userService.InsertUser(validationSpecification, learnerDto);
            return StatusCode((int)HttpStatusCode.Created, userDtoBase);
        }
        /// <summary>
        /// Get learner
        /// </summary>
        /// <param name="learnerid"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}", Name = "learners:get")]
        [HttpGet]
        public IActionResult GetLearner(int learnerid)
        {
            var learner = _userService.GetUsers<LearnerDto>(userIds: new List<int>() { learnerid },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }, getDynamicProperties: true).Items.FirstOrDefault();
            if (learner == null)
                return CreateNotFoundResponse<LearnerDto>();
            return CreateResponse<LearnerDto>(learner);

        }
        /// <summary>
        /// Update learner
        /// </summary>
        /// <param name="learnerid"></param>
        /// <param name="learnerDto"></param>
        /// <param name="skipCheckingEntityVersion"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}", Name = "learners:update")]
        [HttpPut]
        public IActionResult UpdateLearner(int learnerid, [FromBody]LearnerDto learnerDto, bool skipCheckingEntityVersion = false)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                .ValidateDepartment(learnerDto.ParentDepartmentId, ArchetypeEnum.Unknown)
                .SkipCheckingArchetype()
                .WithStatus(EntityStatusEnum.All)
                .IsDirectParent()
                .Create();

            learnerDto.Identity.Id = learnerid;
            var candidate = _userService.UpdateUser(validationSpecification, learnerDto, skipCheckingEntityVersion);
            if (candidate == null)
                return CreateNotFoundResponse(string.Format("Learner not found: LearnerId({0}))", learnerid));
            else
                return CreateResponse(candidate);
        }
        /// <summary>
        /// Get learner's class
        /// </summary>
        /// <param name="learnerid"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}/classes")]
        [HttpGet]
        public IActionResult GetLearnerClass(int learnerid)
        {
            var classDto = _departmentService.GetDepartments<ClassDto>(userIds: new List<int>() { learnerid },
                archetypeIds: new List<int>() { (int)ArchetypeEnum.Class }).Items.FirstOrDefault();
            return CreateResponse<ClassDto>(classDto);
        }

        /// <summary>
        /// Get learner's level
        /// </summary>
        /// <param name="learnerid"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}/level")]
        [HttpGet]
        public IActionResult GetLearnerLevel(int learnerid)
        {
            var level = _userService.GetUserLevel(learnerid, ArchetypeEnum.Learner, ArchetypeEnum.Level).FirstOrDefault();
            return CreateResponse<LevelDto>(level);
        }

        /// <summary>
        /// Get learner memberships
        /// </summary>
        /// <param name="learnerid"></param>
        /// <param name="archetypeIds">membership archetype enum</param>
        /// <param name="statusIds">membership status enum</param>
        /// <param name="membershipIds"></param>
        /// <param name="membershipExtIds"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}/memberships")]
        [HttpGet]
        public IActionResult GetLearnerMemberships(int learnerid,
            [FromQuery] List<ArchetypeEnum> archetypeIds = null,
            [FromQuery] List<EntityStatusEnum> statusIds = null,
            [FromQuery] List<int> membershipIds = null,
            [FromQuery] List<string> membershipExtIds = null)
        {
            var memberships = _userService.GetUserMemberships(learnerid,
                ArchetypeEnum.Learner,
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
                return CreateNotFoundResponse(string.Format("No membership found: LearnerId({0})", learnerid));
            }
        }
        /// <summary>
        /// Make memberships for learner with teachinggroup
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}/memberships/teachinggroups")]
        [HttpPost]
        [ProducesResponseType(typeof(MemberDto), 200)]
        public IActionResult AddLearnerToTeachingGroup(int learnerId, [FromBody]MemberDto memberDto)
        {
            memberDto.MemberRoleId = 1;
            var result = _userGroupUserMemberService.InsertUserGroupUserMembership(learnerId, memberDto);
            return CreateResponse(result);
        }

        /// <summary>
        /// Remove memberships for learner with teachinggroup
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="memberDto">This dto content required teaching group info </param>
        /// <returns></returns>
        [Route("learners/{learnerid}/memberships/teachinggroups")]
        [HttpPut]
        public IActionResult RemoveLearnerInTeachingGroup(int learnerId, [FromBody]MemberDto memberDto)
        {
            var result = _userGroupUserMemberService.InsertUserGroupUserMembership(learnerId, memberDto);
            return CreateResponse(result);
        }
        /// <summary>
        /// Update learner's level
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}/memberships/levels")]
        [HttpPost]
        public IActionResult UpdateLearnerLevel(int learnerId, [FromBody]MemberDto memberDto)
        {
            var result = _userTypeService.UpdateOrInsertUserTypeUser(learnerId,
                memberDto, isUnique: true);
            return CreateResponse(result);
        }
        /// <summary>
        /// Remove learner's Level
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}/memberships/levels")]
        [HttpPut]
        public IActionResult DeleteUserTypeUser(int learnerId, [FromBody]MemberDto memberDto)
        {
            var result = _userTypeService.DeleteUserTypeUser(learnerId,
                memberDto);
            return CreateResponse(result);
        }
        /// <summary>
        /// Add learner to class
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}/memberships/classes")]
        [HttpPost]
        public IActionResult UpdateLearnerClass(int learnerId, [FromBody]MemberDto memberDto)
        {
            var result = _classMemberService.AddLearnerToClass(learnerId, memberDto);
            return CreateResponse(result);
        }
        /// <summary>
        /// Remove learner from class, this learner will be moved to class's school
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="memberDto"></param>
        /// <returns></returns>
        [Route("learners/{learnerid}/memberships/classes")]
        [HttpPut]
        public IActionResult RemoveLearnerFromClass(int learnerId, MemberDto memberDto)
        {
            var result = _classMemberService.RemoveLearnerFromClass(learnerId, memberDto);
            return CreateResponse(result);
        }
        /// <summary>
        /// Insert loginServiceUser that contains claim value for a learner in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        /// <response code="201">Insert loginServiceUser successfully</response>
        /// <response code="400">Give data is invalid, bad format or already existing</response> 
        /// <response code="404">User or login service are not found with given identity</response>
        /// <response code="500">Unexpected error occurs in server</response> 
        [Route("learners/loginservices")]
        [HttpPost]
        //TODO
        //[ValidateIdentityCxToken]
        public IActionResult InsertLoginServiceClaim([Required][FromBody] LoginServiceUserDto loginServiceUser)
        {
            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Learner)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var insertedLoginService = _loginServiceUserService.Insert(loginServiceUser);
            return StatusCode((int)HttpStatusCode.Created, insertedLoginService);
        }
        /// <summary>
        /// Update loginServiceUser that contains claim value for a learner in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        /// <response code="201">Update loginServiceUser successfully</response>
        /// <response code="400">Give data is invalid, bad format</response> 
        /// <response code="404">User or login service or LoginServiceUser are not found with given information</response>
        /// <response code="500">Unexpected error occurs in server</response> 
        [Route("learners/loginservices")]
        [HttpPut]
        //TODO
        //[ValidateIdentityCxToken]
        public IActionResult UpdateLoginServiceClaim([Required][FromBody] LoginServiceUserDto loginServiceUser)
        {
            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Learner)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var updatedLoginService = _loginServiceUserService.Update(loginServiceUser);
            return Ok(updatedLoginService);
        }

        /// <summary>
        /// Delete loginServiceUser that contains claim value for a learner in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        /// <response code="201">Delete loginServiceUser successfully</response>
        /// <response code="400">Give data is invalid, bad format</response> 
        /// <response code="404">User or login service or LoginServiceUser are not found with given information</response>
        /// <response code="500">Unexpected error occurs in server</response> 
        [Route("learners/loginservices")]
        [HttpDelete]
        //TODO
        //[ValidateIdentityCxToken]
        public IActionResult DeleteLoginServiceClaim([Required][FromBody] LoginServiceUserDto loginServiceUser)
        {
            if (loginServiceUser.UserIdentity.Archetype != ArchetypeEnum.Learner)
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM, string.Format("UserIdentity with archetype '{0}' is not supported", loginServiceUser.UserIdentity.Archetype));
            var deletedLoginService = _loginServiceUserService.Delete(loginServiceUser);
            return Ok(deletedLoginService);
        }

        /// <summary>
        /// Get list of loginServiceUser of learner based on given parameters
        /// </summary>
        /// <returns></returns>
        [Route("learners/loginservices")]
        [HttpGet]
        public IActionResult GetLoginServiceClaims(
            [FromQuery] List<int> learnerIds = null,
            [FromQuery] List<string> learnerExtIds = null,
            [FromQuery] List<int> loginServiceIds = null,
            [FromQuery] List<string> primaryClaimTypes = null,
            [FromQuery] List<EntityStatusEnum> learnerStatuses = null,
            [FromQuery] List<int> siteIds = null,
            [FromQuery] bool? includeLoginServiceHasNullSiteId = null,
            [FromQuery] List<string> claimValues = null,
            [FromQuery] DateTime? createdAfter = null,
            [FromQuery] DateTime? createdBefore = null)
        {

            var loginServiceUsers = _loginServiceUserService.Get(userIds: learnerIds,
                userExtIds: learnerExtIds,
                userArchetypes: new List<ArchetypeEnum> { ArchetypeEnum.Learner },
                loginServiceIds: loginServiceIds,
                primaryClaimTypes: primaryClaimTypes,
                userEntityStatuses: learnerStatuses,
                siteIds: siteIds,
                includeLoginServiceHasNullSiteId: includeLoginServiceHasNullSiteId,
                claimValues: claimValues,
                createdAfter: createdAfter,
                createdBefore: createdBefore);

            return CreateResponse(loginServiceUsers);
        }
        /// <summary>
        /// Deactivate learners
        /// </summary>
        /// <returns></returns>
        [Route("deactivate_learners")]
        [HttpPut]
        [ProducesResponseType(typeof(List<DeactivateUsersResultDto>), 200)]
        public IActionResult DeactivateLearners([FromBody] DeactivateLearnersDto deactivateLeaner)
        {

            var deactivateLearnerResults = _deactivateUserService.Deactivate(deactivateLeaner);
            var status = deactivateLearnerResults.MaxStatus();
            if (status == 0)
                return NoContent();
            var statusCode = status == (int)HttpStatusCode.NoContent
                ? (int)HttpStatusCode.OK : status;
            return StatusCode(statusCode, deactivateLearnerResults);
        }
    }
}
