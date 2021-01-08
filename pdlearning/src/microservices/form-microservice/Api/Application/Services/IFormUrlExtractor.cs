using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FormEntity = Microservice.Form.Domain.Entities.Form;
using FormQuestionEntity = Microservice.Form.Domain.Entities.FormQuestion;

namespace Microservice.Form.Application.Services
{
    /// <summary>
    /// To extract URL(s) from a learning content.
    /// </summary>
    public interface IFormUrlExtractor
    {
        Task ExtractAll();

        Task ExtractFormUrl(FormEntity form, List<FormQuestionEntity> formQuestion);

        Task DeleteExtractedUrls(Guid formId);
    }
}
