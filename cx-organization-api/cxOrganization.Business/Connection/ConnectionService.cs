using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Business.CandidateList;
using cxOrganization.Business.Common;
using cxOrganization.Business.Exceptions;
using cxOrganization.Business.Extensions;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain;
using cxOrganization.Domain.ApiClient;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using cxPlatform.Core.Extentions;
using cxPlatform.Core.Extentions.OrderByExtension;
using Microsoft.Extensions.Options;

namespace cxOrganization.Business.Connection
{
    public class ConnectionService : IConnectionService
    {

        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUGMemberRepository _ugMemberRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWorkContext _workContext;
        private readonly Func<ArchetypeEnum, IUserGroupService> _userGroupService;
        private readonly Func<ArchetypeEnum, IUGMemberService> _userGroupMemberService;
        private readonly ICacheProvider _memoryCacheProvider;
        private readonly ConnectionConfig _connectionConfig;
        private readonly IEventLogDomainApiClient _eventClientService;
        private readonly IUserGroupUserMappingService _userGroupUserMappingService;
        private readonly AppSettings _appSettings;
        public ConnectionService(IUserGroupRepository userGroupRepository,
            IUGMemberRepository ugMemberRepository,
            IDepartmentRepository departmentRepository,
            IUserRepository userRepository,
            Func<ArchetypeEnum, IUserGroupService> userGroupService,
            Func<ArchetypeEnum, IUGMemberService> userGroupMemberService,
            IWorkContext workContext,
            ICacheProvider cacheProvider,
            IOptions<ConnectionConfig> connectionConfigOption,
            IEventLogDomainApiClient eventClientService,
            IUserGroupUserMappingService userGroupUserMappingService,
            IOptions<AppSettings> appSettingsOption)
        {
            _userGroupRepository = userGroupRepository;
            _ugMemberRepository = ugMemberRepository;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _userGroupService = userGroupService;
            _userGroupMemberService = userGroupMemberService;
            _workContext = workContext;
            _memoryCacheProvider = cacheProvider;
            _connectionConfig = connectionConfigOption.Value;
            _eventClientService = eventClientService;
            _userGroupUserMappingService = userGroupUserMappingService;
            _appSettings = appSettingsOption.Value;
        }

        public List<ConnectionDto> GetConnections(
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
            List<ArchetypeEnum> sourceParentArchetypes = null)
        {

            var customerIds = new List<int> { _workContext.CurrentCustomerId };
            var userGroupTypes = connectionTypes.ConvertToGroupTypes();

            var filterOnValidTime = HasAnyData(validFromAfter, validFromBefore, validToAfter, validToBefore);
            var filterOnOtherMemberInfo = HasAnyData(memberIds, memberStatuses,
                memberExtIds, memberArchetypes, memberReferrerArchetypes, memberReferrerResources,
                memberReferrerTokens, memberAgeRanges, memberGenders);

            if (_connectionConfig.GetMemberInGetTimeAsDefault &&
                !filterOnValidTime && (countOnMember || includeMember || filterOnOtherMemberInfo))
            {
                validFromBefore = DateTime.Now;
                validToAfter = DateTime.Now;
            }

            var filterOnMember = filterOnValidTime || filterOnOtherMemberInfo;


            //Need to execute this function after checking DoesNeedToFilterOnUGMember to make sure filter on member is required by client 
            SetDefaultGettingStatusesIfNullOrEmpty(ref sourceStatuses, ref memberStatuses);

            List<ArchetypeEnum> parentDepartmentArchetypes, parentUserArchetypes;
            List<int> parentDepartmentIds, parentUserIds;

            AnalyzeSourceParentFilteInfo(sourceParentIds, sourceParentArchetypes, out parentDepartmentArchetypes, out parentUserArchetypes, out parentDepartmentIds, out parentUserIds);

            var userGroups = GetUserGroupEnities(sourceArchetypes, sourceStatuses, sourceIds, sourceExtIds, sourceReferrerTokens, sourceReferrerResources,
                referercxTokens, sourceReferrerArchetypes, customerIds, userGroupTypes, parentDepartmentArchetypes, parentUserArchetypes, parentDepartmentIds, parentUserIds);

            List<UGMemberEntity> ugMemberEntities = null;
            Dictionary<int, int> countMemberGroupByUserGroup = null;
            if (userGroups.Count > 0 && (includeMember || countOnMember || filterOnMember))
            {
                ugMemberEntities = GetUgMemberEntities(_workContext.CurrentOwnerId, customerIds, memberArchetypes,
                    memberIds, memberExtIds, memberStatuses,
                    memberReferrerTokens, memberReferrerResources,
                    memberReferrerArchetypes, memberAgeRanges,
                    memberGenders, userGroups, validFromBefore, validFromAfter, validToBefore,
                    validToAfter, filterOnMember, includeMember, countOnMember, out countMemberGroupByUserGroup);
            }

            return ConnectionBuilder.BuildConnectionDtos(ugMemberEntities, userGroups, countMemberGroupByUserGroup, filterOnMember, countOnMember, includeConnectionHasNoMember);
        }

        private List<UserGroupEntity> GetUserGroupEnities(List<ArchetypeEnum> sourceArchetypes,
            List<EntityStatusEnum> sourceStatuses,
            List<int> sourceIds,
            List<string> sourceExtIds,
            List<string> sourceReferrerTokens,
            List<string> sourceReferrerResources,
            List<string> referercxTokens,
            List<ArchetypeEnum> sourceReferrerArchetypes,
            List<int> customerIds,
            List<GrouptypeEnum> userGroupTypes,
            List<ArchetypeEnum> parentDepartmentArchetypes,
            List<ArchetypeEnum> parentUserArchetypes,
            List<int> parentDepartmentIds, List<int> parentUserIds)
        {
            List<UserGroupEntity> userGroups = new List<UserGroupEntity>();

            if (!parentDepartmentIds.IsNullOrEmpty() || !parentDepartmentArchetypes.IsNullOrEmpty()
                || (parentUserArchetypes.IsNullOrEmpty() && parentUserIds.IsNullOrEmpty()))
            {
                //If there is any filtering on parent department, we get usergroup here
                //If there is no any filter on parent department either parent user, we get usergroup here
                //If there is any filter on parent user but parent department, we do NOT get  usergroup here

                userGroups.AddRange(_userGroupRepository.GetUserGroupsWithoutPaging(
                    userGroupIds: sourceIds, customerIds: customerIds, userGroupArchetypeIds: sourceArchetypes,
                    userGroupExtIds: sourceExtIds, userGroupStatuses: sourceStatuses, userGroupTypeIds: userGroupTypes,
                    referercxTokens: referercxTokens, referrerResources: sourceReferrerResources, referrerArchetypes: sourceReferrerArchetypes,
                    referrerTokens: sourceReferrerTokens,
                    ownerId: _workContext.CurrentOwnerId, includeMemberUsers: false, parentDepartmentIds: parentDepartmentIds, parentDepartmentArchetypes: parentDepartmentArchetypes));
            }
            if (!parentUserArchetypes.IsNullOrEmpty() || !parentUserIds.IsNullOrEmpty())
            {
                //There is any filtering on parent user

                userGroups.AddRange(_userGroupRepository.GetUserGroupsWithoutPaging(
                    userGroupIds: sourceIds, customerIds: customerIds, userGroupArchetypeIds: sourceArchetypes,
                    userGroupExtIds: sourceExtIds, userGroupStatuses: sourceStatuses, userGroupTypeIds: userGroupTypes,
                    referercxTokens: referercxTokens, referrerResources: sourceReferrerResources, referrerArchetypes: sourceReferrerArchetypes,
                    referrerTokens: sourceReferrerTokens,
                    ownerId: _workContext.CurrentOwnerId, includeMemberUsers: false, parentUserIds: parentUserIds, parentUserArchetypes: parentUserArchetypes));
            }

            return userGroups;
        }

