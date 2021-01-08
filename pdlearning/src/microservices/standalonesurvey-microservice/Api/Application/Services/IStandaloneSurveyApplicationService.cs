using System;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public interface IStandaloneSurveyApplicationService
    {
        Task<SurveyWithQuestionsModel> CreateSurveys(CreateSurveyRequestDto dto, Guid userId);

        Task<SurveyWithQuestionsModel> CloneSurveys(CloneSurveyRequestDto dto, Guid userId);

        Task<SurveyWithQuestionsModel> UpdateSurveys(UpdateSurveyRequestDto dto, Guid userId);

        Task ImportStandaloneSurveys(ImportStandaloneSurveyRequest dtos, Guid userId);

        Task<SurveyWithQuestionsModel> UpdateSurveyStatusAndData(UpdateSurveyRequestDto dto, Guid userId);

        Task DeleteSurvey(Guid formId, Guid userId);

        Task<PagedResultDto<StandaloneSurveyModel>> SearchSurveys(SearchSurveysRequestDto dto, Guid userId);

        Task<PagedResultDto<SurveyQuestionModel>> SearchSurveyQuestions(SearchSurveyQuestionsRequestDto dto, Guid userId);

        Task<SurveyWithQuestionsModel> GetSurveyWithQuestionsById(GetSurveyWithQuestionsByIdRequestDto dto, Guid userId);

        Task<SurveyWithQuestionsModel> GetSurveyParticipantById(GetSurveyParticipantByIdRequestDto dto, Guid userId);

        Task TransferCourseOwnership(TransferOwnershipRequest request);

        Task<VersionTrackingSurveyDataModel> GetSurveyDataByVersionTrackingId(GetSurveyDataByVersionTrackingIdRequestDto dto, Guid userId);
    }
}
