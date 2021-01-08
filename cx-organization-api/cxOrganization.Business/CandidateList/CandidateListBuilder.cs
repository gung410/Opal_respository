using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cxOrganization.Adapter.JobMatch;
using cxOrganization.Adapter.JobMatch.Models;
using cxOrganization.Business.Connection;
using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core.Cache;
using cxPlatform.Core.Caching;

namespace cxOrganization.Business.CandidateList
{
    public static class CandidateListBuilder
    {
        public static List<CandidateJobmatch> BuildCandidateJobmatches(string correlationId, string jobmatchLetters,
            IJobMatchAdapter jobMatchAdapter, ICacheProvider cacheProvider, CandidateListJobmatchConfig candidateListJobmatchConfig, string locale)
        {
            if (string.IsNullOrEmpty(jobmatchLetters)) return new List<CandidateJobmatch>();

            if (candidateListJobmatchConfig.UseJobmatchService)
            {
                return BuildCandidateJobmatchesFromService(correlationId, jobmatchLetters, jobMatchAdapter, cacheProvider, candidateListJobmatchConfig, locale);
            }
            else
            {
                return BuildCandidateJobmatchesInteral(jobmatchLetters);
            }
        }
        private static List<CandidateJobmatch> BuildCandidateJobmatchesFromService(string correlationId, string jobmatchLetters,
            IJobMatchAdapter jobMatchAdapter, ICacheProvider cacheProvider, CandidateListJobmatchConfig candidateListJobmatchConfig, string locale)
        {
            var cacheKey = GenerateCacheKeyForBuildCandidateJobmatchesFromService(jobmatchLetters, candidateListJobmatchConfig, locale);

            List<CandidateJobmatch> candidateJobmatches;
            if (cacheProvider.TryGet(cacheKey, out candidateJobmatches))
            {
                return candidateJobmatches;
            }

            candidateJobmatches = new List<CandidateJobmatch>();

            var jobmatchDtos = jobMatchAdapter.GetJobmatchesFromRiasecLettersByGroupLevel(correlationId,
                  candidateListJobmatchConfig.GroupLevel, jobmatchLetters,
                  new List<Adapter.JobMatch.Models.ClassificationEnum> { candidateListJobmatchConfig.Classification }, candidateListJobmatchConfig.Combination, locale);

            foreach (var jobmatchDto in jobmatchDtos)
            {
                var candidateJobmatch = new CandidateJobmatch
                {
                    Code = jobmatchDto.Code,
                    MatchRiasec = jobmatchDto.Riasec,
                    Riasec= jobmatchLetters,
                    MatchRate = GetMatchRate(jobmatchDto.Score),
                    Score = MapJobmatchScore(jobmatchDto.Score)
                };
                candidateJobmatches.Add(candidateJobmatch);
                
            }

            cacheProvider.Add(cacheKey, candidateJobmatches, TimeSpan.FromSeconds(candidateListJobmatchConfig.CacheJobmatchInSeconds));

            return candidateJobmatches;
        }
        private static JobmatchScoreDto MapJobmatchScore(ScoreDto scoreDto)
        {
            if (scoreDto == null) return null;
            return new JobmatchScoreDto
            {
                Min = scoreDto.Min,
                Max = scoreDto.Max,
                Raw = scoreDto.Raw,
                Scaled = scoreDto.Scaled
            };
        }
        private static MatchRate GetMatchRate(ScoreDto jobmatchScore)
        {
            if (jobmatchScore == null) return MatchRate.NotSet;
            if (jobmatchScore.Raw == jobmatchScore.Max) return MatchRate.Good;
            if (jobmatchScore.Raw > jobmatchScore.Min && jobmatchScore.Raw < jobmatchScore.Max) return MatchRate.Decent;

            return MatchRate.Neutral;
        }
        private static CacheKey GenerateCacheKeyForBuildCandidateJobmatchesFromService(string jobmatchLetters, CandidateListJobmatchConfig candidateListJobmatchConfig, string locale)
        {
            var cacheKeyBuilder = new StringBuilder("BuildCandidateJobmatchesFromService");
            cacheKeyBuilder.AppendFormat("jobmatchLetters={0}", jobmatchLetters);
            cacheKeyBuilder.AppendFormat("locale={0}", locale);
            cacheKeyBuilder.AppendFormat("groupLevel={0}", candidateListJobmatchConfig.GroupLevel);
            cacheKeyBuilder.AppendFormat("classification={0}", candidateListJobmatchConfig.Classification);
            cacheKeyBuilder.AppendFormat("combination={0}", candidateListJobmatchConfig.Combination);

            return new CacheKey(cacheKeyBuilder.ToString());
        }

