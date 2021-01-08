using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.LnaForm.Application.Services
{
    public interface IFormApplicationService
    {
        Task<FormWithQuestionsModel> CreateForm(CreateFormRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> CloneForm(CloneFormRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> UpdateForm(UpdateFormRequestDto dto, Guid userId);

        Task ImportForms(ImportFormRequest dtos, Guid userId);

        Task<FormWithQuestionsModel> UpdateFormStatusAndData(UpdateFormRequestDto dto, Guid userId);

        Task DeleteForm(Guid formId, Guid userId);

        Task<PagedResultDto<FormModel>> SearchForms(SearchFormsRequestDto dto, Guid userId);

        Task<PagedResultDto<FormQuestionModel>> SearchFormQuestions(SearchFormQuestionsRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> GetFormWithQuestionsById(GetFormWithQuestionsByIdRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> GetFormStandaloneById(GetFormStandaloneByIdRequestDto dto, Guid userId);

        Task TransferCourseOwnership(TransferOwnershipRequest request);

        Task<VersionTrackingFormDataModel> GetFormDataByVersionTrackingId(GetFormDataByVersionTrackingIdRequestDto dto, Guid userId);
    }
}
