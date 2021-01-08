using System.Collections.Generic;
using System.Linq;
using Microservice.Form.Application;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Constants;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Exceptions;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Domain.Services
{
    public class FormBusinessLogicService : IFormBusinessLogicService
    {
        public void EnsureFormValidToSave(FormEntity form)
        {
            if (DomainConstants.PostCourseSurveyTemplateTitles.Any(title => title.ToLower().Equals(form.Title.ToLower())))
            {
                throw new FormDuplicateTitleWithSurveyTemplateException();
            }
        }

        public void EnsureFormQuestionsValidToSave(FormEntity form, IEnumerable<FormQuestion> formQuestions, IEnumerable<FormSection> formSections)
        {
            var duplicatedByPriorityQuestions = formQuestions.DuplicatedItemGroups(p => new { p.MinorPriority, p.FormSectionId, p.Priority });
            if (duplicatedByPriorityQuestions.Any())
            {
                throw new BusinessLogicException("All form question priorities must be unique in a form");
            }

            var hasDuplicatedQuestionOptionCodes = formQuestions.Any(p => p.Question_Options != null && p.Question_Options.Any() && p.Question_Options.DuplicatedItemGroups(opt => opt.Code).Any());
            if (hasDuplicatedQuestionOptionCodes)
            {
                throw new BusinessLogicException("All form question option codes must be unique in a question");
            }

            var hasQuestionWithOptionsLessThanTwoOptions = formQuestions.Any(p => p.Question_Options != null
                && p.Question_Options.Any()
                && p.Question_Options.Count() < 2
                && p.Question_Type != QuestionType.Section
                && p.Question_Type != QuestionType.Note
                && p.Question_Type != QuestionType.Smatrix
                && p.Question_Type != QuestionType.Qset
                && p.Question_Type != QuestionType.Criteria
                && p.Question_Type != QuestionType.Scale);
            if (hasQuestionWithOptionsLessThanTwoOptions)
            {
                throw new BusinessLogicException("All form question with options must have at least 2 options");
            }

            var sectionIdsHasQuestion = formQuestions.Select(question => question.FormSectionId);
            var formSectionIds = formSections.Select(section => section.Id);
            var hasSectionHasNoQuestion = formSectionIds.Count() > 0 && formSections.Any(section => !sectionIdsHasQuestion.Contains(section.Id));
            if (hasSectionHasNoQuestion)
            {
                throw new BusinessLogicException("All form section must have at least 1 question");
            }
        }
    }
}