        private static List<CandidateJobmatch> BuildCandidateJobmatchesInteral(string jobmatchLetters)
        {
            var candidateJobMatchs = new List<CandidateJobmatch>();
            if (string.IsNullOrEmpty(jobmatchLetters)) return candidateJobMatchs;
            var values = Enum.GetValues(typeof(JobFamilyEnum));
            foreach (JobFamilyEnum jobFamilyEnum in values)
            {
                string matchLetter;
                var matchRate = JobFamilyMapping.GetMatchRate(jobmatchLetters, jobFamilyEnum, out matchLetter);
                if (matchRate.Equals(MatchRate.NotSet)) continue;
                candidateJobMatchs.Add(new CandidateJobmatch()
                {
                    Code = ((int)jobFamilyEnum).ToString(),
                    MatchRate = matchRate,
                    Riasec = jobmatchLetters,
                    MatchRiasec = matchLetter,
                    Score = new JobmatchScoreDto
                    {
                        Min = (int)MatchRate.NotSet,
                        Max = (int)MatchRate.Good,
                        Raw = (int)matchRate,
                        Scaled = Math.Round((double)matchRate / (double)MatchRate.Good, 2)
                    }
                });
            }
            return candidateJobMatchs;
        }


     
        public static CandidateListItem BuildCandidateListItem(CandidateListMemberDto candidateListMember, List<ProfileLetters> profileLetters, List<ContentItem> contentItems,
                List<CandidateJobmatch> jobmatches, List<string> jobCodes, bool includeCvCompleteness)
        {
            var totalMatchRate = CandidateListCalculator.CalculateTotalJobmatchRate(jobmatches, jobCodes);
            var connectionMember = candidateListMember.ConnectionMember;
            var candidateListItem = new CandidateListItem()
            {
                Identity = connectionMember.UserIdentity,
                DateOfBirth = connectionMember.DateOfBirth,
                EmailAddress = connectionMember.EmailAddress,
                FirstName = connectionMember.FirstName,
                LastName = connectionMember.LastName,
                MobileNumber = connectionMember.MobileNumber,
                Gender = connectionMember.Gender,
                MobileCountryCode = connectionMember.MobileCountryCode,
                Jobmatches = jobmatches,
                ProfileLetters = profileLetters,
                JobmatchTotalRate = totalMatchRate,
                Connected = connectionMember.Created,
                ContentItems = contentItems
            };
            if (includeCvCompleteness)
                candidateListItem.CvCompleteness = candidateListMember.CvCompleteness;
            return candidateListItem;
        }

