using System.Collections.Generic;
using System.Linq;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.StandaloneSurvey.Domain.Services
{
    public class SurveyBusinessLogicService : ISurveyBusinessLogicService
    {
        public void EnsureFormQuestionsValidToSave(Entities.StandaloneSurvey form, IEnumerable<SurveyQuestion> formQuestions, IEnumerable<SurveySection> formSections)
        {
            var duplicatedByPriorityQuestions = formQuestions.DuplicatedItemGroups(p => new { p.MinorPriority, FormSectionId = p.SurveySectionId, p.Priority });
            if (duplicatedByPriorityQuestions.Any())
            {
                throw new BusinessLogicException("All form question priorities must be unique in a form");
            }

            var hasDuplicatedQuestionOptionCodes = formQuestions.Any(p => p.Options != null && p.Options.Any() && p.Options.DuplicatedItemGroups(opt => opt.Code).Any());
            if (hasDuplicatedQuestionOptionCodes)
            {
                throw new BusinessLogicException("All form question option codes must be unique in a question");
            }

            var hasQuestionWithOptionsLessThanTwoOptions = formQuestions.Any(p => p.Options != null
                && p.Options.Any()
                && p.Options.Count() < 2
                && p.QuestionType != QuestionType.Section
                && p.QuestionType != QuestionType.Note
                && p.QuestionType != QuestionType.Smatrix
                && p.QuestionType != QuestionType.Qset);
            if (hasQuestionWithOptionsLessThanTwoOptions)
            {
                throw new BusinessLogicException("All form question with options must have at least 2 options");
            }

            var sectionIdsHasQuestion = formQuestions.Select(question => question.SurveySectionId);
            var formSectionIds = formSections.Select(section => section.Id);
            var hasSectionHasNoQuestion = formSectionIds.Count() > 0 && formSections.Any(section => !sectionIdsHasQuestion.Contains(section.Id));
            if (hasSectionHasNoQuestion)
            {
                throw new BusinessLogicException("All form section must have at least 1 question");
            }
        }
    }
}