        private static void AnalyzeSourceParentFilteInfo(List<int> sourceParentIds, List<ArchetypeEnum> sourceParentArchetypes, out List<ArchetypeEnum> parentDepartmentArchetypes, out List<ArchetypeEnum> parentUserArchetypes, out List<int> parentDepartmentIds, out List<int> parentUserIds)
        {
            //Filter on source parent could be user or department or mixed, we need  to analyze to extract parent department and user

            parentDepartmentArchetypes = null;
            parentUserArchetypes = null;
            parentDepartmentIds = null;
            parentUserIds = null;
            if (!sourceParentArchetypes.IsNullOrEmpty())
            {
                parentDepartmentArchetypes = sourceParentArchetypes.Where(a => a.IsDepartmentArchetype()).ToList();
                parentUserArchetypes = sourceParentArchetypes.Where(a => a.IsUserArchetype()).ToList();

                if (!parentDepartmentArchetypes.IsNullOrEmpty())
                {
                    parentDepartmentIds = sourceParentIds;
                }

                if (!parentUserArchetypes.IsNullOrEmpty())
                {
                    parentUserIds = sourceParentIds;
                }
            }
            else
            {

                parentDepartmentIds = sourceParentIds;
                parentUserIds = sourceParentIds;
            }

        }

        public List<ConnectionSourceDto> GetConnectionSources(
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
            List<ArchetypeEnum> sourceParentArchetypes = null)
        {

            var connectionDtos = GetConnections(
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
                includeMember: false,
                countOnMember: countOnMember,
                includeConnectionHasNoMember: includeConnectionHasNoMember,
                sourceParentIds: sourceParentIds,
                sourceParentArchetypes: sourceParentArchetypes);

            return connectionDtos.Select(c => c.Source).ToList();

        }

        public CustomPaginatedList<ConnectionMemberDto> GetPaginatedConnectionMembers(int ownerId,
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
            string memberSearchKey = null)
        {
            var entityOrderBy = OrderByTranslator.TranslatingDTOQueryIntoEntityQuery<ConnectionMemberDto>(orderBy);

            SetDefaultGettingStatusesIfNullOrEmpty(ref sourceStatuses, ref memberStatuses);

            var filterOnValidTime = HasAnyData(validFromAfter, validFromBefore, validToAfter, validToBefore);
            if (!filterOnValidTime && _connectionConfig.GetMemberInGetTimeAsDefault)
            {
                validFromBefore = DateTime.Now;
                validToAfter = DateTime.Now;
            }

            var paginatedMemberEntities = _ugMemberRepository
                .GetPaginatedUGMembers(ownerId: ownerId,
                    customerIds: customerIds,
                    ugMemberIds: ids,
                    ugMemberExtIds: extIds,
                    userGroupArchetypeIds: sourceArchetypes,
                    userGroupExtIds: sourceExtIds,
                    userGroupReferrerTokens: sourceReferrerTokens,
                    userGroupReferrerResources: sourceReferrerResources,
                    userGroupReferrerArchetypes: sourceReferrerArchetypes,
                    userGroupTypeIds: connectionTypes.ConvertToGroupTypes(),
                    userGroupStatuses: sourceStatuses,
                    userIds: memberIds,
                    userGroupIds: sourceIds,
                    ugMemberStatus: memberStatuses,
                    userExtIds: memberExtIds,
                    userArchetypes: memberArchetypes,
                    referrerTokens: memberReferrerTokens,
                    referrerResources: memberReferrerResources,
                    referrerArchetypes: memberReferrerArchetypes,
                    memberAgeRanges: memberAgeRanges,
                    memberGenders: memberGenders,
                    validFromBefore: validFromBefore,
                    validFromAfter: validFromAfter,
                    validToBefore: validToBefore,
                    validToAfter: validToAfter,
                    createdBefore: createdBefore,
                    createdAfter: createdAfter,
                    lastUpdatedBefore: lastUpdatedBefore,
                    lastUpdatedAfter: lastUpdatedAfter,
                    includeUser: true,
                    includeUserGroup: includeConnectionSource,
                    pageIndex: pageIndex,
                    pageSize: pageSize,
                    orderBy: entityOrderBy,
                    getTotalItemCount: getTotalItemCount,
                    distinct: distinct,
                    userSearchKey: memberSearchKey);


            var connectionsDtos = paginatedMemberEntities.Items
                .Select(i => ConnectionBuilder
                    .GenerateConnectionMemberDto(
                    i,
                    _workContext.CurrentOwnerId,
                    includeConnectionSource: includeConnectionSource,
                    shouldHideDateOfBirth: ShouldHideDateOfBirth()))
                .ToList();

            return new CustomPaginatedList<ConnectionMemberDto>(connectionsDtos,
                paginatedMemberEntities.PageIndex,
                paginatedMemberEntities.PageSize,
                paginatedMemberEntities.HasMoreData,
                paginatedMemberEntities.TotalItemCount);
        }

