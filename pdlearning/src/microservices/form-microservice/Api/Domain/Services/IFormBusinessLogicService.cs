using System.Collections.Generic;
using Microservice.Form.Domain.Entities;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Domain.Services
{
    public interface IFormBusinessLogicService
    {
        void EnsureFormValidToSave(FormEntity form);

        void EnsureFormQuestionsValidToSave(FormEntity form, IEnumerable<FormQuestion> formQuestions, IEnumerable<FormSection> formSections);
    }
}
