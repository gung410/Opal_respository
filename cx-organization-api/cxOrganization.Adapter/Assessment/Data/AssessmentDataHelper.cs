using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Shared.Common;

namespace cxOrganization.Adapter.Assessment.Data
{
    public static class AssessmentDataHelper
    {
        public static List<dynamic> GetLocalizedData<T>(ICollection<T> ltEntities, 
            List<LanguageEntity> listLanguage, 
            Func<T, int> idSelector, 
            Func<T, int> languageIdSelector)
        {
            var result = new List<dynamic>();
            foreach (var item in ltEntities)
            {
                dynamic localizedDataDto = new ExpandoObject();
                localizedDataDto.Id = idSelector(item);
                var language = listLanguage.FirstOrDefault(p => p.LanguageID == languageIdSelector(item));
                localizedDataDto.LanguageCode = language != null ? language.LanguageCode : String.Empty;
                localizedDataDto.Fields = new List<dynamic>();
                foreach (var prop in item.GetType().GetProperties())
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        dynamic localizedField=new ExpandoObject();
                        localizedField.Name = prop.Name;
                        localizedField.LocalizedText = prop.GetValue(item, null).ToString();

                        localizedDataDto.Fields.Add(localizedField);
                    }
                }
                result.Add(localizedDataDto);
            }
            return result;
        }

        public static dynamic GenerateLevelGroup(int ownerId, LevelGroupEntity levelgroupEntity)
        {
            dynamic levelGroup = new ExpandoObject();
            levelGroup.Identity = GenerateIdentity(ownerId: ownerId, id: levelgroupEntity.LevelGroupId, customerId: levelgroupEntity.CustomerId ?? 0);
            levelGroup.ActivityId = levelgroupEntity.ActivityId;
            levelGroup.DepartmentId = levelgroupEntity.DepartmentId;
            levelGroup.No = levelgroupEntity.No;
            levelGroup.RoleId = levelgroupEntity.RoleId;
            levelGroup.Tag = levelgroupEntity.Tag;
            return levelGroup;

        }

        public static dynamic GenerateLevelLimit(int ownerId, int customerId, LevelLimitEntity levelLimitEntity)
        {
            dynamic levelLimit = new ExpandoObject();
            levelLimit.Identity = GenerateIdentity(ownerId: ownerId, customerId: customerId, id: levelLimitEntity.LevelLimitId, extId: levelLimitEntity.ExtId);
            levelLimit.AlternativeId = levelLimitEntity.AlternativeId;
            levelLimit.CategoryId = levelLimitEntity.CategoryId;
            levelLimit.ItemId = levelLimitEntity.ItemId;
            levelLimit.LevelGroupId = levelLimitEntity.LevelGroupId;
            levelLimit.MaxValue = levelLimitEntity.MaxValue;
            levelLimit.MinValue = levelLimitEntity.MinValue;
            levelLimit.MatchingType = levelLimitEntity.MatchingType;
            levelLimit.NegativeTrend = levelLimitEntity.NegativeTrend;
            levelLimit.OwnerColorId = levelLimitEntity.OwnerColorId;
            levelLimit.QuestionId = levelLimitEntity.QuestionId;
            levelLimit.SigChange = levelLimitEntity.Sigchange;

            //We init OwnerColorEntity when levelLimitEntity.OwnerColor is null to make sure dynamic always has these properties
            var ownerColor = levelLimitEntity.OwnerColor ?? new OwnerColorEntity();

            levelLimit.BorderColor = ownerColor.BorderColor;
            levelLimit.FillColor = ownerColor.FillColor;
            levelLimit.FriendlyName = ownerColor.FriendlyName;
            levelLimit.No = ownerColor.No;
            levelLimit.TextColor = ownerColor.TextColor;
            return levelLimit;
        }

        public static dynamic GenerateAssessmentConfiguration(ActivityEntity activityEntity)
        {
            dynamic assessmentConfiguration = new ExpandoObject();
            assessmentConfiguration.Identity = GenerateIdentity(
                    ownerId: activityEntity.OwnerID,
                    id: activityEntity.ActivityID,
                    extId: activityEntity.ExtID,
                    archetype: ArchetypeEnum.Activity)
                ;
            assessmentConfiguration.Type = (ActivityTypeEnum) activityEntity.Type;
            assessmentConfiguration.AssessmentStatus = new List<dynamic>();
            return assessmentConfiguration;
        }

        public static dynamic GenerateIdentity(int ownerId = 0, int customerId = 0, long id = 0, string extId = "", ArchetypeEnum archetype = ArchetypeEnum.Unknown)
        {
            dynamic identity = new ExpandoObject();
            identity.Archetype = archetype;
            identity.Id = id;
            identity.ExtId = extId;
            identity.OwnerId = ownerId;
            identity.CustomerId = customerId;
            return identity;
        }
        public static dynamic GenerateBasicdentity(long id = 0, string extId = "", ArchetypeEnum archetype = ArchetypeEnum.Unknown)
        {
            dynamic identity = new ExpandoObject();
            identity.Archetype = archetype;
            identity.Id = id;
            identity.ExtId = extId;
            return identity;
        }

        public static dynamic BuildAssessmentProfile(
            dynamic assessmentIdentity,
            dynamic userIdentity,
            int activityNo,
            string activityName,
            string activityDisplayName,
            string startText,
            int activityId,
            string activityExtId,
            DateTime? assessmentStartDate,
            DateTime? assessmentEndDate,
            short assessmentPageNo,
            int? assessmentStatusId,
            DateTime assessmentLastUpdated,
            List<dynamic> includingAnswers)
        {
            dynamic assessment = new ExpandoObject();
            assessment.Identity = assessmentIdentity;
            assessment.UserIdentity = userIdentity;
            assessment.ActivityNo = activityNo;
            assessment.ActivityName = activityName;
            assessment.ActivityDisplayName = activityDisplayName;
            assessment.StartText = startText;
            assessment.ActivityId = activityId;
            assessment.ActivityExtId = activityExtId;
            assessment.StartDate = assessmentStartDate;
            assessment.EndDate = assessmentEndDate;
            assessment.PageNo = assessmentPageNo;
            assessment.AssessmentStatusId = assessmentStatusId;
            assessment.LastUpdated = assessmentLastUpdated;
            assessment.IncludingAnswers = includingAnswers;
            return assessment;
        }

        public static string GetAnswerValue(AnswerEntity answerEntity, string locale)
        {
            if (answerEntity.DateValue.HasValue)
            {
                return answerEntity.DateValue.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            if (!string.IsNullOrEmpty(answerEntity.Free))
            {
                return answerEntity.Free;
            }
            return ((decimal)answerEntity.Value).ToString(CultureInfo.CreateSpecificCulture(locale));
        }

        public static List<dynamic> BuildIncludingAnswers(List<AnswerEntity> answers, List<AlternativeEntity> allAlternatives, string displayLanguageCode)
        {
            List<dynamic> includingAnswers = new List<dynamic>();

            foreach (var answer in answers)
            {
                var alternativeExtId = "";
                var alternative = allAlternatives.FirstOrDefault(a => a.AlternativeID == answer.AlternativeID);
                if (alternative != null)
                {
                    alternativeExtId = alternative.ExtID;
                }
                dynamic includingAnswer = new ExpandoObject();
                includingAnswer.AnswerId = answer.AnswerID;
                includingAnswer.AlternativeId = answer.AlternativeID;
                includingAnswer.AlternativeExtId = alternativeExtId;
                includingAnswer.QuestionId = answer.QuestionID;
                includingAnswer.Value = AssessmentDataHelper.GetAnswerValue(answer, displayLanguageCode);
                includingAnswer.ItemId = answer.ItemId;
                includingAnswers.Add(includingAnswer);

            }

            return includingAnswers;
        }

    }


}
