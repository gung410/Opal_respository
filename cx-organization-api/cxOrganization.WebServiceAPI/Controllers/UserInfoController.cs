using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Business.PDPlanner;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxOrganization.WebServiceAPI.Extensions;
using cxOrganization.WebServiceAPI.Models;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Route("userinfo")]
    public class UserInfoController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserInfoService _userInfoService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly ILearningNeedsAnalysisService _learningNeedsAnalysisService;

        private readonly AppSettings _appSettings;
        public UserInfoController(
            ILogger<UserInfoController> logger,
            IWorkContext workContext,
            IUserService userService,
            IUserInfoService userInfoService,
            ILearningNeedsAnalysisService learningNeedsAnalysisService,
            IOptions<AppSettings> appSettingOptions)
        {
            _logger = logger;
            _workContext = workContext;
            _userService = userService;
            _userInfoService = userInfoService;
            _learningNeedsAnalysisService = learningNeedsAnalysisService;
            _appSettings = appSettingOptions.Value;
        }

        /// <summary>
        /// Get list of user 
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        [Route("users")]
        [HttpGet]
        [ProducesResponseType(typeof(Adapter.Shared.Common.PaginatedList<UserWithIdpInfoDto>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] List<string> ids = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 100,
            string orderBy = "",
            string searchKey = "",
            [FromQuery] List<string> jsonDynamicData = null,
            bool? externallyMastered = null)
        {
            if (Request.ShouldLimitPageSize())
            {
                pageSize = _appSettings.LimitUserPageSize(pageSize);
            }

            var users = await _userService.GetUsersWithIdpInfoAsync(searchKey: searchKey, pageIndex: pageIndex,
            pageSize: pageSize, includeUGMembers: true,
            includeDepartment: true, getRoles: true, loginServiceClaims: ids,
            orderBy: orderBy, jsonDynamicData: jsonDynamicData,
            externallyMastered: externallyMastered);
            return CreateResponse(users);
        }


        /// <summary>
        /// Get list of user 
        /// </summary>
        /// <param name="getUserBasicInfoParameters"></param>
        /// <returns></returns>
        [HttpPost("basic")]
        [ProducesResponseType(typeof(PaginatedList<UserBasicInfo>), 200)]
        public async Task<IActionResult> GetBasicUsers([FromBody] GetUserBasicInfoParameters getUserBasicInfoParameters)
        {
            var token = HttpContext.Request.Headers["Authorization"][0];

            if (token is null)
            {
                return BadRequest();
            }

            //Limit max item per page to avoid over load 
            if (Request.ShouldLimitPageSize())
            {
                getUserBasicInfoParameters.PageSize = _appSettings.LimitUserPageSize(getUserBasicInfoParameters.PageSize);
            }

            var users = await _userInfoService.GetUserWithBasicInfos(userIds: getUserBasicInfoParameters.UserIds,
                extIds: getUserBasicInfoParameters.ExtIds,
                emails: getUserBasicInfoParameters.Emails,
                entityStatuses: getUserBasicInfoParameters.EntityStatuses,
                searchKey: getUserBasicInfoParameters.SearchKey,
                departmentIds: getUserBasicInfoParameters.DepartmentIds,
                externallyMastered: getUserBasicInfoParameters.ExternallyMastered,
                userTypeIds: getUserBasicInfoParameters.UserTypeIds,
                userTypeExtIds: getUserBasicInfoParameters.UserTypeExtIds,
                multiUserTypeFilters: getUserBasicInfoParameters.MultiUserTypeFilters,
                multiUserTypeExtIdFilters: getUserBasicInfoParameters.MultiUserTypeExtIdFilters,
                pageSize: getUserBasicInfoParameters.PageSize,
                pageIndex: getUserBasicInfoParameters.PageIndex,
                orderBy: getUserBasicInfoParameters.OrderBy,
                getFullIdentity: getUserBasicInfoParameters.GetFullIdentity,
                getEntityStatus: getUserBasicInfoParameters.GetEntityStatus,
                userGroupIds:  getUserBasicInfoParameters.UserGroupIds,
                exceptUserIds: getUserBasicInfoParameters.ExceptUserIds,
                systemRolePermissions: getUserBasicInfoParameters.SystemRolePermissions,
                token: token);

            return CreateResponse(users);
        }

        /// <summary>
        /// Get list of user hierarchy info
        /// </summary>
        /// <param name="getUserHierarchyInfoParameters"></param>
        /// <returns></returns>
        [HttpPost("hierarchyInfos")]
        [ProducesResponseType(typeof(PaginatedList<UserHierarchyInfo>), 200)]
        public async Task<IActionResult> GetUserHierarchyInfos([FromBody] GetUserHierarchyInfoParameters getUserHierarchyInfoParameters)
        {
            //Limit max item per page to avoid over load 
            if (Request.ShouldLimitPageSize())
            {
                getUserHierarchyInfoParameters.PageSize = _appSettings.LimitUserPageSize(getUserHierarchyInfoParameters.PageSize);
            }

            var users = await _userInfoService.GetUserHierarchyInfos(userIds: getUserHierarchyInfoParameters.UserIds,
                extIds: getUserHierarchyInfoParameters.ExtIds,
                emails: getUserHierarchyInfoParameters.Emails,
                entityStatuses: getUserHierarchyInfoParameters.EntityStatuses,
                departmentIds: getUserHierarchyInfoParameters.DepartmentIds,
                pageSize: getUserHierarchyInfoParameters.PageSize,
                pageIndex: getUserHierarchyInfoParameters.PageIndex,
                orderBy: getUserHierarchyInfoParameters.OrderBy);

            return CreateResponse(users);
        }


        /// <summary>
        /// Get the user info including the tags based on the user's profile
        ///     and the latest published Learning Needs Analysis of the user.
        /// </summary>
        /// <param name="id">The external identifier of the user.</param>
        /// <param name="includeUserBasicInfo">Set "True" if the user basic info should be returned. Default is "True".</param>
        /// <param name="includeUserTags">Set "True" if the tags based on the user's profile should be returned.</param>
        /// <param name="includeUserTagGroups"></param>
        /// <param name="includeLearningAreaTagsMarkedHighPriority">Set "True" to return the tags relevant to the learning areas with HIGH priority based on the latest published Learning Needs Analysis of the user.</param>
        /// <param name="includeLearningAreaTagsMarkedModeratePriority">Set "True" to return the tags relevant to the learning areas with MODERATE priority based on the latest published Learning Needs Analysis of the user.</param>
        /// <param name="includeLowLnaTag">Set "True" to return the tags relevant to the learning areas with LOW priority based on the latest published Learning Needs Analysis of the user.</param>
        /// <param name="includeTagIdsGroupByPriority">Set "True" to group tagIds by priority</param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(UserInfoDto), 200)]
        public async Task<IActionResult> GetUserInfo([FromRoute] string id,
            [FromQuery] bool includeUserBasicInfo = false,
            [FromQuery] bool includeUserTags = true,
            [FromQuery] bool includeUserTagGroups = false,
            [FromQuery] bool includeLearningAreaTagsMarkedHighPriority = true,
            [FromQuery] bool includeLearningAreaTagsMarkedModeratePriority = true,
            [FromQuery] bool includeLowLnaTag = false,
            [FromQuery] bool includeTagIdsGroupByPriority = false)
        {
            var (userId, userInfo) = await _userInfoService.GetUserInfoAsync(
                id,
                includeUserBasicInfo,
                includeUserTags,
                includeUserTagGroups);

            await AddLearningAreaPriorityTagIds(includeLearningAreaTagsMarkedHighPriority, includeLearningAreaTagsMarkedModeratePriority, includeLowLnaTag, userId, userInfo, includeTagIdsGroupByPriority);

            return CreateResponse(userInfo);
        }

        [Route("public")]
        [HttpPost]
        [ProducesResponseType(typeof(List<PublicUserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPublicInfo([FromBody][Required] SearchPublicUserInfoParameters searchPublicUserInfoParameters)
        {
            var info = await _userInfoService.GetPublicUserInfoAsync(searchPublicUserInfoParameters.UserCxIds);
            return CreateResponse(info);
        }

        [Route("clearcache/learningareatags")]
        [HttpPost]
        [ProducesResponseType(typeof(List<PublicUserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ClearLearningAreaPriorityFromCache(
            [FromBody] ClearLearningAreaPriorityCacheDto clearLearningAreaPriorityCacheDto)
        {
            var clearCacheResults = await _learningNeedsAnalysisService.ClearLearningAreaPriorityFromCache(clearLearningAreaPriorityCacheDto.UserIds);
            if (clearCacheResults.Count == 0) return NoContent();
         
            var results = clearCacheResults.Select(a => new {UserId = a.Key, Success = a.Value}).ToList();

            return Ok(results);
          
        }

        private async Task<UserInfoDto> AddLearningAreaPriorityTagIds(bool includeLearningAreaTagsMarkedHighPriority, bool includeLearningAreaTagsMarkedModeratePriority, bool includeLearningAreaTagsMarkedLowPriority,  int userId, UserInfoDto userInfo, bool includeTagIdsGroupByPriority)
        {
            if (includeLearningAreaTagsMarkedHighPriority || includeLearningAreaTagsMarkedModeratePriority || includeLearningAreaTagsMarkedLowPriority)
            {
                var learningAreaPriorityTagIdsInfos = await _learningNeedsAnalysisService
                   .GetLearningAreaPriorityTagIdsFromLNAData(userId, includeLearningAreaTagsMarkedHighPriority, includeLearningAreaTagsMarkedModeratePriority, includeLearningAreaTagsMarkedLowPriority);
                var learningAreaPriorityTagIds = learningAreaPriorityTagIdsInfos.AllTagIds;
                if (learningAreaPriorityTagIds.Any())
                {
                    if (userInfo.TagIds == null) userInfo.TagIds = new List<string>();
                    userInfo.TagIds.AddRange(learningAreaPriorityTagIds);
                    userInfo.TagIds = userInfo.TagIds.Distinct().ToList();
                }

                if (includeTagIdsGroupByPriority && learningAreaPriorityTagIdsInfos.LearningAreaPriority != null)
                {
                    userInfo.TagIdsGroupByPriority = new TagPriorityGroup();

                    if (includeLearningAreaTagsMarkedHighPriority)
                    {
                        userInfo.TagIdsGroupByPriority.HighTagIds =
                            learningAreaPriorityTagIdsInfos.LearningAreaPriority.HighPriorities;
                    }

                    if (includeLearningAreaTagsMarkedModeratePriority)
                    {
                        userInfo.TagIdsGroupByPriority.ModerateTagIds =
                            learningAreaPriorityTagIdsInfos.LearningAreaPriority.ModeratePriorities;
                    }

                    if (includeLearningAreaTagsMarkedLowPriority)
                    {
                        userInfo.TagIdsGroupByPriority.LowTagIds =
                            learningAreaPriorityTagIdsInfos.LearningAreaPriority.LowPriorities;
                    }
                }
            }
            return userInfo;
        }

        [Route("getusercountingbyusertypes")]
        [HttpPost]
        [ProducesResponseType(typeof(List<UserCountingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetUserCountingByUserTypes([FromBody][Required] UserCountingParameters userCountingParameters)
        {
            var userCountingDtos = await _userInfoService.GetUserCountByUserTypes(
                userCountingParameters.UserTypeArchetypes,
                userCountingParameters.UserGroupIds,
                userCountingParameters.UserIds);

            return CreateResponse(UserCountingDto.CreateListFrom(userCountingDtos));
        }

        private bool ValidateMinimalFilter(
            List<string> loginServiceClaims,
            string searchKey,
            List<string> jsonDynamicData)
        {
            //Only validate when authorized by user token
            if (!string.IsNullOrEmpty(_workContext.Sub))
            {

                var hasFilterOnUserIdentity = !loginServiceClaims.IsNullOrEmpty()
                                              || !string.IsNullOrEmpty(searchKey)
                                              || !jsonDynamicData.IsNullOrEmpty();

                if (!hasFilterOnUserIdentity)
                {
                    _logger.LogWarning("For security reason, it requires minimal filter on identity of user, department or user group to be able to retrieve users.");
                    return false;
                }
            }

            return true;
        }

     }
}