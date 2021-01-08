using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Assessment.Data.Repositories;
using cxOrganization.Adapter.Shared.Entity;
using cxOrganization.Adapter.Shared.Extensions;
using cxPlatform.Core.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cxOrganization.Adapter.Assessment
{
    public class AssessmentAdapter : IAssessmentAdapter
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IResultRepository _resultRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IAlternativeRepository _alternativeRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IStatusTypeRepository _statusTypeRepository;
        private readonly IActivityStatusTypeRepository _activityStatusTypeRepository;
        private readonly ILevelGroupRepository _levelgroupRepository;
        private readonly ILevelLimitRepository _levelLimitRepository;
        public AssessmentAdapter(
            IActivityRepository activityRepository, 
            ISurveyRepository surveyRepository,
            IResultRepository resultRepository,
            IAnswerRepository answerRepository,
            IAlternativeRepository alternativeRepository,
            ILanguageRepository languageRepository, 
            IStatusTypeRepository statusTypeRepository,
            IActivityStatusTypeRepository activityStatusTypeRepository,
            ILevelGroupRepository levelgroupRepository,
            ILevelLimitRepository levelLimitRepository)
        {
            _activityRepository = activityRepository;
            _surveyRepository = surveyRepository;
            _resultRepository = resultRepository;
            _answerRepository = answerRepository;
            _alternativeRepository = alternativeRepository;
            _languageRepository = languageRepository;
            _statusTypeRepository = statusTypeRepository;
            _activityStatusTypeRepository = activityStatusTypeRepository;
            _levelgroupRepository = levelgroupRepository;
            _levelLimitRepository = levelLimitRepository;
        }
        public List<dynamic> GetAssessmentConfigurations(int ownerId, List<int> activityIds=null, List<string> activityExtIds=null, List<int> surveyIds = null,
            DateTime? createdAfter = null, DateTime? createdBefore = null, bool includeStatusType=false)
        {
            var activities = _activityRepository.GetActivities(ownerId: ownerId,
                activityIds: activityIds,
                extIds: activityExtIds,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                surveyIds: surveyIds
               );
            if (activities.Count > 0)
            {
                return includeStatusType ?
                    BuildAssessmentConfigurationsWithStatusType(activities) : 
                    BuildAssessmentConfigurationsWithoutStatusType(activities);
            }
            return new List<dynamic>(); 
        }
        

        public List<dynamic> GetLevelGroups(int ownerId, int customerId, List<int> activityIds = null,
            List<int> levelgroupIds = null,
            List<int> departmentIds = null,
            List<int> roleIds = null,
            List<string> tags = null,
            bool includeLocalizedData = false)
        {
            var levelGroupEntities = _levelgroupRepository.GetLevelGroups(
                levelgroupIds: levelgroupIds,
                activityIds: activityIds,
                customerIds: customerId > 0 ? new List<int> {customerId} : null,
                departmentIds: departmentIds,
                roleIds: roleIds,
                tags: tags,
                includeLocalizedData: includeLocalizedData);
            if (levelGroupEntities.Count == 0) return new List<dynamic>();
            if (includeLocalizedData)
            {
                
                var listLanguage = _languageRepository.GetLanguages();
                var levelGroups = new List<dynamic>();
                foreach (var levelGroupEntity in levelGroupEntities)
                {
                    var levelgroup = AssessmentDataHelper.GenerateLevelGroup(ownerId, levelGroupEntity);
                    levelgroup.LocalizedData = AssessmentDataHelper.GetLocalizedData(
                        levelGroupEntity.LT_LevelGroups,
                        listLanguage, l => l.LevelGroupId, l => l.LanguageId);
                    levelGroups.Add(levelgroup);
                }
                return levelGroups;
            }
            else
            {
                return levelGroupEntities.Select(lg => AssessmentDataHelper.GenerateLevelGroup(ownerId, lg)).ToList();
            }

        }

        public List<dynamic> GetLevelLimits(int ownerId, int customerId, List<int> activityIds = null,
            List<int> levelLimitIds = null,
            List<int> levelGroupIds = null,
            List<int> categoryIds = null,
            List<int> questionIds = null,
            List<int> itemIds = null,
            List<string> levelGroupTags = null,
            bool includeLocalizedData = false)
        {
            var levelLimitEntities = _levelLimitRepository.GetLevelLimits(
                                 activityIds: activityIds,
                                 levelLimitIds: levelLimitIds,
                                 levelGroupIds: levelGroupIds,
                                 categoryIds: categoryIds,
                                 itemIds: itemIds,
                                 questionIds: questionIds,
                                 levelGroupTags: levelGroupTags,
                                 includeLocalizedData: includeLocalizedData);
            if (includeLocalizedData)
            {
                var listLanguage = _languageRepository.GetLanguages();
                var levelGroups = new List<dynamic>();
                foreach (var levelLimitEntity in levelLimitEntities)
                {
                    var levelgroup = AssessmentDataHelper.GenerateLevelLimit(ownerId, customerId, levelLimitEntity);
                    levelgroup.LocalizedData = AssessmentDataHelper.GetLocalizedData(
                        levelLimitEntity.LT_LevelLimits,
                        listLanguage, l => l.LevelLimitId, l => l.LanguageId);
                    levelGroups.Add(levelgroup);
                }
                return levelGroups;
            }
            else
            {
                return levelLimitEntities.Select(ll => AssessmentDataHelper.GenerateLevelLimit(ownerId, customerId, ll)).ToList();
            }

        }

        public List<dynamic> GetAssessmentProfiles(int ownerId, int customerId, List<int> userIds,
            List<int> activityIds = null,
            List<string> actvityExtIds = null,
            List<int> statustypeIds = null,
            List<int> entityStatusIds = null,
            DateTime? surveyStartDateBefore = null,
            DateTime? surveyStartDateAfter = null,
            DateTime? surveyEndDateBefore = null,
            DateTime? surveyEndDateAfter = null,
            List<string> alternativeExtIdsToIncludeAnswer = null,
            string displayLocale = "",
            string fallbackDisplayLocale = "",
            string answerLocale = "",
            bool defaultCurrentSurveyIfNoFiltering = true,
            bool getLatestResultOnActivityOfUserOnly = false)
        {
            var languageEntities = GetLanguageEntities(displayLocale, fallbackDisplayLocale, answerLocale);
            if (languageEntities.Count == 0)
                throw new CXValidationException(cxExceptionCodes.VALIDATION_LANGUAGEID_NOT_FOUND);

            var displayLanguage = GetLanguageEntity(languageEntities, displayLocale);
            var fallbackDisplayLanguage = GetLanguageEntity(languageEntities, fallbackDisplayLocale);
            var answerLanguage = GetLanguageEntity(languageEntities, answerLocale);

            var activities = _activityRepository.GetActivities(ownerId, activityIds: activityIds, extIds: actvityExtIds);
            if (activities.Count == 0)
            {
                return new List<dynamic>();
            }

            var profileActivityIds = activities.Select(a => a.ActivityID).ToList();
            var isFilteringOnSurveyDate = surveyStartDateBefore != null || surveyStartDateAfter != null
                                          || surveyEndDateBefore != null || surveyEndDateAfter != null;
            if (!isFilteringOnSurveyDate && defaultCurrentSurveyIfNoFiltering)
            {
                //Get current surveys if there is no given survey date filter
                surveyStartDateBefore = DateTime.Now;
                surveyEndDateAfter = DateTime.Now;
            }

            var surveys = _surveyRepository.GetSurveys(activityIds: profileActivityIds, startDateBefore: surveyStartDateBefore,
                startDateAfter: surveyStartDateAfter, endDateBefore: surveyEndDateBefore, endDateAfter: surveyEndDateAfter);
            if (surveys.Count == 0)
            {
                return new List<dynamic>();
            }
            var surveyIds = surveys.Select(s => s.SurveyID).ToList();
            var entityStatusEnums = entityStatusIds == null ? new List<EntityStatusEnum>() : entityStatusIds.Select(x => (EntityStatusEnum) x).ToList();

            var assessments = _resultRepository.GetResults(ownerId, new List<int> {customerId}, userIds: userIds,
                    surveyIds: surveyIds, statusTypeIds: statustypeIds, statusIds: entityStatusEnums, includeAllAnswers: false)
                .Items.ToList();
            if (!assessments.Any())
            {
                return new List<dynamic>();
            }
            if (getLatestResultOnActivityOfUserOnly)
            {
                //If user have more than one filtered result, we only get the one is started at last.
                assessments = GetLatestAssessmentResultsOnActivityOfUser(surveys, assessments);
            }
        
            var assesmentResultIds = assessments.Select(a => a.ResultID).ToList();
            var allIncludingAnswers = new List<AnswerEntity>();
            var answerLanguageEntity = answerLanguage ?? displayLanguage ?? fallbackDisplayLanguage;
            var includingAlterntives = new List<AlternativeEntity>();
            if (alternativeExtIdsToIncludeAnswer != null && alternativeExtIdsToIncludeAnswer.Count > 0)
            {

                includingAlterntives = _alternativeRepository.GetAlternatives(extIds: alternativeExtIdsToIncludeAnswer, activityIds: activityIds);
                if (includingAlterntives.Count > 0)
                {
                    var alternativeIds = includingAlterntives.Select(a => a.AlternativeID).ToList();
                    allIncludingAnswers = _answerRepository.GetAnswers(resultIds: assesmentResultIds,
                        alternativeIds: alternativeIds).Items.Where(x => x.ItemId == null || x.ItemId == answerLanguageEntity.LanguageID).ToList();
                }
            }

            return BuildProfileAssessments(ownerId, activities, surveys, assessments, allIncludingAnswers, includingAlterntives, displayLanguage, fallbackDisplayLanguage, answerLanguageEntity);
        }

        /// <summary>
        /// Gets the learning area priorities from the latest published Learning Needs Analysis.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="customerId"></param>
        /// <param name="userId"></param>
        /// <param name="activityExtId"></param>
        /// <param name="statusTypeCodeNames"></param>
        /// <param name="learningAreaPriorityPropertyPrefix"></param>
        /// <returns></returns>
        public async Task<LearningAreaPriority> GetLearningAreaPrioritiesFromLatestPublishedAnswer(
            int ownerId, int customerId,
            int userId,
            string activityExtId,
            List<string> statusTypeCodeNames,
            string learningAreaPriorityPropertyPrefix)
        {
            var activities = await _activityRepository.GetActivitiesAsync(ownerId, extIds: new List<string> { activityExtId });
            if (activities.Count == 0) return null;

            var activityIds = activities.Select(a => a.ActivityID).ToList();
            var surveys = await _surveyRepository.GetSurveysAsync(activityIds: activityIds, startDateBefore: DateTime.Now);
            if (surveys.Count == 0) return null;

            var surveyIds = surveys.Select(s => s.SurveyID).ToList();
            var entityStatusEnums = new List<EntityStatusEnum> { EntityStatusEnum.Active };
            var statusTypeIds = await _statusTypeRepository.GetStatusTypeIdsByCodeNames(statusTypeCodeNames);

            if (statusTypeIds.IsNullOrEmpty())
            {
                return null;
            }

            var assessments = (await _resultRepository.GetResultsAsync(ownerId, new List<int> { customerId }, userIds: new List<int> { userId },
                    surveyIds: surveyIds, statusTypeIds: statusTypeIds, statusIds: entityStatusEnums, includeAllAnswers: false))
                .Items.ToList();

            if (!assessments.Any()) return null;

            var latestAssessment = assessments.LastOrDefault(p => p.ValidTo == null);
            if (latestAssessment == null) return null;

            var freeAnswer = (await _answerRepository.GetAnswersAsync(resultIds: new List<long> { latestAssessment.ResultID }))
                .Items.FirstOrDefault(x => x.ItemId == null && !string.IsNullOrEmpty(x.Free));
            if (freeAnswer == null) return null;

            return BuildLearningAreaPriorities(freeAnswer.Free, learningAreaPriorityPropertyPrefix);
        }

        private LearningAreaPriority BuildLearningAreaPriorities(string lnaJSONAnswer, string learningAreaPriorityPropertyPrefix)
        {
            var jTokens = JsonConvert.DeserializeObject<JToken>(lnaJSONAnswer);
            var highPriorities = new List<string>();
            var moderatePriorities = new List<string>();
            var lowPriorities = new List<string>();
            foreach (var jToken in jTokens)
            {
                if (!jToken.HasValues)
                {
                    continue;
                }
                if (jToken.Path.StartsWith(learningAreaPriorityPropertyPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    var prioritiesJToken = jToken.FirstOrDefault();
                    if (prioritiesJToken == null) continue;

                    var priorities = prioritiesJToken.ToObject<Dictionary<string, string>>();

                    foreach (var priority in priorities)
                    {
                        if ("H".Equals(priority.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            highPriorities.Add(priority.Key);
                        }
                        else if ("M".Equals(priority.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            moderatePriorities.Add(priority.Key);
                        }
                        else if ("L".Equals(priority.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            lowPriorities.Add(priority.Key);
                        }
                    }
                }
            }

            return new LearningAreaPriority
            {
                HighPriorities = highPriorities, ModeratePriorities = moderatePriorities, LowPriorities = lowPriorities
            };
        }


        private List<ResultEntity> GetLatestAssessmentResultsOnActivityOfUser(List<SurveyEntity> surveyEntities, List<ResultEntity> assessments)
        {
            var latestResults = new List<ResultEntity>();

            var surveyGroupsByActivity = surveyEntities.GroupBy(s => s.ActivityID);
            foreach (var surveyGroupByActivity in surveyGroupsByActivity)
            {
                var surveyIdsInActivity = surveyGroupByActivity.Select(s => s.SurveyID).ToList();
                var assessmentsInActitvity = assessments.Where(a => surveyIdsInActivity.Contains(a.SurveyID)).ToList();
                var latestAssessmentsOfUseOnActivity = assessmentsInActitvity.GroupBy(a=>a.UserID).Select(g=>GetLatestResult(g.ToList()));
                latestResults.AddRange(latestAssessmentsOfUseOnActivity);

            }
            return latestResults;
        }
        private static ResultEntity GetLatestResult(List<ResultEntity> assessmentResults)
        {
            if (assessmentResults.Count <= 1) return assessmentResults.FirstOrDefault();
            return assessmentResults.OrderByDescending(a => HasNoValue(a.StartDate) ? DateTime.MaxValue : a.StartDate).FirstOrDefault();

        }

        private static bool HasNoValue(DateTime? dateTime)
        {
            return dateTime == null ||
                                    (DateTime.Compare(dateTime.Value, DateTime.MinValue) == 0);
        }
        /// <summary>
        /// Get language for given id. Return current language if it is not found
        /// </summary>
        /// <param name="givenlocale"></param>
        /// <returns></returns>
        private LanguageEntity GetLanguageEntity(List<LanguageEntity> languageEntities, string givenlocale)
        {
            if (!string.IsNullOrEmpty(givenlocale))
            {
                return languageEntities.FirstOrDefault(l => string.Equals(l.LanguageCode, givenlocale));
            }
            return null;
        }
        private List<LanguageEntity> GetLanguageEntities(params string[] locale)
        {
            if (locale != null && locale.Length > 0)
            {
                return _languageRepository.GetLanguages().Where(l => locale.Contains(l.LanguageCode)).ToList();

            }
            return new List<LanguageEntity>();


        }
        public static List<dynamic> BuildProfileAssessments(int ownerId,
            List<ActivityEntity> activityEntities,
            List<SurveyEntity> surveyEntities,
            List<ResultEntity> assessments,
            List<AnswerEntity> allIncludingAnswers,
            List<AlternativeEntity> alternativesToIncludeAnswer,
            LanguageEntity displayLanguageEntity, LanguageEntity fallBackDisplayLanguage, LanguageEntity answerLanguage)
        {

            var activitiesOrderedByNo = activityEntities.OrderBy(a => a.No).ToList();

            //Process order by activity NO
            var results = new List<dynamic>();
            var displayLanguageCode = (displayLanguageEntity ?? fallBackDisplayLanguage ?? answerLanguage).LanguageCode;

            foreach (var activityOrderedByNo in activitiesOrderedByNo)
            {
                var surveysInActivity = surveyEntities.Where(s => s.ActivityID == activityOrderedByNo.ActivityID).Select(s => s.SurveyID).ToList();
                var assessmentsOfActivity = assessments.Where(a => surveysInActivity.Contains(a.SurveyID)).ToList();


                var activityName = string.Empty;
                var activityDisplayName = string.Empty;
                var startText = string.Empty;

                LtActivityEntity ltActivityEntity = null;

                if (displayLanguageEntity != null)
                {
                    ltActivityEntity = activityOrderedByNo.LtActivities.FirstOrDefault(lt => lt.LanguageID == displayLanguageEntity.LanguageID);
                }
                if (ltActivityEntity == null && fallBackDisplayLanguage != null)
                {
                    ltActivityEntity = activityOrderedByNo.LtActivities.FirstOrDefault(lt => lt.LanguageID == fallBackDisplayLanguage.LanguageID);
                }            

                if (ltActivityEntity != null)
                {
                    activityName = ltActivityEntity.Name;
                    activityDisplayName = ltActivityEntity.DisplayName;
                    startText = ltActivityEntity.StartText;
                }
                foreach (var assessmentResult in assessmentsOfActivity)
                {
                    var answerInResults = allIncludingAnswers.Where(x => x.ResultID == assessmentResult.ResultID).ToList();

                    dynamic assessmentIdentity = AssessmentDataHelper.GenerateIdentity(ownerId: ownerId, customerId: assessmentResult.CustomerID.Value,
                        id: assessmentResult.ResultID, archetype: Shared.Common.ArchetypeEnum.Assessment);
                    dynamic userIdenity = AssessmentDataHelper.GenerateBasicdentity(
                        id: assessmentResult.UserID.Value);

                    var includingAnswers = AssessmentDataHelper.BuildIncludingAnswers(answerInResults, alternativesToIncludeAnswer,  displayLanguageCode);

                    var assessment = AssessmentDataHelper.BuildAssessmentProfile(
                        assessmentIdentity: assessmentIdentity,
                        userIdentity: userIdenity,
                        activityNo: activityOrderedByNo.No,
                        activityName: activityName,
                        activityDisplayName: activityDisplayName,
                        startText: startText,
                        activityId: activityOrderedByNo.ActivityID,
                        activityExtId: activityOrderedByNo.ExtID,
                        assessmentStartDate: assessmentResult.StartDate,
                        assessmentEndDate: assessmentResult.EndDate,
                        assessmentPageNo: assessmentResult.PageNo,
                        assessmentStatusId: assessmentResult.StatusTypeID,
                        assessmentLastUpdated: assessmentResult.LastUpdated,
                        includingAnswers: includingAnswers);
                    results.Add(assessment);
                }
            }
            return results;
        }
     
        private List<dynamic> BuildAssessmentConfigurationsWithoutStatusType(List<ActivityEntity> activities)
        {
            return activities.Select(AssessmentDataHelper.GenerateAssessmentConfiguration).ToList();
        }

        private List<dynamic> BuildAssessmentConfigurationsWithStatusType(List<ActivityEntity> activities)
        {
            List<dynamic> assessmentConfigurations = new List<dynamic>();
            var statusTypes = _statusTypeRepository.GetStatusTypes();
            foreach (var activityEntity in activities)
            {
                dynamic assessmentConfiguration = AssessmentDataHelper.GenerateAssessmentConfiguration(activityEntity);
                var statusTypesOfActivity = _activityStatusTypeRepository.GetActivityStatusByActivityId(activityEntity.ActivityID);
                foreach (var activityStatusType in statusTypesOfActivity)
                {
                    var statusType = statusTypes.FirstOrDefault(p => p.StatusTypeID == activityStatusType.StatusTypeID);
                    if (statusType != null)
                    {
                        dynamic assessmentStatus=new ExpandoObject();
                        assessmentStatus.CodeName = statusType.CodeName;
                        assessmentStatus.AssessmentStatusId = activityStatusType.StatusTypeID;
                        assessmentConfiguration.AssessmentStatus.Add(assessmentStatus);
                    }
                }
                assessmentConfigurations.Add(assessmentConfiguration);
            }
            return assessmentConfigurations;
        }
    }
}