        private void SetDefaultGettingStatusesIfNullOrEmpty(ref List<EntityStatusEnum> sourceStatuses, ref List<EntityStatusEnum> memberStatuses)
        {

            if (sourceStatuses.IsNullOrEmpty())
            {
                sourceStatuses = _connectionConfig.AcceptedStatusesForGetting;
            }
            if (memberStatuses.IsNullOrEmpty())
            {
                memberStatuses = _connectionConfig.AcceptedStatusesForGetting;
            }

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ConnectionDto InsertConnection(ConnectionDto connectionDto)
        {
            var connectionSource = connectionDto.Source;

            ValidateSupportedConnectionSource(connectionSource);

            var connectionTypeConfig = _connectionConfig.ConnectionTypeConfigs.GetByConnectionType(connectionDto.Source.Type);

            var updatedByIdentity = GetUpdatedByIdentity(connectionDto.UpdatedByIdentity);

            var insertedConnectionSource = GetOrInsertConnectionSourceAsUserGroup(connectionSource, updatedByIdentity, false);

            if (IsNotAllowedStatusToEdit(insertedConnectionSource.EntityStatus.StatusId))
            {
                throw new InvalidException(string.Format("Do not allow to add connection member into source (type {0}, archetype {1}, id {2}, extId '{3}') which is {4}.",
                    insertedConnectionSource.Type, insertedConnectionSource.Identity.Archetype, insertedConnectionSource.Identity.Id, insertedConnectionSource.Identity.ExtId, insertedConnectionSource.EntityStatus.StatusId));
            }

            var insertedConnectionMembers = HandleInsertingConnectionMemberAsUGMember(insertedConnectionSource, connectionDto.Members, updatedByIdentity, connectionTypeConfig);

            var insertedConnection = new ConnectionDto
            {
                Source = insertedConnectionSource,
                Members = insertedConnectionMembers,
                UpdatedByIdentity = updatedByIdentity
            };

            ClearRelatedMemmoryCache(insertedConnectionSource.Identity.OwnerId, insertedConnectionSource.Identity.CustomerId, insertedConnectionSource.ReferrerToken);

            return insertedConnection;

        }

        private bool IsNotAllowedStatusToEdit(EntityStatusEnum entityStatus)
        {
            //We ignore checking status  <=0 since we will set it to Active as default
            return _connectionConfig.AcceptedStatusesForEditting.Count > 0 && entityStatus > 0
                   && !_connectionConfig.AcceptedStatusesForEditting.Contains(entityStatus);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ConnectionDto UpdateConnection(ConnectionDto connectionDto, bool updateConnectSource = false)
        {
            var connectionSource = connectionDto.Source;

            ValidateSupportedConnectionSource(connectionSource);

            var connectionTypeConfig = _connectionConfig.ConnectionTypeConfigs.GetByConnectionType(connectionDto.Source.Type);

            var updatedByIdentity = GetUpdatedByIdentity(connectionDto.UpdatedByIdentity);

            var existingConnectionSource = GetConnectionSourceAsUserGroupForUpdating(connectionSource, _connectionConfig.AcceptedStatusesForEditting);

            if (updateConnectSource)
            {
                existingConnectionSource = HandleUpdatingConnectionSourceAsUserGroup(connectionDto.Source, existingConnectionSource, updatedByIdentity);
            }

            var updatedConnectionMembers = HandleUpdatingConnectionMemberAsUGMember(existingConnectionSource, connectionDto.Members, updatedByIdentity, connectionTypeConfig);

            var updatedConnection = new ConnectionDto
            {
                Source = existingConnectionSource,
                Members = updatedConnectionMembers,
                UpdatedByIdentity = updatedByIdentity
            };

            ClearRelatedMemmoryCache(existingConnectionSource.Identity.OwnerId, existingConnectionSource.Identity.CustomerId, existingConnectionSource.ReferrerToken);

            return updatedConnection;

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>

        public ConnectionDtoBase InsertConnectionSource(ConnectionDtoBase connectionDtoBase)
        {
            ValidateSupportedConnectionSource(connectionDtoBase.Source);

            connectionDtoBase.Source.Identity.Id = null;

            var updatedByIdentity = GetUpdatedByIdentity(connectionDtoBase.UpdatedByIdentity);

            var insertedConnectionSource = GetOrInsertConnectionSourceAsUserGroup(connectionDtoBase.Source, updatedByIdentity, true);

            ClearRelatedMemmoryCache(insertedConnectionSource.Identity.OwnerId, insertedConnectionSource.Identity.CustomerId, insertedConnectionSource.ReferrerToken);

            return new ConnectionDtoBase()
            {
                Source = insertedConnectionSource,
                UpdatedByIdentity = updatedByIdentity
            };

        }

        public ConnectionDtoBase UpdateConnectionSource(ConnectionDtoBase connectionDtoBase)
        {
            ValidateSupportedConnectionSource(connectionDtoBase.Source);

            var existingConnectionSource = GetConnectionSourceAsUserGroupForUpdating(connectionDtoBase.Source, _connectionConfig.AcceptedStatusesForEditting);

            var updatedByIdentity = GetUpdatedByIdentity(connectionDtoBase.UpdatedByIdentity);

            var updatedConnectionSource = HandleUpdatingConnectionSourceAsUserGroup(connectionDtoBase.Source, existingConnectionSource, updatedByIdentity);

            ClearRelatedMemmoryCache(updatedConnectionSource.Identity.OwnerId, updatedConnectionSource.Identity.CustomerId, updatedConnectionSource.ReferrerToken);

            return new ConnectionDtoBase()
            {
                Source = updatedConnectionSource,
                UpdatedByIdentity = updatedByIdentity
            };

        }



        private void ValidateSupportedConnectionSource(ConnectionSourceDto connectionSource)
        {
            if (!connectionSource.Identity.Archetype.IsUserGroupArchetype())
            {
                throw new InvalidException(string.Format("Connection source with archetype '{0}' is unsupported to handle connection. It's expected to be user group archetype for processing.", connectionSource.Identity.Archetype));
            }

        }

        private List<ConnectionMemberDto> HandleUpdatingConnectionMemberAsUGMember(ConnectionSourceDto existingConnectionSource,
            List<ConnectionMemberDto> connectionMemberDtos, IdentityDto updatedByIdentity, ConnectionTypeConfig connectionTypeConfig)
        {
            if (connectionMemberDtos.Count == 0) return new List<ConnectionMemberDto>();

            var validConnectionMembers = connectionMemberDtos
                .Where(IsValidConnectionMember)
                .ToList();
            if (validConnectionMembers.Count == 0)
            {
                throw new InvalidException("There is no any connection members that is valid owner, customer and archetype");
            }


            List<UserEntity> existingUserEntities;
            var connectionSourceId = (int)existingConnectionSource.Identity.Id.Value;
            var existingUgMembers = GetExistingUGMemberEntities(connectionSourceId, validConnectionMembers, _connectionConfig.AcceptedStatusesForEditting, out existingUserEntities);

            var userGroupMemberservice = _userGroupMemberService(existingConnectionSource.Identity.Archetype);
            var updatedConnectionMembers = new List<ConnectionMemberDto>();
            foreach (var connectionMemberDto in connectionMemberDtos)
            {
                var existingUgMemberEntity = GetExistingUgMemberEntity(connectionMemberDto, existingUgMembers, true, true);
                var oldStatus = (EntityStatusEnum?)existingUgMemberEntity.EntityStatusId;

                ConnectionHelper.EnsureConnectionMemberValuesForUpdating(connectionMemberDto, existingUgMemberEntity);

                var defaultStatus = oldStatus ?? EntityStatusEnum.Active;

                var userMembership = ConnectionHelper.BuildUserMembership(existingConnectionSource.Identity.OwnerId, existingConnectionSource.Identity.CustomerId,
                    connectionSourceId, connectionMemberDto, updatedByIdentity, _workContext.CurrentUserId, defaultStatus);

                var updatedUserMembership = userGroupMemberservice.Update(userMembership);

                var updatedConnectionMember = ConnectionBuilder.GenerateConnectionMemberDto(
                    updatedUserMembership,
                    existingUgMemberEntity.User,
                    ShouldHideDateOfBirth());

                var eventTypeNames = connectionTypeConfig.GetEventTypeNames(oldStatus, updatedConnectionMember.EntityStatus.StatusId);
                if (eventTypeNames.Count > 0)
                {
                    IdentityBaseDto departmentIdentity = null;

                    if (connectionMemberDto.IsUserMember())
                    {
                        var userEntity = GetExistingUserEntity(updatedConnectionMember, existingUserEntities);
                        departmentIdentity = new IdentityBaseDto
                        {
                            Archetype = (ArchetypeEnum)(userEntity.Department.ArchetypeId ?? 0),
                            Id = userEntity.DepartmentId

                        };

                    }

                    ProcessBussinessEventAsync(existingConnectionSource, updatedConnectionMember, departmentIdentity,
                        existingConnectionSource.Type, eventTypeNames, updatedByIdentity);

                }

                updatedConnectionMembers.Add(updatedConnectionMember);

            }

            return updatedConnectionMembers;
        }


        private ConnectionSourceDto HandleUpdatingConnectionSourceAsUserGroup(ConnectionSourceDto givenConnectionSource, ConnectionSourceDto existingConnectionSource, IdentityDto updatedByIdentity)
        {
            givenConnectionSource.Identity = existingConnectionSource.Identity;
            givenConnectionSource.Parent = givenConnectionSource.Parent == null
                ? existingConnectionSource.Parent
                : GetConnectionSourceParentIdentity(givenConnectionSource.Parent); //Given source parent might not contain id, we need to get id from extid


            givenConnectionSource.EntityStatus = givenConnectionSource.EntityStatus ?? existingConnectionSource.EntityStatus;
            givenConnectionSource.EntityStatus.LastUpdated = DateTime.Now;
            givenConnectionSource.EntityStatus.EntityVersion = givenConnectionSource.EntityStatus.EntityVersion ?? existingConnectionSource.EntityStatus.EntityVersion;

            var userGroupService = _userGroupService(existingConnectionSource.Identity.Archetype);
            var validation = GetUserGroupHierarchyDepartmentValidation(givenConnectionSource.Parent);

            var userGroupForUpdating = ConnectionHelper.GenerateConnectionSourceAsUserGroup(givenConnectionSource.Type.ConvertToGroupType()
                , givenConnectionSource, updatedByIdentity, _workContext.CurrentUserId, existingConnectionSource.EntityStatus.StatusId);
            var updatedUserGroup = userGroupService.UpdateUserGroup(validation, userGroupForUpdating);

            givenConnectionSource.EntityStatus = updatedUserGroup.EntityStatus;
            return givenConnectionSource;

        }

        private List<ConnectionMemberDto> HandleInsertingConnectionMemberAsUGMember(ConnectionSourceDto existingConnectionSource,
            List<ConnectionMemberDto> connectionMemberDtos, IdentityDto updatedByIdentity, ConnectionTypeConfig connectionTypeConfig)
        {
            if (connectionMemberDtos.Count == 0) return new List<ConnectionMemberDto>();

            var validConnectionMembers = connectionMemberDtos.Where(IsValidConnectionMember).ToList();
            if (validConnectionMembers.Count == 0)
            {
                throw new InvalidException("There is no any connection members that is valid identity or referrer information");
            }
            List<UserEntity> existingUserEntities;
            var connectionSourceId = (int)existingConnectionSource.Identity.Id.Value;
            var existingUgMembers = GetExistingUGMemberEntities(connectionSourceId, validConnectionMembers, _connectionConfig.AcceptedStatusesForEditting, out existingUserEntities);

            var userGroupMemberservice = _userGroupMemberService(existingConnectionSource.Identity.Archetype);
            var insertedConnectionMembers = new List<ConnectionMemberDto>();
            foreach (var connectionMemberDto in connectionMemberDtos)
            {
                var existingUgMemberEntity = GetExistingUgMemberEntity(connectionMemberDto, existingUgMembers, false, false);
                if (existingUgMemberEntity == null)
                {
                    if (connectionMemberDto.EntityStatus != null && IsNotAllowedStatusToEdit(connectionMemberDto.EntityStatus.StatusId))
                    {
                        throw new InvalidException(string.Format("Connection member with archetype '{0}', id '{1}', extId '{2}' is not allowed to insert with status {3}", connectionMemberDto.UserIdentity.Archetype, connectionMemberDto.UserIdentity.Id, connectionMemberDto.UserIdentity.ExtId, connectionMemberDto.EntityStatus.StatusId));
                    }

                    var userEntity = GetExistingUserEntity(connectionMemberDto, existingUserEntities);
                    if (connectionMemberDto.IsUserMember() && userEntity == null)
                    {
                        throw new NotFoundException(string.Format("User entity of connection member with archetype '{0}', id '{1}', extId '{2}' is not found", connectionMemberDto.UserIdentity.Archetype, connectionMemberDto.UserIdentity.Id, connectionMemberDto.UserIdentity.ExtId));
                    }

                    ConnectionHelper.EnsureConnectionMemberValuesForInserting(existingConnectionSource.Identity.OwnerId, connectionMemberDto, userEntity, updatedByIdentity, _workContext.CurrentUserId);

                    var userMembership = ConnectionHelper.BuildUserMembership(existingConnectionSource.Identity.OwnerId, existingConnectionSource.Identity.CustomerId,
                        connectionSourceId, connectionMemberDto, updatedByIdentity, _workContext.CurrentUserId);


                    var insertedUserMembership = userGroupMemberservice.Insert(userMembership);

                    AddToExistingUgMemberEntities(existingUgMembers, insertedUserMembership);

                    connectionMemberDto.Identity = connectionMemberDto.Identity ?? new IdentityDto();
                    connectionMemberDto.Identity.Id = insertedUserMembership.MemberId;
                    connectionMemberDto.Identity.OwnerId = insertedUserMembership.Identity.OwnerId;
                    connectionMemberDto.Identity.CustomerId = insertedUserMembership.Identity.CustomerId;

                    var insertedConnectionMemberDto = ConnectionBuilder.GenerateConnectionMemberDto(
                        insertedUserMembership,
                        userEntity,
                        ShouldHideDateOfBirth());

                    var eventTypeNames = connectionTypeConfig.GetEventTypeNames(null, insertedConnectionMemberDto.EntityStatus.StatusId);

                    if (eventTypeNames.Count > 0)
                    {
                        var departmentIdentity = userEntity != null
                            ? new IdentityBaseDto
                            {
                                Archetype = (ArchetypeEnum)(userEntity.Department.ArchetypeId ?? 0),
                                Id = userEntity.DepartmentId
                            }
                            : null;

                        ProcessBussinessEventAsync(existingConnectionSource, insertedConnectionMemberDto, departmentIdentity,
                            existingConnectionSource.Type, eventTypeNames, updatedByIdentity);
                    }

                    insertedConnectionMembers.Add(insertedConnectionMemberDto);
                }
                else
                {

                    insertedConnectionMembers.Add(ConnectionBuilder.GenerateConnectionMemberDto(existingUgMemberEntity, existingConnectionSource.Identity.OwnerId));
                }

            }
            return insertedConnectionMembers;
        }

        private bool IsValidConnectionMember(ConnectionMemberDto connectionMember)
        {
            if (connectionMember.UserIdentity == null)
            {
                return !string.IsNullOrEmpty(connectionMember.ReferrerToken)
                       && !string.IsNullOrEmpty(connectionMember.ReferrerResource)
                       && connectionMember.ReferrerArchetype != null;
            }

            return connectionMember.UserIdentity.Archetype.IsUserArchetype()
                   && (connectionMember.UserIdentity.OwnerId == 0 || connectionMember.UserIdentity.OwnerId == _workContext.CurrentOwnerId)
                   && (connectionMember.UserIdentity.CustomerId == 0 || connectionMember.UserIdentity.CustomerId == _workContext.CurrentCustomerId);


        }

        private void AddToExistingUgMemberEntities(List<UGMemberEntity> existingUgMembers, MembershipDto insertedUserMembership)
        {
            existingUgMembers.Add(_userGroupUserMappingService.ToUGMemberEntity(insertedUserMembership));
        }

        private List<UGMemberEntity> GetExistingUGMemberEntities(int connectionSourceId, List<ConnectionMemberDto> validConnectionMembers, List<EntityStatusEnum> memberStatuses, out List<UserEntity> existingUserEntities)
        {
            var customerIds = new List<int> { _workContext.CurrentCustomerId };
            var existingUgMembers = GetExistingUserUgMembers(connectionSourceId, validConnectionMembers, customerIds, memberStatuses, out existingUserEntities);
            existingUgMembers.AddRange(GetExistingNonuserUgMembers(connectionSourceId, validConnectionMembers, customerIds, memberStatuses));
            return existingUgMembers;
        }

        private List<UGMemberEntity> GetExistingUserUgMembers(int connectionSourceId, List<ConnectionMemberDto> validConnectionMembers, List<int> customerIds, List<EntityStatusEnum> memberStatuses, out List<UserEntity> existingUserEntities)
        {
            var userConnectionMembers = validConnectionMembers.Where(c => c.IsUserMember()).ToList();
            var archetypes = userConnectionMembers.Select(c => c.UserIdentity.Archetype).Distinct().ToList();

            existingUserEntities = GetExistingUserEntities(userConnectionMembers, customerIds, archetypes);

            if (existingUserEntities.Count == 0) return new List<UGMemberEntity>();
            var existingUgMembers = _ugMemberRepository.GetUGMembers(ownerId: _workContext.CurrentOwnerId, customerIds: customerIds,
                userGroupIds: new List<int> { connectionSourceId },
                userGroupStatus: new List<EntityStatusEnum> { EntityStatusEnum.All },
                userIds: existingUserEntities.Select(u => u.UserId).ToList(), ugMemberStatuses: memberStatuses, userArchetypes: archetypes,
                includeUser: true);
            return existingUgMembers;
        }
        private List<UGMemberEntity> GetExistingNonuserUgMembers(int connectionSourceId, List<ConnectionMemberDto> validConnectionMembers, List<int> customerIds, List<EntityStatusEnum> memberStatuses)
        {
            var nonuserMembers = validConnectionMembers.Where(m => m.IsNonuserMember()).ToList();

            if (nonuserMembers.Count == 0) return new List<UGMemberEntity>();

            var referrerArchetypes = nonuserMembers.Select(m => m.ReferrerArchetype.Value).Distinct().ToList();
            var referrerTokens = nonuserMembers.Select(m => m.ReferrerToken).Distinct().ToList();
            var referrResources = nonuserMembers.Select(m => m.ReferrerResource).Distinct().ToList();

            var existingUgMembers = _ugMemberRepository.GetUGMembers(ownerId: _workContext.CurrentOwnerId, customerIds: customerIds,
                userGroupIds: new List<int> { connectionSourceId },
                userGroupStatus: new List<EntityStatusEnum> { EntityStatusEnum.All },
                ugMemberStatuses: memberStatuses,
                referrerArchetypes: referrerArchetypes, referrerResources: referrResources, referrerTokens: referrerTokens,
                nonuserMemberOnly: true,
                includeUser: false);
            return existingUgMembers;
        }
        private List<UserEntity> GetExistingUserEntities(List<ConnectionMemberDto> validUserConnectionMembers, List<int> customerIds, List<ArchetypeEnum> archetypes)
        {
            var connectionMemberUserIds = validUserConnectionMembers.Where(c => c.UserIdentity.Id > 0)
                .Select(c => (int)c.UserIdentity.Id.Value).ToList();

            var connectionMemberExtIds = validUserConnectionMembers
                .Where(c => (c.UserIdentity.Id == null || c.UserIdentity.Id <= 0) && !string.IsNullOrEmpty(c.UserIdentity.ExtId))
                .Select(c => c.UserIdentity.ExtId).ToList();

            var existingUserEntities = new List<UserEntity>();
            if (connectionMemberUserIds.Count > 0)
            {
                var userEntitiesFoundById = GetUserEntities(_workContext.CurrentOwnerId, customerIds, archetypes, connectionMemberUserIds, null);
                existingUserEntities.AddRange(userEntitiesFoundById);
            }
            if (connectionMemberExtIds.Count > 0)
            {
                var userEntitiesFoundByExtId = GetUserEntities(_workContext.CurrentOwnerId, customerIds, archetypes, null, connectionMemberExtIds);
                existingUserEntities.AddRange(userEntitiesFoundByExtId);
            }
            return existingUserEntities;
        }

        private static UserEntity GetExistingUserEntity(ConnectionMemberDto connectionMemberDto, List<UserEntity> existingUserEntities)
        {
            if (connectionMemberDto.IsUserMember())
            {
                if (connectionMemberDto.UserIdentity.Id > 0)
                {
                    return existingUserEntities.FirstOrDefault(u => u.ArchetypeId == (int)connectionMemberDto.UserIdentity.Archetype
                                                                    && (u.UserId == connectionMemberDto.UserIdentity.Id));
                }
                if (!string.IsNullOrEmpty(connectionMemberDto.UserIdentity.ExtId))
                {
                    return existingUserEntities.FirstOrDefault(u => u.ArchetypeId == (int)connectionMemberDto.UserIdentity.Archetype
                                                                    && string.Equals(u.ExtId, connectionMemberDto.UserIdentity.ExtId, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            return null;
        }

        private static UGMemberEntity GetExistingUgMemberEntity(ConnectionMemberDto connectionMemberDto, List<UGMemberEntity> existingUgMembers,
            bool throwExceptionIfNotExisting, bool allowGettingById)
        {
            if (allowGettingById && connectionMemberDto.Identity != null && connectionMemberDto.Identity.Id > 0)
            {
                //TODO: handle finding UGMember by extId
                return GetExistingUgMemberEntityById(connectionMemberDto, existingUgMembers, throwExceptionIfNotExisting);
            }

            if (connectionMemberDto.IsUserMember())
            {
                if (connectionMemberDto.UserIdentity.Id > 0)
                {
                    return GetExistingUgMemberEntityByUserId(connectionMemberDto, existingUgMembers, throwExceptionIfNotExisting);
                }
                if (!string.IsNullOrEmpty(connectionMemberDto.UserIdentity.ExtId))
                {
                    return GetExistingUgMemberEntityByUserExtId(connectionMemberDto, existingUgMembers, throwExceptionIfNotExisting);
                }
            }
            else
            {

                return GetExistingUgMemberEntityByReferrerInfo(connectionMemberDto, existingUgMembers, throwExceptionIfNotExisting);
            }
            return null;
        }

        private static UGMemberEntity GetExistingUgMemberEntityById(ConnectionMemberDto connectionMemberDto, List<UGMemberEntity> existingUgMembers, bool throwExceptionIfNotExisting)
        {
            var entity = existingUgMembers.FirstOrDefault(u => u.UGMemberId == connectionMemberDto.Identity.Id.Value);
            if (entity == null && throwExceptionIfNotExisting)
            {
                throw new NotFoundException(string.Format("Existing connection member with id '{0}' is not found", connectionMemberDto.Identity.Id));

            }
            return entity;
        }

        private static UGMemberEntity GetExistingUgMemberEntityByReferrerInfo(ConnectionMemberDto connectionMemberDto, List<UGMemberEntity> existingUgMembers, bool throwExceptionIfNotExisting)
        {
            var entity = existingUgMembers.FirstOrDefault(u => DoesMatchReferrerInfo(connectionMemberDto, u)
                                                          && DoesMatchValidDateTime(connectionMemberDto, u)
                                                          && DoesMatchRole(connectionMemberDto, u));
            if (entity == null && throwExceptionIfNotExisting)
            {

                throw new NotFoundException(string.Format("Existing connection member with referrer token '{0}', referrer resource '{1}', referrer archetype '{2}' is not found", connectionMemberDto.ReferrerToken, connectionMemberDto.ReferrerResource, connectionMemberDto.ReferrerArchetype));


            }
            return entity;
        }

        private static UGMemberEntity GetExistingUgMemberEntityByUserExtId(ConnectionMemberDto connectionMemberDto, List<UGMemberEntity> existingUgMembers, bool throwExceptionIfNotExisting)
        {
            var entity = existingUgMembers.FirstOrDefault(u => u.User.ArchetypeId == (int)connectionMemberDto.UserIdentity.Archetype
                                                               && string.Equals(u.User.ExtId, connectionMemberDto.UserIdentity.ExtId, StringComparison.CurrentCultureIgnoreCase)
                                                               && DoesMatchValidDateTime(connectionMemberDto, u)
                                                               && DoesMatchRole(connectionMemberDto, u));
            if (entity == null && throwExceptionIfNotExisting)
            {
                throw new NotFoundException(string.Format("Existing connection member with archetype '{0}', user extId '{1}' is not found", connectionMemberDto.UserIdentity.Archetype, connectionMemberDto.UserIdentity.ExtId));

            }
            return entity;
        }

        private static UGMemberEntity GetExistingUgMemberEntityByUserId(ConnectionMemberDto connectionMemberDto, List<UGMemberEntity> existingUgMembers, bool throwExceptionIfNotExisting)
        {
            var entity = existingUgMembers.FirstOrDefault(u => u.User != null && u.User.ArchetypeId == (int)connectionMemberDto.UserIdentity.Archetype
                                                               && (u.UserId == connectionMemberDto.UserIdentity.Id)
                                                               && DoesMatchValidDateTime(connectionMemberDto, u)
                                                               && DoesMatchRole(connectionMemberDto, u));
            if (entity == null && throwExceptionIfNotExisting)
            {
                throw new NotFoundException(string.Format("Existing connection member with archetype '{0}', user id '{1}' is not found", connectionMemberDto.UserIdentity.Archetype, connectionMemberDto.UserIdentity.Id));

            }
            return entity;
        }


        private static bool DoesMatchReferrerInfo(ConnectionMemberDto connectionMember, UGMemberEntity ugMemberEntity)
        {
            return string.Equals(connectionMember.ReferrerToken, ugMemberEntity.ReferrerToken, StringComparison.CurrentCultureIgnoreCase)
                   && string.Equals(connectionMember.ReferrerResource, ugMemberEntity.ReferrerResource, StringComparison.CurrentCultureIgnoreCase)
                   && (int?)connectionMember.ReferrerArchetype == ugMemberEntity.ReferrerArchetypeId;
        }

        private static bool DoesMatchValidDateTime(ConnectionMemberDto connectionMember, UGMemberEntity ugMemberEntity)
        {
            var existingvalidFrom = ugMemberEntity.validFrom ?? DateTime.MinValue;
            var existingValidTo = ugMemberEntity.ValidTo ?? DateTime.MaxValue;

            var givenvalidFrom = connectionMember.validFrom ?? DateTime.Now;
            var givenValidTo = connectionMember.ValidTo ?? DateTime.Now;

            var givenValidDateIsBetweenExistingValidDate = existingvalidFrom <= givenvalidFrom && givenValidTo <= existingValidTo;
            var existingValidaDateIsBetweenGivenValidDate = givenvalidFrom <= existingvalidFrom && existingValidTo <= givenValidTo;
            return givenValidDateIsBetweenExistingValidDate || existingValidaDateIsBetweenGivenValidDate;

        }

        private static bool DoesMatchRole(ConnectionMemberDto connectionMember, UGMemberEntity ugMemberEntity)
        {
            return (ugMemberEntity.MemberRoleId == null && connectionMember.MemberRoleId == null)
                   || (ugMemberEntity.MemberRoleId == (int?)connectionMember.MemberRoleId);
        }

        private List<UserEntity> GetUserEntities(int ownerId, List<int> customerIds, List<ArchetypeEnum> archetypes, List<int> userIds, List<string> extIds)
        {
            var userEntities = new List<UserEntity>();
            var hasMoreData = true;
            int pageIndex = 1;
            do
            {
                var paginatedUserEntities = _userRepository.GetUsers(ownerId: ownerId, customerIds: customerIds, userIds: userIds,
                    extIds: extIds, archetypeIds: archetypes, pageIndex: pageIndex, includeDepartment: IncludeDepartmentOption.Department);
                hasMoreData = paginatedUserEntities.HasMoreData;
                pageIndex++;
                if (paginatedUserEntities.Items.Count > 0)
                {
                    userEntities.AddRange(paginatedUserEntities.Items);
                }

            } while (hasMoreData);
            return userEntities;
        }

        private ConnectionSourceDto GetConnectionSourceAsUserGroupForUpdating(ConnectionSourceDto connectionSource, List<EntityStatusEnum> entityStatuses)
        {
            var connectionSourceFilterIdentity = GetValidConnectionSourceFilter(connectionSource);
            var groupType = connectionSource.Type.ConvertToGroupType();

            var existingConnectionSource =
                _userGroupRepository.GetUserGroupsWithoutPaging(
                        ownerId: connectionSourceFilterIdentity.OwnerId,
                        customerIds: connectionSourceFilterIdentity.CustomerIds,
                        userGroupIds: connectionSourceFilterIdentity.Ids,
                        userGroupExtIds: connectionSourceFilterIdentity.ExtIds,
                        userGroupArchetypeIds: connectionSourceFilterIdentity.Archetypes,
                        userGroupStatuses: new List<EntityStatusEnum> { EntityStatusEnum.All },
                        userGroupTypeIds: new List<GrouptypeEnum> { groupType })
                    .FirstOrDefault();
            if (existingConnectionSource == null)
            {
                throw new NotFoundException(string.Format("Connection source with type '{0}', archetype '{1}', id '{2}', extId '{3}' is not found.", connectionSource.Type,
                    connectionSource.Identity.Archetype, connectionSource.Identity.Id, connectionSource.Identity.ExtId));

            }
            if (entityStatuses.All(e => (int)e != existingConnectionSource.EntityStatusId))
            {
                throw new InvalidException(string.Format("Connection source with type '{0}', archetype '{1}', id '{2}', extId '{3}' is not allowed to update due to entity status '{4}'.", connectionSource.Type,
                    connectionSource.Identity.Archetype, connectionSource.Identity.Id, connectionSource.Identity.ExtId, (EntityStatusEnum?)existingConnectionSource.EntityStatusId));

            }
            return ConnectionBuilder.GenerateConnectionSourceDto(existingConnectionSource);
        }

        private ConnectionSourceDto GetOrInsertConnectionSourceAsUserGroup(ConnectionSourceDto connectionSource, IdentityDto updateIdentity, bool throwExceptionIfExisting)
        {
            var connectionSourceFilterIdentity = GetValidConnectionSourceFilter(connectionSource);
            var groupType = connectionSource.Type.ConvertToGroupType();

            var existingConnectionSources =
                _userGroupRepository.GetUserGroupsWithoutPaging(
                    ownerId: connectionSourceFilterIdentity.OwnerId,
                    customerIds: connectionSourceFilterIdentity.CustomerIds,
                    userGroupIds: connectionSourceFilterIdentity.Ids,
                    userGroupExtIds: connectionSourceFilterIdentity.ExtIds,
                    userGroupArchetypeIds: connectionSourceFilterIdentity.Archetypes,
                    userGroupStatuses: new List<EntityStatusEnum> { EntityStatusEnum.All });


            var existingConnectionSourceMatchWithType = existingConnectionSources
                .FirstOrDefault(u => u.UserGroupTypeId == (int)groupType);

            if (existingConnectionSourceMatchWithType == null)
            {
                ValidateConnectionSourceWhenInserting(connectionSource, existingConnectionSources);

                var existingParentIdentity = GetConnectionSourceParentIdentity(connectionSource.Parent);
                connectionSource.Parent = existingParentIdentity;

                var userGroupForInserting = ConnectionHelper.GenerateConnectionSourceAsUserGroup(groupType, connectionSource, updateIdentity, _workContext.CurrentUserId);
                userGroupForInserting.Identity.Id = null;

                var hierarchyDepartmentValidation = GetUserGroupHierarchyDepartmentValidation(existingParentIdentity);

                var userGroupService = _userGroupService(connectionSource.Identity.Archetype);

                var insertedUserGroup = (UserGroupDtoBase)userGroupService.InsertUserGroup(hierarchyDepartmentValidation, userGroupForInserting);

                return ConnectionBuilder.GenerateConnectionSourceDto(insertedUserGroup, existingParentIdentity);

            }
            else
            {
                if (throwExceptionIfExisting)
                {
                    throw new ConflictException(string.Format("Connection source with type '{0}', archetype '{1}', id '{2}', extId '{3}' already exists with status '{4}'",
                        ((GrouptypeEnum)(existingConnectionSourceMatchWithType.UserGroupTypeId ?? 0)).ConvertToConnectionType(),
                        (ArchetypeEnum)(existingConnectionSourceMatchWithType.ArchetypeId ?? 0),
                        existingConnectionSourceMatchWithType.UserGroupId, connectionSource.Identity.ExtId, existingConnectionSourceMatchWithType.EntityStatusId));
                }
                return ConnectionBuilder.GenerateConnectionSourceDto(existingConnectionSourceMatchWithType);
            }
        }

        private void ValidateConnectionSourceWhenInserting(ConnectionSourceDto connectionSource, List<UserGroupEntity> existingConnectionSources)
        {
            if (connectionSource.EntityStatus != null && IsNotAllowedStatusToEdit(connectionSource.EntityStatus.StatusId))
            {
                throw new InvalidException(string.Format("Connection source (type '{0}', archetype '{1}', id '{2}', extId '{3}') is not allowed to inserted with status {4}",
                    connectionSource.Type, connectionSource.Identity.Archetype, connectionSource.Identity.Id, connectionSource.Identity.ExtId, connectionSource.EntityStatus.StatusId));
            }

            if (connectionSource.Parent == null)
            {
                throw new InvalidException("Missing parent identity of connection source");
            }
            var existingConnectionSourceWithOtherType = string.IsNullOrEmpty(connectionSource.Identity.ExtId) ? null : existingConnectionSources
                .FirstOrDefault(s => string.Equals(s.ExtId, connectionSource.Identity.ExtId, StringComparison.CurrentCultureIgnoreCase));

            if (existingConnectionSourceWithOtherType != null)
            {
                throw new ConflictException(string.Format("ExtId '{0}' is already used by connection source with type '{1}' (archetype '{2}', id '{3}')",
                    connectionSource.Identity.ExtId,
                    ((GrouptypeEnum)(existingConnectionSourceWithOtherType.UserGroupTypeId ?? 0)).ConvertToConnectionType(),
                    (ArchetypeEnum)(existingConnectionSourceWithOtherType.ArchetypeId ?? 0), existingConnectionSourceWithOtherType.UserGroupId));
            }
        }

        private static HierarchyDepartmentValidationSpecification GetUserGroupHierarchyDepartmentValidation(IdentityDto parentIdentity)
        {
            return parentIdentity.Archetype.IsDepartmentArchetype()
                ? (new HierarchyDepartmentValidationBuilder())
                      .ValidateDepartment((int)parentIdentity.Id.Value, parentIdentity.Archetype)
                      .WithStatus(EntityStatusEnum.All)
                      .IsDirectParent()
                      .Create()
                 : null;
        }

        private IdentityDto GetConnectionSourceParentIdentity(IdentityDto parentIdentity)
        {
            var parentFilterIdentity = GetValidConnectionSourceParentFilter(parentIdentity);

            bool isSupporArchetype = false;
            IdentityDto existingParentIdentity = null;
            if (parentIdentity.Archetype.IsDepartmentArchetype())
            {
                isSupporArchetype = true;
                existingParentIdentity = GetDepartmentIdenity(parentFilterIdentity);

            }
            else if (parentIdentity.Archetype.IsUserArchetype())
            {
                isSupporArchetype = true;
                existingParentIdentity = GetUserIdenity(parentFilterIdentity);
            }

            if (existingParentIdentity != null)
                return existingParentIdentity;

            if (isSupporArchetype)
            {
                throw new NotFoundException(String.Format("Connection source parent with archetype '{0}', id '{1}', extId '{2}' is not found.", parentIdentity.Archetype,
                   parentIdentity.Id, parentIdentity.ExtId));
            }
            throw new InvalidException(string.Format("Connection source parent with archetype '{0}' is unsupported to handle connection. Department or User archetype is expected.", parentIdentity.Archetype));



        }

        private IdentityDto GetUserIdenity(DomainFilterIdentity filterIdentity)
        {
            var userEntity = _userRepository.GetUsers(ownerId: filterIdentity.OwnerId, customerIds: filterIdentity.CustomerIds,
                 userIds: filterIdentity.Ids, extIds: filterIdentity.ExtIds, archetypeIds: filterIdentity.Archetypes,
                 statusIds: null)
             .Items.FirstOrDefault();
            if (userEntity != null)
            {
                return new IdentityDto()
                {
                    OwnerId = userEntity.OwnerId,
                    CustomerId = userEntity.CustomerId ?? 0,
                    Archetype = (ArchetypeEnum)(userEntity.ArchetypeId ?? 0),
                    ExtId = userEntity.ExtId,
                    Id = userEntity.UserId,
                };

            }

            return null;
        }

        private IdentityDto GetDepartmentIdenity(DomainFilterIdentity filterIdentity)
        {
            var departmentEntity = _departmentRepository.GetDepartments(ownerId: filterIdentity.OwnerId, customerIds: filterIdentity.CustomerIds,
                    departmentIds: filterIdentity.Ids, extIds: filterIdentity.ExtIds, archetypeIds: filterIdentity.Archetypes.ToInts(),
                    userIds: null, statusIds: null, parentDepartmentId: 0, childrenDepartmentId: 0, departmetTypeExtIds: null)
                .Items.FirstOrDefault();
            if (departmentEntity != null)
            {
                return new IdentityDto()
                {
                    OwnerId = departmentEntity.OwnerId,
                    CustomerId = departmentEntity.CustomerId ?? 0,
                    Archetype = (ArchetypeEnum)(departmentEntity.ArchetypeId ?? 0),
                    ExtId = departmentEntity.ExtId,
                    Id = departmentEntity.DepartmentId,
                };

            }

            return null;
        }

        private IdentityDto GetUpdatedByIdentity(IdentityDto updatedByIdentity)
        {
            //Update by might come from another system, we need to keep these info for writting event
            if (updatedByIdentity == null || updatedByIdentity.OwnerId != _workContext.CurrentOwnerId) return updatedByIdentity;

            if (updatedByIdentity.Archetype.IsUserArchetype())
            {
                var identityFilter = DomainFilterIdentity.CreateFrom(updatedByIdentity);
                var userEntity = _userRepository.GetUsers(ownerId: identityFilter.OwnerId, customerIds: identityFilter.CustomerIds,
                    userIds: identityFilter.Ids, extIds: identityFilter.ExtIds, archetypeIds: identityFilter.Archetypes,
                    statusIds: null).Items.FirstOrDefault();
                if (userEntity != null)
                {
                    return new IdentityDto()
                    {
                        OwnerId = updatedByIdentity.OwnerId,
                        CustomerId = updatedByIdentity.CustomerId,
                        Archetype = updatedByIdentity.Archetype,
                        ExtId = userEntity.ExtId,
                        Id = userEntity.UserId,
                    };
                }
            }
            return null;
        }


        private DomainFilterIdentity GetValidConnectionSourceParentFilter(IdentityDto sourceParent)
        {

            if (sourceParent.OwnerId != _workContext.CurrentOwnerId
                && sourceParent.CustomerId != _workContext.CurrentCustomerId)
            {
                throw new InvalidException("Connection source parent is invalid owner or customer");
            }
            var parentFilterIdentity = DomainFilterIdentity.CreateFrom(sourceParent);

            if (!parentFilterIdentity.CanIdentify())
            {
                throw new InvalidException("Connection source parent is missing when creating connection source");
            }
            return parentFilterIdentity;
        }

        private DomainFilterIdentity GetValidConnectionSourceFilter(ConnectionSourceDto connectionSource)
        {
            //We only handle connection source for usergroup  archetype for now

            if (connectionSource.Identity.OwnerId != _workContext.CurrentOwnerId
                && connectionSource.Identity.CustomerId != _workContext.CurrentCustomerId)
            {
                throw new InvalidException("Connection source is invalid owner or customer");
            }

            var connectionSourceFilterIdentity = DomainFilterIdentity.CreateFrom(connectionSource.Identity);

            if (!connectionSourceFilterIdentity.CanIdentify())
            {
                throw new InvalidException("Connection source is missing identity information to find object.");
            }
            return connectionSourceFilterIdentity;
        }

        private List<UGMemberEntity> GetUgMemberEntities(int ownerId, List<int> customerIds, List<ArchetypeEnum> memberArchetypes, List<int> memberIds, List<string> memberExtIds, List<EntityStatusEnum> memberStatuses, List<string> memberReferrerTokens, List<string> memberReferrerResources, List<ArchetypeEnum> memberReferrerArchetypes, List<AgeRange> memberAgeRanges, List<Gender> memberGenders, List<UserGroupEntity> userGroups, DateTime? validFromBefore, DateTime? validFromAfter, DateTime? validToBefore, DateTime? validToAfter, bool filterOnMember, bool includeMember, bool countOnMember, out Dictionary<int, int> countMemberGroupByUserGroup)
        {
            List<UGMemberEntity> ugMemberEntities = null;
            countMemberGroupByUserGroup = null;
            var userGroupIds = userGroups.Select(u => u.UserGroupId).ToList();

            //We already to filter by usergroupId, then we need to accept all user group statues
            var userGroupStatuses = new List<EntityStatusEnum> { EntityStatusEnum.All };
            if (includeMember)
            {
                ugMemberEntities = _ugMemberRepository
                    .GetUGMembers(ownerId: ownerId,
                        customerIds: customerIds,
                        userGroupIds: userGroupIds,
                        userGroupStatus: userGroupStatuses,
                        ugMemberStatuses: memberStatuses,
                        userIds: memberIds,
                        userExtIds: memberExtIds,
                        userArchetypes: memberArchetypes,
                        referrerResources: memberReferrerResources,
                        referrerTokens: memberReferrerTokens,
                        referrerArchetypes: memberReferrerArchetypes,
                        memberGenders: memberGenders,
                        memberAgeRanges: memberAgeRanges,
                        validFromAfter: validFromAfter,
                        validFromBefore: validFromBefore,
                        validToAfter: validToAfter,
                        validToBefore: validToBefore,
                        includeUser: true);
            }
            //If member is not included, we need to get number of member when consumer need to count or filter on member
            else if (countOnMember || filterOnMember)
            {
                countMemberGroupByUserGroup = _ugMemberRepository.CountMemberGroupByUserGroup(ownerId: ownerId,
                    customerIds: customerIds,
                    userGroupIds: userGroupIds,
                    userGroupStatus: userGroupStatuses,
                    ugMemberStatus: memberStatuses,
                    userIds: memberIds,
                    userExtIds: memberExtIds,
                    userArchetypes: memberArchetypes,
                    referrerResources: memberReferrerResources,
                    referrerTokens: memberReferrerTokens,
                    referrerArchetypes: memberReferrerArchetypes,
                    memberGenders: memberGenders,
                    memberAgeRanges: memberAgeRanges,
                    validFromAfter: validFromAfter,
                    validFromBefore: validFromBefore,
                    validToAfter: validToAfter,
                    validToBefore: validToBefore);
            }
            return ugMemberEntities;
        }

        public List<ConnectionMemberDto> GetLatestConnectionMembers(int ownerId = 0,
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
            string memberSearchKey = null)

        {

            SetDefaultGettingStatusesIfNullOrEmpty(ref sourceStatuses, ref memberStatuses);

            var filterOnValidTime = HasAnyData(validFromAfter, validFromBefore, validToAfter, validToBefore);
            if (!filterOnValidTime && _connectionConfig.GetMemberInGetTimeAsDefault)
            {
                validFromBefore = DateTime.Now;
                validToAfter = DateTime.Now;
            }

            const int defaultItems = 100000;
            //We cannot distinct by user on UGMember in repository, we need to get from repository then process distinct here.

            var finalUgMemberEntities = new List<UGMemberEntity>();
            var needMoreData = false;

            var expectedPageSize = topItems <= 0 ? defaultItems : topItems;
            var proccessingPageSize = expectedPageSize;
            var proccessingPageSizePageIndex = 1;

            //In case we need to distinct by user, the page size we pass to repository should be greater than page size which give by client to reduce calling to repository
            var coefficient = distinctByUser ? 1.1 : 1;
            var userGroupTypes = connectionTypes.ConvertToGroupTypes();
            Dictionary<GrouptypeEnum, List<int>> sourceIdsGroupByUserGroupType = null;
            if (sourceIdsGroupByConnectionType != null)
            {
                sourceIdsGroupByUserGroupType = new Dictionary<GrouptypeEnum, List<int>>();
                foreach (var sourceIdGroup in sourceIdsGroupByConnectionType)
                {
                    sourceIdsGroupByUserGroupType.Add(sourceIdGroup.Key.ConvertToGroupType(), sourceIdGroup.Value);

                }
            }
            do
            {
                var pageSizeToRepository = (int)(proccessingPageSize * coefficient);

                var paginatedMemberEntities = _ugMemberRepository
                    .GetPaginatedUGMembers(ownerId: ownerId,
                        customerIds: customerIds,
                        userGroupArchetypeIds: sourceArchetypes,
                        userGroupExtIds: sourceExtIds,
                        userGroupReferrerTokens: sourceReferrerTokens,
                        userGroupReferrerResources: sourceReferrerResources,
                        userGroupReferrerArchetypes: sourceReferrerArchetypes,
                        userGroupTypeIds: userGroupTypes,
                        userGroupStatuses: sourceStatuses,
                        userIds: memberIds,
                        userGroupIds: sourceIds,
                        ugMemberStatus: memberStatuses,
                        userExtIds: memberExtIds,
                        userArchetypes: memberArchetypes,
                        referrerTokens: memberReferrerTokens,
                        referrerResources: memberReferrerResources,
                        referrerArchetypes: memberReferrerArchetypes,
                        userGroupIdsGroupsByType: sourceIdsGroupByUserGroupType,
                        memberAgeRanges: memberAgeRanges,
                        memberGenders: memberGenders,
                        validFromBefore: validFromBefore,
                        validFromAfter: validFromAfter,
                        validToBefore: validToBefore,
                        validToAfter: validToAfter,
                        includeUser: true,
                        pageIndex: proccessingPageSizePageIndex,
                        pageSize: pageSizeToRepository,
                        orderBy: "Created Desc",
                        userSearchKey: memberSearchKey);


                var paginatedMembers = paginatedMemberEntities.Items;

                if (distinctByUser)
                {
                    if (finalUgMemberEntities.Count == 0)
                    {
                        paginatedMembers = paginatedMembers.DistinctBy(i => i.UserId).Take(proccessingPageSize).ToList();
                    }
                    else
                    {
                        //If final list 'ugmemberEntities' has element, only get member that has not been exist in this list.
                        paginatedMembers = paginatedMembers
                            .Where(newMember => finalUgMemberEntities.Any(existMember => existMember.UserId == newMember.UserId))
                            .DistinctBy(i => i.UserId).ToList();
                    }
                }


                finalUgMemberEntities.AddRange(paginatedMembers);

                if (finalUgMemberEntities.Count < expectedPageSize && paginatedMemberEntities.HasMoreData)
                {
                    proccessingPageSize = expectedPageSize - finalUgMemberEntities.Count; //remaining expected item count
                    proccessingPageSizePageIndex++;
                    needMoreData = true;
                }
                else
                {
                    needMoreData = false;
                }


            } while (needMoreData);

            return finalUgMemberEntities.Select(i => ConnectionBuilder.GenerateConnectionMemberDto(i, _workContext.CurrentOwnerId)).ToList();

        }
        private bool HasAnyData(params object[] ugMemberFilterCheckingArray)
        {
            for (int i = 0; i < ugMemberFilterCheckingArray.Length; i++)
            {
                var currentParam = ugMemberFilterCheckingArray[i];
                if (currentParam == null)
                    continue;
                if (currentParam is IList)
                {
                    if (((IList)currentParam).Count != 0)
                        return true;
                }
                if (currentParam is DateTime? && ((DateTime?)currentParam).HasValue)
                {
                    return true;
                }
                if (currentParam is bool && (bool)currentParam == true)
                {
                    return true;
                }
            }
            return false;
        }

        private void ClearRelatedMemmoryCache(int ownerId, int customerId, string referrerToken)
        {
            Task.Factory.StartNew(() =>
            {
                _memoryCacheProvider.Remove(CandidateListBuilder.GeneratePatternForRemovingCache(ownerId, customerId, referrerToken));
            });

        }

        private void ProcessBussinessEventAsync(ConnectionSourceDto connectionSource,
            ConnectionMemberDto connectionMemberDto, IdentityBaseDto departmentIdentity, ConnectionType connectionType,
          List<string> eventTypeNames, IdentityDto lastUpdatedByIdentity)
        {
            var requestContext = new RequestContext(_workContext);
            Task.Factory.StartNew(() =>
            {
                ProcessBussinessEvent(requestContext, connectionSource, connectionMemberDto,
                    departmentIdentity, connectionType, eventTypeNames, lastUpdatedByIdentity);
            });
        }
        private void ProcessBussinessEvent(IRequestContext requestContext, ConnectionSourceDto connectionSource,
            ConnectionMemberDto connectionMemberDto, IdentityBaseDto departmentIdentity, ConnectionType connectionType,
          List<string> eventTypeNames, IdentityDto lastUpdatedByIdentity)
        {
            var memberUserIdentity = connectionMemberDto.UserIdentity;

            if (departmentIdentity == null
                && connectionSource.Parent != null
                && connectionSource.Parent.Archetype.IsDepartmentArchetype())
            {
                departmentIdentity = new IdentityBaseDto()
                {
                    Id = connectionSource.Parent.Id,
                    Archetype = connectionSource.Parent.Archetype
                };
            }

            var referrerInfos = BuildEventReferrerInfos(connectionMemberDto, lastUpdatedByIdentity);

            var bussinessEvents = ConnectionHelper.GenerateBusinessEventDto(requestContext, connectionSource.Identity,
                memberUserIdentity, departmentIdentity, lastUpdatedByIdentity, connectionType, eventTypeNames, referrerInfos);

            foreach (var businessEventDto in bussinessEvents)
            {
                _eventClientService.WriteBusinessEvent(businessEventDto, requestContext);

            }
        }


        private static List<string> BuildEventReferrerInfos(ConnectionMemberDto connectionMemberDto, IdentityDto lastUpdatedByIdentity)
        {
            var referrerInfos = new List<string>();
            if (!string.IsNullOrEmpty(connectionMemberDto.ReferrerToken))
            {
                referrerInfos.Add(string.Format("MemeberReferrerToken={0}", connectionMemberDto.ReferrerToken));
            }
            if (!string.IsNullOrEmpty(connectionMemberDto.ReferrerResource))
            {
                referrerInfos.Add(string.Format("MemeberReferrerResource={0}", connectionMemberDto.ReferrerResource));
            }
            if (connectionMemberDto.ReferrerArchetype != null)
            {
                referrerInfos.Add(string.Format("MemeberReferrerArchetype={0}", connectionMemberDto.ReferrerArchetype));
            }
            if (lastUpdatedByIdentity != null)
            {
                referrerInfos.Add(string.Format("UpdatedBy={0}", lastUpdatedByIdentity.ToReferrerString()));
            }
            return referrerInfos;
        }

        private bool ShouldHideDateOfBirth()
        {
            //We should only hide date of birth when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideDateOfBirth && !string.IsNullOrEmpty(_workContext.Sub);
        }
    }
}