using System;
using System.Collections.Generic;
using System.Net;
using cxOrganization.Business.Connection;
using cxOrganization.Client;
using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Flag group API 
    /// </summary>
    [Authorize]
    public class ConnectionsController : ApiControllerBase
    {
        readonly IWorkContext _workContext;
        readonly IConnectionService _connectionService;
        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionsController(IWorkContext workContext,
            IConnectionService connectionService)
        {
            _workContext = workContext;
            _connectionService = connectionService;
        }

        /// <summary>
        /// Get list connections for a specific type
        /// </summary>
        /// <param name="connectionType"></param>
        /// <param name="sourceArchetypes"></param>
        /// <param name="sourceIds"></param>
        /// <param name="sourceExtIds"></param>
        /// <param name="sourceReferrerTokens"></param>
        /// <param name="sourceReferrerResources"></param>
        /// <param name="sourceReferrerArchetypes"></param>
        /// <param name="referercxTokens"></param>
        /// <param name="sourceStatuses"></param>
        /// <param name="memberArchetypes"></param>
        /// <param name="memberIds"></param>
        /// <param name="memberExtIds"></param>
        /// <param name="memberStatuses"></param>
        /// <param name="memberReferrerTokens"></param>
        /// <param name="memberReferrerResources"></param>
        /// <param name="memberReferrerArchetypes"></param>
        /// <param name="memberGenders"></param>
        /// <param name="validFromBefore"></param>
        /// <param name="validFromAfter"></param>
        /// <param name="validToBefore"></param>
        /// <param name="validToAfter"></param>
        /// <param name="includeMember"></param>
        /// <param name="memberAgeRanges"></param>
        /// <param name="countOnMember"></param>
        /// <param name="includeConnectionHasNoMember"></param>
        /// <param name="sourceParentIds"></param>
        /// <param name="sourceParentArchetypes"></param>
        /// <returns></returns>
        [Route("connections/{connectionType}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ConnectionDto>),200)]
        public IActionResult GetConnections(ConnectionType connectionType,
                [FromQuery] List<ArchetypeEnum> sourceArchetypes = null,
                [FromQuery] List<int> sourceIds = null,
                [FromQuery] List<string> sourceExtIds = null,
                [FromQuery] List<string> sourceReferrerTokens = null,
                [FromQuery] List<string> sourceReferrerResources = null,
                [FromQuery] List<string> referercxTokens = null,
                [FromQuery] List<ArchetypeEnum> sourceReferrerArchetypes = null,
                [FromQuery] List<EntityStatusEnum> sourceStatuses = null,
                [FromQuery] List<ArchetypeEnum> memberArchetypes = null,
                [FromQuery] List<int> memberIds = null,
                [FromQuery] List<string> memberExtIds = null,
                [FromQuery] List<EntityStatusEnum> memberStatuses = null,
                [FromQuery] List<string> memberReferrerTokens = null,
                [FromQuery] List<string> memberReferrerResources = null,
                [FromQuery] List<ArchetypeEnum> memberReferrerArchetypes = null,
                [FromQuery] List<AgeRange> memberAgeRanges = null,
                [FromQuery] List<Gender> memberGenders = null,
                [FromQuery] DateTime? validFromBefore = null,
                [FromQuery] DateTime? validFromAfter = null,
                [FromQuery] DateTime? validToBefore = null,
                [FromQuery]DateTime? validToAfter = null,
                [FromQuery] bool includeMember = false,
                [FromQuery] bool countOnMember = false,
                [FromQuery] bool includeConnectionHasNoMember = false,
                [FromQuery] List<int> sourceParentIds = null,
                [FromQuery] List<ArchetypeEnum> sourceParentArchetypes = null)
        {
            var connectionDtos = _connectionService.GetConnections(
                sourceArchetypes: sourceArchetypes,
                sourceIds: sourceIds,
                sourceExtIds: sourceExtIds,
                connectionTypes: new List<ConnectionType> { connectionType },
                sourceReferrerArchetypes: sourceReferrerArchetypes,
                sourceReferrerResources: sourceReferrerResources,
                sourceReferrerTokens: sourceReferrerTokens,
                referercxTokens: referercxTokens,
                sourceStatuses: sourceStatuses,
                memberArchetypes: memberArchetypes,
                memberIds: memberIds,
                memberExtIds: memberExtIds,
                memberReferrerArchetypes: memberReferrerArchetypes,
                memberReferrerTokens: memberReferrerTokens,
                memberReferrerResources: memberReferrerResources,
                memberStatuses: memberStatuses,
                memberGenders: memberGenders,
                memberAgeRanges: memberAgeRanges,
                validFromAfter: validFromAfter,
                validFromBefore: validFromBefore,
                validToAfter: validToAfter,
                validToBefore: validToBefore,
                includeMember: includeMember,
                countOnMember: countOnMember,
                includeConnectionHasNoMember: includeConnectionHasNoMember,
                sourceParentIds: sourceParentIds,
                sourceParentArchetypes: sourceParentArchetypes);

            return CreateResponse(connectionDtos);

        }
        /// <summary>
        /// Get list connections for multiple types
        /// </summary>
        /// <param name="connectionTypes"></param>
        /// <param name="sourceArchetypes"></param>
        /// <param name="sourceIds"></param>
        /// <param name="sourceExtIds"></param>
        /// <param name="sourceReferrerTokens"></param>
        /// <param name="sourceReferrerResources"></param>
        /// <param name="sourceReferrerArchetypes"></param>
        /// <param name="referercxTokens"></param>
        /// <param name="sourceStatuses"></param>
        /// <param name="memberArchetypes"></param>
        /// <param name="memberIds"></param>
        /// <param name="memberExtIds"></param>
        /// <param name="memberStatuses"></param>
        /// <param name="memberReferrerTokens"></param>
        /// <param name="memberReferrerResources"></param>
        /// <param name="memberReferrerArchetypes"></param>
        /// <param name="memberGenders"></param>
        /// <param name="validFromBefore"></param>
        /// <param name="validFromAfter"></param>
        /// <param name="validToBefore"></param>
        /// <param name="validToAfter"></param>
        /// <param name="includeMember"></param>
        /// <param name="memberAgeRanges"></param>
        /// <param name="countOnMember"></param>
        /// <param name="includeConnectionHasNoMember"></param>
        /// <returns></returns>
        [Route("connections")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ConnectionDto>),200)]
        public IActionResult GetConnectionWithMultipleTypes([FromQuery] List<ConnectionType> connectionTypes,
                [FromQuery] List<ArchetypeEnum> sourceArchetypes = null,
                [FromQuery] List<int> sourceIds = null,
                [FromQuery] List<string> sourceExtIds = null,
                [FromQuery] List<string> sourceReferrerTokens = null,
                [FromQuery] List<string> sourceReferrerResources = null,
                [FromQuery] List<string> referercxTokens = null,
                [FromQuery] List<ArchetypeEnum> sourceReferrerArchetypes = null,
                [FromQuery] List<EntityStatusEnum> sourceStatuses = null,
                [FromQuery] List<ArchetypeEnum> memberArchetypes = null,
                [FromQuery] List<int> memberIds = null,
                [FromQuery] List<string> memberExtIds = null,
                [FromQuery] List<EntityStatusEnum> memberStatuses = null,
                [FromQuery] List<string> memberReferrerTokens = null,
                [FromQuery] List<string> memberReferrerResources = null,
                [FromQuery] List<ArchetypeEnum> memberReferrerArchetypes = null,
                [FromQuery] List<AgeRange> memberAgeRanges = null,
                [FromQuery] List<Gender> memberGenders = null,
                [FromQuery] DateTime? validFromBefore = null,
                [FromQuery] DateTime? validFromAfter = null,
                [FromQuery] DateTime? validToBefore = null,
                [FromQuery]DateTime? validToAfter = null,
                [FromQuery] bool includeMember = false,
                [FromQuery] bool countOnMember = false,
                [FromQuery] bool includeConnectionHasNoMember = false,
                [FromQuery] List<int> sourceParentIds = null,
                [FromQuery] List<ArchetypeEnum> sourceParentArchetypes = null)
        {
            var connectionDtos = _connectionService.GetConnections(
                sourceArchetypes: sourceArchetypes,
                sourceIds: sourceIds,
                sourceExtIds: sourceExtIds,
                connectionTypes: connectionTypes,
                sourceReferrerArchetypes: sourceReferrerArchetypes,
                sourceReferrerResources: sourceReferrerResources,
                sourceReferrerTokens: sourceReferrerTokens,
                referercxTokens: referercxTokens,
                sourceStatuses: sourceStatuses,
                memberArchetypes: memberArchetypes,
                memberIds: memberIds,
                memberExtIds: memberExtIds,
                memberReferrerArchetypes: memberReferrerArchetypes,
                memberReferrerTokens: memberReferrerTokens,
                memberReferrerResources: memberReferrerResources,
                memberStatuses: memberStatuses,
                memberGenders: memberGenders,
                memberAgeRanges: memberAgeRanges,
                validFromAfter: validFromAfter,
                validFromBefore: validFromBefore,
                validToAfter: validToAfter,
                validToBefore: validToBefore,
                includeMember: includeMember,
                countOnMember: countOnMember,
                includeConnectionHasNoMember: includeConnectionHasNoMember,
                sourceParentIds: sourceParentIds,
                sourceParentArchetypes: sourceParentArchetypes);

            return CreateResponse(connectionDtos);

        }

        /// <summary>
        /// Get list connection members in a specific types
        /// </summary>
        /// <param name="connectionType">Type of connection</param>
        /// <param name="sourceArchetypes">List of archetype of connection source which the member belong to</param>
        /// <param name="sourceIds">List of identities of connection source which the member belong to</param>
        /// <param name="sourceExtIds">List of external identities of connection source which the member belong to</param>
        /// <param name="sourceReferrerTokens">List of referrer tokens of connection source which the member belong to</param>
        /// <param name="sourceReferrerResources">List of referrer tokens of connection source which the member belong to</param>
        /// <param name="sourceReferrerArchetypes">List of referrer tokens of connection source which the member belong to</param>
        /// <param name="sourceStatuses">List of entity statues of connection source which the member belong to</param>
        /// <param name="memberArchetypes">List of archetypes of user that is member in connection</param>
        /// <param name="memberIds">List of identity of user that is member in connection</param>
        /// <param name="memberExtIds">List of external identities of user that is member in connection</param>
        /// <param name="ids">List of identities of connection member</param>
        /// <param name="extIds">List of external identities of connection member</param>
        /// <param name="memberStatuses">List of statuses of connection member</param>
        /// <param name="memberReferrerTokens">List of referrer tokes of connection member</param>
        /// <param name="memberReferrerResources">List of referrer resources of connection member</param>
        /// <param name="memberReferrerArchetypes">List of referrer archetypes of connection member</param>
        /// <param name="memberAgeRanges">List of age ranges of user that is member in connection</param>
        /// <param name="memberGenders">List of genders of user that is member in connection</param>   
        /// <param name="validFromBefore">The date time value when connection member has valid-from less than or equal</param>
        /// <param name="validFromAfter">The date time value when connection member has valid-from greater than or equal</param>
        /// <param name="validToBefore">The date time value when connection member has valid-to less than or equal</param>
        /// <param name="validToAfter">The date time value when connection member has valid-to greater than or equal</param>
        /// <param name="createdAfter">The date time value when connection member has created date greater than or equal</param>
        /// <param name="createdBefore">The date time value when connection member has created date less than or equal</param>
        /// <param name="lastUpdatedAfter">The date time value when connection member has last updated date greater than or equal</param>
        /// <param name="lastUpdatedBefore">The date time value when connection member has last updated date less than or equal</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orderBy">Order by  expression</param>
        /// <param name="includeConnectionSource">set true include connection source for each member</param>
        /// <param name="getTotalItemCount">Set true to get total item count</param>
        /// <param name="distinct">Set true to get distinct member (the latest member grouped by user and referrer info)</param>
        /// <param name="memberSearchKey">A keyword to search member on FirstName, LastName, Email or SS if member is an user</param>
        [Route("connections/{connectionType}/members")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ConnectionMemberDto>),200)]
        public IActionResult GetConnectionMembers(ConnectionType connectionType,
            [FromQuery] List<ArchetypeEnum> sourceArchetypes = null,
            [FromQuery] List<int> sourceIds = null,
            [FromQuery] List<string> sourceExtIds = null,
            [FromQuery] List<string> sourceReferrerTokens = null,
            [FromQuery] List<string> sourceReferrerResources = null,
            [FromQuery] List<ArchetypeEnum> sourceReferrerArchetypes = null,
            [FromQuery] List<EntityStatusEnum> sourceStatuses = null,
            [FromQuery] List<ArchetypeEnum> memberArchetypes = null,
            [FromQuery] List<int> memberIds = null,
            [FromQuery] List<string> memberExtIds = null,
            [FromQuery] List<long> ids = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<EntityStatusEnum> memberStatuses = null,
            [FromQuery] List<string> memberReferrerTokens = null,
            [FromQuery] List<string> memberReferrerResources = null,
            [FromQuery] List<ArchetypeEnum> memberReferrerArchetypes = null,
            [FromQuery] List<AgeRange> memberAgeRanges = null,
            [FromQuery] List<Gender> memberGenders = null,
            [FromQuery] DateTime? validFromBefore = null,
            [FromQuery] DateTime? validFromAfter = null,
            [FromQuery] DateTime? validToBefore = null,
            [FromQuery] DateTime? validToAfter = null,
            [FromQuery] DateTime? createdAfter = null,
            [FromQuery] DateTime? createdBefore = null,
            [FromQuery] DateTime? lastUpdatedAfter = null,
            [FromQuery] DateTime? lastUpdatedBefore = null,
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] string orderBy = null,
            [FromQuery] bool includeConnectionSource = false,
            [FromQuery] bool getTotalItemCount = false,
            [FromQuery] bool distinct = false,
            [FromQuery] string memberSearchKey = null)
        {
            var paginatedConnectionMembers = _connectionService.GetPaginatedConnectionMembers(
                ownerId: _workContext.CurrentOwnerId,
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                ids: ids,
                extIds: extIds,
                sourceArchetypes: sourceArchetypes,
                sourceIds: sourceIds,
                sourceExtIds: sourceExtIds,
                connectionTypes: new List<ConnectionType> { connectionType },
                sourceReferrerArchetypes: sourceReferrerArchetypes,
                sourceReferrerResources: sourceReferrerResources,
                sourceReferrerTokens: sourceReferrerTokens,
                sourceStatuses: sourceStatuses,
                memberArchetypes: memberArchetypes,
                memberIds: memberIds,
                memberExtIds: memberExtIds,
                memberReferrerArchetypes: memberReferrerArchetypes,
                memberReferrerTokens: memberReferrerTokens,
                memberReferrerResources: memberReferrerResources,
                memberStatuses: memberStatuses,
                memberGenders: memberGenders,
                memberAgeRanges: memberAgeRanges,
                validFromAfter: validFromAfter,
                validFromBefore: validFromBefore,
                validToAfter: validToAfter,
                validToBefore: validToBefore,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                includeConnectionSource: includeConnectionSource,
                getTotalItemCount: getTotalItemCount,
                distinct: distinct,
                memberSearchKey: memberSearchKey);

            return CreatePagingResponse(paginatedConnectionMembers);
        }

        /// <summary>
        /// Get list connection members
        /// </summary>
        /// <param name="connectionTypes">List of connection types</param>
        /// <param name="sourceArchetypes">List of archetype of connection source which the member belong to</param>
        /// <param name="sourceIds">List of identities of connection source which the member belong to</param>
        /// <param name="sourceExtIds">List of external identities of connection source which the member belong to</param>
        /// <param name="sourceReferrerTokens">List of referrer tokens of connection source which the member belong to</param>
        /// <param name="sourceReferrerResources">List of referrer tokens of connection source which the member belong to</param>
        /// <param name="sourceReferrerArchetypes">List of referrer tokens of connection source which the member belong to</param>
        /// <param name="sourceStatuses">List of entity statues of connection source which the member belong to</param>
        /// <param name="memberArchetypes">List of archetypes of user that is member in connection</param>
        /// <param name="memberIds">List of identity of user that is member in connection</param>
        /// <param name="memberExtIds">List of external identities of user that is member in connection</param>
        /// <param name="ids">List of identities of connection member</param>
        /// <param name="extIds">List of external identities of connection member</param>
        /// <param name="memberStatuses">List of statuses of connection member</param>
        /// <param name="memberReferrerTokens">List of referrer tokes of connection member</param>
        /// <param name="memberReferrerResources">List of referrer resources of connection member</param>
        /// <param name="memberReferrerArchetypes">List of referrer archetypes of connection member</param>
        /// <param name="memberAgeRanges">List of age ranges of user that is member in connection</param>
        /// <param name="memberGenders">List of genders of user that is member in connection</param>   
        /// <param name="validFromBefore">The date time value when connection member has valid-from less than or equal</param>
        /// <param name="validFromAfter">The date time value when connection member has valid-from greater than or equal</param>
        /// <param name="validToBefore">The date time value when connection member has valid-to less than or equal</param>
        /// <param name="validToAfter">The date time value when connection member has valid-to greater than or equal</param>
        /// <param name="createdAfter">The date time value when connection member has created date greater than or equal</param>
        /// <param name="createdBefore">The date time value when connection member has created date less than or equal</param>
        /// <param name="lastUpdatedAfter">The date time value when connection member has last updated date greater than or equal</param>
        /// <param name="lastUpdatedBefore">The date time value when connection member has last updated date less than or equal</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orderBy">Order by  expression</param>
        /// <param name="includeConnectionSource">set true include connection source for each member</param>
        /// <param name="getTotalItemCount">Set true to get total item count</param>
        ///<param name="memberSearchKey">A keyword to search member on FirstName, LastName, Email or SS if member is an user</param>
        [Route("connections/members")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ConnectionMemberDto>),200)]
        public IActionResult GetConnectionMembersWithMultipleTypes([FromQuery] List<ConnectionType> connectionTypes,
            [FromQuery] List<ArchetypeEnum> sourceArchetypes = null,
            [FromQuery] List<int> sourceIds = null,
            [FromQuery] List<string> sourceExtIds = null,
            [FromQuery] List<string> sourceReferrerTokens = null,
            [FromQuery] List<string> sourceReferrerResources = null,
            [FromQuery] List<ArchetypeEnum> sourceReferrerArchetypes = null,
            [FromQuery] List<EntityStatusEnum> sourceStatuses = null,
            [FromQuery] List<ArchetypeEnum> memberArchetypes = null,
            [FromQuery] List<int> memberIds = null,
            [FromQuery] List<string> memberExtIds = null,
            [FromQuery] List<long> ids = null,
            [FromQuery] List<string> extIds = null,
            [FromQuery] List<EntityStatusEnum> memberStatuses = null,
            [FromQuery] List<string> memberReferrerTokens = null,
            [FromQuery] List<string> memberReferrerResources = null,
            [FromQuery] List<ArchetypeEnum> memberReferrerArchetypes = null,
            [FromQuery] List<AgeRange> memberAgeRanges = null,
            [FromQuery] List<Gender> memberGenders = null,
            [FromQuery] DateTime? validFromBefore = null,
            [FromQuery] DateTime? validFromAfter = null,
            [FromQuery] DateTime? validToBefore = null,
            [FromQuery] DateTime? validToAfter = null,
            [FromQuery] DateTime? createdAfter = null,
            [FromQuery] DateTime? createdBefore = null,
            [FromQuery] DateTime? lastUpdatedAfter = null,
            [FromQuery] DateTime? lastUpdatedBefore = null,
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] string orderBy = null,
            [FromQuery] bool includeConnectionSource = false,
            [FromQuery] bool getTotalItemCount = false,
            [FromQuery] string memberSearchKey = null)
        {


            var paginatedConnectionMembers = _connectionService.GetPaginatedConnectionMembers(
                ownerId: _workContext.CurrentOwnerId,
                customerIds: new List<int> {_workContext.CurrentCustomerId},
                ids: ids,
                extIds: extIds,
                sourceArchetypes: sourceArchetypes,
                sourceIds: sourceIds,
                sourceExtIds: sourceExtIds,
                connectionTypes: connectionTypes,
                sourceReferrerArchetypes: sourceReferrerArchetypes,
                sourceReferrerResources: sourceReferrerResources,
                sourceReferrerTokens: sourceReferrerTokens,
                sourceStatuses: sourceStatuses,
                memberArchetypes: memberArchetypes,
                memberIds: memberIds,
                memberExtIds: memberExtIds,
                memberReferrerArchetypes: memberReferrerArchetypes,
                memberReferrerTokens: memberReferrerTokens,
                memberReferrerResources: memberReferrerResources,
                memberStatuses: memberStatuses,
                memberGenders: memberGenders,
                memberAgeRanges: memberAgeRanges,
                validFromAfter: validFromAfter,
                validFromBefore: validFromBefore,
                validToAfter: validToAfter,
                validToBefore: validToBefore,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: orderBy,
                includeConnectionSource: includeConnectionSource,
                getTotalItemCount: getTotalItemCount,                
                memberSearchKey: memberSearchKey);

            return CreatePagingResponse(paginatedConnectionMembers);
        }

        /// <summary>
        /// Get list connection members
        /// </summary>
        /// <returns></returns>
        [Route("getlatestconnectionmembers")]
        [HttpPost]
        [ProducesResponseType(typeof(List<ConnectionMemberDto>),200)]
        public IActionResult GetLatestConnectionMembers([FromBody]ConnectionMemberFilter connectionMemberFilter)
        {
            if (connectionMemberFilter == null)
                return BadRequest("Request filter parameters is missing or could not be parsed");

            var latestConnectionMembers = _connectionService.GetLatestConnectionMembers(ownerId: _workContext.CurrentOwnerId,
                customerIds: new List<int> {_workContext.CurrentCustomerId},
                sourceArchetypes: connectionMemberFilter.SourceArchetypes,
                sourceIds: connectionMemberFilter.SourceIds,
                sourceExtIds: connectionMemberFilter.SourceExtIds,
                connectionTypes: connectionMemberFilter.ConnectionTypes,
                sourceReferrerArchetypes: connectionMemberFilter.SourceReferrerArchetypes,
                sourceReferrerResources: connectionMemberFilter.SourceReferrerResources,
                sourceReferrerTokens: connectionMemberFilter.SourceReferrerTokens,
                sourceStatuses: connectionMemberFilter.SourceStatuses,
                sourceIdsGroupByConnectionType: connectionMemberFilter.OtherTypeSourceIds,
                memberArchetypes: connectionMemberFilter.MemberArchetypes,
                memberIds: connectionMemberFilter.MemberIds,
                memberExtIds: connectionMemberFilter.MemberExtIds,
                memberReferrerArchetypes: connectionMemberFilter.MemberReferrerArchetypes,
                memberReferrerTokens: connectionMemberFilter.MemberReferrerTokens,
                memberReferrerResources: connectionMemberFilter.MemberReferrerResources,
                memberStatuses: connectionMemberFilter.MemberStatuses,
                memberGenders: connectionMemberFilter.MemberGenders,
                memberAgeRanges: connectionMemberFilter.MemberAgeRanges,
                validFromAfter: connectionMemberFilter.ValidFromAfter,
                validFromBefore: connectionMemberFilter.ValidFromBefore,
                validToAfter: connectionMemberFilter.ValidToAfter,
                validToBefore: connectionMemberFilter.ValidToBefore,
                distinctByUser: connectionMemberFilter.DistinctByUser,
                topItems: connectionMemberFilter.TopItems,
                memberSearchKey: connectionMemberFilter.MemberSearchKey);

            return CreateResponse(latestConnectionMembers);
            
        }

        /// <summary>
        /// Insert a collection of connection. 
        /// For each connection, we will create connection source if not existing and add connection member(s) into connection source
        /// </summary>
        /// <param name="connections"></param>
        /// <returns></returns>
        [Route("connections")]
        [HttpPost]
        [ProducesResponseType(typeof(List<ConnectionDto>),200)]
        public IActionResult InsertConnections([FromBody] List<ConnectionDto> connections)
        {
            var responseConnectionDtos = new List<ConnectionDto>();
            foreach (var connectionDto in connections)
            {
                var insertedConnection = _connectionService.InsertConnection(connectionDto);
                responseConnectionDtos.Add(insertedConnection ?? connectionDto);
            }
            return CreateResponse(responseConnectionDtos);


        }
        /// <summary>
        /// Insert a collection of connection source. 
        /// For each connection, we will create connection source if not existing
        /// </summary>
        /// <param name="connectionSources"></param>
        /// <returns></returns>
        [Route("connections/source")]
        [HttpPost]
        [ProducesResponseType(typeof(List<ConnectionDtoBase>),200)]
        public IActionResult InsertConnectionSources([FromBody] List<ConnectionDtoBase> connectionSources)
        {
            var responseConnectionDtos = new List<ConnectionDtoBase>();
            foreach (var connectionDto in connectionSources)
            {
                var insertedConnection = _connectionService.InsertConnectionSource(connectionDto);
                responseConnectionDtos.Add(insertedConnection ?? connectionDto);
            }
            return CreateResponse(responseConnectionDtos);


        }
        /// <summary>
        /// Update a collection of existing connection. 
        /// For each connection, we will update connection source if parameter updateSource=true and update connection member(s)(only update membership info, do not update user info) 
        /// </summary> 
        /// <param name="connectionDtos"></param>
        /// <param name="updateSource">Set true to update connection source also</param>
        /// <returns></returns>
        [Route("connections")]
        [HttpPut]
        [ProducesResponseType(typeof(List<ConnectionDto>),200)]
        public IActionResult UpdateConnections([FromBody] List<ConnectionDto> connectionDtos, [FromQuery] bool updateSource = false)
        {
            var responseConnectionDtos = new List<ConnectionDto>();
            foreach (var connectionDto in connectionDtos)
            {
                var updatedConnection = _connectionService.UpdateConnection(connectionDto, updateSource);
                responseConnectionDtos.Add(updatedConnection ?? connectionDto);
            }
            return CreateResponse(responseConnectionDtos);


        }
        /// <summary>
        /// Update a collection of existing connection source. 
        /// </summary>
        /// <param name="connectionSources"></param>
        /// <returns></returns>
        [Route("connections/source")]
        [HttpPut]
        [ProducesResponseType(typeof(List<ConnectionSourceDto>),200)]
        public IActionResult UpdateConnectionSources([FromBody] List<ConnectionDtoBase> connectionSources)
        {
            var responseConnectionDtos = new List<ConnectionDtoBase>();
            foreach (var connectionSource in connectionSources)
            {
                var updatedConnection = _connectionService.UpdateConnectionSource(connectionSource);
                responseConnectionDtos.Add(updatedConnection ?? connectionSource);
            }
            return CreateResponse(responseConnectionDtos);


        }

        /// <summary>
        /// Get list connections
        /// </summary>
        /// <param name="connectionTypes"></param>
        /// <param name="sourceArchetypes"></param>
        /// <param name="sourceIds"></param>
        /// <param name="sourceExtIds"></param>
        /// <param name="sourceReferrerTokens"></param>
        /// <param name="sourceReferrerResources"></param>
        /// <param name="sourceReferrerArchetypes"></param>
        /// <param name="referercxTokens"></param>
        /// <param name="sourceStatuses"></param>
        /// <param name="memberArchetypes"></param>
        /// <param name="memberIds"></param>
        /// <param name="memberExtIds"></param>
        /// <param name="memberStatuses"></param>
        /// <param name="memberReferrerTokens"></param>
        /// <param name="memberReferrerResources"></param>
        /// <param name="memberReferrerArchetypes"></param>
        /// <param name="memberGenders"></param>
        /// <param name="validFromBefore"></param>
        /// <param name="validFromAfter"></param>
        /// <param name="validToBefore"></param>
        /// <param name="validToAfter"></param>
        /// <param name="memberAgeRanges"></param>
        /// <param name="countOnMember"></param>
        /// <param name="includeConnectionHasNoMember"></param>
        /// <param name="sourceParentIds"></param>
        /// <param name="sourceParentArchetypes"></param>
        /// <returns></returns>
        [Route("connections/source")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ConnectionSourceDto>),200)]
        public IActionResult GetConnectionSources([FromQuery] List<ConnectionType> connectionTypes,
            [FromQuery] List<ArchetypeEnum> sourceArchetypes = null,
            [FromQuery] List<int> sourceIds = null,
            [FromQuery] List<string> sourceExtIds = null,
            [FromQuery] List<string> sourceReferrerTokens = null,
            [FromQuery] List<string> sourceReferrerResources = null,
            [FromQuery] List<string> referercxTokens = null,
            [FromQuery] List<ArchetypeEnum> sourceReferrerArchetypes = null,
            [FromQuery] List<EntityStatusEnum> sourceStatuses = null,
            [FromQuery] List<ArchetypeEnum> memberArchetypes = null,
            [FromQuery] List<int> memberIds = null,
            [FromQuery] List<string> memberExtIds = null,
            [FromQuery] List<EntityStatusEnum> memberStatuses = null,
            [FromQuery] List<string> memberReferrerTokens = null,
            [FromQuery] List<string> memberReferrerResources = null,
            [FromQuery] List<ArchetypeEnum> memberReferrerArchetypes = null,
            [FromQuery] List<AgeRange> memberAgeRanges = null,
            [FromQuery] List<Gender> memberGenders = null,
            [FromQuery] DateTime? validFromBefore = null,
            [FromQuery] DateTime? validFromAfter = null,
            [FromQuery] DateTime? validToBefore = null,
            [FromQuery] DateTime? validToAfter = null,
            [FromQuery] bool countOnMember = false,
            [FromQuery] bool includeConnectionHasNoMember = false,
            [FromQuery] List<int> sourceParentIds = null,
            [FromQuery] List<ArchetypeEnum> sourceParentArchetypes = null)
        {
            var connectionSources = _connectionService.GetConnectionSources(
                sourceArchetypes: sourceArchetypes,
                sourceIds: sourceIds,
                sourceExtIds: sourceExtIds,
                connectionTypes: connectionTypes,
                sourceReferrerArchetypes: sourceReferrerArchetypes,
                sourceReferrerResources: sourceReferrerResources,
                sourceReferrerTokens: sourceReferrerTokens,
                referercxTokens: referercxTokens,
                sourceStatuses: sourceStatuses,
                memberArchetypes: memberArchetypes,
                memberIds: memberIds,
                memberExtIds: memberExtIds,
                memberReferrerArchetypes: memberReferrerArchetypes,
                memberReferrerTokens: memberReferrerTokens,
                memberReferrerResources: memberReferrerResources,
                memberStatuses: memberStatuses,
                memberGenders: memberGenders,
                memberAgeRanges: memberAgeRanges,
                validFromAfter: validFromAfter,
                validFromBefore: validFromBefore,
                validToAfter: validToAfter,
                validToBefore: validToBefore,
                countOnMember: countOnMember,
                includeConnectionHasNoMember: includeConnectionHasNoMember,
                sourceParentIds: sourceParentIds,
                sourceParentArchetypes: sourceParentArchetypes);
            return CreateResponse(connectionSources);

        }

        protected IActionResult CreatePagingResponse<T>(CustomPaginatedList<T> paginatedList ) where T : class
        {
            var response = base.CreatePagingResponse(paginatedList.Items, paginatedList.PageIndex, paginatedList.PageSize, paginatedList.HasMoreData);
            if (Response.StatusCode != (int)HttpStatusCode.NoContent && paginatedList.TotalItemCount.HasValue)
            {
                Response.Headers.Add("X-Paging-TotalItemCount", paginatedList.TotalItemCount.ToString());
            }
            return response;
        }
    }
}
