using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.DomainExtensions;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Microservice.Course.Infrastructure;
using Telerik.Reporting;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Helpers;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ECertificateBuilderLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseUser> _userReadOnlyRepository;
        private readonly GetAggregatedECertificateTemplateSharedQuery _aggregatedECertificateTemplateSharedQuery;
        private readonly GetAggregatedRegistrationSharedQuery _aggregatedRegistrationSharedQuery;
        private readonly IStorageService _storageService;

        public ECertificateBuilderLogic(
            IReadOnlyRepository<CourseUser> userReadOnlyRepository,
            GetAggregatedECertificateTemplateSharedQuery aggregatedECertificateTemplateSharedQuery,
            IStorageService storageService,
            GetAggregatedRegistrationSharedQuery aggregatedRegistrationSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _userReadOnlyRepository = userReadOnlyRepository;
            _aggregatedECertificateTemplateSharedQuery = aggregatedECertificateTemplateSharedQuery;
            _storageService = storageService;
            _aggregatedRegistrationSharedQuery = aggregatedRegistrationSharedQuery;
        }

        public async Task<string> BuildForParticipantBase64(
            Registration forParticipantRegistration,
            CourseUser forParticipantUser,
            CourseEntity forCourse,
            ECertificateTemplateAggregatedEntityModel forCourseECertificateTemplate,
            CourseUser principalUser,
            ReportGeneralOutputFormatType format,
            CancellationToken cancellationToken = default)
        {
            var autoPopulatedParams = forCourseECertificateTemplate.Layout.Params
                .Where(p => p.IsAutoPopulated)
                .Select(
                p =>
                {
                    switch (p.Key)
                    {
                        case ECertificateSupportedField.FullName:
                            return new Parameter(ECertificateSupportedField.FullName.ToString(), forParticipantUser.FullName());
                        case ECertificateSupportedField.Principal:
                            return new Parameter(ECertificateSupportedField.Principal.ToString(), principalUser?.FullName().TakeFirst(70) ?? "n/a");
                        case ECertificateSupportedField.CourseName:
                            return new Parameter(ECertificateSupportedField.CourseName.ToString(), forCourse.CourseNameInECertificate ?? forCourse.CourseName.TakeFirst(100));
                        case ECertificateSupportedField.CompletedDate:
                            return new Parameter(
                                ECertificateSupportedField.CompletedDate.ToString(),
                                forParticipantRegistration.LearningCompletedDate.GetValueOrDefault().ToDateTimeInSystemTimeZone().ToString(DateTimeFormatConstant.OnlyDate));
                        default:
                            return new Parameter(p.Key.ToString(), string.Empty);
                    }
                })
                .ToList();

            return await BuildForLayoutBase64(
                forCourseECertificateTemplate.Layout,
                format,
                forCourseECertificateTemplate.Template.Params,
                autoPopulatedParams,
                cancellationToken);
        }

        public async Task<string> BuildForParticipantBase64(
            Registration forParticipantRegistration,
            CourseUser forParticipantUser,
            CourseEntity forCourse,
            ReportGeneralOutputFormatType format,
            CancellationToken cancellationToken = default)
        {
            // Get e-certificate template and layout information
            var aggregatedECertificateTemplate = await _aggregatedECertificateTemplateSharedQuery.ById(forCourse.ECertificateTemplateId.GetValueOrDefault());
            var firstFacilitator = await _userReadOnlyRepository.GetAsync(forCourse.CourseFacilitatorIds.First());

            return await BuildForParticipantBase64(
                forParticipantRegistration,
                forParticipantUser,
                forCourse,
                aggregatedECertificateTemplate,
                firstFacilitator,
                format,
                cancellationToken);
        }

        public async Task<string> BuildForParticipantBase64(
            Guid registrationId,
            ReportGeneralOutputFormatType format,
            CancellationToken cancellationToken = default)
        {
            var aggregatedRegistration = await _aggregatedRegistrationSharedQuery.ById(registrationId, cancellationToken);
            return await BuildForParticipantBase64(
                aggregatedRegistration.Registration,
                aggregatedRegistration.User,
                aggregatedRegistration.Course,
                format,
                cancellationToken);
        }

        public async Task<string> BuildForLayoutBase64(
            ECertificateLayout ecertificateLayout,
            ReportGeneralOutputFormatType format,
            List<ECertificateTemplateParam> certificateTemplateParams,
            List<Parameter> autoPopulatedReportParams,
            CancellationToken cancellationToken = default)
        {
            var templateParamsDic = certificateTemplateParams.ToDictionary(p => p.Key, p => (object)p.Value);
            var autoPopulatedReportParamsDic = autoPopulatedReportParams?.ToDictionary(p => p.Name, p => p.Value) ?? new Dictionary<string, object>();

            var ecertificateParamTasks = ecertificateLayout.Params
                .Select(async p =>
                {
                    var autoPopulatedValue = autoPopulatedReportParamsDic.GetValueOrDefault(p.Key.ToString(), string.Empty);

                    if (p.Type == ECertificateParamType.Text)
                    {
                        return new Parameter(p.Key.ToString(), templateParamsDic.GetValueOrDefault(p.Key, autoPopulatedValue));
                    }

                    if (p.Type == ECertificateParamType.Image)
                    {
                        var imageUrl = templateParamsDic.GetValueOrDefault(p.Key, autoPopulatedValue)?.ToString();
                        return !imageUrl.IsNullOrEmpty()
                            ? new Parameter(p.Key.ToString(), await _storageService.GetFileAsBase64String(imageUrl, cancellationToken))
                            : new Parameter(p.Key.ToString(), string.Empty);
                    }

                    return new Parameter(p.Key.ToString(), string.Empty);
                })
                .ToArray();

            var reportParams = (await Task.WhenAll(ecertificateParamTasks)).ToArray();

            var contentBase64 = format switch
            {
                ReportGeneralOutputFormatType.IMAGE => ExportHelper.TelerikReportExportMultiFormatBase64String(
                    ecertificateLayout.GetLayoutFilePath(),
                    ReportGeneralOutputFormatType.IMAGE,
                    "JPEG",
                    reportParams),
                ReportGeneralOutputFormatType.PDF => ExportHelper.TelerikReportExportSingleFormatBase64String(
                    ecertificateLayout.GetLayoutFilePath(),
                    ReportGeneralOutputFormatType.PDF,
                    reportParams),
                _ => throw new ArgumentException("invalid enum value", nameof(format))
            };

            return contentBase64;
        }
    }
}