        public static Dictionary<long, List<ContentItem>> BuildContentItemsGroupsByUserFromAssessmentProfiles(List<dynamic> assessmentProfiles)
        {
            var contentItemsGroupByUser = new Dictionary<long, List<ContentItem>>();
            var assessmentProfileGroupsByUser = assessmentProfiles.GroupBy(a => a.UserIdentity.Id);
            foreach (var assessmentProfileGroupByUser in assessmentProfileGroupsByUser)
            {
                var contentItems = new List<ContentItem>();
                foreach (var assessmentProfileDto in assessmentProfileGroupByUser)
                {
                    contentItems.Add(GenerateContentItem(assessmentProfileDto));                   

                }
                contentItemsGroupByUser.Add(assessmentProfileGroupByUser.Key, contentItems);
            }
            return contentItemsGroupByUser;
        }
        public static Dictionary<long, List<ProfileLetters>> BuildProfileLettersGroupsByUserFromContentItems(Dictionary<long, List<ContentItem>> contentItemsGroupByUser, Dictionary<int, List<LetterInfo>> letterInfosGroupsByActivity, string letterAlternativeExtId)
        {
            var profileLetterGroupsByUser = new Dictionary<long, List<ProfileLetters>>();
            if (contentItemsGroupByUser == null) return profileLetterGroupsByUser;
            foreach (var contentItemsGroup in contentItemsGroupByUser)
            {
                var profileLettersList = new List<ProfileLetters>();
                foreach (var contentItem in contentItemsGroup.Value)
                {
                    if (contentItem.Values != null)
                    {
                        var activityId = (int)contentItem.Identity.Id;
                        var activityExtId = contentItem.Identity.ExtId;
                        var letterContentValues= contentItem.Values.Where(v => string.Equals(v.AlternativeExtId, letterAlternativeExtId, StringComparison.CurrentCultureIgnoreCase)).ToList();
                        if (letterContentValues.Count > 0)
                        {
                            var letterInfosInActivity = letterInfosGroupsByActivity.ContainsKey(activityId)
                                ? letterInfosGroupsByActivity[activityId]
                                : new List<LetterInfo>();

                            foreach (var letterContentValue in letterContentValues)
                            {
                                var profileLetters = new ProfileLetters
                                {
                                    RawLetters = letterContentValue.Value,
                                    Profile = activityExtId,

                                };
                                if (!string.IsNullOrEmpty(profileLetters.RawLetters))
                                {
                                    profileLetters.Letters = BuildProfileLetterItems(profileLetters.RawLetters, letterInfosInActivity);

                                }
                                profileLettersList.Add(profileLetters);
                            }

                        }
                    }

                }
                if (profileLettersList.Count > 0)
                {
                    profileLetterGroupsByUser.Add(contentItemsGroup.Key, profileLettersList);
                }

            }
            return profileLetterGroupsByUser;
        }
        private static List<ProfileLetterItem> BuildProfileLetterItems(string letterValue, List<LetterInfo> letterInfos)
        {
            int position = 1;
            var profileLetterItems= new List<ProfileLetterItem>();
            foreach (var letter in letterValue.ToCharArray())
            {
                var profileLetterItem = new ProfileLetterItem
                {
                    Letter = letter.ToString(),
                    PositionNumber = position
                };
                var letterInfo = letterInfos.FirstOrDefault(l => string.Equals(l.Letter, profileLetterItem.Letter, StringComparison.CurrentCultureIgnoreCase));
                if(letterInfo!=null)
                {
                    profileLetterItem.Color = letterInfo.Description;//// color is configured in field description
                    profileLetterItem.DisplayLetter = letterInfo.DisplayLetter;
                }
                profileLetterItems.Add(profileLetterItem);
                position++;
            }
            return profileLetterItems;
        }
        private static ContentItem GenerateContentItem(dynamic assessmentProfile)
        {
            var contentItem = new ContentItem
            {
                Identity = new IdentityDto
                {
                    Id = assessmentProfile.ActivityId,
                    ExtId = assessmentProfile.ActivityExtId,
                    OwnerId = assessmentProfile.Identity.OwnerId,
                    CustomerId = assessmentProfile.Identity.CustomerId,
                    Archetype = ArchetypeEnum.Activity
                },
                AssessmentIdentity = new IdentityDto
                {
                    Id = assessmentProfile.Identity.Id,
                    ExtId = assessmentProfile.Identity.ExtId,
                    OwnerId = assessmentProfile.Identity.OwnerId,
                    CustomerId = assessmentProfile.Identity.CustomerId,
                    Archetype = (ArchetypeEnum)(int)assessmentProfile.Identity.Archetype
                },
                ContentName = assessmentProfile.ActivityName,
                ContentDisplayName = assessmentProfile.ActivityDisplayName,
                StatusTypeId = assessmentProfile.AssessmentStatusId,
                UserIdenity = new IdentityDto
                {
                    Id = assessmentProfile.UserIdentity.Id,
                    ExtId = assessmentProfile.UserIdentity.ExtId,
                    OwnerId = assessmentProfile.Identity.OwnerId,
                    CustomerId = assessmentProfile.Identity.CustomerId,
                    Archetype =(ArchetypeEnum) (int) assessmentProfile.UserIdentity.Archetype
                },
                Values = BuildContentValues(assessmentProfile)

            };


            return contentItem;
        }

        private static List<ContentValue> BuildContentValues(dynamic assessmentProfile)
        {
            var profileAnswers = ((List<dynamic>)assessmentProfile.IncludingAnswers);
            if (profileAnswers != null && profileAnswers.Count > 0)
            {
                List<ContentValue> values = new List<ContentValue>();
                foreach (dynamic profileAnswer in profileAnswers)
                {
                    ContentValue contentValue = new ContentValue
                    {
                        AnswerId = profileAnswer.AnswerId,
                        QuestionId = profileAnswer.QuestionId,
                        AlternativeId = profileAnswer.AlternativeId,
                        AlternativeExtId = profileAnswer.AlternativeExtId,
                        Value = profileAnswer.Value

                    };
                    values.Add(contentValue);
                }
                return values;
            }
            return null;
        }

