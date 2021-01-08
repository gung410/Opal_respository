using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.RequestDtos.RegistrationRequest.ImportParticipant;
using Microservice.Course.Application.Services;
using Microservice.Course.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.Course.Controllers
{
    [Route("api/registration")]
    public class RegistrationController : BaseController<RegistrationService>
    {
        public RegistrationController(IUserContext userContext, RegistrationService appService) : base(userContext, appService)
        {
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<RegistrationModel>> SearchRegistration([FromBody] SearchRegistrationRequest request)
        {
            return await AppService.SearchRegistration(request);
        }

        [HttpGet("{registrationId:guid}")]
        public async Task<RegistrationModel> GetRegistrationById(Guid registrationId)
        {
            return await AppService.GetRegistrationById(registrationId);
        }

        [HttpPost("getByIds")]
        public async Task<List<RegistrationModel>> GetRegistrationByIds([FromBody] List<Guid> registrationIds)
        {
            return await AppService.GetRegistrationByIds(registrationIds);
        }

        [HttpPut("changeStatus")]
        public async Task ChangeRegistrationStatus([FromBody] ChangeRegistrationStatusRequest request)
        {
            await AppService.ChangeRegistrationStatus(request);
        }

        [HttpPut("overrideRegistrationCourseCriteria")]
        public async Task OverrideRegistrationCourseCriteria([FromBody] OverrideRegistrationCourseCriteriaRequest request)
        {
            await AppService.OverrideRegistrationCourseCriteria(request);
        }

        [HttpPut("changeStatusByCourseClassRun")]
        public async Task ChangeRegistrationStatusByCourseClassRun([FromBody] ChangeRegistrationByLearnerRequest request)
        {
            await AppService.ChangeRegistrationStatusByLearner(request);
        }

        [HttpPost("createRegistration")]
        public async Task<List<RegistrationModel>> CreateRegistration([FromBody] CreateRegistrationRequest request)
        {
            return await AppService.SaveRegistration(request);
        }

        [HttpPut("changeWithdrawStatus")]
        public async Task ChangeRegistrationWithdrawStatus([FromBody] ChangeRegistrationWithdrawStatusRequest request)
        {
            await AppService.ChangeRegistrationWithdrawStatus(request);
        }

        [HttpPut("changeClassRunChangeStatus")]
        public async Task ChangeRegistrationClassRunChangeStatus([FromBody] ChangeRegistrationClassRunChangeStatusRequest request)
        {
            await AppService.ChangeRegistrationClassRunChangeStatus(request);
        }

        [HttpGet("getApprovalRegistration")]
        public async Task<PagedResultDto<RegistrationModel>> GetApprovalRegistration(GetApprovalRegistrationRequest request)
        {
            return await AppService.GetApprovalRegistration(request);
        }

        [HttpPost("nominateLearner")]
        public async Task NominateLearner([FromBody] UserNominationRequest request)
        {
            await AppService.NominateUser(request);
        }

        [HttpPost("nominateLearner/validate")]
        public async Task<List<ValidateNominateLearnerResultModel>> ValidateNominateLearner([FromBody] ValidateNominateLearnersRequest request)
        {
            return await AppService.ValidateNominatedLearners(request);
        }

        [HttpPost("massNominate")]
        public async Task MassNominateLearners([FromBody] MassUserNominationRequest request)
        {
            await AppService.MassNominateUsers(request);
        }

        [HttpPost("massNominate/validate")]
        public async Task<List<ValidateNominateLearnerResultModel>> ValidateMassNominateLearners([FromBody] ValidateMassNominateLearnersRequest request)
        {
            return await AppService.ValidateMassNominatedLearners(request);
        }

        [HttpGet("getCompletionRate/{classRunId:guid}")]
        public async Task<double> GetCompletionRate(Guid classRunId)
        {
            return await AppService.GetCompletionRate(classRunId);
        }

        [HttpPost("completeOrIncompleteRegistration")]
        public async Task CompleteOrIncompleteRegistration([FromBody]ChangeLearnerStatusRequest request)
        {
            await AppService.CompleteOrIncompleteRegistration(request);
        }

        [HttpPost("changeClassRun")]
        public async Task ChangeClassRun([FromBody] ChangeClassRunRequest request)
        {
            await AppService.ChangeClassRunRegistration(request);
        }

        [HttpPost("massChangeClassRun")]
        public async Task MassChangeClassRun([FromBody] MassChangeClassRunRequest request)
        {
            await AppService.MassChangeClassRunRegistration(request);
        }

        [HttpPost("addParticipants")]
        public async Task<AddParticipantsModel> AddParticipants([FromBody] AddParticipantsRequest request)
        {
            return await AppService.AddParticipants(request);
        }

        [HttpPost("{courseId:guid}/importParticipant")]
        public async Task<AddParticipantsModel> ImportParticipant(ImportPartcipantRequest request)
        {
            return await AppService.ImportParticipant(request);
        }

        [HttpGet("getLearnerCourseViolation")]
        public async Task<GetLearnerCourseViolationQueryResult> GetLearnerCourseViolation(GetLearnerViolationRequest request)
        {
            return await AppService.GetLearnerCourseViolation(request);
        }

        [HttpGet("getNoOfRegistrationFinished")]
        public async Task<NoOfFinishedRegistrationModel> GetNoOfFinishedRegistration(GetNoOfFinishedRegistrationRequest request)
        {
            return await AppService.GetNoOfFinishedRegistration(request);
        }

        [HttpPut("{registrationId:guid}/completePostEvaluation")]
        public async Task CompletePostEvaluation(Guid registrationId)
        {
            await AppService.CompletePostEvaluation(registrationId);
        }

        [HttpPost("exportParticipants")]
        public async Task<IActionResult> ExportParticipants([FromBody]ExportParticipantsRequest request)
        {
            var exportParticipantsResult = await AppService.ExportParticipants(request);
            if (request.FileFormat == ExportParticipantsFileFormat.Csv)
            {
                return DownloadFile(exportParticipantsResult.FileContent, "text/csv", exportParticipantsResult.GetFileName());
            }

            return DownloadFile(exportParticipantsResult.FileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", exportParticipantsResult.GetFileName());
        }

        [HttpPost("exportParticipantTemplate")]
        public IActionResult ExportParticipantTemplate([FromBody] ExportParticipantTemplateRequest request)
        {
            var exportParticipantTemplateResult = AppService.ExportParticipantTemplate(request);
            if (request.FileFormat == ExportParticipantTemplateFileFormat.Csv)
            {
                return DownloadFile(exportParticipantTemplateResult.FileContent, "text/csv", exportParticipantTemplateResult.GetFileName());
            }

            return DownloadFile(
                exportParticipantTemplateResult.FileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                exportParticipantTemplateResult.GetFileName());
        }

        [HttpPost("migrateRegistrationsEvent")]
        public async Task<PagedResultDto<Guid>> MigrateRegistrationsEvent([FromBody] MigrateRegistrationRequest request)
        {
            EnsureValidPermission(UserContext.IsSysAdministrator());

            return await AppService.MigrateRegistrations(request);
        }

        [HttpPost("migrateRegistrationECertificates")]
        public async Task<PagedResultDto<Guid>> MigrateRegistrationECertificates([FromBody] MigrateRegistrationECertificateRequest request)
        {
            EnsureValidPermission(UserContext.IsSysAdministrator());

            return await AppService.MigrateRegistrationECertificates(request);
        }

        [HttpPost("migrateLearningRecordsEvent")]
        public async Task<PagedResultDto<Guid>> MigrateLearningRecordsEvent([FromBody] MigrateLearningRecordRequest request)
        {
            EnsureValidPermission(UserContext.IsSysAdministrator());

            return await AppService.MigrateLearningRecords(request);
        }

        [HttpGet("{registrationId:guid}/getCertificate")]
        public async Task<RegistrationECertificateModel> GetECertificateByRegistrationId(Guid registrationId)
        {
            return await AppService.GetECertificateByRegistrationId(registrationId);
        }

        [HttpGet("getMyCertificates")]
        public async Task<PagedResultDto<RegistrationModel>> GetMyCertificates(GetMyCertificatesRequest request)
        {
            return await AppService.GetMyCertificates(request);
        }
    }
}
