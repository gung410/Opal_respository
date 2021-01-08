using System.Collections.Generic;
using Microservice.LnaForm.Domain.Entities;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Domain.Services
{
    public interface IFormBusinessLogicService
    {
        void EnsureFormQuestionsValidToSave(FormEntity form, IEnumerable<FormQuestion> formQuestions, IEnumerable<FormSection> formSections);
    }
}
