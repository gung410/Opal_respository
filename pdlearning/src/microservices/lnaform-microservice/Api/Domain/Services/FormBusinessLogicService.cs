using System.Collections.Generic;
using System.Linq;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Exceptions;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Domain.Services
{
    public class FormBusinessLogicService : IFormBusinessLogicService
    {
        public void EnsureFormQuestionsValidToSave(FormEntity form, IEnumerable<FormQuestion> formQuestions, IEnumerable<FormSection> formSections)
        {
            var duplicatedByPriorityQuestions = formQuestions.DuplicatedItemGroups(p => new { p.MinorPriority, p.FormSectionId, p.Priority });
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
