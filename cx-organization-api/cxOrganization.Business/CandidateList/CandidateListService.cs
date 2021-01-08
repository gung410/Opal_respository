using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using cxOrganization.Adapter.Assessment;
using cxOrganization.Adapter.JobChannel;
using cxOrganization.Adapter.JobMatch;
using cxOrganization.Business.Connection;
using cxOrganization.Business.Extensions;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Business.CandidateList
{
    public class CandidateListService : ICandidateListService
    {
        private readonly ILogger _logger;

        private readonly IWorkContext _workContext;
        private readonly IConnectionService _connectionService;
        private readonly IAssessmentAdapter _assessmentAdapter;
        private readonly ICacheProvider _memoryCacheProvider;
       private readonly CandidateListConfig _candidateListConfig;
        private readonly IJobChannelAdapter _jobChannelAdapter;
        private readonly IJobMatchAdapter _jobMatchAdapter;
        public CandidateListService(IWorkContext workContext,
            ILoggerFactory loggerFactory,
            IConnectionService connectionService,
            IAssessmentAdapter assessmentAdapter,
            IOptions<CandidateListConfig> candidateListConfigOption,
            ICacheProvider memoryCacheProvider,
            IJobChannelAdapter jobChannelAdapter,
            IJobMatchAdapter jobMatchAdapter)
        {
            _workContext = workContext;
            _connectionService = connectionService;
            _assessmentAdapter = assessmentAdapter;
            _candidateListConfig = candidateListConfigOption.Value;
            _memoryCacheProvider = memoryCacheProvider;
            _jobChannelAdapter = jobChannelAdapter;
            _jobMatchAdapter = jobMatchAdapter;
            _logger = loggerFactory.CreateLogger<CandidateListService>();

        }

        public CandidateListDto GetCandidateList(CandidateListArguments candidateListArguments)
        {
            //TODO: from BtB: should handle return candidate only connection on single type (such as event or job ..)??


            if (string.IsNullOrEmpty(candidateListArguments.ReferrerToken))
                throw new ArgumentNullException("referrerToken");

            var watch = StartWatch();

            var ownerId = _workContext.CurrentOwnerId;
            var customerId = _workContext.CurrentCustomerId;
            //TODO verify owner and customer

            SetDefaultValueForNullArguments(candidateListArguments);

            CandidateListDto paginatedCandidateListDto;
            if (candidateListArguments.SortField != CandidateListSortField.JobMatch &&
                (candidateListArguments.JobCodes == null || candidateListArguments.JobCodes.Count == 0))
            {

                paginatedCandidateListDto = GetPaginatedCandidateListDtoOrganzationFirst(ownerId, customerId, candidateListArguments);
            }
            else
            {

                paginatedCandidateListDto = GetPaginatedCandidateListDtoJobMatchFirst(ownerId, customerId, candidateListArguments);
            }
            LogWatchResult(watch, "Whole process for getting candidate list", false);
            return paginatedCandidateListDto;
        }

        private void SetDefaultValueForNullArguments(CandidateListArguments candidateListArguments)
        {
            if (candidateListArguments.PageIndex == null || candidateListArguments.PageIndex <= 0) candidateListArguments.PageIndex = 1;
            if (candidateListArguments.PageSize == null || candidateListArguments.PageSize <= 0 || candidateListArguments.PageSize > _candidateListConfig.CandidateListDefaultPageSize)
            {
                candidateListArguments.PageSize = _candidateListConfig.CandidateListDefaultPageSize;
            }
            if (candidateListArguments.SortField == null) candidateListArguments.SortField = CandidateListSortField.ConnectedDate;
            if (candidateListArguments.SortOrder == null) candidateListArguments.SortOrder = SortOrder.Descending;

            if (candidateListArguments.SortField == CandidateListSortField.JobMatch && !candidateListArguments.IncludeJobmatch)
            {
                candidateListArguments.SortField = CandidateListSortField.ConnectedDate;
            }
            if (string.IsNullOrEmpty(candidateListArguments.Locale)) candidateListArguments.Locale = _workContext.CurrentLocale;
        }

        private CandidateListDto GetPaginatedCandidateListDtoJobMatchFirst(int ownerId, int customerId,
            CandidateListArguments candidateListArguments)
        {
            var cacheKey = CandidateListBuilder.BuildCandidateListCachKey(ownerId, customerId, candidateListArguments);

            CandidateListDto orginalCandidateListDto;
            _memoryCacheProvider.TryGet(cacheKey, out orginalCandidateListDto);

            if (orginalCandidateListDto == null)
            {
                orginalCandidateListDto = GetOriginalCandidateListDtoJobMatchFirst(ownerId, customerId, candidateListArguments);

                _memoryCacheProvider.Add(cacheKey, orginalCandidateListDto, _candidateListConfig.CacheTimeOut);

                _logger.LogDebug("Candidate list jobmatch first have been built newly.");
            }
            else
            {
                _logger.LogDebug("Candidate list jobmatch first have been retrieved from memory cache.");
            }

            var paginatedCandidateListDto = GeneratePaginatedCandidateListDto(candidateListArguments.PageSize.Value, candidateListArguments.PageIndex.Value, orginalCandidateListDto);
            return paginatedCandidateListDto;
        }

        private CandidateListDto GetOriginalCandidateListDtoJobMatchFirst(int ownerId, int customerId, CandidateListArguments candidateListArguments)
        {

            //Get candidate list jobmatch first: we get candidate and  process jobmatch to build candidate list items first
            //then we filter on jobmatch, sort and paginate 


            var candidateMembersSortedByConnectedDateDesc = GetCandidateListMembersSortedByConnectedDateDesc(ownerId, customerId, candidateListArguments);


            if (candidateMembersSortedByConnectedDateDesc.Count == 0)
            {
                _logger.LogWarning("There is no candidate member found with filtering");
                return GenerateDefaultCandidateList(candidateListArguments.PageSize.Value, candidateListArguments.PageIndex.Value);
            }

            List<string> profileActivityExtIds = candidateListArguments.ProfileActivityExtIds.IsNotNullOrEmpty()
                ? candidateListArguments.ProfileActivityExtIds
                : _candidateListConfig.ProfileActivityExtIds ?? new List<string>();

            List<string> jobmatchActivityExtIds = candidateListArguments.JobmatchActivityExtIds.IsNotNullOrEmpty()
            ? candidateListArguments.JobmatchActivityExtIds
            : (_candidateListConfig.JobmatchActivityExtIds ?? new List<string>());

            var candidateListItems = GetCandidateListItemsJobMatchFirst(ownerId, customerId,
                candidateListArguments.IncludeAssessment, candidateListArguments.IncludeJobmatch, candidateListArguments.IncludeCvCompleteness, candidateMembersSortedByConnectedDateDesc,
                candidateListArguments.Locale, candidateListArguments.FallbackLocale, candidateListArguments.JobCodes, profileActivityExtIds, jobmatchActivityExtIds);

            var stopWatch = StartWatch();


            candidateListItems = CandidateListFilter.FilterCandidateListItemsByJobFamilies(candidateListArguments.JobCodes, candidateListArguments.IncludeJobmatch, candidateListItems);

            LogWatchResult(stopWatch, "Filter candidate list by job families", true);

            List<CandidateListItem> sortedCandidateList;
            if (candidateListArguments.SortField == CandidateListSortField.ConnectedDate && candidateListArguments.SortOrder == SortOrder.Descending)
            {
                //Candidate list item has been build same order with connection members which are sorted by connected date descending 
                //Then we don't need to sort again in this case

                sortedCandidateList = candidateListItems;

            }
            else
            {
                sortedCandidateList = CandidateListSorting.SortCandidateListItems(candidateListItems, candidateListArguments.SortField.Value, candidateListArguments.SortOrder.Value);
            }

            LogWatchResult(stopWatch, "Sort candidate list", true);

            var candidateListSummary = CandidateListCalculator.CalculateCandidateListSummary(sortedCandidateList, candidateListArguments.JobCodes);

            LogWatchResult(stopWatch, "Calculate candidate list summary");

            var candidateListDto = new CandidateListDto()
            {
                Items = sortedCandidateList,
                HasMoreItem = false,
                PageIndex = 1,
                PageSize = sortedCandidateList.Count,
                Summary = candidateListSummary
            };
            return candidateListDto;
        }

        private CandidateListDto GetPaginatedCandidateListDtoOrganzationFirst(int ownerId, int customerId, CandidateListArguments candidateListArguments)
        {

            //Get candidate list organization first: we get and sort by candidate info connection info first, 
            //then taking paginated candidate for processing jobmatch

            //We should not cache he CandidateListDto at this way since we process jobmatch for each page

            var filteredLatestCandidateMembers = GetCandidateListMembersSortedByConnectedDateDesc(ownerId, customerId, candidateListArguments);


            if (filteredLatestCandidateMembers.Count == 0)
            {
                _logger.LogWarning("There is no candidate member found with filtering");
                return GenerateDefaultCandidateList(candidateListArguments.PageSize.Value, candidateListArguments.PageIndex.Value);
            }

            List<string> profileActivityExtIds = candidateListArguments.ProfileActivityExtIds.IsNotNullOrEmpty()
              ? candidateListArguments.ProfileActivityExtIds
              : _candidateListConfig.ProfileActivityExtIds ?? new List<string>();

            List<string> jobmatchActivityExtIds = candidateListArguments.JobmatchActivityExtIds.IsNotNullOrEmpty()
            ? candidateListArguments.JobmatchActivityExtIds
            : (_candidateListConfig.JobmatchActivityExtIds ?? new List<string>());

            bool hasMoreData;
            var candidateListItems = GetCandidateListItemsOrganizationFirst(ownerId, customerId, candidateListArguments, filteredLatestCandidateMembers, profileActivityExtIds, jobmatchActivityExtIds, out hasMoreData);
            var candidateListDto = new CandidateListDto()
            {
                Items = candidateListItems,
                HasMoreItem = hasMoreData,
                PageIndex = candidateListArguments.PageIndex.Value,
                PageSize = candidateListArguments.PageSize.Value,
                Summary = new CandidateListSummary { TotalItems = filteredLatestCandidateMembers.Count }
            };
            _logger.LogDebug("Candidate list organization first have been built newly.");
            return candidateListDto;
        }

        private List<CandidateListItem> GetCandidateListItemsOrganizationFirst(int ownerId, int customerId,
            CandidateListArguments candidateListArguments,
            List<CandidateListMemberDto> candidateMembersSortedByConnectedDateDesc, List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds, out bool hasMoreData)
        {
            List<CandidateListMemberDto> candidateMembers;


            //The candidate member have been sorted by connected desc already
            //We only sort for other option
            if (candidateListArguments.SortField == CandidateListSortField.ConnectedDate && candidateListArguments.SortOrder == SortOrder.Descending)
            {
                candidateMembers = candidateMembersSortedByConnectedDateDesc;
            }
            else
            {
                candidateMembers = CandidateListSorting.SortCandidateListMembers(candidateMembersSortedByConnectedDateDesc, candidateListArguments.SortField.Value, candidateListArguments.SortOrder.Value);
            }
            var paginatedCandidateMembers = PaginateItems(candidateMembers, candidateListArguments.PageSize.Value, candidateListArguments.PageIndex.Value, out hasMoreData);
            if (paginatedCandidateMembers.Count == 0)
            {
                return new List<CandidateListItem>();
            }
            var candidateListItems = GetCandidateListItemsWithCandidateInfoOnly(paginatedCandidateMembers, candidateListArguments.IncludeCvCompleteness, profileActivityExtIds, jobmatchActivityExtIds);

            if (candidateListArguments.IncludeAssessment && candidateListArguments.IncludeJobmatch)
            {
                SetProfileLetterAndJobmatchesToCandidateListItems(ownerId, customerId, candidateListItems, candidateListArguments.Locale, candidateListArguments.FallbackLocale, candidateListArguments.JobCodes,
                    profileActivityExtIds, jobmatchActivityExtIds);
            }
            else if (candidateListArguments.IncludeAssessment)
            {
                SetProfileLetterToCandidateListItems(ownerId, customerId, candidateListItems, candidateListArguments.Locale, candidateListArguments.FallbackLocale,
                    profileActivityExtIds, jobmatchActivityExtIds);
            }
            else if (candidateListArguments.IncludeJobmatch)
            {
                SetJobMatchesToCandidateListItems(ownerId, customerId, candidateListItems, candidateListArguments.Locale, candidateListArguments.FallbackLocale, candidateListArguments.JobCodes,
                    profileActivityExtIds, jobmatchActivityExtIds);
            }
            return candidateListItems;
        }


        private Stopwatch StartWatch()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                return Stopwatch.StartNew();
            }
            return null;
        }

        private void LogWatchResult(Stopwatch watch, string action, bool startNew = false)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                watch.Stop();
                var time = watch.ElapsedMilliseconds;
                _logger.LogDebug(string.Format("{0} took '{1}'ms", action, time));
                if (startNew)
                {
                    watch.Reset();
                    watch.Start();
                }
            }
        }

        /// <summary>
        /// Get distinct filtered connection members that are ordered by connected date descending
        /// </summary>
        private List<CandidateListMemberDto> GetCandidateListMembersSortedByConnectedDateDesc(int ownerId, int customerId, CandidateListArguments candidateListArguments)
        {

            List<ConnectionType> connectionTypes;
            Dictionary<ConnectionType, List<long>> otherTypeSourceIds;

            var connectionSourceIds = BuildFilteringConnectionSourceIds(candidateListArguments.CorporateIds,
                candidateListArguments.CvIds, candidateListArguments.JobIds, candidateListArguments.EventIds,
                out connectionTypes, out otherTypeSourceIds);

            var sortedByCvCompleteness = candidateListArguments.SortField == CandidateListSortField.CvCompletenessStatus;
            //We don't need add current datetime into cachekey, since it will make cachekey be different for each call
            //Currently we handle cache in short time, then it will be expired soon, does not affect much with current datetime
            var cachKey = CandidateListBuilder.BuidGetConnectionMemberCacheKey(ownerId, customerId, connectionSourceIds, candidateListArguments.ReferrerToken,
                candidateListArguments.Genders, candidateListArguments.AgeRanges, connectionTypes, candidateListArguments.CandidateIds,
                candidateListArguments.CandidateExtIds, otherTypeSourceIds, candidateListArguments.IncludeCvCompleteness, sortedByCvCompleteness,
                candidateListArguments.CvCompletenessRanges);

            List<CandidateListMemberDto> candidateListMembers = null;
            _memoryCacheProvider.TryGet(cachKey, out candidateListMembers);
            if (candidateListMembers != null) return candidateListMembers;

            //We always need to find corporate connection even there is no given corporateIds
            var connectionMembers = GetTopLatestConnectionMembers(ownerId, customerId,
                connectionTypes, connectionSourceIds, candidateListArguments.ReferrerToken,
                candidateListArguments.Genders, candidateListArguments.AgeRanges,
                candidateListArguments.CandidateIds, candidateListArguments.CandidateExtIds,
                otherTypeSourceIds);
            var filterOnCvCompleteness = candidateListArguments.CvCompletenessRanges != null
                                         && candidateListArguments.CvCompletenessRanges.Count > 0;

            var needCvCompletenessInfo = candidateListArguments.IncludeCvCompleteness
                                         || filterOnCvCompleteness
                                         || sortedByCvCompleteness;


            if (needCvCompletenessInfo && connectionMembers.Count > 0)
            {

                var stopWatch = StartWatch();

                var candidateExtIds = connectionMembers.Select(m => m.UserIdentity.ExtId).ToList();
                var completenessStatuses = _jobChannelAdapter.GetCvCompletenessStatuses(candidateExtIds);

                candidateListMembers = filterOnCvCompleteness ?
                    BuildCandidateListMemberWithFilterOnCompleteness(candidateListArguments.CvCompletenessRanges, completenessStatuses, connectionMembers) :
                    BuildCandidateListMemberWithoutFilterOnCompleteness(connectionMembers, completenessStatuses);

                LogWatchResult(stopWatch, string.Format("Handle mapping CV completeness to candidate member {0} filter", filterOnCvCompleteness ? "with" : "without"));
            }
            else
            {
                candidateListMembers = BuildCandidateListMembersWithoutCompleteness(connectionMembers);
            }

            _memoryCacheProvider.Add(cachKey, candidateListMembers, _candidateListConfig.CacheTimeOut);
            return candidateListMembers;
        }

        private static List<CandidateListMemberDto> BuildCandidateListMembersWithoutCompleteness(List<ConnectionMemberDto> connectionMembers)
        {
            return connectionMembers.Select(c => new CandidateListMemberDto { ConnectionMember = c }).ToList();
        }

        private static List<CandidateListMemberDto> BuildCandidateListMemberWithoutFilterOnCompleteness(List<ConnectionMemberDto> connectionMembers, List<dynamic> completenessStatuses)
        {
            if (completenessStatuses.Count == 0)
                return connectionMembers.Select(c => new CandidateListMemberDto { ConnectionMember = c }).ToList();

            return (from connectionMember in connectionMembers
                    join completenessStatus in completenessStatuses on connectionMember.UserIdentity.ExtId equals (string)completenessStatus.ObjectId
                    into leftJoincompletenessStatuses
                    from leftJoincompletenessStatus in leftJoincompletenessStatuses.DefaultIfEmpty()
                    select new CandidateListMemberDto
                    {
                        ConnectionMember = connectionMember,
                        CvCompleteness = leftJoincompletenessStatus == null ? 0 : (int)leftJoincompletenessStatus.Completeness
                    }).ToList();
        }

        private List<CandidateListMemberDto> BuildCandidateListMemberWithFilterOnCompleteness(List<CompletenessRange> cvCompletenessRanges,
            List<dynamic> completenessStatuses, List<ConnectionMemberDto> connectionMembers)
        {

            completenessStatuses = completenessStatuses.Where(c =>
                cvCompletenessRanges.Any(cvCompletenessRange => MatchCompletenessRange(cvCompletenessRange, (int)c.Completeness))).ToList();


            return (from connectionMember in connectionMembers
                    join completenessStatus in completenessStatuses on connectionMember.UserIdentity.ExtId equals (string)completenessStatus.ObjectId
                    select new CandidateListMemberDto
                    {
                        ConnectionMember = connectionMember,
                        CvCompleteness = (int)completenessStatus.Completeness
                    }).ToList();
        }

        private bool MatchCompletenessRange(CompletenessRange completenessRange, int completeness)
        {
            return (completeness >= completenessRange.MinValue)
                   && (completeness <= completenessRange.MaxValue);
        }


        private static List<long> BuildFilteringConnectionSourceIds(List<long> corporateIds, List<long> cvIds, List<long> jobIds, List<long> eventIds,
            out List<ConnectionType> connectionTypes, out Dictionary<ConnectionType, List<long>> otherTypeSourceIds)
        {
            var filterOnCorporate = corporateIds != null && corporateIds.Count > 0;
            var filterOnCvDrop = cvIds != null && cvIds.Count > 0;
            var filterOnEvent = eventIds != null && eventIds.Count > 0;
            var filterOnJob = jobIds != null && jobIds.Count > 0;

            //With corporate and Cv connection, we filter with logic 'or' (Member in corprate or member in cv connections), then we  union two lists
            List<long> connectionSourceIds = null;
            connectionTypes = new List<ConnectionType>();
            if (!filterOnCorporate && !filterOnCvDrop)
            {
                //If there is no filter connection source id on both corporate and cv, we process on all source id of corporate and cv

                connectionTypes.Add(ConnectionType.Corporate);
                connectionTypes.Add(ConnectionType.CV);
            }
            else
            {
                connectionSourceIds = new List<long>();
                if (filterOnCorporate)
                {
                    connectionTypes.Add(ConnectionType.Corporate);
                    connectionSourceIds.AddRange(corporateIds);
                }
                if (filterOnCvDrop)
                {
                    connectionTypes.Add(ConnectionType.CV);
                    connectionSourceIds.AddRange(cvIds);
                }
            }

            //With corporate and Cv connection, we filter with logic 'And' (member in event and member in job connection), then we separate to lists for each type

            otherTypeSourceIds = null;
            if (filterOnEvent || filterOnJob)
            {
                otherTypeSourceIds = new Dictionary<ConnectionType, List<long>>();
                if (filterOnEvent)
                {
                    otherTypeSourceIds.Add(ConnectionType.Event, eventIds);
                }
                if (filterOnJob)
                {
                    otherTypeSourceIds.Add(ConnectionType.Position, jobIds);
                }
            }
            return connectionSourceIds;
        }


        /// <summary>
        /// Get distinct filtered corporate connection members that are ordered by connected date descending
        /// </summary>

        private List<ConnectionMemberDto> GetTopLatestConnectionMembers(int ownerId, int customerId, List<ConnectionType> connectionTypes,
            List<long> connectionSourceIds, string sourceReferrerToken, List<Gender> candidateGenders, List<AgeRange> candidateAgeRanges,
            List<long> candidateIds, List<string> candidateExtIds, Dictionary<ConnectionType, List<long>> otherTypeSourceIds)
        {

            var stopWatch = StartWatch();


            Dictionary<ConnectionType, List<int>> sourceIdsGroupByConnectionType = null;
            if (otherTypeSourceIds != null)
            {
                sourceIdsGroupByConnectionType = otherTypeSourceIds.ToDictionary(d => d.Key, d => d.Value.Select(v => (int)v).ToList());
            }
            DateTime? memberValidFromBefore = null;
            DateTime? memberValidToAfter = null;
            if (!_candidateListConfig.IncludeExpiredMember)
            {
                //Only get member in valid datetime
                memberValidFromBefore = DateTime.Now;
                memberValidToAfter = DateTime.Now;
            }

            var sourceReferrerTokens = string.IsNullOrEmpty(sourceReferrerToken) ? null : new List<string> { sourceReferrerToken };
            var connectionMembers = _connectionService.GetLatestConnectionMembers(ownerId, customerIds: new List<int> { customerId }, connectionTypes: connectionTypes,
                sourceArchetypes: new List<ArchetypeEnum> { ArchetypeEnum.CandidatePool },
                sourceIds: connectionSourceIds.ToInts(),
                sourceReferrerTokens: sourceReferrerTokens,
                sourceIdsGroupByConnectionType: sourceIdsGroupByConnectionType,
                memberArchetypes: new List<ArchetypeEnum> { ArchetypeEnum.Candidate },
                memberIds: candidateIds.ToInts(),
                memberExtIds: candidateExtIds,
                memberGenders: candidateGenders,
                memberAgeRanges: candidateAgeRanges,
                validFromBefore: memberValidFromBefore,
                validToAfter: memberValidToAfter,
                topItems: _candidateListConfig.CandidateListMaxConnectionMembers,
                memberStatuses: _candidateListConfig.AcceptedMemberStatuses,
                distinctByUser: true
            );
            LogWatchResult(stopWatch, "GetConnectionMembersSortedByConnectedDateDesc from organization api");


            return connectionMembers;
        }

        private List<CandidateListItem> GetCandidateListItemsJobMatchFirst(int ownerId, int customerId, bool includeAssessment, bool includeJobmatch, bool includeCvCompleteness,
            List<CandidateListMemberDto> connectionMembers, string locale, string fallbackLocale, List<string> jobCodes, List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            if (includeAssessment && includeJobmatch)
            {
                return GetCandidateListItemsWithAssessmentAndJobmatch(ownerId, customerId, connectionMembers, locale, fallbackLocale, jobCodes, includeCvCompleteness, profileActivityExtIds, jobmatchActivityExtIds);
            }

            if (includeAssessment)
            {
                return GetCandidateListItemsWithAssessment(ownerId, customerId, connectionMembers, locale, fallbackLocale, includeCvCompleteness, profileActivityExtIds, jobmatchActivityExtIds);
            }

            if (includeJobmatch)
            {
                return GetCandidateListItemsWithJobmatch(ownerId, customerId, connectionMembers, locale, fallbackLocale, jobCodes, includeCvCompleteness, profileActivityExtIds, jobmatchActivityExtIds);
            }

            return GetCandidateListItemsWithCandidateInfoOnly(connectionMembers, includeCvCompleteness, profileActivityExtIds, jobmatchActivityExtIds);
        }


        private List<CandidateListItem> GetCandidateListItemsWithCandidateInfoOnly(List<CandidateListMemberDto> candidateMembers, bool includeCvCompleteness, List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            var watch = StartWatch();
            var candidateListItems = new List<CandidateListItem>();
            foreach (var connectionMember in candidateMembers)
            {
                var candidateListItem = CandidateListBuilder.BuildCandidateListItem(connectionMember, null,null, null, null, includeCvCompleteness);
                candidateListItems.Add(candidateListItem);
            }
            LogWatchResult(watch, "GetCandidateListItemsWithCandidateInfoOnly");
            return candidateListItems;
        }

        private List<CandidateListItem> GetCandidateListItemsWithAssessment(int ownerId, int customerId, List<CandidateListMemberDto> candidateMembers, string locale, string fallbackLocale, bool includeCvCompleteness,
            List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            var watch = StartWatch();

            var candidateIds = candidateMembers.Select(c => (int)c.ConnectionMember.UserIdentity.Id).ToList();
            var lettersGroupsByUserInfo = GetContentInfoGroupByUserInfo(ownerId, customerId, candidateIds, locale, fallbackLocale,
                getProfileLetters: true, getJobmatchLetters: false, profileActivityExtIds: profileActivityExtIds, jobmatchActivityExtIds: jobmatchActivityExtIds);


            var candidateListItems = new List<CandidateListItem>();
            foreach (var candidateMember in candidateMembers)
            {
                var contentItems = GetContentItemssOfUser(lettersGroupsByUserInfo.ContentItems, candidateMember.ConnectionMember.UserIdentity.Id.Value);
                var profileLetters = GetProfileLettersOfUser(lettersGroupsByUserInfo.ProfileLettersGroups, candidateMember.ConnectionMember.UserIdentity.Id.Value);
                var candidateListItem = CandidateListBuilder.BuildCandidateListItem(candidateMember, profileLetters, contentItems, null, null, includeCvCompleteness);
                candidateListItems.Add(candidateListItem);
            }
            LogWatchResult(watch, "GetCandidateListItemsWithAssessment");
            return candidateListItems;
        }

        private List<CandidateListItem> GetCandidateListItemsWithAssessmentAndJobmatch(int ownerId, int customerId, List<CandidateListMemberDto> candidateListMembers, string locale, string fallbackLocale, 
            List<string> jobCodes, bool includeCvCompleteness, List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            var watch = StartWatch();

            var candidateIds = candidateListMembers.Select(c => (int)c.ConnectionMember.UserIdentity.Id).ToList();
            var letterGroupsByUserInfo = GetContentInfoGroupByUserInfo(ownerId, customerId, candidateIds, locale, fallbackLocale, getProfileLetters: true, getJobmatchLetters: true,
                profileActivityExtIds: profileActivityExtIds, jobmatchActivityExtIds: jobmatchActivityExtIds);


            var candidateListItems = new List<CandidateListItem>();
            foreach (var candidateListMember in candidateListMembers)
            {
                var contentItems = GetContentItemssOfUser(letterGroupsByUserInfo.ContentItems, candidateListMember.ConnectionMember.UserIdentity.Id.Value);
                var profileLetters = GetProfileLettersOfUser(letterGroupsByUserInfo.ProfileLettersGroups, candidateListMember.ConnectionMember.UserIdentity.Id.Value);

                var jobmatchLetters = GetJobmatchLettersOfUser(letterGroupsByUserInfo.JobmatchLettersGroups, candidateListMember.ConnectionMember.UserIdentity.Id.Value);
                var candidateJobmatches = BuildCandidateJobmatches(jobmatchLetters, locale);
                var candidateListItem = CandidateListBuilder.BuildCandidateListItem(candidateListMember, profileLetters, contentItems, candidateJobmatches, jobCodes, includeCvCompleteness);
                candidateListItems.Add(candidateListItem);
            }

            LogWatchResult(watch, "GetCandidateListItemsWithAssessmentAndJobmatch");

            return candidateListItems;
        }

        private List<CandidateListItem> GetCandidateListItemsWithJobmatch(int ownerId, int customerId, List<CandidateListMemberDto> candidateListMembers,
            string locale, string fallbackLocale, List<string> jobCodes, bool includeCvCompleteness, List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            var watch = StartWatch();

            var candidateIds = candidateListMembers.Select(c => (int)c.ConnectionMember.UserIdentity.Id).ToList();
            var letterGroupsByUserInfo = GetContentInfoGroupByUserInfo(ownerId, customerId, candidateIds, locale, fallbackLocale, getProfileLetters: false, getJobmatchLetters: true,
                profileActivityExtIds: profileActivityExtIds, jobmatchActivityExtIds: jobmatchActivityExtIds);


            var candidateListItems = new List<CandidateListItem>();
            foreach (var candidateListMember in candidateListMembers)
            {
                var jobmatchLetters = GetJobmatchLettersOfUser(letterGroupsByUserInfo.JobmatchLettersGroups, candidateListMember.ConnectionMember.UserIdentity.Id.Value);
                var candidateJobmatches = BuildCandidateJobmatches(jobmatchLetters, locale);
                var candidateListItem = CandidateListBuilder.BuildCandidateListItem(candidateListMember, null,null, candidateJobmatches, jobCodes, includeCvCompleteness);
                candidateListItems.Add(candidateListItem);
            }

            LogWatchResult(watch, "GetCandidateListItemsWithJobmatch");

            return candidateListItems;
        }


        private void SetProfileLetterToCandidateListItems(int ownerId, int customerId, List<CandidateListItem> candidateListItems, string locale, string fallbackLocale,
            List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            var watch = StartWatch();

            var candidateIds = candidateListItems.Select(c => (int)c.Identity.Id).ToList();
            var lettersGroupsByUserInfo = GetContentInfoGroupByUserInfo(ownerId, customerId, candidateIds, locale, fallbackLocale, getProfileLetters: true, getJobmatchLetters: false,
                profileActivityExtIds: profileActivityExtIds, jobmatchActivityExtIds: jobmatchActivityExtIds);


            foreach (var candidateListItem in candidateListItems)
            {

                var profileLetters = GetProfileLettersOfUser(lettersGroupsByUserInfo.ProfileLettersGroups, candidateListItem.Identity.Id.Value);
                var contentItems = GetContentItemssOfUser(lettersGroupsByUserInfo.ContentItems, candidateListItem.Identity.Id.Value);
                candidateListItem.ProfileLetters = profileLetters;
                candidateListItem.ContentItems = contentItems;
            }
            LogWatchResult(watch, "SetProfileLetterToCandidateListItems");
        }

        private void SetProfileLetterAndJobmatchesToCandidateListItems(int ownerId, int customerId, List<CandidateListItem> candidateListItems, string locale, string fallbackLocale, List<string> jobCodes,
            List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            var watch = StartWatch();

            var candidateIds = candidateListItems.Select(c => (int)c.Identity.Id).ToList();
            var letterGroupsByUserInfo = GetContentInfoGroupByUserInfo(ownerId, customerId, candidateIds, locale, fallbackLocale, getProfileLetters: true, getJobmatchLetters: true,
                profileActivityExtIds: profileActivityExtIds, jobmatchActivityExtIds: jobmatchActivityExtIds);


            foreach (var candidateListItem in candidateListItems)
            {
                var contentItems = GetContentItemssOfUser(letterGroupsByUserInfo.ContentItems, candidateListItem.Identity.Id.Value);
                var profileLetters = GetProfileLettersOfUser(letterGroupsByUserInfo.ProfileLettersGroups, candidateListItem.Identity.Id.Value);
                candidateListItem.ProfileLetters = profileLetters;
                candidateListItem.ContentItems = contentItems;

                var jobmatchLetters = GetJobmatchLettersOfUser(letterGroupsByUserInfo.JobmatchLettersGroups, candidateListItem.Identity.Id.Value);
                var candidateJobmatches = BuildCandidateJobmatches(jobmatchLetters, locale);
                candidateListItem.Jobmatches = candidateJobmatches;

                var totalMatchRate = CandidateListCalculator.CalculateTotalJobmatchRate(candidateJobmatches, jobCodes);
                candidateListItem.JobmatchTotalRate = totalMatchRate;
            }

            LogWatchResult(watch, "SetProfileLetterAndJobmatchesToCandidateListItems");

        }
        private void SetJobMatchesToCandidateListItems(int ownerId, int customerId, List<CandidateListItem> candidateListItems, string locale, string fallbackLocale, List<string> jobCodes,
             List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            var watch = StartWatch();

            var candidateIds = candidateListItems.Select(c => (int)c.Identity.Id).ToList();
            var letterGroupsByUserInfo = GetContentInfoGroupByUserInfo(ownerId, customerId, candidateIds, locale, fallbackLocale, getProfileLetters: false, getJobmatchLetters: true,
                profileActivityExtIds: profileActivityExtIds, jobmatchActivityExtIds: jobmatchActivityExtIds);


            foreach (var candidateListItem in candidateListItems)
            {
                var jobmatchLetters = GetJobmatchLettersOfUser(letterGroupsByUserInfo.JobmatchLettersGroups, candidateListItem.Identity.Id.Value);
                var candidateJobmatches = BuildCandidateJobmatches(jobmatchLetters, locale);
                candidateListItem.Jobmatches = candidateJobmatches;

                var totalMatchRate = CandidateListCalculator.CalculateTotalJobmatchRate(candidateJobmatches, jobCodes);
                candidateListItem.JobmatchTotalRate = totalMatchRate;
            }

            LogWatchResult(watch, "SetJobMatchesToCandidateListItems");

        }

        private List<CandidateJobmatch> BuildCandidateJobmatches(string jobmatchLetters, string locale)
        {
            return CandidateListBuilder.BuildCandidateJobmatches(string.IsNullOrEmpty(_workContext.CorrelationId) ? Guid.NewGuid().ToString() : _workContext.CorrelationId, jobmatchLetters, _jobMatchAdapter,
                _memoryCacheProvider, _candidateListConfig.JobmatchConfig, locale);
        }

        private static List<ProfileLetters> GetProfileLettersOfUser(Dictionary<long, List<ProfileLetters>> profileLetterGroupsByUser, long candidateId)
        {
            List<ProfileLetters> profileLetters = null;
            if (profileLetterGroupsByUser.ContainsKey(candidateId))
            {
                profileLetters = profileLetterGroupsByUser[candidateId];
            }
            return profileLetters;
        }
        private static List<ContentItem> GetContentItemssOfUser(Dictionary<long, List<ContentItem>> profileLetterGroupsByUser, long candidateId)
        {
            List<ContentItem> contentItems = null;
            if (profileLetterGroupsByUser.ContainsKey(candidateId))
            {
                contentItems = profileLetterGroupsByUser[candidateId];
            }
            return contentItems;
        }
        private static string GetJobmatchLettersOfUser(Dictionary<long, string> jobmacthLetterGroupsByUser, long candidateId)
        {
            string jobmatchLetters = null;
            if (jobmacthLetterGroupsByUser.ContainsKey(candidateId))
            {
                jobmatchLetters = jobmacthLetterGroupsByUser[candidateId];
            }
            return jobmatchLetters;
        }

        private CandidateListDto GenerateDefaultCandidateList(int pageSize, int pageIndex)
        {
            return new CandidateListDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = new List<CandidateListItem>(),
                HasMoreItem = false,
                Summary = new CandidateListSummary()

            };
        }

        private CandidateListDto GeneratePaginatedCandidateListDto(int pageSize, int pageIndex, CandidateListDto orginalCandidateListDto)
        {

            bool hasMoreData;
            var paginatedItems = PaginateItems(orginalCandidateListDto.Items, pageSize, pageIndex, out hasMoreData);
            return new CandidateListDto
            {
                Items = paginatedItems,
                PageIndex = pageIndex,
                PageSize = pageSize,
                HasMoreItem = hasMoreData,
                Summary = orginalCandidateListDto.Summary
            };
        }

        private List<T> PaginateItems<T>(List<T> items, int pageSize, int pageIndex, out bool hasMoreData)
        {
            var paginatedItems = items.Skip((pageIndex - 1) * pageSize).Take(pageSize + 1).ToList();

            if (paginatedItems.Count > pageSize)
            {
                hasMoreData = true;
                paginatedItems.RemoveAt(paginatedItems.Count - 1);
            }
            else
            {
                hasMoreData = false;
            }
            return paginatedItems;
        }

        private ContentInfoGroupByUserInfo GetContentInfoGroupByUserInfo(int ownerId, int customerId, List<int> candidateIds, string locale, string fallbackLocale, bool getProfileLetters, bool getJobmatchLetters,
            List<string> profileActivityExtIds, List<string> jobmatchActivityExtIds)
        {
            var watch = StartWatch();
            var activityExtIds = new List<string>();
            if (getProfileLetters && profileActivityExtIds.IsNotNullOrEmpty())
            {
                activityExtIds.AddRange(profileActivityExtIds);
            }
            if (getJobmatchLetters && jobmatchActivityExtIds.IsNotNullOrEmpty())
            {
                activityExtIds.AddRange(jobmatchActivityExtIds);
            }
            activityExtIds = activityExtIds.Distinct().ToList();

            var allAssessmentProfiles = GetLatestAssessmentProfiles(ownerId, customerId, candidateIds, activityExtIds, locale, fallbackLocale);

            var lettersGroupByUserInfo = new ContentInfoGroupByUserInfo();
            if (getProfileLetters)
            {
                var assessmentProfiles = allAssessmentProfiles
                    .Where(a => profileActivityExtIds.Contains((string)a.ActivityExtId, StringComparer.CurrentCultureIgnoreCase))
                    .ToList();
                var activityIds = assessmentProfiles.Select(a => (int)a.ActivityId).Distinct().ToList();
                var letterInfosGroupByActivity = GetLetterInfoGroupsByActivity(ownerId, customerId, activityIds, locale, fallbackLocale);
                lettersGroupByUserInfo.ContentItems = CandidateListBuilder.BuildContentItemsGroupsByUserFromAssessmentProfiles(assessmentProfiles);
                lettersGroupByUserInfo.ProfileLettersGroups = CandidateListBuilder.BuildProfileLettersGroupsByUserFromContentItems(lettersGroupByUserInfo.ContentItems, letterInfosGroupByActivity, _candidateListConfig.LetterAlternativeExtId);

            }

            if (getJobmatchLetters)
            {

                lettersGroupByUserInfo.JobmatchLettersGroups = CandidateListBuilder.BuildJobmatchLettersGroupsByUserFromAssessmentProfiles(allAssessmentProfiles, jobmatchActivityExtIds, _candidateListConfig.LetterAlternativeExtId);
            }

            LogWatchResult(watch, "GetLettersGroupsByUserInfo", false);

            return lettersGroupByUserInfo;


        }


        /// <summary>
        /// Build color letters group by activity
        /// </summary>

        private Dictionary<int, List<LetterInfo>> GetLetterInfoGroupsByActivity(int ownerId, int customerId, List<int> activityIds, string locale, string fallbackLocale)
        {
            var watch = StartWatch();

            var letterColorInActivity = new Dictionary<int, List<LetterInfo>>();
            var levelLimitGroupsByActivity = GetLevelLimitGroupsByActivity(ownerId, customerId, activityIds);
            foreach (var levelLimitGroupByActivity in levelLimitGroupsByActivity)
            {
                var levelLimitsInActivity = levelLimitGroupByActivity.Value;


                var letterInfoGroupsByExtId = (from levelLimit in levelLimitsInActivity
                                               let localizedData = GetLocalizedLevelLimit(levelLimit, locale, fallbackLocale)
                                               where localizedData != null
                                               select new LetterInfo
                                               {
                                                   Letter = levelLimit.Identity.ExtId, //LevelLimit is configured ExtId same with letter
                                                   DisplayLetter = GetName(localizedData),
                                                   Description = GetDescription(localizedData)
                                               }).ToList();


                letterColorInActivity.Add(levelLimitGroupByActivity.Key, letterInfoGroupsByExtId);
            }

            LogWatchResult(watch, "GetLetterInfoGroupsByActivity", false);

            return letterColorInActivity;
        }

        private dynamic GetLocalizedLevelLimit(dynamic levelLimit, string locale, string fallbackLocale)
        {
            List<dynamic> localizedData = ((List<dynamic>)levelLimit.LocalizedData);
            var localizedValue = localizedData.FirstOrDefault(l => string.Equals((string)l.LanguageCode, locale, StringComparison.CurrentCulture));
            if (localizedValue == null)
                return localizedData.FirstOrDefault(l => string.Equals((string)l.LanguageCode, fallbackLocale, StringComparison.CurrentCulture));
            return localizedValue;
        }
        private string GetName(dynamic localizedData)
        {
            return GetLocalizedField(localizedData, "Name");


        }
        private string GetDescription(dynamic localizedData)
        {
            return GetLocalizedField(localizedData, "Description");

        }
        private string GetLocalizedField(dynamic localizedData, string fieldName)
        {
            var localizedField = ((List<dynamic>)localizedData.Fields).FirstOrDefault(f => f.Name == fieldName);
            if (localizedField != null) return (string)localizedField.LocalizedText;
            return string.Empty;

        }
        private Dictionary<int, List<dynamic>> GetLevelLimitGroupsByActivity(int ownerId, int customerId, List<int> activityIds)
        {
            var watch = StartWatch();

            var levelLimitGroupsByActivity = new Dictionary<int, List<dynamic>>();
            var levelGroups = _assessmentAdapter.GetLevelGroups(ownerId, customerId: customerId, activityIds: activityIds, tags: new List<string> { _candidateListConfig.ColorLevelGroupTag });
            if (levelGroups.Count > 0)
            {
                var levelGroupIds = levelGroups.Select(l => (int)l.Identity.Id).ToList();
                var levelLimits = _assessmentAdapter.GetLevelLimits(ownerId, customerId, levelGroupIds: levelGroupIds, includeLocalizedData: true);
                var levelGroupGroupsByActivity = levelGroups.GroupBy(g => g.ActivityId);
                foreach (var levelGroupGroup in levelGroupGroupsByActivity)
                {
                    var levelLimitsOfActivity = levelLimits.Where(l => levelGroupGroup.Any(lg => lg.Identity.Id == l.LevelGroupId)).ToList();
                    levelLimitGroupsByActivity.Add(levelGroupGroup.Key, levelLimitsOfActivity);
                }
            }
            LogWatchResult(watch, "GetLevelLimitGroupsByActivity", false);

            return levelLimitGroupsByActivity;
        }
        
        private List<dynamic> GetLatestAssessmentProfiles(int ownerId, int customerId, List<int> candidateIds, List<string> activityExtIds, string locale, string fallbackLocale)
        {
            var watch = StartWatch();
            if (candidateIds.IsNullOrEmpty() || activityExtIds.IsNullOrEmpty())
            {
                //Prevent consumer send not filter when calling assessement api, this will harm to domain layer
                return new List<dynamic>();
            }
            var includingAnswerAlternativeExtIds =
                (new List<string>(_candidateListConfig.ProfileAlternativeExtIds)
            { _candidateListConfig.LetterAlternativeExtId }).Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList();

            var assessmentProfileDtos = _assessmentAdapter.GetAssessmentProfiles(
               ownerId:  ownerId,
                customerId: customerId,
                userIds: candidateIds,
                actvityExtIds: activityExtIds,
                alternativeExtIdsToIncludeAnswer: includingAnswerAlternativeExtIds,
                statustypeIds: _candidateListConfig.CandidateListProfileStatusTypeIds,
                displayLocale: locale,
                fallbackDisplayLocale:fallbackLocale,
                answerLocale: _candidateListConfig.RiasecLanguageCode, //The language code for getting letter (this language might be different from given local and defautl language
                defaultCurrentSurveyIfNoFiltering: false,
                getLatestResultOnActivityOfUserOnly: true);

            LogWatchResult(watch, string.Format("GetAssessmentProfiles {0} profiles of {1} candidates on {2} activities",
                assessmentProfileDtos.Count, candidateIds.Count, activityExtIds.Count), false);

            return assessmentProfileDtos;



        }



        private class ContentInfoGroupByUserInfo
        {
            public Dictionary<long, List<ProfileLetters>> ProfileLettersGroups { get; set; }
            public Dictionary<long, string> JobmatchLettersGroups { get; set; }
            public Dictionary<long, List<ContentItem>> ContentItems { get; set; }

        }
    }

}