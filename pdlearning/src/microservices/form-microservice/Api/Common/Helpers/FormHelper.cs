using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.ValueObjects.Questions;

namespace Microservice.Form.Common.Helpers
{
    public class FormHelper
    {
        /// <summary>
        /// Return dictionary store priority of questions and sections.
        /// </summary>
        /// <param name="questions">Questions.</param>
        /// <param name="sections">Sections.</param>
        /// <returns>Dictionary store key Id and value Priority.</returns>
        public static Dictionary<Guid, int> GetPriorityDicById(IEnumerable<FormQuestionModel> questions, IEnumerable<FormSectionModel> sections)
        {
            var priorityIdMapping = questions.Select(question => new { question.Id, question.Priority });
            priorityIdMapping = priorityIdMapping.Concat(sections.Select(section => new { section.Id, section.Priority }));
            return priorityIdMapping.ToDictionary(item => item.Id, item => item.Priority);
        }

        /// <summary>
        /// Return dictionary store priority of questions and sections.
        /// </summary>
        /// <param name="questions">Questions.</param>
        /// <param name="sections">Sections.</param>
        /// <returns>Dictionary store key Priority and value Id.</returns>
        public static Dictionary<int, Guid> GetIdDicByPriority(IEnumerable<FormQuestionModel> questions, IEnumerable<FormSectionModel> sections)
        {
            var priorityIdMapping = questions.Where(question => !question.FormSectionId.HasValue).Select(question => new { question.Id, question.Priority });
            priorityIdMapping = priorityIdMapping.Concat(sections.Select(section => new { section.Id, section.Priority }));
            return priorityIdMapping.ToDictionary(item => item.Priority, item => item.Id);
        }

        /// <summary>
        /// Update NextQuestionId after clone new form.
        /// </summary>
        /// <param name="clonedQuestions">Cloned questions.</param>
        /// <param name="priorityDicByOldId">Dictionary store old id and priority.</param>
        /// <param name="newIdDicByPriority">Dictionary store new id and priority.</param>
        /// <returns>Questions has updated NextQuestionId.</returns>
        public static IEnumerable<FormQuestionModel> UpdateClonedNextQuestionId(IEnumerable<FormQuestionModel> clonedQuestions, Dictionary<Guid, int> priorityDicByOldId, Dictionary<int, Guid> newIdDicByPriority)
        {
            foreach (var question in clonedQuestions)
            {
                if (question.NextQuestionId.HasValue)
                {
                    var priority = priorityDicByOldId[question.NextQuestionId.Value];
                    question.NextQuestionId = newIdDicByPriority[priority];
                }

                if (question.QuestionOptions != null
                    && (question.QuestionType == QuestionType.DropDown
                    || question.QuestionType == QuestionType.SingleChoice
                    || question.QuestionType == QuestionType.TrueFalse))
                {
                    foreach (var option in question.QuestionOptions)
                    {
                        if (option.NextQuestionId.HasValue)
                        {
                            var priority = priorityDicByOldId[question.NextQuestionId.Value];
                            option.NextQuestionId = newIdDicByPriority[priority];
                        }
                    }
                }
            }

            return clonedQuestions;
        }

        /// <summary>
        /// Update NextQuestionId after clone new form.
        /// </summary>
        /// <param name="clonedSections">Cloned sections.</param>
        /// <param name="priorityDicByOldId">Dictionary store old id and priority.</param>
        /// <param name="newIdDicByPriority">Dictionary store new id and priority.</param>
        /// <returns>Sections has updated NextQuestionId.</returns>
        public static IEnumerable<FormSectionModel> UpdateClonedNextQuestionId(IEnumerable<FormSectionModel> clonedSections, Dictionary<Guid, int> priorityDicByOldId, Dictionary<int, Guid> newIdDicByPriority)
        {
            foreach (var section in clonedSections)
            {
                if (section.NextQuestionId.HasValue)
                {
                    var priority = priorityDicByOldId[section.NextQuestionId.Value];
                    section.NextQuestionId = newIdDicByPriority[priority];
                }
            }

            return clonedSections;
        }
    }
}
