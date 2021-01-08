using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;
using FormQuestionEntity = Microservice.LnaForm.Domain.Entities.FormQuestion;

namespace Microservice.LnaForm.Application.Services
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
