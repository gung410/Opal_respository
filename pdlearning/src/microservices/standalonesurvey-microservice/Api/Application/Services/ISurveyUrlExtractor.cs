using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Domain.Entities;

namespace Microservice.StandaloneSurvey.Application.Services
{
    /// <summary>
    /// To extract URL(s) from a learning content.
    /// </summary>
    public interface ISurveyUrlExtractor
    {
        Task ExtractAll();

        Task ExtractFormUrl(Domain.Entities.StandaloneSurvey form, List<SurveyQuestion> formQuestion);

        Task DeleteExtractedUrls(Guid formId);
    }
}
