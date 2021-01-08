using System;
using System.Collections.Generic;
using cxOrganization.Business.Exceptions;
using cxOrganization.Client;
using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Connection
{
    public interface IConnectionService
    {
        List<ConnectionDto> GetConnections(
            List<ArchetypeEnum> sourceArchetypes = null,
            List<EntityStatusEnum> sourceStatuses = null,
            List<int> sourceIds = null,
            List<string> sourceExtIds = null,
            List<string> sourceReferrerTokens = null,
            List<string> sourceReferrerResources = null,
            List<string> referercxTokens = null,
            List<ArchetypeEnum> sourceReferrerArchetypes = null,
            List<ConnectionType> connectionTypes = null,
            List<ArchetypeEnum> memberArchetypes = null,
            List<int> memberIds = null,
            List<string> memberExtIds = null,
            List<EntityStatusEnum> memberStatuses = null,
            List<string> memberReferrerTokens = null,
            List<string> memberReferrerResources = null,
            List<ArchetypeEnum> memberReferrerArchetypes = null,
            List<AgeRange> memberAgeRanges = null,
            List<Gender> memberGenders = null,
            DateTime? validFromBefore = null,
            DateTime? validFromAfter = null,
            DateTime? validToBefore = null,
            DateTime? validToAfter = null,
            bool includeMember = false,
            bool countOnMember = false,
            bool includeConnectionHasNoMember = false,
            List<int> sourceParentIds = null,
            List<ArchetypeEnum> sourceParentArchetypes = null);

        List<ConnectionMemberDto> GetLatestConnectionMembers(int ownerId = 0,
            List<int> customerIds = null,
            List<ArchetypeEnum> sourceArchetypes = null,
            List<EntityStatusEnum> sourceStatuses = null,
            List<int> sourceIds = null,
            List<string> sourceExtIds = null,
            List<string> sourceReferrerTokens = null,
            List<string> sourceReferrerResources = null,
            List<ArchetypeEnum> sourceReferrerArchetypes = null,
            List<ConnectionType> connectionTypes = null,
            List<ArchetypeEnum> memberArchetypes = null,
            List<int> memberIds = null,
            List<string> memberExtIds = null,
            List<EntityStatusEnum> memberStatuses = null,
            List<string> memberReferrerTokens = null,
            List<string> memberReferrerResources = null,
            List<ArchetypeEnum> memberReferrerArchetypes = null,
            List<AgeRange> memberAgeRanges = null,
            List<Gender> memberGenders = null,
            Dictionary<ConnectionType, List<int>> sourceIdsGroupByConnectionType = null,
            DateTime? validFromBefore = null,
            DateTime? validFromAfter = null,
            DateTime? validToBefore = null,
            DateTime? validToAfter = null,
            int topItems = 0,
            bool distinctByUser = false,
            string memberSearchKey = null);

        /// <summary>
        /// Insert a connection source if not existing and add connection members into connection source
        /// </summary>
        /// <param name="connectionDto"></param>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="InvalidException"/>
        /// <exception cref="ConflictException"/>
        /// <returns></returns>
        ConnectionDto InsertConnection(ConnectionDto connectionDto);

        /// <summary>
        /// Update a existing connection
        /// </summary>
        /// <param name="connectionDto"></param>
        /// <param name="updateConnectSource">Set true to update connection source also</param>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="InvalidException"/>
        /// <exception cref="ConflictException"/>
        /// <returns></returns>
        ConnectionDto UpdateConnection(ConnectionDto connectionDto, bool updateConnectSource = false);

        /// <summary>
        /// Insert new connection source
        /// </summary>
        /// <param name="connectionDtoBase"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="InvalidException"/>
        /// <exception cref="ConflictException"/>
        ConnectionDtoBase InsertConnectionSource(ConnectionDtoBase connectionDtoBase);


        /// <summary>
        /// Update a existing connection source
        /// </summary>
        /// <param name="connectionDtoBase"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"/>
        /// <exception cref="InvalidException"/>
        /// <exception cref="ConflictException"/>  
        ConnectionDtoBase UpdateConnectionSource(ConnectionDtoBase connectionDtoBase);

        /// <summary>
        /// Get list connection source with filtering
        /// </summary>
        /// <param name="sourceArchetypes"></param>
        /// <param name="sourceIds"></param>
        /// <param name="sourceExtIds"></param>
        /// <param name="connectionTypes"></param>
        /// <param name="sourceReferrerArchetypes"></param>
        /// <param name="sourceReferrerResources"></param>
        /// <param name="sourceReferrerTokens"></param>
        /// <param name="referercxTokens"></param>
        /// <param name="sourceStatuses"></param>
        /// <param name="memberArchetypes"></param>
        /// <param name="memberIds"></param>
        /// <param name="memberExtIds"></param>
        /// <param name="memberReferrerArchetypes"></param>
        /// <param name="memberReferrerTokens"></param>
        /// <param name="memberReferrerResources"></param>
        /// <param name="memberStatuses"></param>
        /// <param name="memberGenders"></param>
        /// <param name="memberAgeRanges"></param>
        /// <param name="validFromAfter"></param>
        /// <param name="validFromBefore"></param>
        /// <param name="validToAfter"></param>
        /// <param name="validToBefore"></param>
        /// <param name="countOnMember"></param>
        /// <param name="includeConnectionHasNoMember"></param>
        /// <returns></returns>
        List<ConnectionSourceDto> GetConnectionSources(
            List<ArchetypeEnum> sourceArchetypes = null,
            List<EntityStatusEnum> sourceStatuses = null,
            List<int> sourceIds = null,
            List<string> sourceExtIds = null,
            List<string> sourceReferrerTokens = null,
            List<string> sourceReferrerResources = null,
            List<string> referercxTokens = null,
            List<ArchetypeEnum> sourceReferrerArchetypes = null,
            List<ConnectionType> connectionTypes = null,
            List<ArchetypeEnum> memberArchetypes = null,
            List<int> memberIds = null,
            List<string> memberExtIds = null,
            List<EntityStatusEnum> memberStatuses = null,
            List<string> memberReferrerTokens = null,
            List<string> memberReferrerResources = null,
            List<ArchetypeEnum> memberReferrerArchetypes = null,
            List<AgeRange> memberAgeRanges = null,
            List<Gender> memberGenders = null,
            DateTime? validFromBefore = null,
            DateTime? validFromAfter = null,
            DateTime? validToBefore = null,
            DateTime? validToAfter = null,
            bool countOnMember = false,
            bool includeConnectionHasNoMember = false,
            List<int> sourceParentIds = null,
            List<ArchetypeEnum> sourceParentArchetypes = null);

        CustomPaginatedList<ConnectionMemberDto> GetPaginatedConnectionMembers(int ownerId,
            List<int> customerIds = null,
            List<long> ids = null,
            List<string> extIds = null,
            List<ArchetypeEnum> sourceArchetypes = null,
            List<int> sourceIds = null,
            List<string> sourceExtIds = null,
            List<ConnectionType> connectionTypes = null,
            List<ArchetypeEnum> sourceReferrerArchetypes = null,
            List<string> sourceReferrerResources = null,
            List<string> sourceReferrerTokens = null,
            List<EntityStatusEnum> sourceStatuses = null,
            List<ArchetypeEnum> memberArchetypes = null,
            List<int> memberIds = null,
            List<string> memberExtIds = null,
            List<ArchetypeEnum> memberReferrerArchetypes = null,
            List<string> memberReferrerTokens = null,
            List<string> memberReferrerResources = null,
            List<EntityStatusEnum> memberStatuses = null,
            List<Gender> memberGenders = null,
            List<AgeRange> memberAgeRanges = null,
            DateTime? validFromAfter = null,
            DateTime? validFromBefore = null,
            DateTime? validToAfter = null,
            DateTime? validToBefore = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = null,
            bool includeConnectionSource = false,
            bool getTotalItemCount = false,
            bool distinct = false,
            string memberSearchKey = null);
    }
}