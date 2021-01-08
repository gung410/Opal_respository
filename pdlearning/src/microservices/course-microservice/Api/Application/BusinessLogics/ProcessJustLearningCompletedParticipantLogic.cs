using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ProcessJustLearningCompletedParticipantLogic : BaseBusinessLogic
    {
        private readonly SendLearningRecordEventLogic _learningRecordEventLogic;
        private readonly ECertificateBuilderLogic _certificateBuilder;
        private readonly RegistrationECertificateCudLogic _registrationECertificateCudLogic;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly GetAggregatedRegistrationSharedQuery _aggregatedRegistrationSharedQuery;
        private readonly GetAggregatedECertificateTemplateSharedQuery _aggregatedECertificateTemplateSharedQuery;

        public ProcessJustLearningCompletedParticipantLogic(
            SendLearningRecordEventLogic learningRecordEventLogic,
            ECertificateBuilderLogic certificateBuilder,
            GetAggregatedRegistrationSharedQuery aggregatedRegistrationSharedQuery,
            IReadOnlyRepository<CourseUser> readUserRepository,
            GetAggregatedECertificateTemplateSharedQuery aggregatedECertificateTemplateSharedQuery,
            RegistrationECertificateCudLogic registrationECertificateCudLogic,
            IUserContext userContext) : base(userContext)
        {
            _learningRecordEventLogic = learningRecordEventLogic;
            _certificateBuilder = certificateBuilder;
            _aggregatedRegistrationSharedQuery = aggregatedRegistrationSharedQuery;
            _readUserRepository = readUserRepository;
            _aggregatedECertificateTemplateSharedQuery = aggregatedECertificateTemplateSharedQuery;
            _registrationECertificateCudLogic = registrationECertificateCudLogic;
        }

        public async Task<List<Registration>> Execute(List<Registration> registrations, CancellationToken cancellationToken = default)
        {
            var completedRegistrations = registrations.Where(x => x.IsCompleted()).ToList();

            await _learningRecordEventLogic.ByRegistrations(completedRegistrations, cancellationToken);

            await CreateECertificateForCompletedParticipant(completedRegistrations, cancellationToken);

            return completedRegistrations;
        }

        public async Task CreateECertificateForCompletedParticipant(List<Registration> completedRegistrations, CancellationToken cancellationToken = default)
        {
            var validToGenerateEcertificateAggregatedRegistrations =
                (await _aggregatedRegistrationSharedQuery.FullByRegistrations(completedRegistrations, cancellationToken))
                .Where(p => p.Course.ECertificateTemplateId.HasValue)
                .ToList();

            var facilitatorIds = validToGenerateEcertificateAggregatedRegistrations
                .SelectMany(p => p.Course.CourseFacilitatorIds)
                .Distinct();
            var facilitatorsDic = await _readUserRepository.GetAll().Where(p => facilitatorIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

            var ecertificateTemplateIds = validToGenerateEcertificateAggregatedRegistrations
                .Select(p => p.Course.ECertificateTemplateId.GetValueOrDefault())
                .Distinct()
                .ToList();
            var aggregatedECertificateTemplatesDic = (await _aggregatedECertificateTemplateSharedQuery.ByIds(ecertificateTemplateIds)).ToDictionary(p => p.Template.Id);

            var registrationIds = validToGenerateEcertificateAggregatedRegistrations.Select(p => p.Registration.Id);
            await _registrationECertificateCudLogic.DeleteManyAsync(registrationIds);

            foreach (var p in validToGenerateEcertificateAggregatedRegistrations)
            {
                var principalUser = facilitatorsDic.GetValueOrDefault(p.Course.CourseFacilitatorIds.First());
                var aggregatedECertificateTemplate = aggregatedECertificateTemplatesDic[p.Course.ECertificateTemplateId.GetValueOrDefault()];

                var base64PdfCertificate = await _certificateBuilder.BuildForParticipantBase64(
                    p.Registration,
                    p.User,
                    p.Course,
                    aggregatedECertificateTemplate,
                    principalUser,
                    ReportGeneralOutputFormatType.PDF,
                    cancellationToken);
                var base64ImageCertificate = await _certificateBuilder.BuildForParticipantBase64(
                    p.Registration,
                    p.User,
                    p.Course,
                    aggregatedECertificateTemplate,
                    principalUser,
                    ReportGeneralOutputFormatType.IMAGE,
                    cancellationToken);

                var participantCertificate = RegistrationECertificate.New(
                    p.Registration,
                    p.User,
                    p.Course,
                    aggregatedECertificateTemplate.Layout,
                    base64PdfCertificate,
                    base64ImageCertificate);

                await _registrationECertificateCudLogic.InsertAsync(participantCertificate);
            }
        }
    }
}
