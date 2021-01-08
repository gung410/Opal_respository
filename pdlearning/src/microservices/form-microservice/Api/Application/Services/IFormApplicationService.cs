using System;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.Services
{
    public interface IFormApplicationService
    {
        Task<FormWithQuestionsModel> CreateForm(CreateFormRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> CloneForm(CloneFormRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> CloneAssessmentForm(CloneFormRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> UpdateForm(UpdateFormRequestDto dto, Guid userId);

        Task ImportForms(ImportFormRequest dtos, Guid userId);

        Task<FormWithQuestionsModel> UpdateFormStatusAndData(UpdateFormRequestDto dto, Guid userId);

        Task DeleteForm(Guid formId, Guid userId);

        Task<PagedResultDto<FormModel>> SearchForms(SearchFormsRequestDto dto, Guid userId);

        Task<PagedResultDto<FormModel>> GetPendingApprovalForms(GetPendingApprovalFormsRequestDto dto, Guid userId);

        Task<PagedResultDto<FormQuestionModel>> SearchFormQuestions(SearchFormQuestionsRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> GetFormWithQuestionsById(GetFormWithQuestionsByIdRequestDto dto, Guid userId);

        Task<FormAssessmentModel> GetFormtAssessmentById(GetFormAssessmentByIdRequestDto dto, Guid userId);

        Task<FormWithQuestionsModel> GetFormStandaloneById(GetFormStandaloneByIdRequestDto dto, Guid userId);

        Task<VersionTrackingFormDataModel> GetFormDataByVersionTrackingId(GetFormDataByVersionTrackingIdRequestDto dto, Guid userId);

        Task TransferCourseOwnership(TransferOwnershipRequest request);

        Task<PagedResultDto<Guid>> MigrateSearchFormData(MigrateSearchEngineDataRequestDto dto);
    }
}
