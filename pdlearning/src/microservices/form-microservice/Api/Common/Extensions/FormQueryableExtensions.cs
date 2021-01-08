using System;
using System.Linq;
using Microservice.Form.Domain.Constants;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Domain.Repositories;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Common.Extensions
{
    public static class FormQueryableExtensions
    {
        public static IQueryable<FormEntity> CombineWithPublicSurveyTemplates(this IQueryable<FormEntity> currentQuery, IRepository<FormEntity> formRepository)
        {
            var publicSurveyQuery = formRepository
                .GetAll()
                .Where(f =>
                    f.IsSurveyTemplate == true
                    && f.Type == FormType.Survey);

            return currentQuery
                .Union(publicSurveyQuery)
                .Distinct();
        }

        public static IQueryable<FormEntity> PrioritizeSurveyTemplate(this IQueryable<FormEntity> currentQuery)
        {
            // Prioritize 4 default types of templates
            // Iterate over the filters and add each as a separate WHERE clause
            foreach (var templateTitle in DomainConstants.PostCourseSurveyTemplateTitles)
            {
                // this just adds to the existing expression tree..
                currentQuery = currentQuery.OrderBy(f => templateTitle.ToLower().Equals(f.Title.ToLower()) ? 0 : 1);
            }

            currentQuery = currentQuery

                // Prioritize other templates
                .OrderBy(f => f.IsSurveyTemplate.Value ? 0 : 1)

                // Order other form by ChangedDate/CreatedDate by default
                .ThenByDescending(f => f.IsSurveyTemplate.Value ?
                    DateTime.MinValue :
                    (f.ChangedDate ?? f.CreatedDate));

            return currentQuery;
        }
    }
}