        public static Dictionary<long, string> BuildJobmatchLettersGroupsByUserFromAssessmentProfiles(List<dynamic> assessmentProfiles, List<string> jobmatchActivityExtIds, string letterAlternativeExtId)
        {
            var jobMatchLetterGroupsByUser = new Dictionary<long, string>();
            var assessmentProfileGroupsByUser = assessmentProfiles
                .Where(a => jobmatchActivityExtIds.Contains((string)a.ActivityExtId, StringComparer.CurrentCultureIgnoreCase) &&
                            a.IncludingAnswers != null && a.IncludingAnswers.Count > 0)
                .GroupBy(a => a.UserIdentity.Id);

            foreach (var assessmentProfileGroupByUser in assessmentProfileGroupsByUser)
            {
                var letterProfile = assessmentProfileGroupByUser
                    .OrderByDescending(a => (DateTime?)a.EndDate)
                    .First();
                var profileAnswers = ((List<dynamic>)letterProfile.IncludingAnswers);
                var letterAnswer = profileAnswers.FirstOrDefault(a => string.Equals((string)a.AlternativeExtId, letterAlternativeExtId, StringComparison.CurrentCultureIgnoreCase));
                if (letterAnswer != null)
                {
                    jobMatchLetterGroupsByUser.Add(assessmentProfileGroupByUser.Key, letterAnswer.Value);

                }
            }
            return jobMatchLetterGroupsByUser;
        }



      

        private static string BuilKeyListStringValues(string key, IList list)
        {
            if (list == null || list.Count == 0) return "";
            var values = new object[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                values[i] = list[i];
            }
            return string.Format("{0}={1}", key, string.Join(",", values));
        }

        private static string BuilKeyListStringValues<T>(string key, List<T> values)
        {
            if (values == null || values.Count == 0) return "";
            return string.Format("{0}={1}", key, string.Join(",", values));
        }

        private static string BuilKeyStringValue(string key, object value)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString())) return "";
            return string.Format("{0}={1}", key, value);
        }

        public static CacheKey BuildCandidateListCachKey(int ownerId, int customerId,
            CandidateListArguments candidateListArguments)
        {
            var keyBuilder = (new StringBuilder())
                .Append(GeneratePatternForRemovingCache(ownerId, customerId, candidateListArguments.ReferrerToken))
                .Append("CandidateList")
                .AppendFormat("ownerId={0}", ownerId)
                .AppendFormat("customerId={0}", customerId);

            var properties = typeof(CandidateListArguments).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(candidateListArguments);
                if (value == null || propertyInfo.Name == "PageSize" || propertyInfo.Name == "PageIndex") continue;

                var list = value as IList;
                keyBuilder.Append(list != null ?
                    BuilKeyListStringValues(propertyInfo.Name, list) :
                    BuilKeyStringValue(propertyInfo.Name, value));
            }


            return new CacheKey(keyBuilder.ToString());
        }

        public static CacheKey BuidGetConnectionMemberCacheKey(int ownerId, int customerId, List<long> connectionSourceIds, string sourceReferrerToken,
            List<Gender> candidateGenders, List<AgeRange> candidateAgeRanges, List<ConnectionType> connectionTypes,
            List<long> candidateIds, List<string> candidateExtIds, Dictionary<ConnectionType, List<long>> otherTypeSourceIds,
            bool includeCvCompeleteness, bool sortedByCvCompleteness, List<CompletenessRange> completenessRanges)
        {
            var cacheKeyBuilder = (new StringBuilder())
                .Append(GeneratePatternForRemovingCache(ownerId, customerId, sourceReferrerToken))
                .Append("GetLatestConnectionMembers")
                .Append(BuilKeyListStringValues("connectionSourceIds", connectionSourceIds))
                .Append(BuilKeyListStringValues("candidateGenders", candidateGenders))
                .Append(BuilKeyListStringValues("candidateAgeRanges", candidateAgeRanges))
                .Append(BuilKeyListStringValues("connectionTypes", connectionTypes))
                .Append(BuilKeyListStringValues("candidateIds", candidateIds))
                .Append(BuilKeyListStringValues("candidateExtIds", candidateExtIds))
                .Append(BuilKeyStringValue("includeCvCompeleteness", includeCvCompeleteness))
                .Append(BuilKeyStringValue("sortedByCvCompleteness", sortedByCvCompleteness))
                .Append(BuilKeyListStringValues("completenessRanges", completenessRanges));

            if (otherTypeSourceIds != null)
            {
                foreach (var sourceIdsGroupByType in otherTypeSourceIds)
                {
                    cacheKeyBuilder.Append(BuilKeyListStringValues(sourceIdsGroupByType.Key.ToString(), sourceIdsGroupByType.Value));
                }
            }

            return new CacheKey(cacheKeyBuilder.ToString());
        }
        public static string GeneratePatternForRemovingCache(int ownerId, int customerId, string referrerToken)
        {
            return string.Format("CandidateList_cxtoken={0}:{1}_reftoken={2}#", ownerId, customerId, referrerToken);
        }

    }
}